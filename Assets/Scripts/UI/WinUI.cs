using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinUI : GameBehaviour
{
    [SerializeField]
    private GameObject winUI;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI killCountText;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private GameBehaviour[] uiBehaviours;

    public override void GameAwake()
    {
        winUI.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public override void GameStart()
    {
        EnemyManager.instance.OnGameClear.AddListener(OpenWinUI);

        uiBehaviours = GetComponents<GameBehaviour>();
    }

    private void OpenWinUI()
    {
        winUI.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        scoreText.text = GameplayDataManager.instance.score.ToString();
        killCountText.text = GameplayDataManager.instance.killCount.ToString();
        timeText.text = GameplayDataManager.instance.timeInString;

        foreach (GameBehaviour behaviour in uiBehaviours)
            if (behaviour != this)
                behaviour.update = false;

        TimeManager.PauseGame();
    }
}
