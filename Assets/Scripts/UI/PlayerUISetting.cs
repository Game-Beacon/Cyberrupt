using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerUISetting : SingletonScriptableObject<PlayerUISetting>
{
    [SerializeField]
    private GameObject _heartSpriteDisplayer;
    public GameObject heartSpriteDisplayer { get { return _heartSpriteDisplayer; } }

    [SerializeField]
    private Sprite _heart;
    public Sprite heart { get { return _heart; } }

    [SerializeField]
    private Sprite _heartEmpty;
    public Sprite heartEmpty { get { return _heartEmpty; } }

    [Space(20), SerializeField]
    private GameObject _bombSpriteDisplayer;
    public GameObject bombSpriteDisplayer { get { return _bombSpriteDisplayer; } }

    [SerializeField]
    private Sprite _bomb;
    public Sprite bomb { get { return _bomb; } }
}
