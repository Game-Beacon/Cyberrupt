using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditUI : GameBehaviour
{
    [SerializeField]
    private List<CanvasGroup> icons = new List<CanvasGroup>();

    public override void GameStart()
    {
        foreach(CanvasGroup c in icons)
        {
            c.alpha = (SaveData.current.completeGame) ? 1 : 0.75f;
            c.blocksRaycasts = SaveData.current.completeGame;
            c.interactable = SaveData.current.completeGame;
        }  
    }
}
