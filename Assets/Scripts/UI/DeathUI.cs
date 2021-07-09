using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DeathUI : GameBehaviour
{
    [SerializeField]
    private GameObject deathUI;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI killCountText;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Space(10), SerializeField]
    private GameObject killHide;
    [SerializeField]
    private float shakeTime;
    [SerializeField]
    private float uiFadeTime;

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
        player.OnDied.AddListener(Death);

        uiBehaviours = GetComponents<GameBehaviour>();
    }

    public void Death()
    {
        deathUI.SetActive(true);
        StartCoroutine(OpenDeathUI());
    }

    IEnumerator OpenDeathUI()
    {
        Instantiate(killHide, new Vector3(0, 0, -1), Quaternion.identity);
        AudioManager.instance.StopAllAudio();
        CameraController.instance.CamShake(shakeTime, 0.3f, true, true);

        yield return new WaitForSecondsRealtime(shakeTime);

        TimeManager.PauseGame();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        scoreText.text = GameplayDataManager.instance.score.ToString();
        killCountText.text = GameplayDataManager.instance.killCount.ToString();
        timeText.text = GameplayDataManager.instance.timeInString;

        foreach (GameBehaviour behaviour in uiBehaviours)
            if (behaviour != this)
                behaviour.update = false;

        canvasGroup.DOFade(1, uiFadeTime).SetUpdate(true);
    }
}
