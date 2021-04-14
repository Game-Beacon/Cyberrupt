using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DanmakuBullet
{
    public Vector2 localPosition;
    public DanmakuBulletData bullet;
}

[CreateAssetMenu(menuName = "Danmaku/Pattern Data")]
public class DanmakuPattern : ScriptableObject
{
    [SerializeField]
    private List<DanmakuBullet> _data = new List<DanmakuBullet>();
    public List<DanmakuBullet> data { get { return _data; } private set { } }
    public int count { get { return _data.Count; } private set { } }


#if UNITY_EDITOR
    public void AddBullets(DanmakuBulletData b, Vector2[] positions)
    {
        int len = positions.Length;
        for(int i = 0; i < len; i++)
            _data.Add(new DanmakuBullet() { bullet = b, localPosition = positions[i] });
    }

    /// <summary>
    /// 清空彈幕。（請不要在程式運行中呼叫此程式，這是給編輯器用的。）
    /// </summary>
    public void ResetPattern()
    {
        _data.Clear();
    }
#endif
}