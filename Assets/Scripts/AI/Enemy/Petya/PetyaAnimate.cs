using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PetyaAnimate : GameBehaviour
{
    private struct TransformBackup
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        public Vector3 position => this._position;
        public Quaternion rotation => this._rotation;
        public Vector3 scale => this._scale;

        public TransformBackup(Transform t)
        {
            this._position = t.localPosition;
            this._rotation = t.localRotation;
            this._scale = t.localScale;
        }

        public void Recover(Transform t)
        {
            t.localPosition = this._position;
            t.localRotation = this._rotation;
            t.localScale = this._scale;
        }
    };

    [SerializeField]
    private Transform root;
    [SerializeField]
    private Transform left;
    [SerializeField]
    private Transform right;

    [Header("Bitcoin")]
    [SerializeField]
    private float strecthDuration = .1f;
    [SerializeField]
    private float waveDuration = .1f;

    private TransformBackup originRoot;
    private TransformBackup originLeft;
    private TransformBackup originRight;

    private Tween currentTween;

    public override void GameAwake()
    {
        this.originRoot = new TransformBackup(this.root);
        this.originLeft = new TransformBackup(this.left);
        this.originRight = new TransformBackup(this.right);
    }

    public override void GameStart()
    {
        var bitcoin = GetComponent<Petya_BitCoin>();
        bitcoin.OnEnter += this.beforeCoinAttack;
        bitcoin.OnExit += this.afterCoinAttack;
    }

    private void resetTransforms()
    {
        this.originRoot.Recover(this.root);
        this.originLeft.Recover(this.left);
        this.originRight.Recover(this.right);
    }

    private Tween doResetTransform(float duration)
    {
        duration /= 2;
        return DOTween.Sequence()
            .Append(this.root.DOLocalMove(this.originRoot.position, duration))
            .Append(this.right.DOLocalMove(this.originRight.position, duration))
            .Join(this.left.DOLocalMove(this.originLeft.position, duration))
            .OnKill(this.resetTransforms);
    }

    private void ensureTween()
    {
        this.currentTween?.Kill();
        this.currentTween = null;
    }

    private Tween strecth(float length, float duration)
    {
        return DOTween.Sequence()
            .Join(this.left.DOLocalMoveY(this.originLeft.position.y + length, duration))
            .Join(this.right.DOLocalMoveY(this.originRight.position.y - length, duration));
    }

    private Tween pingPongMoveX(float boundA, float boundB, float duration)
    {
        float part = duration / 4;
        return DOTween.Sequence()
            .Append(this.root.DOLocalMoveX(boundA, part))
            .Append(this.root.DOLocalMoveX(boundB, part * 2))
            .Append(this.root.DOLocalMoveX(this.originRoot.position.x, part));
    }

    private void beforeCoinAttack()
    {
        this.ensureTween();
        var tempSeq = DOTween.Sequence();
        tempSeq.Append(this.strecth(0.8f, this.strecthDuration));
        tempSeq.Append(this.pingPongMoveX(1, -1, this.waveDuration).SetLoops(10086, LoopType.Restart));
        this.currentTween = tempSeq;
    }

    private void afterCoinAttack()
    {
        this.ensureTween();
        this.currentTween = this.doResetTransform(0.1f);
    }
}