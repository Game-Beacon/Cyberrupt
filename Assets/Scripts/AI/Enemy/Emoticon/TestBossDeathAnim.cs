using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class TestBossDeathAnim : GameBehaviour
{
    [SerializeField]
    public UltEvent OnEnterDeath = new UltEvent();
    [SerializeField]
    public UltEvent OnDeath = new UltEvent();
    
    [Space(10)]
    [SerializeField]
    private Enemy enemy;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private ParticleSystem dyingParticle;
    [SerializeField]
    private ParticleSystem deadParticle;

    [Space(10)]
    [SerializeField]
    private Sprite dyingSprite;
    [SerializeField]
    private float dyingTime;

    public override void GameAwake()
    {
        enemy.Death.AddAction(() => OnEnterDeath.Invoke());
        enemy.Death.AddEnumerator(DyingAnimation());
        enemy.Death.OnEventComplete.AddListener(() => { OnDeath.Invoke(); });

        OnDeath += () =>
        {
            deadParticle.transform.SetParent(null);
            deadParticle.Play();
        };
    }

    IEnumerator DyingAnimation()
    {
        sr.sprite = dyingSprite;
        dyingParticle.Play();
        transform.DOShakePosition(dyingTime, 0.2f, 25, 90, false, false);
        yield return new WaitForSeconds(dyingTime);
    }
}
