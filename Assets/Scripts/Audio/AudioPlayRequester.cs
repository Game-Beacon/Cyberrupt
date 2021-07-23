using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayRequester : GameBehaviour
{
    protected static AudioManager manager = null;
    [SerializeField]
    public ClipSettingGroup group;

    //Clip的編號對應到相對應的audio source
    protected Dictionary<int, AudioSource> playingDictionary = new Dictionary<int, AudioSource>();

    public override void GameStart()
    {
        if (manager == null)
            manager = AudioManager.instance;

        for (int i = 0; i < group.clips.Count; i++)
            playingDictionary.Add(i, null);

        Setup();
        update = false;
    }

    public virtual void Setup() { }

    public void PlayMusic(int index)
    {
        AudioSource source = manager.PlayMusic(this, group.clips[index]);
        playingDictionary[index] = source;
    }

    public void PlayMusicFadeIn(int index, float fadeTime)
    {
        AudioSource source = manager.PlayMusicFadeIn(this, group.clips[index], fadeTime);
        playingDictionary[index] = source;
    }

    public void PlayMusicScheduled(int index, double time)
    {
        AudioSource source = manager.PlayMusicScheduled(this, group.clips[index], time);
        playingDictionary[index] = source;
    }

    public void AudioStop(int index)
    {
        if(playingDictionary.ContainsKey(index) && playingDictionary[index] != null)
        {
            manager.StopRequesterSingleAudio(this, playingDictionary[index]);
            playingDictionary[index] = null;
        }
    }

    public void AudioFadeOut(int index, float fadeTime)
    {
        if (playingDictionary.ContainsKey(index) && playingDictionary[index] != null)
        {
            manager.StopRequesterSingleAudioFadeOut(this, playingDictionary[index], fadeTime);
            playingDictionary[index] = null;
        }
    }

    public void AllAudioStop()
    {
        manager.StopRequesterAllAudio(this);
    }

    public void PlaySFXOneShot(int index)
    {
        manager.PlaySFXOneShot(group.clips[index]);
    }

    public void PlaySFXOneShotIgnore(int index)
    {
        manager.PlaySFXOneShotIgnore(group.clips[index]);
    }

    public void PlaySFXInterruptable(int index)
    {
        AudioSource source = manager.PlaySFXInterruptable(this, group.clips[index]);
        playingDictionary[index] = source;
    }

    public void MusicPhaseShift()
    {
        manager.MusicPhaseShift();
    }
}
