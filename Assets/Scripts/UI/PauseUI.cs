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

    public override void GameAwake()
    {
        pauseUI.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
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
        Time.timeScale = (isUIEnable) ? 0 : 1;
    }

    public void ClosePauseUI()
    {
        isUIEnable = false;
        SetPauseUI();
    }
}
