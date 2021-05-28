using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collision Layer")]
public class CollisionLayer : SingletonScriptableObject<CollisionLayer>
{
    //定義哪些Layer是玩家
    [SerializeField]
    private LayerMask _playerMask;
    public LayerMask playerMask { get { return _playerMask; } }

    //定義哪些Layer是敵人
    [SerializeField]
    private LayerMask _enemyMask;
    public LayerMask enemyMask { get { return _enemyMask; } }

    //定義哪些Layer是螢幕邊界
    [SerializeField]
    private LayerMask _screenMask;
    public LayerMask screenMask { get { return _screenMask; } }

    //定義哪些Layer是危險物
    [SerializeField]
    private LayerMask _hazardousMask;
    public LayerMask hazardousMask { get { return _hazardousMask; } }

    //定義哪些Layer是清彈幕的
    [SerializeField]
    private LayerMask _bombMask;
    public LayerMask bombMask { get { return _bombMask; } }

    public static bool CollideWithMask(GameObject collision, LayerMask mask)
    {
        return ((1 << collision.layer) & mask) != 0;
    }
}
