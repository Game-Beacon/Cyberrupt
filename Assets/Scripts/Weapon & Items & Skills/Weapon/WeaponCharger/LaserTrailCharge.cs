using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LaserTrailCharge : WeaponCharger
{
    [SerializeField]
    private LineRenderer lr1, lr2, lr3;
    [SerializeField]
    private Easing ease;
    [SerializeField]
    private float trailLength;
    [SerializeField]
    private float initSpread;
    [SerializeField]
    private float rotateCycleCount;


    public override void Charge(float percent)
    {
        float truePercent = EaseLibrary.CallEaseFunction(ease, percent);
        float rotation = rotateCycleCount * 360 * truePercent * Mathf.Deg2Rad; //In degree
        float spread = initSpread * (1 - truePercent) * Mathf.Deg2Rad;

        lr1.startColor = new Color(lr1.startColor.r, lr1.startColor.g, lr1.startColor.b, truePercent);
        lr1.endColor = new Color(lr1.endColor.r, lr1.endColor.g, lr1.endColor.b, truePercent);
        lr2.startColor = new Color(lr2.startColor.r, lr2.startColor.g, lr2.startColor.b, truePercent);
        lr2.endColor = new Color(lr2.endColor.r, lr2.endColor.g, lr2.endColor.b, truePercent);
        lr3.startColor = new Color(lr3.startColor.r, lr3.startColor.g, lr3.startColor.b, truePercent);
        lr3.endColor = new Color(lr3.endColor.r, lr3.endColor.g, lr3.endColor.b, truePercent);

        float spread1 = spread * Mathf.Sin(rotation);
        float spread2 = spread * Mathf.Sin(rotation + 120 * Mathf.Deg2Rad);
        float spread3 = spread * Mathf.Sin(rotation + 240* Mathf.Deg2Rad);

        lr1.SetPosition(1, new Vector3(Mathf.Cos(spread1), Mathf.Sin(spread1), 0) * trailLength);
        lr2.SetPosition(1, new Vector3(Mathf.Cos(spread2), Mathf.Sin(spread2), 0) * trailLength);
        lr3.SetPosition(1, new Vector3(Mathf.Cos(spread3), Mathf.Sin(spread3), 0) * trailLength);
    }
}
