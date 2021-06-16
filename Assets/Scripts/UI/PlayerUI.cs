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

        //Wave
        EnemyManager.instance.OnWaveAdvance.AddListener(SetWaveText);
    }

    public override void GameUpdate()
    {
        weaponAmmo.text = (player.weaponController.currentWeapon.ammoCount < 0) ? "∞" : player.weaponController.currentWeapon.ammoCount.ToString();
    }

    //Health and Bomb
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
