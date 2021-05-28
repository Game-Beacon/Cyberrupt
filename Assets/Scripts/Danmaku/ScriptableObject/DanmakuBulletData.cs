using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Danmaku/Bullet Data")]
[System.Serializable]
public class DanmakuBulletData : ScriptableObject
{
    //子彈的圖案
    [SerializeField]
    private List<Sprite> _sprites;
    public List<Sprite> sprites { get { return _sprites; } private set { } }

    [SerializeField]
    private int _frame;
    public int frame { get { return _frame; } }

    //子彈的半徑（一律假設子彈是圓形，碰撞處理比較方便）
    [SerializeField, Range(0f,2f)]
    private float _radius;
    public float radius { get { return _radius; } }
}
