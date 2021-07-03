using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AudioPlayRequester : AudioPlayRequester
{
    private ClipSetting weaponShootSFX = null;
    private ClipSetting weaponChargeSFX = null;
    private ClipSetting switchWeaponSFX = null;

    [SerializeField]
    private int switchWeaponClipIndex;

    private AudioSource weaponChargeAudioSource = null;

    public override void Setup()
    {
        Player player = GetComponent<Player>();

        weaponShootSFX = player.weaponController.currentWeapon.data.shootClip;
        weaponChargeSFX = player.weaponController.currentWeapon.data.chargeClip;
        switchWeaponSFX = group.clips[switchWeaponClipIndex];

        player.weaponController.OnWeaponChange.AddListener(_ => manager.PlaySFXOneShot(switchWeaponSFX));
        player.weaponController.OnWeaponChange.AddListener(x => SwitchWeaponSFX(x));
    }

    private void SwitchWeaponSFX(Weapon newWeapon)
    {
        weaponShootSFX = newWeapon.data.shootClip;
        weaponChargeSFX = newWeapon.data.chargeClip;
    }

    public void PlayWeaponShootSFX()
    {
        manager.PlaySFXOneShot(weaponShootSFX);
    }

    public void StartWeaponChargeSFX()
    {
        if(weaponChargeSFX != null)
            weaponChargeAudioSource = manager.PlaySFXInterruptable(this, weaponChargeSFX);
    }

    public void StopWeaponChargeSFX()
    {
        if (weaponChargeAudioSource != null)
            manager.StopRequesterSingleAudio(this, weaponChargeAudioSource);
    }
}
