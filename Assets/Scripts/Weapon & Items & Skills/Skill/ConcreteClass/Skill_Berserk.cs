using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Berserk : Skill
{
    [SerializeField]
    private ParticleSystem berserkParticle;
    private ParticleSystem playerBerserkParticle;

    public override void CastSkill()
    {
        player.weaponController.timeScale = 2;
        playerBerserkParticle = Instantiate(berserkParticle, player.transform);
        playerBerserkParticle.gameObject.SetActive(true);
    }

    public override void UncastSkill()
    {
        player.weaponController.timeScale = 1;
        playerBerserkParticle.Stop();
    }
}
