using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSceneMusic_AudioPlayRequester : AudioPlayRequester
{
    private AudioSource music = null;

    public override void Setup()
    {
        EnemyManager enemyManager = GetComponent<EnemyManager>();

        music = manager.PlayMusic(this, group.clips[0]);
    }
}
