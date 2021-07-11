using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyMaterialSetting")]
public class EnemyMaterialSetting : SingletonScriptableObject<EnemyMaterialSetting>
{
    [SerializeField]
    private Material _normalMaterial;
    public Material normalMaterial { get { return _normalMaterial; } }

    [SerializeField]
    private Material _hurtMaterial;
    public Material hurtMaterial { get { return _hurtMaterial; } }
}
