using DG.Tweening;
using UniRx;
using UnityEngine;

public class PlayerAnimate : GameBehaviour
{
    [SerializeField]
    private Transform face;
    [SerializeField]
    [Tooltip("Offset of face when moving")]
    private float offset;
    [SerializeField]
    private float emitInterval;
    private Player player;

    private ParticleSystem particle;
    private float emitTimer;

    private void OnDestroy()
    {
        transform.DOKill();
        this.face.DOKill();
    }

    public override void GameStart()
    {
        this.particle = GetComponentInChildren<ParticleSystem>();
        this.player = GetComponent<Player>();
        this.player.IsDashing
            .Skip(1)
            .Subscribe(d =>
            {
                transform.DOKill();
                // Start dashing
                if(d)
                {
                    if(Mathf.Abs(this.player.velocity.x) > 0)
                        transform.DOScaleY(0.5f, 0.1f);
                    else
                        transform.DOScaleX(0.5f, 0.1f);
                }
                // End dashing
                else
                {
                    DOTween.Sequence()
                        .Append(transform.DOScale(Vector3.one, 0.1f))
                        .Append(transform.DOPunchScale(Vector3.one * 0.25f, 0.2f))
                        // Recover scale
                        .OnKill(() => transform.localScale = Vector3.one);
                }
            })
            .AddTo(this);
    }

    public override void GameUpdate()
    {
        var dir = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2) transform.position;
        // Move face toward mouse position
        this.face.DOKill();
        this.face.DOLocalMove(dir.normalized * this.offset, 0.1f);
        if(this.player.velocity.sqrMagnitude > 0)
        {
            this.emitTimer -= Time.deltaTime;
            if(this.emitTimer <= 0)
            {
                this.emitTimer = this.emitInterval;
                this.trailEmit(5);
            }
        }
        else
        {
            this.emitTimer = 0;
        }
    }

    private void trailEmit(int count)
    {
        var angle = Mathf.Atan2(this.player.velocity.y, this.player.velocity.x) * Mathf.Rad2Deg;
        angle = Mathf.Repeat(angle + 180, 360);
        var shape = this.particle.shape;
        var newRotate = shape.rotation;
        newRotate.x = -angle;
        shape.rotation = newRotate;
        this.particle.Emit(count);
    }
}