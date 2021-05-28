using System;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Enemy
{
    [SerializeField]
    private Enemy main;
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private float angularSpeed;


    protected override void EnemyAwake()
    {
        main.OnDeath += Die;
    }

    protected override void EnemyUpdate()
    {
        float rotation = parent.rotation.eulerAngles.z;
        rotation += angularSpeed * Time.deltaTime;
        parent.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public override void OnKilled()
    {
        if (main != null)
            main.OnDeath -= Die;
        Destroy(parent.gameObject, 0.1f);
    }
}