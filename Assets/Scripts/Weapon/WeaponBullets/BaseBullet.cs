using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : WeaponBullet
{
    public override void GameStart()
    {
        rb.velocity = direction * speed;
    }

    public override void GameUpdate()
    {
        float currentSpeed = Mathf.Clamp(rb.velocity.magnitude, speed / 1.5f, speed);

        rb.velocity = rb.velocity.normalized * currentSpeed;

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
