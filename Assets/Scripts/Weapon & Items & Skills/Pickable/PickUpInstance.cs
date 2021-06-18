using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PickUpInstance : GameBehaviour
{
    [SerializeField]
    private SpriteRenderer iconDisplayer;
    [SerializeField]
    private SpriteRenderer background;

    private IPickable pickable = null;
    private float remainTime;

    private LayerMask playerLayer;

    private IEnumerator gameStart()
    {
        var halfTime = this.remainTime / 2;
        yield return new WaitForSeconds(halfTime);
        halfTime = this.remainTime - halfTime;
        // Start fade out
        foreach(var sr in new [] { iconDisplayer, background })
        {
            sr.DOFade(0.1f, halfTime)
                .SetEase(Ease.InBounce);
        }
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            pickable.OnPick(player);
            KillBehaviour(true);
        }
    }
}