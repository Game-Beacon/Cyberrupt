using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class DanmakuBulletData : ScriptableObject
{
    //子彈的圖案
    [SerializeField]
    private Sprite _sprite;
    public Sprite sprite { get { return _sprite; } private set { } }

    [SerializeField]
    private Sprite _rageSprite;
    public Sprite rageSprite { get { return _rageSprite; } private set { } }

    //子彈的半徑（一律假設子彈是圓形，碰撞處理比較方便）
    [SerializeField, Range(0f,2f)]
    private float _radius;
    public float radius { get { return _radius; } }
}
