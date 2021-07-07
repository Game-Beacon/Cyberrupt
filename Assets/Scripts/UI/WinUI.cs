using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WinUI : GameBehaviour
{
    [SerializeField]
    private GameObject winUI;
    [SerializeField]
    private Image background;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI killCountText;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float bgFadeTime;
    [SerializeField]
    private float uiFadeTime;
    [SerializeField]
    private float camShakeStrenth;
    [SerializeField]
    private Easing camShakeEase;

    private GameBehaviour[] uiBehaviours;

    public override void GameAwake()
    {
        winUI.SetActive(false);
        background.gameObject.SetActive(false);
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
        StartCoroutine(UIFadeIn());
    }

    IEnumerator UIFadeIn()
    {
        yield return new WaitForSeconds(waitTime);

        CameraController.instance.CamShakeEase(bgFadeTime, camShakeStrenth, camShakeEase, true);
        background.gameObject.SetActive(true);
        background.DOFade(1, bgFadeTime).SetEase(Ease.InSine);
        
        yield return new WaitForSeconds(bgFadeTime);
        
        winUI.SetActive(true);
        TimeManager.PauseGame();

        scoreText.text = GameplayDataManager.instance.score.ToString();
        killCountText.text = GameplayDataManager.instance.killCount.ToString();
        timeText.text = GameplayDataManager.instance.timeInString;

        foreach (GameBehaviour behaviour in uiBehaviours)
            if (behaviour != this)
                behaviour.update = false;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1,uiFadeTime).SetUpdate(true);
    }
}
