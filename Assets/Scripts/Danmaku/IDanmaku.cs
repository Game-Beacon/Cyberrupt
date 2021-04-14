using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDanmaku
{
    Transform parent { get; }
    int activeCount { get; }
    bool spawnBullets { get; }

    void Update(float delta);

    void OnDispose();
}
