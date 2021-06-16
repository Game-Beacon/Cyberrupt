using UnityEngine;
using DG.Tweening;
using UniRx;

public class PlayerAnimate : GameBehaviour
{
    [SerializeField]
    private Transform face;
    [SerializeField]
    [Tooltip("Offset of face when moving")]
    private float offset;

    private Player player;

    public override void GameStart()
    {
        this.player = GetComponent<Player>();
        this.player.IsDashing
            .Skip(1)
            .Subscribe(d => {
                // Start dashing
                if(d)
                {
                    if(Mathf.Abs (this.player.velocity.x) > 0)
                        transform.DOScaleY(0.5f, 0.1f);
                    else
                        transform.DOScaleX(0.5f, 0.1f);
                }
                // End dashing
                else
                    transform.DOScale(Vector3.one, 0.1f);
            })
            .AddTo(this);
    }

    public override void GameUpdate()
    {
        this.face.DOLocalMove(this.player.velocity.normalized * this.offset, 0.1f);
    }
}
