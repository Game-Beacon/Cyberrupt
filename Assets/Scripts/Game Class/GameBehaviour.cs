using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    protected static GameSystem game = null;
    private int _id = -1;   //ID，初始化之前值應該要是-1，初始化之後是任意不重複且不小於零的整數
    public int id { get { return _id; } private set { } }

    [HideInInspector]
    public bool update = true;
    
    private bool _persistent = false;
    public bool persistent { get { return _persistent; } private set { } }

    [SerializeField,Tooltip("指定腳本的執行順序，數字越大越晚執行")]
    protected int _executeOrder = 0;
    public int executeOrder { get { return _executeOrder; } private set { } }

    private bool _destroyGameObjectWhenKilled = false;
    public bool destroyGameObjectWhenKilled { get { return _destroyGameObjectWhenKilled; } private set { } }

    private void Start()
    {
        if (game == null)
            game = GameSystem.instance;

        game.AddBehaviour(this, ref _id);
        //Debug.Log("Start");
    }

    public virtual void GameAwake() { }
    public virtual void GameStart() { }
    public virtual void GameUpdate() { }
    public virtual void GameFixedUpdate() { }

    public virtual void OnKilled() { }

    public void KillBehaviour(bool killGameObject = false)
    {
        update = false;
        game.RemoveAndKillBehaviour(this);

        if (killGameObject)
        {
            _destroyGameObjectWhenKilled = true;
            GameBehaviour[] behaviours = GetComponentsInChildren<GameBehaviour>();

            for (int i = 0; i < behaviours.Length; i++)
                if (behaviours[i] != this)
                    behaviours[i].KillBehaviour(false);
        } 
        else
            _destroyGameObjectWhenKilled = false;

        OnKilled();
    }

    protected void DontKillSelfOnLoad()
    {
        DontDestroyOnLoad(gameObject);
        _persistent = true;
    }
}
