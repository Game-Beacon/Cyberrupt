using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUp/Bomb PickUp")]
public class BombPickUp : ScriptableObject, IPickable
{
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private int count;
    Sprite IPickable.icon { get { return _icon; } }

    public void OnPick(Player player)
    {
        player.weaponController.AddBomb(count);
    }
}
