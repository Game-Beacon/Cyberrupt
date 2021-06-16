using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : WeaponBullet
{
    public override void GameStart()
    {
        _rb.velocity = direction * speed;
    }

    public override void GameUpdate()
    {
        float currentSpeed = Mathf.Clamp(_rb.velocity.magnitude, speed / 1.5f, speed);

        _rb.velocity = _rb.velocity.normalized * currentSpeed;

        float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
