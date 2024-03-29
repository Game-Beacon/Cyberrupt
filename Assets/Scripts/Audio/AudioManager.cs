﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class AudioManager : GameBehaviour
{
    private static AudioManager _instance = null;
    public static AudioManager instance { get { return _instance; } private set { } }

    [Space(10), SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private List<AudioSource> _musicSources = new List<AudioSource>();
    [SerializeField]
    private List<AudioSource> _sfxSources = new List<AudioSource>();
    [SerializeField]
    private AudioSource _sfxOneShotSource;
    [SerializeField]
    private AudioSource _sfxOneShotSourceIgnore;

    private Dictionary<AudioSource, AudioPlayRequester> requesterDictionary = new Dictionary<AudioSource, AudioPlayRequester>();

    //Is it really a good practice to put audio managing and remixing in a same script?
    [Space(10), SerializeField]
    private int musicLowPassOn;
    [SerializeField]
    private int musicLowPassOff;
    [SerializeField]
    private AnimationCurve musicPhaseShiftCurve;
    private Coroutine phaseShiftCoroutine = null;

    public override void GameAwake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontKillSelfOnLoad();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        TimeManager.OnGamePause.AddListener(OnPause);
        TimeManager.OnGameUnpause.AddListener(OnUnpause);

        foreach (AudioSource source in _musicSources)
            source.ignoreListenerPause = true;
        _sfxOneShotSourceIgnore.ignoreListenerPause = true;

        foreach (AudioSource source in _musicSources)
            requesterDictionary.Add(source, null);
        foreach (AudioSource source in _sfxSources)
            requesterDictionary.Add(source, null);

        SetVolume("MasterVol", 0.75f);
        SetVolume("MusicVol", 0.75f);
        SetVolume("SfxVol", 0.75f);
    }

    private void SetAudioData(AudioSource source, ClipSetting setting)
    {
        source.DOKill();
        source.clip = setting.clip;
        source.volume = setting.volume;
        source.loop = setting.loop;
    }

    public AudioSource PlayMusic(AudioPlayRequester requester, ClipSetting setting)
    {
        AudioSource source = FindIdleMusicSource();
        if (source == null)
            return null;

        SetAudioData(source, setting);
        source.Play();
        requesterDictionary[source] = requester;

        return source;
    }

    public AudioSource PlayMusicFadeIn(AudioPlayRequester requester, ClipSetting setting, float fadeTime)
    {
        AudioSource source = FindIdleMusicSource();
        if (source == null)
            return null;

        SetAudioData(source, setting);
        source.Play();
        source.volume = 0;
        source.DOFade(setting.volume, fadeTime);

        requesterDictionary[source] = requester;

        return source;
    }

    public AudioSource PlayMusicScheduled(AudioPlayRequester requester, ClipSetting setting, double time)
    {
        AudioSource source = FindIdleMusicSource();
        if (source == null)
            return null;

        SetAudioData(source, setting);
        source.PlayScheduled(time);

        requesterDictionary[source] = requester;

        return source;
    }

    public AudioSource PlaySFXInterruptable(AudioPlayRequester requester, ClipSetting setting)
    {
        AudioSource source = FindIdleSFXSource();
        if (source == null)
            return null;

        SetAudioData(source, setting);
        source.Play();

        requesterDictionary[source] = requester;

        return source;
    }

    public void StopRequesterAllAudio(AudioPlayRequester requester)
    {
        List<AudioSource> sources = requesterDictionary.Keys.ToList();

        foreach (AudioSource source in sources)
        {
            if(requesterDictionary[source] == requester && source.isPlaying)
            {
                source.Stop();
                requesterDictionary[source] = null;
            }
        }
    }

    public void StopRequesterSingleAudio(AudioPlayRequester requester, AudioSource source)
    {
        if (!requesterDictionary.ContainsKey(source))
            return;
        if (requesterDictionary[source] != requester)
            return;
        if (source.isPlaying)
        {
            source.Stop();
            requesterDictionary[source] = null;
        }
    }

    public void StopRequesterSingleAudioFadeOut(AudioPlayRequester requester, AudioSource source, float fadeTime)
    {
        if (!requesterDictionary.ContainsKey(source))
            return;
        if (requesterDictionary[source] != requester)
            return;
        if (source.isPlaying)
        {
            source.DOFade(0, fadeTime).OnComplete(() => 
            {
                if (source.isPlaying)
                    source.Stop();
                requesterDictionary[source] = null;
            });
        }
    }

    public void PlaySFXOneShot(ClipSetting setting)
    {
        _sfxOneShotSource.PlayOneShot(setting.clip, setting.volume);
    }

    public void PlaySFXOneShotIgnore(ClipSetting setting)
    {
        _sfxOneShotSourceIgnore.PlayOneShot(setting.clip, setting.volume);
    }

    public void StopAllAudio()
    {
        List<AudioSource> sources = requesterDictionary.Keys.ToList();

        foreach (AudioSource source in sources)
        {
            if (source.isPlaying)
            {
                source.Stop();
                requesterDictionary[source] = null;
            }
        }
    }

    private AudioSource FindIdleMusicSource()
    {
        foreach (AudioSource source in _musicSources)
        {
            if (!source.isPlaying)
                return source;
        }
        return null;
    }

    private AudioSource FindIdleSFXSource()
    {
        foreach (AudioSource source in _sfxSources)
        {
            if (!source.isPlaying)
                return source;
        }
        return null;
    }

    public float GetVolume(string name)
    {
        float value;
        mixer.GetFloat(name, out value);
        return Mathf.Pow(10, value / 20f);
    }

    public void SetVolume(string name, float value)
    {
        bool whatever = mixer.SetFloat(name, Mathf.Log10(value) * 20);
    }

    private void OnPause()
    {
        AudioListener.pause = true;
        mixer.SetFloat("MusicLowPass", musicLowPassOn);
    }

    private void OnUnpause()
    {
        AudioListener.pause = false;
        mixer.SetFloat("MusicLowPass", musicLowPassOff);
    }

    public void MusicPhaseShift()
    {
        if (phaseShiftCoroutine != null)
            StopCoroutine(phaseShiftCoroutine);
        phaseShiftCoroutine = StartCoroutine(DoPhaseShift());
    }

    IEnumerator DoPhaseShift()
    {
        float time = musicPhaseShiftCurve.keys[musicPhaseShiftCurve.keys.Length - 1].time;
        float timer = 0;

        while(timer < time)
        {
            mixer.SetFloat("MusicPitch", musicPhaseShiftCurve.Evaluate(timer));
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        mixer.SetFloat("MusicPitch", musicPhaseShiftCurve.keys[musicPhaseShiftCurve.keys.Length - 1].value);
    }

    private void OnSceneUnloaded(Scene current)
    {
        foreach (AudioSource source in _musicSources)
            source.Stop();
        foreach (AudioSource source in _sfxSources)
            source.Stop();
        _sfxOneShotSource.Stop();
        _sfxOneShotSourceIgnore.Stop();
    }
}
