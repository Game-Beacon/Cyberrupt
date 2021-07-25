using System.Collections;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HeartBleedAnimate : MonoBehaviour
{
    [SerializeField]
    private int beatCount;
    [SerializeField]
    private float lastBeatDuration;
    [SerializeField]
    private GameObject burst;
    [SerializeField]
    private GameObject dirBurst;
    [SerializeField]
    private float burstRadius;

    void Start()
    {
        // Register death animation
        GetComponent<HeartBleed>().Death
            .AddEnumerator(this.playDeathAnimation());
    }

    private IEnumerator playDeathAnimation()
    {
        // Disable attack
        GetComponent<AIStateMachine>().update = false;
        GetComponent<EnemyGroup>().enableColliders = false;
        GetComponent<SpawnDanmakuHelper>().KillAll();
        // Move to center
        yield return transform.DOMove(Vector3.zero, 1).WaitForCompletion();
        for(int i = 0; i < this.beatCount; i++)
        {
            CameraController.instance.CamShake(0.3f, 0.4f);
            var nextPoint = Random.onUnitSphere * Random.Range(0.5f, 1);
            nextPoint.z = transform.position.z;
            var revPoint = transform.position - (nextPoint - transform.position).normalized * this.burstRadius;
            var angle = Vector3.SignedAngle(
                Vector3.right,
                revPoint - transform.position,
                Vector3.forward
            );
            Instantiate(this.dirBurst, revPoint, Quaternion.Euler(0, 0, angle));
            yield return transform.DOMove(nextPoint, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .WaitForCompletion();
            yield return new WaitForSeconds(0.2f);
        }
        var step = 15;
        for(int i = 0; i < 360; i += step)
        {
            foreach(var angle in new [] { i, i + 180 })
            {
                var spawnPoint = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad),
                    transform.position.z) * this.burstRadius;
                Instantiate(this.dirBurst, spawnPoint, Quaternion.Euler(0, 0, angle));
            }
            yield return new WaitForSeconds(0.1f);
        }
        CameraController.instance.CamShake(0.6f, 0.4f);
        yield return transform.DOShakePosition(
                this.lastBeatDuration,
                vibrato : 15,
                fadeOut : true)
            .WaitForCompletion();
        yield return new WaitForSeconds(0.3f);
        // Big burst
        Instantiate(this.burst, transform.position, Quaternion.identity).GetComponent<ParticleSystem>()?.Play();
        // Start death animation clip
        var clip = "Dead";
        var anim = GetComponent<Animation>();
        anim.Play(clip);
        CameraController.instance.CamShake(1, 0.4f);
        yield return new WaitForSeconds(1);
        // Sprite fade
        foreach(var sp in GetComponentsInChildren<SpriteRenderer>())
        {
            var t = sp.DOFade(0, 1.5f);
            // Ensure tweener will be released
            sp.OnDestroyAsObservable()
                .Subscribe(_ => t.Kill())
                .AddTo(gameObject);
        }
        // Wait for animation clip
        while(anim.IsPlaying(clip))
        {
            yield return null;
        }
        CameraController.instance.CamShake(1, 0.8f);
    }
}