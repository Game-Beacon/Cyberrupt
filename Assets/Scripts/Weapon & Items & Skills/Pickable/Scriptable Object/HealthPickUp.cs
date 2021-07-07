using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUp/Health PickUp")]
public class HealthPickUp : ScriptableObject, IPickable
{
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private int heal;
    Sprite IPickable.icon { get { return _icon; } }

    PickUpType IPickable.type { get { return PickUpType.Other; } }

    public void OnPick(Player player)
    {
        player.AddHp(heal);    
    }
}
