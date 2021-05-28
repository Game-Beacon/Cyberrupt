using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUp/Weapon PickUp")]
public class WeaponPickUp : ScriptableObject, IPickable
{
    [SerializeField]
    private WeaponData weapon;
    [SerializeField]
    private int ammoCount;
    Sprite IPickable.icon { get { return weapon.icon; } }

    public void OnPick(Player player)
    {
        player.weaponController.AddWeapon(weapon, ammoCount);
    }
}
