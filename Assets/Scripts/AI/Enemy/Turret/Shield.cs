using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Enemy
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private float angularSpeed;

    protected override void EnemyUpdate()
    {
        float rotation = parent.rotation.eulerAngles.z;
        rotation += angularSpeed * Time.deltaTime;
        parent.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public override void OnKilled()
    {
        Destroy(parent.gameObject, 0.1f);
    }
}