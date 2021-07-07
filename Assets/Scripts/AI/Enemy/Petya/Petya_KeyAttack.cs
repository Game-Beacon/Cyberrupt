using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class Petya_KeyAttack : AIState
{
    private Transform target = null;
    private EnemyGroup group = null;
    private ScreenBound screen = null;

    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private float haltTime;

    [Space(20), SerializeField]
    public UltEvent OnWarn = new UltEvent();
    [SerializeField]
    public UltEvent OnAttack = new UltEvent();
    [SerializeField]
    public UltEvent OnFinalAttack = new UltEvent();

    [Space(20), SerializeField]
    private GameObject projectile;
    [SerializeField]
    private int attackCount;
    [SerializeField]
    private int projectileCount;
    [SerializeField]
    private float attackWarnTime;
    [SerializeField]
    private float attackHaltTime;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        if (target == null)
            target = GetComponent<ITarget>().target;
        if (group == null)
            group = GetComponent<EnemyGroup>();
        if (screen == null)
            screen = ScreenBound.instance;

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        group.StopParticles();
        group.enableColliders = false;
        DOTween.To(() => group.alpha, x => group.alpha = x, 0, fadeTime);
        yield return new WaitForSeconds(fadeTime + haltTime);
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        for(int i = 0; i < attackCount; i ++)
        {
            Projectile[] projectiles = new Projectile[projectileCount];
            for(int j = 0; j < projectileCount; j ++)
            {
                projectiles[j] = Instantiate(projectile).GetComponent<Projectile>();
                projectiles[j].transform.position = screen.GetRandomPointOnScreenEdge();

                Vector2 direction = target.position - projectiles[j].transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectiles[j].transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            OnWarn.Invoke();
            yield return new WaitForSeconds(attackWarnTime);

            OnAttack.Invoke();
            for (int j = 0; j < projectileCount; j++)
                projectiles[j].SpawnProjectile();
            
            yield return new WaitForSeconds(attackHaltTime);
        }

        Projectile[] final = new Projectile[8];
        for (int j = 0; j < 8; j++)
        {
            final[j] = Instantiate(projectile).GetComponent<Projectile>();
            final[j].transform.position = transform.position;
            float angle = 45 * j;
            final[j].transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        OnWarn.Invoke();
        yield return new WaitForSeconds(attackWarnTime);

        OnFinalAttack.Invoke();
        for (int j = 0; j < 8; j++)
            final[j].SpawnProjectile();

        DOTween.To(() => group.alpha, x => group.alpha = x, 1, attackWarnTime);
        group.enableColliders = true;

        yield return new WaitForSeconds(attackWarnTime);
        group.StartParticles();

        SelfEndState();
    }
}
