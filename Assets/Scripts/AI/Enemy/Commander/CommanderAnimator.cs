using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CommanderAnimator : GameBehaviour
{
    [SerializeField]
    private float initRotateSpeed;
    private ReactiveProperty<float> targetRatio = new ReactiveProperty<float>(1);
    private float ratio = 1;
    private float rotationSpeed => this.initRotateSpeed * this.ratio;
    private IDisposable resumeRatio;

    public override void GameAwake()
    {
        // Control target speed ratio
        Tweener ratioTweener = null;
        this.targetRatio
            .Subscribe(r =>
            {
                ratioTweener?.Kill();
                ratioTweener = DOTween.To(
                    () => this.ratio,
                    v => this.ratio = v,
                    r, 0.3f
                );
            })
            .AddTo(this);
        // test
        var interval = Observable.Interval(
            TimeSpan.FromSeconds(3f),
            Scheduler.MainThread
        );
        interval.Subscribe(_ =>
            {
                if(UnityEngine.Random.value < 0.6)
                    this.SetRotateRatio(-5);
            })
            .AddTo(this);
    }

    public override void GameStart()
    {
        GetComponent<Commander>().OnUpdateTransform
            .AddListener(this.updateTransform);
    }

    public void RotateTriangle()
    {

    }

    // Smoothly spped up the rotation speed by ratio
    public void SetRotateRatio(float ratio, float duration = .5f)
    {
        this.targetRatio.Value = ratio;
        this.resumeRatio?.Dispose();
        this.resumeRatio = this.FixedUpdateAsObservable()
            .Delay(TimeSpan.FromSeconds(duration))
            .Subscribe(_ =>
            {
                this.targetRatio.Value = 1;
            })
            .AddTo(this);
    }

    private void updateTransform()
    {
        // Update rotation
        transform.Rotate(Vector3.forward * (this.rotationSpeed * Time.fixedDeltaTime));
    }
}