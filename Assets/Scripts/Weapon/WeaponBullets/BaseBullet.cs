using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : WeaponBullet
{
    public override void GameUpdate()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
}
