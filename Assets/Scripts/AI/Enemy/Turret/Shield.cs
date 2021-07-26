using UnityEngine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

public class Shield : Enemy
{
    [SerializeField]
    private Enemy main;
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private float angularSpeed;
    [SerializeField]
    private float knockbackDistance;

    protected override void EnemyAwake()
    {
        main.OnDeath += Die;
        // Subscribe knockback
        Tween knockback = null;
        var originX = transform.localPosition.x;
        this.OnHit += () =>
        {
            knockback?.Kill();
            knockback = transform.DOLocalMoveX(originX - this.knockbackDistance, 0.1f);
            knockback.OnComplete(() => {
                knockback = transform.DOLocalMoveX(originX, 1.5f);
            });
        };
        transform.OnDestroyAsObservable()
            .Subscribe(_ => knockback.Kill())
            .AddTo(this);
    }

    protected override void EnemyUpdate()
    {
        float rotation = parent.rotation.eulerAngles.z;
        rotation += angularSpeed * Time.deltaTime;
        parent.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public override void OnKilled()
    {
        if (main != null)
            main.OnDeath -= Die;
        if (parent != null)
            DestroySafe(parent.gameObject/*, 0.1f*/);
    }
}