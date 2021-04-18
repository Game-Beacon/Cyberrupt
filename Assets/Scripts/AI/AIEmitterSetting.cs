using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Emitter Setting")]
[System.Serializable]
public class AIEmitterSetting : ScriptableObject 
{
    public DanmakuParticleData data;
    public bool followParent;
    public Vector2 position;
    public float rotation;
    public float time;
    public bool updatePosition;
    public bool updateRotation;
    public bool updateScale;

    public DanmakuParticleEmitter MakeEmitter(Transform parent)
    {
        GameObject go = new GameObject();
        if (parent != null)
            go.transform.SetParent(parent);
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        if (!followParent)
            go.transform.SetParent(null);
        DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(data, go.transform, time);
        return emitter;
    }
}
