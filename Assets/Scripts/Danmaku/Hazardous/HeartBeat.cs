using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class HeartBeat : GameBehaviour
{
    [SerializeField]
    private float warnTime;

    [SerializeField]
    private float keepTime;

    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private Color warnColor;

    private Vector3 scale;

    [SerializeField]
    private Collider2D trigger;

    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private UltEvent OnHeartBeat = new UltEvent();

    public override void GameAwake()
    {
        trigger.enabled = false;
        sr.color = new Color(warnColor.r, warnColor.g, warnColor.b, 0);
        scale = transform.localScale;
        transform.localScale = scale * 0.6f;
    }

    public override void GameStart()
    {
        StartCoroutine(HeartBeatEvent());
    }

    IEnumerator HeartBeatEvent()
    {
        transform.DOShakePosition(warnTime, 0.1f, 50, 90, false, false).SetEase(Ease.Linear);
        transform.DOScale(scale, warnTime);
        sr.DOFade(0.75f, warnTime);
        yield return new WaitForSeconds(warnTime);

        transform.localScale = scale * 1.2f;

        sr.color = Color.white;
        sr.DOColor(warnColor, keepTime);
        transform.DOScale(scale, keepTime);

        trigger.enabled = true;
        OnHeartBeat.Invoke();

        yield return new WaitForSeconds(keepTime);
        
        trigger.enabled = false;
        sr.DOFade(0, fadeTime);

        yield return new WaitForSeconds(fadeTime);
        
        KillBehaviour(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
            player.OnHit();
    }
}
