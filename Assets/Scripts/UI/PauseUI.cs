using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : GameBehaviour
{
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private CanvasGroup canvasGroup;
    
    private bool isUIEnable = false;

    //These are for god mode, should be removed on the final build.
    [SerializeField]
    private Button godModeButton;
    private Player player;

    public override void GameAwake()
    {
        pauseUI.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public override void GameStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        godModeButton.onClick.AddListener(() =>
        {
            player.SetInvulnerability(!player.isInvulnerable);
            ColorBlock block = godModeButton.colors;
            block.normalColor = (player.isInvulnerable) ? Color.yellow : Color.gray;
            block.highlightedColor = block.normalColor;
            block.selectedColor = block.normalColor;
            godModeButton.colors = block;
        });
    }

    public override void GameUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isUIEnable = !isUIEnable;
            SetPauseUI();
        }
    }

    private void SetPauseUI()
    {
        pauseUI.SetActive(isUIEnable);
        canvasGroup.alpha = (isUIEnable)? 1 : 0;
        canvasGroup.blocksRaycasts = isUIEnable;
        canvasGroup.interactable = isUIEnable;
        if (TimeManager.paused)
            TimeManager.UnpauseGame();
        else
            TimeManager.PauseGame();
    }

    /*public void ClosePauseUI()
    {
        isUIEnable = false;
        SetPauseUI();
    }*/
}
