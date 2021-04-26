using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerUI : GameBehaviour
{
    //Place holder，之後等UI的圖上傳之後要再修改
    [SerializeField]
    private TextMeshProUGUI hpText;

    private Player player;

    public override void GameStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        player.OnHpChange.AddListener(UpdateHp);
        UpdateHp(player.hp);
    }

    public void UpdateHp(int hp)
    {
        hpText.text = "HP : " + hp.ToString();
    }
}
