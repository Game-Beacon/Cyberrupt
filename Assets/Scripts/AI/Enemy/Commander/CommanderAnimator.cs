using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CommanderAnimator : GameBehaviour
{
    [SerializeField]
    private Transform barGroup;
    [SerializeField]
    private float initRotateSpeed;
    private ReactiveProperty<float> targetRatio = new ReactiveProperty<float>(1);
    private float ratio = 1;
    private float rotationSpeed => this.initRotateSpeed * this.ratio;
    private IDisposable resumeRatio;

    private void AddBarTweener(Transform barRoot)
    {
        var far = 0.05f;
        var bar = barRoot.GetChild(0);
        bar.DOLocalMoveX(bar.localPosition.x - far, 1)
            .SetLoops(-1, LoopType.Yoyo);
        bar.OnDestroyAsObservable()
            .Subscribe(_ => bar.DOKill());
    }

    public override void GameAwake()
    {
        // Register OnDestroy listeners
        this.OnDestroyAsObservable()
            .Subscribe(_ => transform.DOKill());
        // Control target speed ratio
        Tweener ratioTweener = null;
        this.targetRatio
            // Smoothly update speed ratio
            .Subscribe(r =>
            {
                // Kill previous tweener if exists
                ratioTweener?.Kill();
                // Update tweener
                ratioTweener = DOTween.To(
                    () => this.ratio,
                    v => this.ratio = v,
                    r, 0.3f
                );
            })
            .AddTo(this);
        // Probabilistically inverse rotate every 3 sec.
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
        // Register bar tweener
        Debug.Assert(this.barGroup, "Bar group isn't set.");
        foreach(Transform barRoot in this.barGroup)
        {
            this.AddBarTweener(barRoot);
        }
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