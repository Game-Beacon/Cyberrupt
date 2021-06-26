using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyWaveGroup))]
public class EnemyWaveGroupEditor : Editor
{
    private EnemyWaveGroup inspecting = null;
    private string directoryString;

    private void OnEnable()
    {
        inspecting = (EnemyWaveGroup)target;

        string directoryRaw = AssetDatabase.GetAssetPath(inspecting);
        string[] splits = directoryRaw.Split('/');

        directoryString = "";
        for (int i = 2; i < splits.Length - 1; i++)
        {
            directoryString += splits[i];
            directoryString += "/";
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        base.OnInspectorGUI();
        EditorGUILayout.Space(20);
        
        directoryString = EditorGUILayout.TextField("資料夾路徑：", directoryString);
        if (GUILayout.Button("匯入敵人陣型"))
            LoadAllLevel();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void LoadAllLevel()
    {
        Object[] obj = Resources.LoadAll(directoryString, typeof(EnemyWave));
        List<EnemyWave> waves = new List<EnemyWave>();

        for (int i = 0; i < obj.Length; i++)
            waves.Add((EnemyWave)obj[i]);

        inspecting.SetCandidates(waves);
    }
}
