using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using DG.Tweening;
using TMPro;

public class BonusInvokeButton : GameBehaviour
{
    [SerializeField]
    private UltEvent OnInvoke = new UltEvent();
    [SerializeField]
    private int invokeThreshold;
    [SerializeField]
    private TextMeshProUGUI showText;
    [SerializeField]
    private List<string> showTextCandidates = new List<string>();
    private int clickCounter = 0;

    public void Click()
    {
        showText.text = showTextCandidates[Mathf.Min(clickCounter, showTextCandidates.Count - 1)];
        clickCounter++;
        transform.DOKill();
        transform.DOShakePosition(0.25f, new Vector3(10f, 10f, 0), 25);
        if (clickCounter == invokeThreshold)
            OnInvoke.Invoke();
    }
}
