using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    Sprite icon { get; }

    void OnPick(Player player);
}
