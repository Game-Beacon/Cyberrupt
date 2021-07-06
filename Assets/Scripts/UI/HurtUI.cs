using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HurtUI : GameBehaviour
{
    [SerializeField]
    private CanvasGroup group;
    [SerializeField]
    private float fadeTime;

    public override void GameStart()
    {
        Player player = DependencyContainer.GetDependency<Player>() as Player;
        player.OnReceiveDamage += HurtVFX;
    }

    private void HurtVFX()
    {
        group.alpha = 1;
        group.DOFade(0, fadeTime);
    }
}
