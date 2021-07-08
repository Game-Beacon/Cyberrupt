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
        PlayMusic(0);

        enemyManager.OnEnterBossWave.AddListener(EnterBoss);
        enemyManager.OnBossSpawn.AddListener(_ => PlayMusic(1));
        enemyManager.OnExitBossWave.AddListener(ExitBoss);
    }

    public void EnterBoss()
    {
        AudioFadeOut(0, enterBossFadeTime);
    }

    public void ExitBoss()
    {
        AudioFadeOut(1, exitBossFadeTime);
        PlayMusicFadeIn(0, exitBossFadeTime);
    }
}
