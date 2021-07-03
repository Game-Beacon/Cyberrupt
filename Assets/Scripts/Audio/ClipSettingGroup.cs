using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClipSetting
{
    public AudioClip clip;
    public string name;
    [Range(0, 1)]
    public float volume;
    public bool loop;
}

[CreateAssetMenu(menuName = "Audio/Clip Setting Group")]
public class ClipSettingGroup : ScriptableObject
{
    [SerializeField]
    private List<ClipSetting> _clips = new List<ClipSetting>();

    public List<ClipSetting> clips { get { return _clips; } }
}
