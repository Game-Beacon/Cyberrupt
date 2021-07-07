using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : GameBehaviour
{
    private static CameraController _instance = null;
    public static CameraController instance { get { return _instance; } }

    private class CamShakeData
    {
        public bool locked = false;
        public Vector2 offset = Vector2.zero;
    }

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Easing camShakeEase;
    [SerializeField]
    private float vibro;

    private CamShakeData[] shakes = new CamShakeData[10];

    public override void GameAwake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < shakes.Length; i++)
            shakes[i] = new CamShakeData();
    }

    public override void GameUpdate()
    {
        Vector2 totalOffset = Vector2.zero;
        for (int i = 0; i < shakes.Length; i++)
            totalOffset += shakes[i].offset;
        cam.transform.position = new Vector3(totalOffset.x, totalOffset.y, cam.transform.position.z);
    }

    public void CamShakeDirectional(Vector2 direction, float time, float strenth, bool burstShake = true)
    {
        StartCoroutine(Shake(direction.normalized, time, strenth, burstShake));
    }

    public void CamShake(float time, float strenth, bool burstShake = true)
    {
        StartCoroutine(Shake(Vector2.zero, time, strenth, burstShake));
    }

    public void CamShakeEase(float time, float strenth, Easing ease, bool startFromZero)
    {
        StartCoroutine(ShakeEase(Vector2.zero, time, strenth, ease, startFromZero));
    }

    IEnumerator Shake(Vector2 dirNormed, float time, float strenth, bool burstShake)
    {
        int shakeIndex = -1;
        for(int i = 0; i < shakes.Length; i++)
        {
            if(!shakes[i].locked)
            {
                shakes[i].locked = true;
                shakeIndex = i;
                break;
            }
        }
        if (shakeIndex < 0)
            yield break;

        float timer = 0;
        Vector2 actualDir;
        float actualStr;
        while (timer < time)
        {
            timer += Time.deltaTime;
            actualDir = (dirNormed == Vector2.zero) ? Random.insideUnitCircle.normalized : dirNormed;
            actualStr = (burstShake) ? strenth * (1 - EaseLibrary.CallEaseFunction(camShakeEase, timer / time)) : strenth;
            shakes[shakeIndex].offset = actualDir * Mathf.Cos(timer * 6.2832f * vibro) * actualStr;
            yield return null;
        }

        shakes[shakeIndex].offset = Vector2.zero;
        shakes[shakeIndex].locked = false;
    }

    IEnumerator ShakeEase(Vector2 dirNormed, float time, float strenth, Easing ease, bool startFromZero)
    {
        int shakeIndex = -1;
        for (int i = 0; i < shakes.Length; i++)
        {
            if (!shakes[i].locked)
            {
                shakes[i].locked = true;
                shakeIndex = i;
                break;
            }
        }
        if (shakeIndex < 0)
            yield break;

        float timer = 0;
        Vector2 actualDir;
        float actualStr;
        while (timer < time)
        {
            timer += Time.deltaTime;
            actualDir = (dirNormed == Vector2.zero) ? Random.insideUnitCircle.normalized : dirNormed;
            actualStr = strenth * ((startFromZero)? EaseLibrary.CallEaseFunction(ease, timer / time) : (1 - EaseLibrary.CallEaseFunction(ease, timer / time)));
            shakes[shakeIndex].offset = actualDir * Mathf.Cos(timer * 6.2832f * vibro) * actualStr;
            yield return null;
        }

        shakes[shakeIndex].offset = Vector2.zero;
        shakes[shakeIndex].locked = false;
    }
}
