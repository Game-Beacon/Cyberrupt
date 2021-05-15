using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInstance : GameBehaviour
{
    [SerializeField]
    private SpriteRenderer iconDisplayer;

    private IPickable pickable = null;
    private float remainTime;
    private float timer = 0;

    private LayerMask playerLayer;

    public override void GameUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= remainTime)
            KillBehaviour(true);
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
        if ((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            pickable.OnPick(player);
            KillBehaviour(true);
        }
    }
}
