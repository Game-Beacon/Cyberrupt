using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class Weapon
{
    private WeaponController controller;
    private WeaponData _data;
    private int _ammoCount;
    private Transform muzzle;
    
    private WeaponCharger charger = null;

    public WeaponData data { get { return _data; } }
    public int ammoCount { get { return _ammoCount; } }

    public Weapon(WeaponController control, WeaponData weaponData, int ammo, Transform m)
    {
        controller = control;
        _data = weaponData;
        _ammoCount = ammo;
        muzzle = m;

        coolDownTime = 1 / _data.frequency;
    }

    private float coolDownTime;
    private float coolDown;
    private float chargeMeter;


    public void OnSelected()
    {
        if (_data.shootType == WeaponShootType.SemiAuto)
            controller.OnKeyDown.AddListener(Shoot);
        if (_data.shootType == WeaponShootType.Auto)
            controller.OnKey.AddListener(Shoot);
        if (_data.shootType == WeaponShootType.Charge)
        {
            controller.OnKey.AddListener(Charge);
            controller.OnKeyUp.AddListener(Discharge);
        }
    }

    public void OnDeselected()
    {
        chargeMeter = 0;

        if (_data.shootType == WeaponShootType.SemiAuto)
            controller.OnKeyDown.RemoveListener(Shoot);
        if (_data.shootType == WeaponShootType.Auto)
            controller.OnKey.RemoveListener(Shoot);
        if (_data.shootType == WeaponShootType.Charge)
        {
            controller.OnKey.RemoveListener(Charge);
            controller.OnKeyUp.RemoveListener(Discharge);
            if (charger != null)
                Object.Destroy(charger.gameObject);
        }
    }

    public void AddAmmo(int count)
    {
        _ammoCount += count;
    }

    public void UpdateTime(float delta)
    {
        coolDown -= delta;
    }

    public void Shoot(float delta)
    {
        if (coolDown > 0)
            return;

        CreateBullets();
        coolDown = coolDownTime;
        _ammoCount--;
        controller.CancelKeyDown();
    }

    public void Charge(float delta)
    {
        if (coolDown > 0)
            return;

        chargeMeter += delta;
        if(charger == null)
            charger = Object.Instantiate(_data.weaponChargeVFX.gameObject, muzzle).GetComponent<WeaponCharger>();
        if(charger != null)
            charger.Charge(Mathf.Clamp01(chargeMeter / _data.chargeTime));
    }

    public void Discharge(float delta)
    {
        if (chargeMeter >= _data.chargeTime)
        {
            CreateBullets();
            coolDown = coolDownTime;
            _ammoCount--;
        }
        if (charger != null)
            Object.Destroy(charger.gameObject);
        controller.CancelKeyUp();
        chargeMeter = 0;
    }

    private void CreateBullets()
    {
        //TODO: Let the data be modified dynamically by the modifiers.
        //For example: a buff that increase the cps and spread, then the data needs to be modified.

        //TODO2: Perhaps this should move to WeaponData (The scriptable object that stores weapon information.)? 
        for (int i = 0; i < _data.cps; i++)
        {
            float angle = muzzle.rotation.eulerAngles.z + Random.Range(-_data.spreadAngle / 2f, _data.spreadAngle / 2f);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            WeaponBullet bullet = Object.Instantiate(_data.bullet, muzzle.position, rotation);

            float multiplier = 1 + Random.Range(-_data.speedRand / 2f, _data.speedRand / 2f);
            bullet.SetBulletProperty(_data.damage, _data.speed * multiplier, direction);
        }
    }
}
