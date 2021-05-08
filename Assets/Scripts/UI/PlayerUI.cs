using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : GameBehaviour
{
    private Player player;
    private PlayerUISetting setting;

    [SerializeField]
    private GameObject playerHpBar;
    private List<Image> hpBar = new List<Image>();
    [SerializeField]
    private GameObject playerBombBar;
    private List<Image> bombBar = new List<Image>();


    public override void GameStart()
    {
        setting = PlayerUISetting.instance;
        player = DependencyContainer.GetDependency<Player>() as Player;
        
        player.OnHpChange.AddListener(UpdateHp);
        player.weaponController.OnBombCountChange.AddListener(UpdateBomb);
        SetHPAndBomb();
    }

    private void SetHPAndBomb()
    {
        for(int i = 0; i < player.maxHp; i++)
        {
            GameObject hp = Instantiate(setting.heartSpriteDisplayer, playerHpBar.transform);
            hpBar.Add(hp.GetComponent<Image>());
            hpBar[i].sprite = setting.heart;
        }
        for (int i = 0; i < player.weaponController.bombCount; i++)
        {
            GameObject bomb = Instantiate(setting.bombSpriteDisplayer, playerBombBar.transform);
            bombBar.Add(bomb.GetComponent<Image>());
            bombBar[i].sprite = setting.bomb;
        }
    }

    public void UpdateHp(int hp)
    {
        for (int i = 0; i < player.maxHp; i++)
            hpBar[i].sprite = (i < hp) ? setting.heart : setting.heartEmpty;
    }

    public void UpdateBomb(int bombCount)
    {
        if(bombCount > bombBar.Count)
        {
            int delta = bombCount - bombBar.Count;
            for(int i = 0; i < delta; i++)
            {
                GameObject bomb = Instantiate(setting.bombSpriteDisplayer, playerBombBar.transform);
                Image bombImage = bomb.GetComponent<Image>();
                bombBar.Add(bombImage);
                bombImage.sprite = setting.bomb;
            }
        }
        else
        {
            int delta = bombBar.Count - bombCount;
            for (int i = 0; i < delta; i++)
            {
                GameObject bomb = bombBar[bombBar.Count - 1].gameObject;
                bombBar.RemoveAt(bombBar.Count - 1);
                Destroy(bomb);
            }
        }
    }
}
