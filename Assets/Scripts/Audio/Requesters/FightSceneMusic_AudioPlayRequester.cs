using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSceneMusic_AudioPlayRequester : AudioPlayRequester
{
    [SerializeField]
    private float enterBossFadeTime;
    [SerializeField]
    private float exitBossFadeTime;

    public override void Setup()
    {
        EnemyManager enemyManager = GetComponent<EnemyManager>();
        BaseBGM();

        enemyManager.OnEnterBossWave.AddListener(EnterBoss);
        enemyManager.OnBossSpawn.AddListener(_ => BossBGM());
        enemyManager.OnExitBossWave.AddListener(ExitBoss);
    }

    public void BaseBGM()
    {
        double clipLength = (double)group.clips[0].clip.samples / group.clips[0].clip.frequency;
        PlayMusicScheduled(0, AudioSettings.dspTime + 0.1);
        PlayMusicScheduled(1, AudioSettings.dspTime + 0.1 + clipLength);
    }

    public void BossBGM()
    {
        double clipLength = (double)group.clips[2].clip.samples / group.clips[2].clip.frequency;
        PlayMusicScheduled(2, AudioSettings.dspTime + 0.1);
        PlayMusicScheduled(3, AudioSettings.dspTime + 0.1 + clipLength);
    }

    public void EnterBoss()
    {
        AudioFadeOut(0, enterBossFadeTime);
        AudioFadeOut(1, enterBossFadeTime);
    }

    public void ExitBoss()
    {
        AudioFadeOut(2, exitBossFadeTime);
        AudioFadeOut(3, exitBossFadeTime);
        BaseBGM();
    }
}
