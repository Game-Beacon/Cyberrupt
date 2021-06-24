using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyWave))]
public class EnemyWaveEditor : Editor
{
    //修改波數所需的資料 
    GameObject[] enemies = new GameObject[0];
    string[] enemyNames = new string[0];
    int enemyIndex;

    Vector2 position;


    //可視化波數所需的資料
    EnemyWave inspecting = null;
    List<GameObject> show = new List<GameObject>();

    private void OnEnable()
    {
        inspecting = (EnemyWave)target;
        Object[] obj = Resources.LoadAll("AI/Enemy", typeof(GameObject));
        DestroyAllViewObject();

        enemyIndex = 0;
        enemies = new GameObject[obj.Length];
        enemyNames = new string[obj.Length + 1];
        enemyNames[0] = "None";

        for(int i = 0; i < obj.Length; i++)
        {
            enemies[i] = (GameObject)obj[i];
            enemyNames[i + 1] = enemies[i].name;
        }

        foreach(EnemySpawnData spawn in inspecting.spawns)
        {
            GameObject go = Instantiate(spawn.enemy);
            go.transform.position = spawn.position;
            show.Add(go);
        }

        SceneView.duringSceneGui += OnSceneGUI;
    }

    public void OnSceneGUI(SceneView scene)
    {
        if (enemyIndex != 0)
            DrawWireSolidDisc(position, 0.25f, Color.yellow);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        enemyIndex = EditorGUILayout.Popup("敵人種類：", enemyIndex, enemyNames);

        if (enemyIndex != 0)
        {
            position = EditorGUILayout.Vector2Field("生成位置：", position);
            EditorGUILayout.Space(10);
            if (GUILayout.Button("生成敵人"))
                AddEnemy();     
        }
        else
            EditorGUILayout.Space(10);

        if (GUILayout.Button("清空所有敵人"))
        {
            DestroyAllViewObject();
            inspecting.ResetSpawnData();
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void AddEnemy()
    {
        EnemySpawnData data = new EnemySpawnData();
        data.enemy = enemies[enemyIndex - 1];
        data.position = position;
        inspecting.AddSpawnData(data);

        GameObject go = Instantiate(enemies[enemyIndex - 1]);
        go.transform.position = position;
        show.Add(go);
    }

    private void OnDisable()
    {
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);

        inspecting = null;
        DestroyAllViewObject();
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void DestroyAllViewObject()
    {
        while (show.Count > 0)
        {
            GameObject go = show[show.Count - 1];
            show.RemoveAt(show.Count - 1);
            DestroyImmediate(go);
        }
    }

    private void DrawWireSolidDisc(Vector2 position, float radius, Color color)
    {
        Handles.color = new Color(color.r, color.g, color.b, 0.15f);
        Handles.DrawSolidDisc(position, Vector3.back, radius);
        Handles.color = color;
        Handles.DrawWireDisc(position, Vector3.back, radius);
    }
}
