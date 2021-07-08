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
        creditButton.interactable = SaveData.current.completeGame;
    }
}
