using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Singularity_ForceField : GameBehaviour
{
    [SerializeField]
    private float size;
    [SerializeField]
    private float expandTime;
    [SerializeField]
    private LayerMask weaponLayer;
    [SerializeField]
    private LayerMask weakenLayer;

    private int weapon;
    private int weaken;

    public override void GameAwake()
    {
        int weaponMask = weaponLayer.value;
        int weakenMask = weakenLayer.value;
        weapon = -1;
        weaken = -1;

        while(weaponMask > 0)
        {
            weaponMask = weaponMask >> 1;
            weapon++;
        }

        while(weakenMask > 0)
        {
            weakenMask = weakenMask >> 1;
            weaken++;
        }

        Vector3 finalVector = new Vector3(size,size,1);
        transform.DOScale(finalVector, expandTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & weaponLayer) != 0)
            collision.gameObject.layer = weaken;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & weakenLayer) != 0)
            collision.gameObject.layer = weapon;
    }
}
