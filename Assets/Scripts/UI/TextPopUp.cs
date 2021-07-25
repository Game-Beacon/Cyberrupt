using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[ExecuteInEditMode]
public class TextPopUp : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private CanvasGroup group;

    // Update is called once per frame
    void Update()
    {
        rect.sizeDelta = new Vector2(text.preferredWidth + 50 , text.preferredHeight + 20);
    }

    public void OnHover()
    {
        group.DOFade(1, 0.15f);
    }

    public void OnHoverExit()
    {
        group.DOFade(0, 0.15f);
    }
}
