using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class Projectile : GameBehaviour
{
    [SerializeField]
    private Color color;
    [SerializeField, Range(0f, 1f)]
    private float minAlpha;
    [SerializeField, Range(0f, 1f)]
    private float maxAlpha;
    [SerializeField]
    private float flickerPeriod;

    [Space(20),SerializeField]
    private List<SpriteRenderer> srs = new List<SpriteRenderer>();
    [SerializeField]
    private UltEvent OnSpawnProjectile = new UltEvent();

    public override void GameAwake()
    {
        foreach(SpriteRenderer sr in srs)
        {
            sr.color = new Color(color.r, color.g, color.b, minAlpha);
            sr.DOFade(maxAlpha, flickerPeriod).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void SpawnProjectile()
    {
        OnSpawnProjectile.Invoke();
        KillBehaviour(true);
    }

    public override void OnKilled()
    {
        foreach (SpriteRenderer sr in srs)
            sr.DOKill();
    }
}
