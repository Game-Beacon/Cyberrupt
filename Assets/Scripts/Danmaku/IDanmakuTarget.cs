using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDanmakuTarget
{
    Transform target { get; }
    float hitRadius { get; }
    bool isImmune { get; }
}
