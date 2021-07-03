using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnemyGroup : MonoBehaviour
{
    List<SpriteRenderer> srs = new List<SpriteRenderer>();
    List<Collider2D> colliders = new List<Collider2D>();
    [SerializeField]
    List<ParticleSystem> pss = new List<ParticleSystem>();

    [SerializeField, Range(0, 1f)]
    public float alpha = 1;
    [SerializeField]
    public bool enableColliders = true;

    private void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>().ToList();
        colliders = GetComponentsInChildren<Collider2D>().ToList();
    }

    private void Update()
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
}
