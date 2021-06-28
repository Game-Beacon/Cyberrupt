using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioClipSetting")]
public class AudioClipSetting : ScriptableObject
{
    [SerializeField]
    private List<ClipSetting> _settings = new List<ClipSetting>();
    public List<ClipSetting> settings { get { return _settings; } private set { } }
}

[System.Serializable]
public struct ClipSetting
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
    public bool loop;
}
