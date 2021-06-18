using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletModifier : GameBehaviour
{
    public override sealed void GameAwake()
    {
        update = false;
    }

    public abstract void OnHitEnemy(WeaponBullet bullet, Collider2D collision);
    public abstract void OnHitWall(WeaponBullet bullet, RaycastHit2D raycast);
}
