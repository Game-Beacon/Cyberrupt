using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ClipSetting
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
    public bool loop;
}
