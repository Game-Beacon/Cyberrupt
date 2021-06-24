using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Berserk : Skill
{
    public override void CastSkill()
    {
        player.weaponController.timeScale = 2;
    }

    public override void UncastSkill()
    {
        player.weaponController.timeScale = 1;
    }
}
