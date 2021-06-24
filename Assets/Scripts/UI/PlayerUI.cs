using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    //Score
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI multiplerText;
    [SerializeField]
    private Slider multiplierSlider;

    private float scoreMultipler = 1;
    private float multiplierExp = 0;
    private const float multiplierThreshold = 50;
    private int[] thresholdAmp = { 1, 2, 4, 8, 16, 48 };

    private int score = 0;

    //Weapon
    [SerializeField]
    private Image weaponIcon;
    [SerializeField]
    private TextMeshProUGUI weaponName;
    [SerializeField]
    private TextMeshProUGUI weaponAmmo;

    //Skill
    [SerializeField]
    private Image skillIconFront;
    [SerializeField]
    private Image skillIconBack;
    [SerializeField]
    private TextMeshProUGUI skillName;
    [SerializeField]
    private GameObject playerSkillList;
    private SkillController playerSkillController;
    private List<Image> skillList = new List<Image>();
    private bool skillIsActive = false;


    //Wave
    [SerializeField]
    private TextMeshProUGUI waveText;

    public override void GameStart()
    {
        if(setting == null)
        setting = PlayerUISetting.instance;
        player = DependencyContainer.GetDependency<Player>() as Player;

        //Health and Bomb
        player.OnHpChange.AddListener(UpdateHp);
        player.weaponController.OnBombCountChange.AddListener(UpdateBomb);
        SetHPAndBomb();

        //Score
        EnemyManager.instance.OnEnemyDied.AddListener(UpdateScore);
        player.OnReceiveDamage.AddListener(ResetMultiplier);
        EnemyManager.instance.OnEnemySpawned.AddListener(SubscribeEnemy);
        EnemyManager.instance.OnEnemyDied.AddListener(UnsubscribeEnemy);

        //Weapon
        player.weaponController.OnWeaponChange.AddListener(UpdateWeapon);
        SetWeapon();

        //Skill
        playerSkillController = player.skillController;
        playerSkillController.OnUpdateSkillList.AddListener(UpdateSkillList);
        playerSkillController.OnActiveSkill.AddListener(SkillActive);
        playerSkillController.OnDeactiveSkill.AddListener(SkillDeactive);

        //Wave
        EnemyManager.instance.OnWaveAdvance.AddListener(SetWaveText);
    }

    public override void GameUpdate()
    {
        UpdateAmmoText();
        if (skillIsActive)
            UpdateSkillProgress();
    }

    //Health and Bomb
    private void SetHPAndBomb()
    {
        for(int i = 0; i < player.hp; i++)
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
        while(hpBar.Count != player.hp)
        {
            if (hpBar.Count > player.hp)
            {
                Destroy(hpBar[hpBar.Count - 1].gameObject);
                hpBar.RemoveAt(hpBar.Count - 1);
            }  
            else
            {
                GameObject newHp = Instantiate(setting.heartSpriteDisplayer, playerHpBar.transform);
                hpBar.Add(newHp.GetComponent<Image>());
            }
        }

        for (int i = 0; i < player.hp; i++)
            hpBar[i].sprite = setting.heart;
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

    //Score
    public void UpdateScore(Enemy enemy)
    {
        score += (int)(enemy.score * scoreMultipler);
        string scoreString = score.ToString().PadLeft(8, '0');
        scoreText.text = scoreString;
    }

    public void UpdateMultiplier(float exp)
    {
        multiplierExp += exp;

        int level = 0;
        float trueThreshold = multiplierThreshold * thresholdAmp[level];
        while (multiplierExp >= trueThreshold)
        {
            level++;
            if (level == thresholdAmp.Length)
                break;
            trueThreshold = multiplierThreshold * thresholdAmp[level];
        }

        scoreMultipler = 1 + 0.5f * level;
        multiplierSlider.value = Mathf.Clamp01(multiplierExp / trueThreshold);
        multiplerText.text = "x" + scoreMultipler.ToString();
    }

    public void ResetMultiplier()
    {
        multiplierExp = 0;
        UpdateMultiplier(0);
    }

    public void SubscribeEnemy(Enemy enemy)
    {
        enemy.OnReceiveDamage.AddListener(UpdateMultiplier);
    }

    public void UnsubscribeEnemy(Enemy enemy)
    {
        enemy.OnReceiveDamage.RemoveListener(UpdateMultiplier);
    }

    //Weapon
    public void UpdateWeapon(Weapon weapon)
    {
        weaponIcon.sprite = weapon.data.icon;
        weaponName.text = weapon.data.weaponName;
        weaponAmmo.text = (weapon.ammoCount < 0) ? "∞" : weapon.ammoCount.ToString();
    }

    private void UpdateAmmoText()
    {
        weaponAmmo.text = (player.weaponController.currentWeapon.ammoCount < 0) ? "∞" : player.weaponController.currentWeapon.ammoCount.ToString();
    }

    //Skill
    public void UpdateSkillList(List<Skill> skills)
    {
        if(skills.Count == 0)
        {
            skillIconFront.color = Color.clear;
            skillIconBack.color = Color.clear;
            skillName.text = "";
        }
        else
        {
            skillIconFront.color = Color.white;
            skillIconBack.color = new Color(1, 1, 1, 0.25f);
            skillIconFront.sprite = skills[0].icon;
            skillIconBack.sprite = skills[0].icon;
            skillName.text = skills[0].skillName;
        }

        if (skills.Count - 1 > skillList.Count)
        {
            int delta = skills.Count - 1 - skillList.Count;
            for (int i = 0; i < delta; i++)
            {
                GameObject newSkill = new GameObject();
                newSkill.transform.SetParent(playerSkillList.transform);
                newSkill.transform.localScale = new Vector3(1, 1, 1);
                Image newSkillImage = newSkill.AddComponent<Image>();
                skillList.Add(newSkillImage);
            }
        }
        else
        {
            int delta = skillList.Count - (skills.Count - 1);
            for (int i = 0; i < delta; i++)
            {
                if (skillList.Count == 0)
                    break;
                Destroy(skillList[skillList.Count - 1].gameObject);
                skillList.RemoveAt(skillList.Count - 1);
            }  
        }

        for (int i = 0; i < skills.Count - 1; i++)
            skillList[i].sprite = skills[i + 1].icon;
    }

    public void SkillActive()
    {
        skillIsActive = true;
    }

    public void SkillDeactive()
    {
        skillIsActive = false;
        skillIconFront.fillAmount = 1;
    }

    private void UpdateSkillProgress()
    {
        skillIconFront.fillAmount = 1 - playerSkillController.currentSkillTask.progress;
    }

    //Wave
    public void SetWaveText(int wave)
    {
        waveText.text = "Wave " + wave.ToString();
        waveText.color = Color.white;
        StartCoroutine(WaveTextFade());
    }

    private IEnumerator WaveTextFade()
    {
        yield return new WaitForSeconds(1f);
        waveText.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        waveText.text = "";
    }
}
