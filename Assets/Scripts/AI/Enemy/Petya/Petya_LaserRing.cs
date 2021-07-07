using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class Petya_LaserRing : AIState
{
    [Space(20), SerializeField]
    public UltEvent OnSpawnRing = new UltEvent();
    [SerializeField]
    private float startDelay;
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float spawnGap;

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
    [SerializeField]
    private float rotateSpeed;

    private float timer = 0;

    private Laser laser = null;
    private Laser laser2 = null;
    private Laser laser3 = null;
    private Laser laser4 = null;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        laser = Instantiate(laserPrefab, transform).GetComponent<Laser>();
        laser2 = Instantiate(laserPrefab, transform).GetComponent<Laser>();
        laser2.transform.localRotation = Quaternion.Euler(0, 0, 90);
        laser3 = Instantiate(laserPrefab, transform).GetComponent<Laser>();
        laser3.transform.localRotation = Quaternion.Euler(0, 0, 180);
        laser4 = Instantiate(laserPrefab, transform).GetComponent<Laser>();
        laser4.transform.localRotation = Quaternion.Euler(0, 0, 270);

        timer = 0;
        StartCoroutine(SpawnRings());
    }

    protected override void OnStateUpdate(float delta)
    {
        base.OnStateUpdate(delta);
        float currentLength = length * EaseLibrary.CallEaseFunction(laserEase, timer / reachMaxLengthTime);
        laser.SetLaserProperty(width, currentLength);
        laser2.SetLaserProperty(width, currentLength);
        laser3.SetLaserProperty(width, currentLength);
        laser4.SetLaserProperty(width, currentLength);

        timer += delta;
    }

    protected override void UpdateTransform(float delta)
    {
        float angle = transform.rotation.eulerAngles.z + rotateSpeed * delta;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
        laser.transform.SetParent(null);
        laser.Kill(0.5f);
        laser = null;
        laser2.transform.SetParent(null);
        laser2.Kill(0.5f);
        laser2 = null;
        laser3.transform.SetParent(null);
        laser3.Kill(0.5f);
        laser3 = null;
        laser4.transform.SetParent(null);
        laser4.Kill(0.5f);
        laser4 = null;
    }

    IEnumerator SpawnRings()
    {
        yield return new WaitForSeconds(startDelay);
        for(int i = 0; i < spawnCount; i ++)
        {
            OnSpawnRing.Invoke();
            yield return new WaitForSeconds(spawnGap);
        }
        SelfEndState();
    }
}
