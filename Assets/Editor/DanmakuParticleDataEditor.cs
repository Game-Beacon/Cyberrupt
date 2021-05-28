using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DanmakuParticleData))]
public class DanmakuParticleDataEditor : Editor
{
    bool emitterFoldout;
    bool shapeFoldout;
    bool patternFoldout;
    bool pathFoldout;
    //bool subEmitterFoldout;

    private void OnEnable()
    {
        emitterFoldout = false;
        shapeFoldout = false;
        patternFoldout = false;
        pathFoldout = false;
        //subEmitterFoldout = false;
    }

    override public void OnInspectorGUI()
    {
        DanmakuParticleData data = target as DanmakuParticleData;

        serializedObject.Update();

        SerializedProperty property = null;
        GUIContent content = new GUIContent();

        GUI.backgroundColor = Color.gray;

        emitterFoldout = EditorGUILayout.Foldout(emitterFoldout, "發射器設定", true, EditorStyles.toolbarPopup);
        if (emitterFoldout)
        {
            GUILayout.Space(10);
            SetSerializedProperty("_emitModule._patterns", "主彈幕", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);
            SetSerializedProperty("_emitModule._liveTime", "存活時間", ref property, ref content);
            EditorGUILayout.Slider(property, 0.1f, 20f, content);
            SetSerializedProperty("_emitModule._rateOverTime", "發射頻率", ref property, ref content);
            EditorGUILayout.Slider(property, 0.1f, 50f, content);
            SetSerializedProperty("_shapeModule._burstCount", "每發發射個數", ref property, ref content);
            EditorGUILayout.IntSlider(property, 1, 30, content);
        }

        GUILayout.Space(10);

        shapeFoldout = EditorGUILayout.Foldout(shapeFoldout, "發射器形狀設定", true, EditorStyles.toolbarPopup);
        if (shapeFoldout)
        {
            GUILayout.Space(10);
            SetSerializedProperty("_shapeModule._shapeType", "發射器形狀", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);

            if (data.shapeModule.shapeType == ShapeType.Sector)
            {
                SetSerializedProperty("_shapeModule._spread", "弧角", ref property, ref content);
                EditorGUILayout.Slider(property, 0f, 360f, content);
                SetSerializedProperty("_shapeModule._radius", "半徑", ref property, ref content);
                EditorGUILayout.Slider(property, 0f, 10f, content);
                SetSerializedProperty("_shapeModule._radiusThickness", "弧厚度", ref property, ref content);
                EditorGUILayout.Slider(property, 0f, 1f, content);
            }
            if (data.shapeModule.shapeType == ShapeType.Edge)
            {
                SetSerializedProperty("_shapeModule._edgeWidth", "線長", ref property, ref content);
                EditorGUILayout.Slider(property, 0f, 50f, content);
            }

            SetSerializedProperty("_shapeModule._distributeType", "發射分佈", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);

            if (data.shapeModule.distributeType == DistributeType.Repeat || data.shapeModule.distributeType == DistributeType.RepeatRandom)
            {
                SetSerializedProperty("_shapeModule._cycleTime", "週期", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 10f, content);
                SetSerializedProperty("_shapeModule._reverseCycle", "週期反轉?", ref property, ref content);
                EditorGUILayout.PropertyField(property, content);
            }
        }

        GUILayout.Space(10);

        patternFoldout = EditorGUILayout.Foldout(patternFoldout, "彈幕縮放旋轉設定", true, EditorStyles.toolbarPopup);
        if (patternFoldout)
        {
            GUILayout.Space(10);
            SetSerializedProperty("_patternModule._easeType", "彈幕緩動類型", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);
            SetSerializedProperty("_patternModule._scaleType", "彈幕縮放類型", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);
            SetSerializedProperty("_patternModule._easeTime", "緩動時間", ref property, ref content);
            EditorGUILayout.Slider(property, 0.1f, 50f, content);
            if (data.patternModule.scaleType == ValueType.Constant)
            {
                SetSerializedProperty("_patternModule._scale", "彈幕縮放", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 100f, content);
            }
            if (data.patternModule.scaleType == ValueType.Random)
            {
                EditorGUILayout.HelpBox("請不要讓最小的數值大於最大的數值，拜託QwQ", MessageType.Info);
                SetSerializedProperty("_patternModule._minScale", "彈幕最小縮放", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 100f, content);
                SetSerializedProperty("_patternModule._maxScale", "彈幕最大縮放", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 100f, content);
            }

            SetSerializedProperty("_patternModule._rotationType", "彈幕轉速類型", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);
            if (data.patternModule.rotationType == ValueType.Constant)
            {
                SetSerializedProperty("_patternModule._rotation", "彈幕轉速", ref property, ref content);
                EditorGUILayout.Slider(property, -360f, 360f, content);
            }
            if (data.patternModule.rotationType == ValueType.Random)
            {
                EditorGUILayout.HelpBox("請不要讓最小的數值大於最大的數值，拜託QwQ", MessageType.Info);
                SetSerializedProperty("_patternModule._minRotation", "彈幕最小轉速", ref property, ref content);
                EditorGUILayout.Slider(property, -360f, 360f, content);
                SetSerializedProperty("_patternModule._maxRotation", "彈幕最大轉速", ref property, ref content);
                EditorGUILayout.Slider(property, -360f, 360f, content);
            }
        }

        GUILayout.Space(10);

        pathFoldout = EditorGUILayout.Foldout(pathFoldout, "彈幕路徑設定", true, EditorStyles.toolbarPopup);
        if (pathFoldout)
        {
            GUILayout.Space(10);
            SetSerializedProperty("_pathModule._pathType", "彈幕路徑", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);
            if (data.pathModule.pathType == PathType.Sine)
            {
                SetSerializedProperty("_pathModule._waveAmp", "週期波震幅", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 10f, content);
                SetSerializedProperty("_pathModule._waveHz", "週期波頻率", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 2f, content);
                SetSerializedProperty("_pathModule._waveOffest", "週期波偏移", ref property, ref content);
                EditorGUILayout.Slider(property, 0f, 1f, content);
            }
            SetSerializedProperty("_pathModule._speedType", "彈幕移速類型", ref property, ref content);
            EditorGUILayout.PropertyField(property, content);
            if (data.pathModule.speedType == ValueType.Constant)
            {
                SetSerializedProperty("_pathModule._speed", "彈幕移速", ref property, ref content);
                EditorGUILayout.Slider(property, -50f, 50f, content);
            }
            if (data.pathModule.speedType == ValueType.Random)
            {
                EditorGUILayout.HelpBox("請不要讓最小的數值大於最大的數值，拜託QwQ", MessageType.Info);
                SetSerializedProperty("_pathModule._minSpeed", "彈幕最小移速", ref property, ref content);
                EditorGUILayout.Slider(property, -50f, 50f, content);
                SetSerializedProperty("_pathModule._maxSpeed", "彈幕最大移速", ref property, ref content);
                EditorGUILayout.Slider(property, -50f, 50f, content);
            }
        }

        /*GUILayout.Space(30);

        SetSerializedProperty("_useSubEmitter", "使用子發射器", ref property, ref content);
        EditorGUILayout.PropertyField(property, content);
        if (data.useSubEmitter)
        {
            GUILayout.Space(10);
            subEmitterFoldout = EditorGUILayout.Foldout(subEmitterFoldout, "子發射器設定", true, EditorStyles.toolbarPopup);
            if(subEmitterFoldout)
            {
                GUILayout.Space(10);
                SetSerializedProperty("_subEmitModule._pattern", "副彈幕", ref property, ref content);
                EditorGUILayout.ObjectField(property, typeof(DanmakuPattern), content);
                SetSerializedProperty("_subEmitModule._spawnTime", "生成間隔", ref property, ref content);
                EditorGUILayout.Slider(property, 0.05f, 1f, content);
                SetSerializedProperty("_subEmitModule._easeType", "彈幕緩動類型", ref property, ref content);
                EditorGUILayout.PropertyField(property, content);
                SetSerializedProperty("_subEmitModule._easeTime", "緩動時間", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 50f, content);
                SetSerializedProperty("_subEmitModule._scale", "彈幕縮放", ref property, ref content);
                EditorGUILayout.Slider(property, 0.1f, 100f, content);
                SetSerializedProperty("_subEmitModule._rotation", "彈幕轉速", ref property, ref content);
                EditorGUILayout.Slider(property, -360f, 360f, content);
            }
        }*/

        serializedObject.ApplyModifiedProperties();
    }

    private void SetSerializedProperty(string path, string name, ref SerializedProperty property, ref GUIContent content)
    {
        property = serializedObject.FindProperty(path);
        content.text = name;
    }
}
