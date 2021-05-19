using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DanmakuPattern))]
public class DanmakuPatternEditor : Editor
{
    private enum DrawOption
    {
        None = 0,
        Dot = 1,
        Circle = 2,
        Polygon = 3,
        Line = 4
    }

    //把彈幕顯示出來需要的資料
    DanmakuPattern inspecting = null;
    List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    //繪製彈幕的選項
    DrawOption drawOption = DrawOption.None;
    DanmakuBulletData[] danmakuBulletDatas = new DanmakuBulletData[0];
    string[] bulletNames = new string[0];
    int bulletIndex = 0;

    //繪製彈幕的變數
    Vector2 drawOrigin;             //繪製的中心點（dot, circle, polygon適用）
    float radius;                   //半徑（circle, polygon適用）
    float rotation;                 //旋轉（circle, polygon適用）
    float arc = 360;                //弧角（circle適用）
    int bulletCountForCircle;       //圓形彈幕的子彈數（circle適用）
    int edgeCount;                  //多邊形邊數（polygon適用）
    int bulletCountForPolygonEdge;  //邊的子彈數（polygon適用）
    Vector2 pointA;                 //點A（line適用）
    Vector2 pointB;                 //點B（line適用）
    int bulletCountForLine;         //線的子彈數（line適用）

    //修改彈幕用
    Vector2 offset;

    private void OnEnable()
    {
        inspecting = (DanmakuPattern)target;
        DestroyAllViewObject();

        Object[] obj = Resources.LoadAll("", typeof(DanmakuBulletData));
        danmakuBulletDatas = new DanmakuBulletData[obj.Length];
        bulletNames = new string[obj.Length + 1];
        bulletNames[0] = "None";

        for(int i = 0; i < obj.Length; i ++)
        {
            danmakuBulletDatas[i] = (DanmakuBulletData)obj[i];
            bulletNames[i + 1] = danmakuBulletDatas[i].name;
        }

        bulletIndex = 0;

        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        inspecting = null;
        DestroyAllViewObject();
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void OnSceneGUI(SceneView scene)
    {
        if (drawOption != DrawOption.None && bulletIndex > 0)
            DrawPreview();

        ResizeRenderers();
        DrawPattern();
        SceneView.RepaintAll();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bulletIndex = EditorGUILayout.Popup("子彈樣式：", bulletIndex, bulletNames);
        drawOption = (DrawOption)EditorGUILayout.EnumPopup("繪製樣式：", drawOption);
        EditorGUILayout.Space(10);

        if (drawOption == DrawOption.Dot || drawOption == DrawOption.Circle || drawOption == DrawOption.Polygon)
            drawOrigin = EditorGUILayout.Vector2Field("繪製中心：", drawOrigin);
        if (drawOption == DrawOption.Circle || drawOption == DrawOption.Polygon)
        {
            radius = EditorGUILayout.Slider("半徑：", radius, 0f, 5f);
            rotation = EditorGUILayout.Slider("旋轉：", rotation, 0f, 360f);
        }
        if (drawOption == DrawOption.Circle)
        {
            arc = EditorGUILayout.Slider("弧角：", arc, 0f, 360f);
            bulletCountForCircle = EditorGUILayout.IntSlider("圓弧子彈數量：", bulletCountForCircle, 3, 50);
        }
        if (drawOption == DrawOption.Polygon)
        {
            edgeCount = EditorGUILayout.IntSlider("多邊形邊數：", edgeCount, 3, 8);
            bulletCountForPolygonEdge = EditorGUILayout.IntSlider("邊的子彈數量：", bulletCountForPolygonEdge, 1, 10);
        }
        if (drawOption == DrawOption.Line)
        {
            pointA = EditorGUILayout.Vector2Field("起點：", pointA);
            pointB = EditorGUILayout.Vector2Field("終點：", pointB);
            bulletCountForLine = EditorGUILayout.IntSlider("線段的子彈數量：", bulletCountForLine, 2, 20);
        }
        if (drawOption != DrawOption.None)
        {
            EditorGUILayout.Space(10);
            if(bulletIndex > 0)
            {
                
                if (GUILayout.Button("生成彈幕"))
                    AddPattern();
                EditorGUILayout.Space(10);
            }
        }

        if (GUILayout.Button("清空彈幕"))
            inspecting.ResetPattern();

        EditorGUILayout.Space(10);
        offset = EditorGUILayout.Vector2Field("彈幕位移：", offset);

        if (GUILayout.Button("移動彈幕"))
        {
            inspecting.OffsetBullets(offset);
            offset = Vector2.zero;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("子彈總數：" + inspecting.count.ToString(), MessageType.Info);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void DrawPreview()
    {
        if (drawOption == DrawOption.Dot)
            DrawWireSolidDisc(drawOrigin, danmakuBulletDatas[bulletIndex - 1].radius, Color.green);
        if (drawOption == DrawOption.Circle)
        {
            float angle = rotation;
            float delta = (arc != 360f) ? arc / (bulletCountForCircle - 1) : arc / bulletCountForCircle;

            for (int i = 0; i < bulletCountForCircle; i++)
            {
                Vector2 pos = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius + drawOrigin;
                DrawWireSolidDisc(pos, danmakuBulletDatas[bulletIndex - 1].radius, Color.green);
                angle += delta;
            }
        }
        if (drawOption == DrawOption.Polygon)
        {
            float angle = rotation;
            float delta = 360f / edgeCount;
            Vector2 start;
            Vector2 end;

            for (int i = 0; i < edgeCount; i++)
            {
                start = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius + drawOrigin;
                end = new Vector2(Mathf.Cos((angle + delta) * Mathf.Deg2Rad), Mathf.Sin((angle + delta) * Mathf.Deg2Rad)) * radius + drawOrigin;
                for (int j = 0; j < bulletCountForPolygonEdge; j++)
                {
                    Vector2 pos = Vector2.Lerp(start, end, (float)j / bulletCountForPolygonEdge);
                    DrawWireSolidDisc(pos, danmakuBulletDatas[bulletIndex - 1].radius, Color.green);
                }
                angle += delta;
            }
        }
        if (drawOption == DrawOption.Line)
        {
            for(int i = 0; i < bulletCountForLine; i++)
            {
                Vector2 pos = Vector2.Lerp(pointA, pointB, (float)i / (bulletCountForLine - 1));
                DrawWireSolidDisc(pos, danmakuBulletDatas[bulletIndex - 1].radius, Color.green);
            }
        }
    }

    private void AddPattern()
    {
        Vector2[] positions = new Vector2[0];

        if (drawOption == DrawOption.Dot)
        {
            positions = new Vector2[1];
            positions[0] = drawOrigin;
        }
        if (drawOption == DrawOption.Circle)
        {
            positions = new Vector2[bulletCountForCircle];

            float angle = rotation;
            float delta = (arc != 360f) ? arc / (bulletCountForCircle - 1) : arc / bulletCountForCircle;

            for (int i = 0; i < bulletCountForCircle; i++)
            {
                Vector2 pos = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius + drawOrigin;
                positions[i] = pos;
                angle += delta;
            }
        }
        if (drawOption == DrawOption.Polygon)
        {
            positions = new Vector2[edgeCount * bulletCountForPolygonEdge];

            float angle = rotation;
            float delta = 360f / edgeCount;
            Vector2 start;
            Vector2 end;

            for (int i = 0; i < edgeCount; i++)
            {
                start = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius + drawOrigin;
                end = new Vector2(Mathf.Cos((angle + delta) * Mathf.Deg2Rad), Mathf.Sin((angle + delta) * Mathf.Deg2Rad)) * radius + drawOrigin;
                for (int j = 0; j < bulletCountForPolygonEdge; j++)
                {
                    Vector2 pos = Vector2.Lerp(start, end, (float)j / bulletCountForPolygonEdge);
                    positions[i * bulletCountForPolygonEdge + j] = pos;
                }
                angle += delta;
            }
        }
        if (drawOption == DrawOption.Line)
        {
            positions = new Vector2[bulletCountForLine];

            for (int i = 0; i < bulletCountForLine; i++)
            {
                Vector2 pos = Vector2.Lerp(pointA, pointB, (float)i / (bulletCountForLine - 1));
                positions[i] = pos;
            }
        }

        inspecting.AddBullets(danmakuBulletDatas[bulletIndex - 1], positions);
    }

    private void DrawPattern()
    { 
        int i = 0;

        foreach (DanmakuBullet data in inspecting.data)
        {
            Vector2 bulletPos = data.localPosition;
            renderers[i].sprite = data.bullet.sprites[0];
            renderers[i].gameObject.transform.position = bulletPos;
            DrawWireSolidDisc(bulletPos, data.bullet.radius, Color.red);
            i++;
        }
    }

    private void ResizeRenderers()
    {
        if (inspecting.data.Count == renderers.Count)
            return;
        else if(inspecting.data.Count > renderers.Count)
        {
            while (inspecting.data.Count != renderers.Count)
            {
                GameObject go = new GameObject();
                SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                renderers.Add(renderer);
            }
        }
        else
        {
            while (inspecting.data.Count != renderers.Count)
            {
                SpriteRenderer renderer = renderers[renderers.Count - 1];
                renderers.RemoveAt(renderers.Count - 1);
                DestroyImmediate(renderer.gameObject);
            }
        }
    }

    private void DestroyAllViewObject()
    {
        while (renderers.Count > 0)
        {
            SpriteRenderer renderer = renderers[renderers.Count - 1];
            renderers.RemoveAt(renderers.Count - 1);
            DestroyImmediate(renderer.gameObject);
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
