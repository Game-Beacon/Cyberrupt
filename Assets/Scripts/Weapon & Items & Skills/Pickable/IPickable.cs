using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    Sprite icon { get; }

    PickUpType type { get; }

    void OnPick(Player player);
}
