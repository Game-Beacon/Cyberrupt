using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current = null;
    public static SaveData current
    {
        get
        {
            if (_current == null)
                _current = new SaveData();
            return _current;
        }
    }

    public static void OverwriteSaveData(SaveData newData)
    {
        if (newData != null)
            _current = newData;
    }

    public bool completeGame = false;
    public int score = 0;
}
