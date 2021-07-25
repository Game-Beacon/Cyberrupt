using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleUI : GameBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private Button creditButton;

    public override void GameStart()
    {
        highScoreText.text = SaveData.current.score.ToString();

        var colors = creditButton.colors;
        colors.normalColor = (SaveData.current.completeGame) ? Color.yellow : colors.normalColor;
        colors.highlightedColor = (SaveData.current.completeGame) ? Color.yellow * 0.9f : colors.highlightedColor;
        colors.pressedColor = (SaveData.current.completeGame) ? Color.yellow * 0.8f : colors.pressedColor;
        creditButton.colors = colors;

        //creditButton.interactable = SaveData.current.completeGame;
    }
}
