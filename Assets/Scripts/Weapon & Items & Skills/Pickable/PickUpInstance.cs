using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PickUpInstance : GameBehaviour
{
    [SerializeField]
    ParticleSystem ps;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private SpriteRenderer iconDisplayer;
    [SerializeField]
    private SpriteRenderer background;
    [SerializeField]
    private Image countDownBG;
    [SerializeField]
    private Image countDown;
    private SpriteRenderer[] srs => new [] { iconDisplayer, background };

    private IPickable pickable = null;
    private float remainTime;

    //private LayerMask playerLayer;

    private void clearTween()
    {
        // TODO: Move this call to a resonable location,
        //  the transform tween is not created inside this
        //  component, but there does not exist anyway to
        //  get notification on destroy (or kill) event now.
        transform.DOKill();
        foreach(var sr in this.srs)
            sr.DOKill();
    }

    private IEnumerator gameStart()
    {
        float halfTime = remainTime / 2;
        countDown.DOFillAmount(0, remainTime).SetEase(Ease.Linear);

        yield return new WaitForSeconds(halfTime);

        float timer = 0;
        float nextTimer = 0;
        float tick = 0.4f;
        bool light = false;

        while(timer < halfTime)
        {
            if(timer >= nextTimer)
            {
                tick = Mathf.Max(tick - 0.025f, 0.05f);
                nextTimer += tick;
                countDown.color = new Color(countDown.color.r, countDown.color.g, countDown.color.b, (light) ? 1 : 0);
                foreach (SpriteRenderer sr in srs)
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (light) ? 1 : 0.3f);
                light = !light;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        countDown.color = new Color(countDown.color.r, countDown.color.g, countDown.color.b);
        foreach (SpriteRenderer sr in srs)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b);

        countDown.DOFade(0, 0.15f);
        countDownBG.DOFade(0, 0.15f);
        foreach (SpriteRenderer sr in srs)
            sr.DOFade(0, 0.15f);

        yield return new WaitForSeconds(0.15f);

        KillBehaviour(true);
    }

    public override void GameStart()
    {
        rb.velocity = Random.insideUnitCircle.normalized * 2f;
        StartCoroutine(this.gameStart());
    }

    public void InjectData(IPickable data, float remain)
    {
        pickable = data;
        iconDisplayer.sprite = pickable.icon;
        remainTime = remain;

        PickUpTypeData d = PickUpHelper.instance.GetData(data.type);
        background.color = d.color;

        var main = ps.main;
        main.startColor = d.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((1 << collision.gameObject.layer & CollisionLayer.instance.playerMask) != 0)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            AudioManager.instance.PlaySFXOneShot(PickUpHelper.instance.GetData(pickable.type).clip);
            pickable.OnPick(player);

            ps.transform.SetParent(null);
            ps.Play();

            // Ensure that tweeners are killed
            this.clearTween();
            KillBehaviour(true);
        }
    }
}