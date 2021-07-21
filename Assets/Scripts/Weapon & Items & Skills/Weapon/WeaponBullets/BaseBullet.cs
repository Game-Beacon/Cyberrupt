using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : WeaponBullet
{
    public override void GameStart()
    {
        _rb.velocity = direction * speed;
    }

    protected override void BulletUpdate()
    {
        _rb.velocity = _rb.velocity.normalized * Mathf.Clamp(_rb.velocity.magnitude, 0.6f * speed, 1.2f * speed);

        float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
