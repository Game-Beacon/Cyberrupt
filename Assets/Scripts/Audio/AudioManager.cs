using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : GameBehaviour
{
    private static AudioManager _instance = null;
    public static AudioManager instance { get { return _instance; } private set { } }

    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private List<AudioSource> _musicSources = new List<AudioSource>();
    [SerializeField]
    private AudioSource _sfxSources;
    [SerializeField]
    private AudioSource _sfxSourcesIgnore;

    public override void GameAwake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontKillSelfOnLoad();
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
        _sfxSourcesIgnore.ignoreListenerPause = true;
    }

    public AudioSource PlayMusic(ClipSetting setting)
    {
        AudioSource source = FindIdleMusicSource();
        if (source == null)
            return null;

        source.clip = setting.clip;
        source.volume = setting.volume;
        source.loop = setting.loop;
        source.Play();

        return source;
    }

    public AudioSource PlayMusicScheduled(ClipSetting setting, double time)
    {
        AudioSource source = FindIdleMusicSource();
        if (source == null)
            return null;

        source.clip = setting.clip;
        source.volume = setting.volume;
        source.loop = setting.loop;
        source.PlayScheduled(time);

        return source;
    }

    public void PlaySFX(ClipSetting setting)
    {
        _sfxSources.PlayOneShot(setting.clip, setting.volume);
    }

    public void PlaySFXIgnore(ClipSetting setting)
    {
        _sfxSourcesIgnore.PlayOneShot(setting.clip, setting.volume);
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

    private void OnPause()
    {
        AudioListener.pause = true;
    }

    private void OnUnpause()
    {
        AudioListener.pause = false;
    }
}
