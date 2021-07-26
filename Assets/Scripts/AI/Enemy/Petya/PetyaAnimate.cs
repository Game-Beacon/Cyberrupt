using System.Collections;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
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
    private Transform body;
    [SerializeField]
    private Transform shoulder;
    [SerializeField]
    private Transform left;
    [SerializeField]
    private Transform right;
    [SerializeField]
    private GameObject burst;

    [Header("Animation params")]
    [SerializeField]
    private float strecthDuration = .1f;
    [Header("Bitcoin")]
    [SerializeField]
    private float waveDuration = .1f;
    [Header("Flower")]
    [SerializeField]
    private float flowerRotateDuration = 1;
    [Header("Laser ring")]
    [SerializeField]
    private float laserRingBeatInterval = .1f;

    private float strecthLength = 0.8f;
    private float resetTransformDuration = 0.3f;
    private TransformBackup originBody;
    private TransformBackup originShoulder;
    private TransformBackup originLeft;
    private TransformBackup originRight;

    private Tween currentTween;

    public override void GameAwake()
    {
        this.originBody = new TransformBackup(this.body);
        this.originShoulder = new TransformBackup(this.shoulder);
        this.originLeft = new TransformBackup(this.left);
        this.originRight = new TransformBackup(this.right);
    }

    public override void GameStart()
    {
        var bitcoin = GetComponent<Petya_BitCoin>();
        bitcoin.OnEnter += this.beforeCoinAttack;
        bitcoin.OnExit += this.afterCoinAttack;
        var flower = GetComponent<Petya_FlowerSpread>();
        flower.OnEnter += this.beforeFlower;
        flower.OnExit += this.afterFlower;
        var bigLaser = GetComponent<Petya_BigLaser>();
        bigLaser.OnEnter += this.beforeBigLaser;
        bigLaser.OnExit += this.afterBigLaser;
        var laserRing = GetComponent<Petya_LaserRing>();
        laserRing.OnEnter += this.beforeLaserRing;
        laserRing.OnExit += this.afterLaserRing;
        GetComponent<Petya>().Death
            .AddEnumerator(this.deathAnimation());
    }

    private void resetTransforms()
    {
        this.originBody.Recover(this.body);
        this.originShoulder.Recover(this.shoulder);
        this.originLeft.Recover(this.left);
        this.originRight.Recover(this.right);
    }

    private Tween doResetTransform(float duration)
    {
        duration /= 3;
        var resetShoulder = DOTween.Sequence()
            .Append(this.shoulder.DOLocalMove(this.originShoulder.position, duration))
            .Append(this.shoulder.DOLocalRotate(this.originShoulder.rotation.eulerAngles, duration));
        var resetArm = DOTween.Sequence()
            .Append(this.right.DOLocalMove(this.originRight.position, duration))
            .Join(this.left.DOLocalMove(this.originLeft.position, duration));
        var resetBody = DOTween.Sequence()
            .Append(this.body.DOLocalMove(this.originBody.position, duration))
            .Append(this.body.DOLocalRotate(this.originBody.rotation.eulerAngles, duration));
        return DOTween.Sequence()
            .Append(resetShoulder)
            .Append(resetArm)
            .Append(resetBody)
            .OnKill(this.resetTransforms);
    }

    private void ensureTween()
    {
        this.currentTween?.Kill();
        this.currentTween = null;
    }

    private Tween strecth(float length, float duration)
    {
        var delta = Vector3.up * length;
        return DOTween.Sequence()
            .Join(this.left.DOLocalMove(this.originLeft.position + delta, duration))
            .Join(this.right.DOLocalMove(this.originRight.position - delta, duration));
    }

    private Tween pingPongMoveX(float boundA, float boundB, float duration)
    {
        float part = duration / 4;
        return DOTween.Sequence()
            .Append(this.shoulder.DOLocalMoveX(boundA, part))
            .Append(this.shoulder.DOLocalMoveX(boundB, part * 2))
            .Append(this.shoulder.DOLocalMoveX(this.originShoulder.position.x, part));
    }

    private void beforeCoinAttack()
    {
        this.ensureTween();
        var tempSeq = DOTween.Sequence();
        tempSeq.Append(this.strecth(this.strecthLength, this.strecthDuration));
        tempSeq.Append(
            this.pingPongMoveX(1, -1, this.waveDuration)
            // Infinte loop is ignored inside sequence
            // set a large number to do that
            .SetLoops(10086, LoopType.Restart)
        );
        this.currentTween = tempSeq;
    }

    private void afterCoinAttack()
    {
        this.ensureTween();
        this.currentTween = this.doResetTransform(this.resetTransformDuration);
    }

    private Tween bodyRotate(float duration)
    {
        return DOTween.Sequence()
            .Join(
                this.shoulder.DORotate(
                    Vector3.forward * 360,
                    duration,
                    RotateMode.WorldAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(10086, LoopType.Incremental)
            )
            .Join(
                this.body.DORotate(
                    Vector3.forward * -360,
                    duration,
                    RotateMode.WorldAxisAdd
                )
                .SetEase(Ease.Linear)
                .SetLoops(10086, LoopType.Incremental)
            );
    }

    private void beforeFlower()
    {
        this.ensureTween();
        var tempSeq = DOTween.Sequence();
        tempSeq.Append(this.strecth(this.strecthLength, this.strecthDuration));
        tempSeq.Append(this.body.DORotate(Vector3.zero, 0.3f));
        tempSeq.Append(this.bodyRotate(this.flowerRotateDuration));
        this.currentTween = tempSeq;
    }

    private void afterFlower()
    {
        this.ensureTween();
        this.currentTween = this.doResetTransform(this.resetTransformDuration);
    }

    private void beforeBigLaser()
    {
        this.ensureTween();
        this.currentTween = DOTween.Sequence()
            .Append(this.strecth(this.strecthLength, this.strecthDuration))
            .Append(this.shoulder.DOLocalMoveX(-1, 12));
    }

    private void afterBigLaser()
    {
        this.ensureTween();
        this.currentTween = this.doResetTransform(this.resetTransformDuration);
    }

    private void beforeLaserRing()
    {
        this.ensureTween();
        var tempSeq = DOTween.Sequence()
            .Append(this.strecth(this.strecthLength, this.strecthDuration))
            .Join(
                DOTween.Sequence()
                .Append(this.shoulder.DOLocalRotate(Vector3.forward * -90, .1f))
                .AppendInterval(this.laserRingBeatInterval)
                .SetLoops(10086, LoopType.Incremental)
            );
        this.currentTween = tempSeq;
    }

    private void afterLaserRing()
    {
        this.ensureTween();
        this.currentTween = this.doResetTransform(this.resetTransformDuration);
    }

    private IEnumerator deathAnimation()
    {
        // Disable attack
        GetComponent<AIStateMachine>().update = false;
        GetComponent<EnemyGroup>().enableColliders = false;
        GetComponent<SpawnDanmakuHelper>().KillAll();
        var audioRequester = GetComponent<AudioPlayRequester>();
        audioRequester.AllAudioStop();
        yield return this.body.DORotate(Vector3.forward * -90, 0.1f);
        CameraController.instance.CamShake(0.5f, 0.8f);
        yield return this.strecth(2, 0.5f).WaitForCompletion();
        var cubes = new [] { this.left, this.right };
        for(int i = 0; i < 4; i++)
        {
            audioRequester.PlaySFXOneShot(9);
            var stepDuration = 0.3f;
            CameraController.instance.CamShake(stepDuration, 0.8f);
            this.body.DORotate(Vector3.forward * -90, stepDuration, RotateMode.WorldAxisAdd);
            foreach(var cube in cubes)
            {
                var nextPoint = this.body.position + Random.onUnitSphere * 4;
                nextPoint.z = cube.position.z;
                cube.DORotate(Vector3.forward * Random.Range(0, 360f), stepDuration);
                cube.DOMove(nextPoint, stepDuration);
                Instantiate(this.burst, cube.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(stepDuration + 0.1f);
        }
        yield return this.strecth(2, 0.5f).WaitForCompletion();
        var infRotate = this.shoulder
            .DORotate(Vector3.forward * 360, 0.6f, RotateMode.WorldAxisAdd)
            .SetLoops(-1, LoopType.Incremental);
        this.shoulder.OnDestroyAsObservable()
            .Subscribe(_ => infRotate.Kill());
        yield return DOTween.Sequence()
            .Append(this.body.DOScale(Vector3.one * 2, 0.3f).SetEase(Ease.OutQuint))
            .Append(this.body.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutQuint))
            .WaitForCompletion();
        CameraController.instance.CamShake(0.5f, 0.8f);
        audioRequester.PlaySFXOneShot(8);
    }
}