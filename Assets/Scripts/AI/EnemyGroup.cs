using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyGroup : GameBehaviour
{
    List<SpriteRenderer> srs = new List<SpriteRenderer>();
    List<Collider2D> colliders = new List<Collider2D>();
    [SerializeField]
    List<ParticleSystem> pss = new List<ParticleSystem>();

    [SerializeField, Range(0, 1f)]
    public float alpha = 1;
    [SerializeField]
    public bool enableColliders = true;

    private static EnemyMaterialSetting materials = null;
    private Coroutine hurtCoroutine = null;

    public override void GameAwake()
    {
        if (materials == null)
            materials = EnemyMaterialSetting.instance;

        srs = GetComponentsInChildren<SpriteRenderer>().ToList();
        colliders = GetComponentsInChildren<Collider2D>().ToList();

        GetComponent<Enemy>().OnHit += () => HurtVFX();
    }

    public override void GameUpdate()
    {
        foreach (SpriteRenderer sr in srs)
            if (sr != null)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        foreach (Collider2D collider in colliders)
            if(collider != null)
                collider.enabled = enableColliders;
    }

    public void StopParticles()
    {
        foreach (ParticleSystem ps in pss)
            ps.Stop();
    }

    public void StartParticles()
    {
        foreach (ParticleSystem ps in pss)
            ps.Play();
    }

    public void HurtVFX()
    {
        hurtCoroutine = StartCoroutine(ChangeMaterialOnHurt());
    }

    IEnumerator ChangeMaterialOnHurt()
    {
        Vector2[] rands = new Vector2[srs.Count];

        for(int i = 0; i < srs.Count; i ++)
        {
            SpriteRenderer sr = srs[i];
            if (sr != null)
            {
                sr.material = materials.hurtMaterial;
                Vector2 rand = Random.insideUnitCircle.normalized * 0.03f;
                sr.transform.position += (Vector3)rand;
                rands[i] = rand;
            }
        }
        
        yield return new WaitForSeconds(0.02f);

        for (int i = 0; i < srs.Count; i++)
        {
            SpriteRenderer sr = srs[i];
            if (sr != null)
            {
                sr.material = materials.normalMaterial;
                sr.transform.position -= (Vector3)rands[i];
            }
        }
    }
}
