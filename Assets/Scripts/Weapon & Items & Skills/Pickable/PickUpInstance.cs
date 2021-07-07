using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PickUpInstance : GameBehaviour
{
    [SerializeField]
    private SpriteRenderer iconDisplayer;
    [SerializeField]
    private SpriteRenderer background;
    private SpriteRenderer[] srs => new [] { iconDisplayer, background };

    private IPickable pickable = null;
    private float remainTime;

    private LayerMask playerLayer;

    private void clearTween()
    {
        // TODO: Move this call to a resonable location,
        //  the transform tween is not created inside this
        //  component, but there does not exist anyway to
        //  get notification on destroy (or kill) event now.
        transform.DOKill();
        foreach(var sr in this.srs)
            sr.DOKill();
    }

    private IEnumerator gameStart()
    {
        var halfTime = this.remainTime / 2;
        yield return new WaitForSeconds(halfTime);
        halfTime = this.remainTime - halfTime;
        // Start fade out
        foreach(var sr in this.srs)
            sr.DOFade(0.1f, halfTime)
                .SetEase(Ease.InBounce);
        yield return new WaitForSeconds(halfTime);
        KillBehaviour(true);
    }

    public override void GameStart()
    {
        StartCoroutine(this.gameStart());
    }

    public void InjectData(IPickable data, LayerMask mask, float remain)
    {
        pickable = data;
        iconDisplayer.sprite = pickable.icon;
        playerLayer = mask;
        remainTime = remain;

        PickUpTypeData d = PickUpHelper.instance.GetData(data.type);
        background.color = d.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            AudioManager.instance.PlaySFXOneShot(PickUpHelper.instance.GetData(pickable.type).clip);
            pickable.OnPick(player);
            // Ensure that tweeners are killed
            this.clearTween();
            KillBehaviour(true);
        }
    }
}