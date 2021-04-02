using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BossEmitterSetting
{
    public DanmakuParticleData particle;
    public Transform location;
    public bool lookAtTarget;
}
