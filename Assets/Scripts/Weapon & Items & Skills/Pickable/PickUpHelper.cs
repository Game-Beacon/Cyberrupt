using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PickUpTypeData
{
    public PickUpType type;
    public Color color;
    public ClipSetting clip;
}

[CreateAssetMenu(menuName = "PickUp/PickUp Type Data")]
public class PickUpHelper : SingletonScriptableObject<PickUpHelper>
{
    [SerializeField]
    private List<PickUpTypeData> _data = new List<PickUpTypeData>();

    [SerializeField]
    private PickUpTypeData _defaultData;

    public PickUpTypeData GetData(PickUpType type)
    {
        foreach (PickUpTypeData d in _data)
            if (d.type == type)
                return d;
        return _defaultData;
    }
}
