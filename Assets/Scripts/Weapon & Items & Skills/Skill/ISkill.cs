using System;

public interface ISkill
{
    /// <summary>
    /// 技能名稱
    /// </summary>
    string skillName { get; }
    
    /// <summary>
    /// 技能持續時間
    /// </summary>
    float skillTime { get; }

    /// <summary>
    /// 施放技能
    /// </summary>
    void CastSkill();

    /// <summary>
    /// 技能結束
    /// </summary>
    void UncastSkill();
}
