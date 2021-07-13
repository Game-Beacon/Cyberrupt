using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class SingularityAnimate : GameBehaviour
{
    [SerializeField]
    private Transform core;
    [SerializeField]
    private Transform outer;
    [SerializeField]
    private float coreRotationSpeedBase;
    [SerializeField]
    private float outerRotationSpeedBase;
    [SerializeField]
    private float attackScalor;

    private int coreRotationDirection = 1;
    private int outerRotationDirection = 1;

    private float coreRotationSpeed => this.coreRotationSpeedBase * this.coreRotationDirection;
    private float outerRotationSpeed => this.outerRotationSpeedBase * this.outerRotationDirection;

    public override void GameStart()
    {
        var interval = Observable.Interval(
            TimeSpan.FromSeconds(3f),
            Scheduler.MainThread
        );
        // Randomly change rotation direction
        interval.Subscribe(_ =>
            {
                if(UnityEngine.Random.value < 0.3)
                    this.coreRotationDirection *= -1;
            })
            .AddTo(this);
        interval.Subscribe(_ =>
            {
                if(UnityEngine.Random.value < 0.3)
                    this.outerRotationDirection *= -1;
            })
            .AddTo(this);
        GetComponent<Singularity_Ring>().OnEnter += () =>
        {
            var scalor = new Vector3(
                this.attackScalor,
                this.attackScalor,
                0
            );
            outer.DOPunchScale(scalor, 1, 1);
        };
    }

    public override void GameUpdate()
    {
        core.Rotate(Vector3.forward * (this.coreRotationSpeed * Time.deltaTime));
        outer.Rotate(Vector3.forward * (this.outerRotationSpeed * Time.deltaTime));
    }
}