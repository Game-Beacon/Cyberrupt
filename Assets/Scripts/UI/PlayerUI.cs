using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : GameBehaviour
{
    private Player player;
    private PlayerUISetting setting;

    //Health and Bomb
    [SerializeField]
    private GameObject playerHpBar;
    private List<Image> hpBar = new List<Image>();
    [SerializeField]
    private GameObject playerBombBar;
    private List<Image> bombBar = new List<Image>();

    //Weapon
    [SerializeField]
    private Image weaponIcon;
    [SerializeField]
    private TextMeshProUGUI weaponName;
    [SerializeField]
    private TextMeshProUGUI weaponAmmo;


    public override void GameStart()
    {
        if(setting == null)
        setting = PlayerUISetting.instance;
        player = DependencyContainer.GetDependency<Player>() as Player;
        
        player.OnHpChange.AddListener(UpdateHp);
        player.weaponController.OnBombCountChange.AddListener(UpdateBomb);
        SetHPAndBomb();

        player.weaponController.OnWeaponChange.AddListener(UpdateWeapon);
        SetWeapon();
    }

    public override void GameUpdate()
    {
        weaponAmmo.text = (player.weaponController.currentWeapon.ammoCount < 0) ? "∞" : player.weaponController.currentWeapon.ammoCount.ToString();
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

    private void SetWeapon()
    {
        weaponIcon.sprite = player.weaponController.currentWeapon.data.icon;
        weaponName.text = player.weaponController.currentWeapon.data.weaponName;
        weaponAmmo.text = (player.weaponController.currentWeapon.ammoCount < 0) ? "∞" : player.weaponController.currentWeapon.ammoCount.ToString();
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

    public void UpdateWeapon(object weaponRaw)
    {
        Weapon weapon = weaponRaw as Weapon;
        weaponIcon.sprite = weapon.data.icon;
        weaponName.text = weapon.data.weaponName;
        weaponAmmo.text = (weapon.ammoCount < 0) ? "∞" : weapon.ammoCount.ToString();
    }
}
