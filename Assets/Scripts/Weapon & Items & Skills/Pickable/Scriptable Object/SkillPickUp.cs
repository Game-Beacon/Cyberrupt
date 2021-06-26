using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUp/Skill PickUp")]
public class SkillPickUp : ScriptableObject, IPickable
{
    [SerializeField]
    private Skill skill;
    Sprite IPickable.icon { get { return skill.icon; } }
    
    public void OnPick(Player player)
    {
        Skill newSkill = Instantiate(skill.gameObject, player.transform).GetComponent<Skill>();
        newSkill.InjectPlayer(player);
        player.skillController.AddSkill(newSkill);
    }
}
