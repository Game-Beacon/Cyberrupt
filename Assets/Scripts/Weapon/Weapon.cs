using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class Weapon
{
    private WeaponController controller;
    private WeaponData data;
    private int ammoCount;
    private Transform muzzle;

    public Weapon(WeaponController control, WeaponData weaponData, int ammo, Transform m)
    {
        controller = control;
        data = weaponData;
        ammoCount = ammo;
        muzzle = m;

        coolDownTime = 1 / data.frequency;
    }

    private float timer;
    private float coolDownTime;
    private float coolDown;
    private float chargeMeter;

    public void OnSelected()
    {
        timer = controller.timer;

        if (data.shootType == WeaponShootType.SemiAuto)
            controller.OnKeyDown.AddListener(Shoot);
        if (data.shootType == WeaponShootType.Auto)
            controller.OnKey.AddListener(Shoot);
        if (data.shootType == WeaponShootType.Charge)
        {
            controller.OnKey.AddListener(Charge);
            controller.OnKeyUp.AddListener(Shoot);
        }
    }

    public void OnDeselected()
    {
        chargeMeter = 0;

        if (data.shootType == WeaponShootType.SemiAuto)
            controller.OnKeyDown.RemoveListener(Shoot);
        if (data.shootType == WeaponShootType.Auto)
            controller.OnKey.RemoveListener(Shoot);
        if (data.shootType == WeaponShootType.Charge)
        {
            controller.OnKey.RemoveListener(Charge);
            controller.OnKeyUp.RemoveListener(Shoot);
        }
    }

    public void AddAmmo(int count)
    {
        ammoCount += count;
    }

    private void Shoot()
    {
        float delta = controller.timer - timer;
        timer = controller.timer;
        coolDown -= delta;

        if (coolDown > 0)
            return;

        if (data.shootType == WeaponShootType.SemiAuto || data.shootType == WeaponShootType.Auto)
        {
            CreateBullets();
            coolDown = coolDownTime;
            ammoCount--;
        }

        if(data.shootType == WeaponShootType.Charge)
        {
            if (chargeMeter >= data.chargeTime)
            {
                CreateBullets();
                coolDown = coolDownTime;
                ammoCount--;
            }
            chargeMeter = 0;
        }
    }

    private void Charge()
    {
        float delta = controller.timer - timer;
        timer = controller.timer;
        coolDown -= delta;
        chargeMeter += delta;
    }

    private void CreateBullets()
    {
        //TODO: Let the data be modified dynamically by the modifiers.
        //For example: a buff that increase the cps and spread, then the data needs to be modified.
        for(int i = 0; i < data.cps; i++)
        {
            float angle = muzzle.rotation.eulerAngles.z + Random.Range(-data.spreadAngle / 2f, data.spreadAngle / 2f);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            WeaponBullet bullet = Object.Instantiate(data.bullet, muzzle.position, rotation);

            float multiplier = 1 + Random.Range(-data.speedRand / 2f, data.speedRand / 2f);
            bullet.SetBulletProperty(data.damage, data.speed * multiplier, direction);
        }
    }
}
