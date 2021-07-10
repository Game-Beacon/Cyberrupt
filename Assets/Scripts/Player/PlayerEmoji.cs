using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmoji : GameBehaviour
{
    //This is pretty bare-boned, brutal, and not effective.
    //But since we have only 4 face, it's probably fine.
    [SerializeField]
    private SpriteRenderer face;
    
    [SerializeField]
    private Sprite idle;
    [SerializeField]
    private Sprite statis;
    [SerializeField]
    private Sprite hurt;
    [SerializeField]
    private Sprite dead;

    private Player player;

    private const float stayTime = 3;
    private float stayTimer = 0;
    private Vector2 previousPosition;

    public override void GameStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        player.OnReceiveDamage += () => StartCoroutine(SwitchToHurt());
        player.OnDied.AddListener(SwitchToDead);

        previousPosition = transform.position;

        face.sprite = idle;
    }

    public override void GameUpdate()
    {
        stayTimer += Time.deltaTime;

        if (previousPosition != (Vector2)transform.position)
            stayTimer = 0;

        previousPosition = transform.position;

        if (stayTimer > stayTime)
            face.sprite = statis;
        else
            face.sprite = idle;
    }

    IEnumerator SwitchToHurt()
    {
        face.sprite = hurt;
        stayTimer = 0;
        update = false;
        yield return new WaitForSeconds(player.hurtTime);
        update = true;
        face.sprite = idle;
    }

    void SwitchToDead()
    {
        StopAllCoroutines();
        update = false;
        face.sprite = dead;
    }
}
