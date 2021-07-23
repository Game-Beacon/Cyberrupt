using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    private static GameSystem _instance = null;
    public static GameSystem instance { get { return _instance; } private set { } }

    private int ID = 0;
    private List<GameBehaviour> behaviours = new List<GameBehaviour>();
    private Queue<GameBehaviour> awakeQueue = new Queue<GameBehaviour>();
    private Queue<GameBehaviour> startQueue = new Queue<GameBehaviour>();
    private Queue<GameBehaviour> destroyQueue = new Queue<GameBehaviour>();

    private void Awake()
    {
        //Debug.Log("Awake");
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Update is called once per frame
    private void Update()
    {
        if (TimeManager.paused)
            return;

        foreach (GameBehaviour behaviour in behaviours)
            if (behaviour != null && behaviour.update)
                behaviour.GameUpdate();
    }

    private void LateUpdate()
    {
        bool behavioursChanged = (awakeQueue.Count > 0);

        while (awakeQueue.Count > 0)
        {
            GameBehaviour b = awakeQueue.Dequeue();
            behaviours.Add(b);
            if (b.update)
                b.GameAwake();
            startQueue.Enqueue(b);
        }

        while (startQueue.Count > 0)
        {
            GameBehaviour b = startQueue.Dequeue();
            if (b.update)
                b.GameStart();
        }

        //如果某個物件因某種原因丟失reference的話（通常是因為在destroy時沒解除綁定），移除該reference
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            if (behaviours[i] == null)
                behaviours.RemoveAt(i);
        }

        if(behavioursChanged)
        {
            behaviours.Sort(delegate (GameBehaviour a, GameBehaviour b)
            {
                return a.executeOrder.CompareTo(b.executeOrder);
            });
        }

        while (destroyQueue.Count > 0)
        {
            GameBehaviour behaviour = destroyQueue.Dequeue();
            if(behaviours.Contains(behaviour))
            {
                behaviours.Remove(behaviour);
                if (behaviour.destroyGameObjectWhenKilled)
                    Destroy(behaviour.gameObject);
                else
                    Destroy(behaviour);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (GameBehaviour behaviour in behaviours)
            if (behaviour != null && behaviour.update)
                behaviour.GameFixedUpdate();
    }

    public void AddBehaviour(GameBehaviour behaviour, ref int id)
    {
        if (id >= 0)
            throw new System.Exception("Invalid id number when trying to add new behaviour.");

        id = ID;
        ID++;

        awakeQueue.Enqueue(behaviour);
    }

    public void RemoveAndKillBehaviour(GameBehaviour behaviour)
    {
        if(behaviour.id < 0)
            throw new System.Exception("Invalid id number when trying to remove new behaviour.");

        destroyQueue.Enqueue(behaviour);
    }

    void OnSceneUnloaded(Scene scene)
    {
        //這是為了確保所有非static/singleton的物體會在場景轉換時自動清除reference
        DependencyContainer.Clear();

        int len = behaviours.Count - 1;
        for (int i = len; i >= 0; i--)
        {
            if (behaviours[i] == null)
            {
                behaviours.RemoveAt(i);
                continue;
            }
            if (behaviours[i] != null && !behaviours[i].persistent)
            {
                behaviours[i].OnKilled();
                behaviours.RemoveAt(i);
            }                
        }
        destroyQueue.Clear();
    }
}
