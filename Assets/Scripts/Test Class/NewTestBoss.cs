using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTestBoss : AIStateMachine
{
    private Player player;

    [SerializeField]
    private float speed;

    protected override void MachineStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
    }

    protected override void UpdateTransform()
    {
        Vector2 direction = _target.position - transform.position;
        transform.position += (Vector3)direction.normalized * speed * Time.fixedDeltaTime;
    }
}
