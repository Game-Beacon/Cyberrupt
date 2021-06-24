using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BossUI : GameBehaviour
{
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private TextMeshProUGUI bossName;
    [SerializeField]
    private CanvasGroup canvasGroup;
    
    private Enemy boss = null;

    public override void GameStart()
    {
        EnemyManager.instance.OnBossSpawn.AddListener(ShowBossUI);
    }

    public override void GameUpdate()
    {
        if (boss != null)
            healthBar.value = boss.hp / boss.maxHP;
    }

    private void ShowBossUI(Enemy inputBoss)
    {
        boss = inputBoss;
        bossName.text = inputBoss.Name;
        boss.OnDeath += HideBossUI;
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.5f);
        healthBar.value = 1;
    }

    private void HideBossUI()
    {
        boss = null;
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.5f);
    }
}
