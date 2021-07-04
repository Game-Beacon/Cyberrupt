using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class EnemySpawnZone : GameBehaviour
{
    [System.Serializable]
    private struct SpawnZoneData
    {
        public float size;
        public Sprite iconSprite;
    }

    [SerializeField]
    private SpriteRenderer outline;
    [SerializeField]
    private SpriteRenderer icon;
    [SerializeField]
    private GameObject repelForceField;
    [SerializeField]
    private Collider2D keepOut;
    [SerializeField]
    private ParticleSystem ps;
    
    [SerializeField]
    private SpawnZoneData normalData;
    [SerializeField]
    private SpawnZoneData eliteData;
    [SerializeField]
    private SpawnZoneData bossData;
    [SerializeField]
    private float warnTime;
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    public UltEvent<Enemy> OnEnemySpawn = new UltEvent<Enemy>();

    private EnemySpawnData data;
    private float size;
    private Sprite iconSprite;

    public void InjectData(EnemySpawnData spawnData)
    {
        data = spawnData;
        ChooseSize();
        StartCoroutine(StartSpawn());
    }

    IEnumerator StartSpawn()
    {
        transform.localScale = new Vector3(size * 0.75f, size * 0.75f, 1);
        icon.sprite = iconSprite;

        transform.DOScale(new Vector3(size, size, 1), warnTime);

        outline.DOFade(1, warnTime);
        icon.DOFade(1, warnTime);
        outline.transform.DOShakePosition(warnTime, 0.1f, 50, 90, false, false).SetEase(Ease.Linear);
        icon.transform.DOShakePosition(warnTime, 0.1f, 50, 90, false, false).SetEase(Ease.Linear);
        keepOut.transform.DOScale(new Vector3(1, 1, 1), warnTime);
        yield return new WaitForSeconds(warnTime);

        repelForceField.SetActive(false);

        Enemy e = Instantiate(data.enemy, data.position, Quaternion.identity).GetComponent<Enemy>();
        OnEnemySpawn.Invoke(e);
        
        ps.transform.SetParent(null);
        ps.Play();
        outline.DOFade(0, fadeTime);
        icon.DOFade(0, fadeTime);
        keepOut.enabled = false;

        yield return new WaitForSeconds(fadeTime);

        Destroy(gameObject);
    }

    private void ChooseSize()
    {
        EnemyBaseProperty property = data.enemy.GetComponent<Enemy>().property;

        switch (property.rank)
        {
            case EnemyRankType.Normal:
                size = normalData.size;
                iconSprite = normalData.iconSprite;
                break;
            case EnemyRankType.Elite:
                size = eliteData.size;
                iconSprite = eliteData.iconSprite;
                break;
            case EnemyRankType.Boss:
                size = bossData.size;
                iconSprite = bossData.iconSprite;
                break;
            default:
                size = normalData.size;
                iconSprite = normalData.iconSprite;
                break;
        }
    }
}
