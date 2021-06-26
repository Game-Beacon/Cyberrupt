using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : GameBehaviour
{
    [SerializeField]
    protected Sprite _icon;
    /// <summary>
    /// 技能圖示
    /// </summary>
    public Sprite icon { get { return _icon; } private set { } }

    [SerializeField]
    protected string _skillName;
    /// <summary>
    /// 技能名稱
    /// </summary>
    public string skillName { get { return _skillName; } private set { } }

    [SerializeField]
    protected float _skillTime;
    /// <summary>
    /// 技能持續時間
    /// </summary>
    public float skillTime { get { return _skillTime; } private set { } }

    protected Player player;

    public void InjectPlayer(Player p)
    {
        player = p;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 施放技能
    /// </summary>
    public abstract void CastSkill();

    /// <summary>
    /// 技能結束
    /// </summary>
    public abstract void UncastSkill();
}
