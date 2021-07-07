using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioPlayRequester), true)]
public class AudioPlayRequesterEditor : Editor
{
    private AudioPlayRequester inspecting = null;

    private void OnEnable()
    {
        inspecting = (AudioPlayRequester)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(10);

        if(inspecting.group != null)
        {
            for (int i = 0; i < inspecting.group.clips.Count; i++)
                EditorGUILayout.LabelField("Clip " + i.ToString() + ":", inspecting.group.clips[i].name);
        }
    }
}
