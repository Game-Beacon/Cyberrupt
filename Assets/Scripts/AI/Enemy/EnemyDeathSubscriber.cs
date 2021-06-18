using UnityEngine;

public class EnemyDeathSubscriber : MonoBehaviour
{
    [SerializeField]
    private GameObject deathVFX;
    [SerializeField]
    private float scaleFactor = 1;

    void Start()
    {
        var enemy = GetComponent<Enemy>();
        enemy.OnDeath += () => this.createVFX(enemy.transform);
    }

    private void createVFX(Transform enemy)
    {
        var go = Instantiate(this.deathVFX, enemy.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * this.scaleFactor;
    }
}