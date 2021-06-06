using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meltdown_Laser : AITimerState
{
    [Space(20), SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private float width;
    [SerializeField]
    private float length;
    [SerializeField]
    private float reachMaxLengthTime;
    [SerializeField]
    private Easing laserEase;

    private Transform target = null;
    private Laser laser = null;

    [Space(20), SerializeField]
    private float speed;
    [SerializeField]
    private float rotateForce;
    [SerializeField]
    private float rotateIncrement;
    private float rotateScalar;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        
        if (target == null)
            target = GetComponent<ITarget>().target;

        laser = Instantiate(laserPrefab, transform).GetComponent<Laser>();
        laser.SetLaserProperty(width, 0);
        rotateScalar = 0;
    }

    protected override void OnStateUpdate(float delta)
    {
        base.OnStateUpdate(delta);
        float currentLength = length * EaseLibrary.CallEaseFunction(laserEase, timer / reachMaxLengthTime);
        laser.SetLaserProperty(width, currentLength);
    }

    protected override void UpdateTransform(float delta)
    {
        if (timer < 0.5f)
            return;
        
        Vector2 direction = (target.position - transform.position).normalized;

        //Deal with position;
        transform.position += (Vector3)direction * speed * delta;

        //Deal with rotation.
        float fromAngle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float toAngle = Mathf.Atan2(direction.y, direction.x);

        Vector3 fromVector = new Vector3(Mathf.Cos(fromAngle), Mathf.Sin(fromAngle));
        Vector3 toVector = new Vector3(Mathf.Cos(toAngle), Mathf.Sin(toAngle));

        float crossValue = Vector3.Cross(fromVector, toVector).z;
        rotateScalar += rotateIncrement * crossValue;

        rotateScalar = Mathf.Clamp(rotateScalar, -1f, 1f);

        float finalAngle = transform.rotation.eulerAngles.z + rotateScalar * rotateForce * delta;
        transform.rotation = Quaternion.Euler(0, 0, finalAngle);
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
        laser.transform.SetParent(null);
        laser.Kill(0.5f);
        laser = null;
    }
}
