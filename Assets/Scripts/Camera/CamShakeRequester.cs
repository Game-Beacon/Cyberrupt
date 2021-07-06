using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShakeRequester : GameBehaviour
{
    protected static CameraController cam = null;

    public override void GameStart()
    {
        if (cam == null)
            cam = CameraController.instance;

        Setup();
    }

    public virtual void Setup() { }

    public void CamShakeDirectional(Vector2 direction, float time, float strenth, bool burstShake)
    {
        cam.CamShakeDirectional(direction, time, strenth, burstShake);
    }

    public void CamShake(float time, float strenth, bool burstShake)
    {
        cam.CamShake(time, strenth, burstShake);
    }
}
