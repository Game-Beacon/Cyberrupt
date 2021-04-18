using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : GameBehaviour
{
    private static PropertyManager _instance = null;
    public static PropertyManager instance { get { return _instance; } }

    public override void GameAwake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            KillBehaviour(true);
            return;
        }
    }
}
