using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DanmakuBulletData))]
public class DanmakuBulletDataEditor : Editor
{
    //把子彈顯示出來需要的資料
    DanmakuBulletData inspecting = null;
    GameObject bullet = null;
    SpriteRenderer renderer = null;

    private void OnEnable()
    {
        inspecting = (DanmakuBulletData)target;

        bullet = new GameObject();
        bullet.transform.position = Vector2.zero;
        renderer = bullet.AddComponent<SpriteRenderer>();

        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        inspecting = null;

        DestroyImmediate(bullet);
        bullet = null;
        renderer = null;

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void OnSceneGUI(SceneView scene)
    {
        if (inspecting.sprite != null)
            renderer.sprite = inspecting.sprite;
        else
            renderer.sprite = null;

        DrawWireSolidDisc(Vector2.zero, inspecting.radius, Color.green);

        SceneView.RepaintAll();
    }

    private void DrawWireSolidDisc(Vector2 position, float radius, Color color)
    {
        Handles.color = new Color(color.r, color.g, color.b, 0.15f);
        Handles.DrawSolidDisc(position, Vector3.back, radius);
        Handles.color = color;
        Handles.DrawWireDisc(position, Vector3.back, radius);
    }
}
