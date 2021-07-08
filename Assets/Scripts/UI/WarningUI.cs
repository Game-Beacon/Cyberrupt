using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UltEvents;

public class WarningUI : GameBehaviour
{
    [SerializeField]
    private CanvasGroup group;
    [SerializeField]
    private RectTransform barUp, barDown;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float warnTime;
    [SerializeField]
    private int warnCount;
    [SerializeField]
    public UltEvent OnWarn = new UltEvent();
    [SerializeField]
    public UltEvent OnWarningDone = new UltEvent();

    public override void GameAwake()
    {
        DependencyContainer.AddDependency(this);
    }

    public void StartWarning()
    {
        StartCoroutine(DoWarning());
    }

    IEnumerator DoWarning()
    {
        float moveValue = speed * warnTime * warnCount;

        barUp.DOLocalMoveX(barUp.transform.localPosition.x - moveValue, warnTime * warnCount).SetEase(Ease.Linear);
        barDown.DOLocalMoveX(barDown.transform.localPosition.x - moveValue, warnTime * warnCount).SetEase(Ease.Linear);

        for (int i = 0; i < warnCount; i++)
        {
            OnWarn.Invoke();
            group.DOFade(1, warnTime / 2).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(warnTime / 2);
            group.DOFade(0, warnTime / 2).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(warnTime / 2);
        }

        barUp.localPosition = new Vector2(barUp.transform.localPosition.x + moveValue, barUp.transform.localPosition.y);
        barDown.localPosition = new Vector2(barDown.transform.localPosition.x + moveValue, barDown.transform.localPosition.y);

        OnWarningDone.Invoke();
        OnWarningDone.Clear();
    }
}
