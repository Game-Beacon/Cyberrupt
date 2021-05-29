using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : GameBehaviour
{
    [SerializeField]
    private GameObject deathUI;
    [SerializeField]
    private CanvasGroup canvasGroup;
    private Player player;

    private GameBehaviour[] uiBehaviours;

    public override void GameAwake()
    {
        deathUI.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public override void GameStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        player.OnDied.AddListener(OpenDeathUI);

        uiBehaviours = GetComponents<GameBehaviour>();
    }

    private void OpenDeathUI()
    {
        deathUI.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        foreach (GameBehaviour behaviour in uiBehaviours)
            if (behaviour != this)
                behaviour.update = false;

        Time.timeScale = 0;
    }
}
