using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayDataManager : GameBehaviour
{
    private static GameplayDataManager _instance = null;
    public static GameplayDataManager instance { get { return _instance; } }

    private EnemyManager enemyManager = null;

    [SerializeField]
    private int _score = 0;
    public int score { get { return _score; } }

    private float scoreMultipler = 1;
    private float multiplierExp = 0;
    private const float multiplierThreshold = 50;
    private int[] thresholdAmp = { 1, 2, 4, 8, 16, 48 };

    [SerializeField]
    private int _killCount = 0;
    public int killCount { get { return _killCount; } }
    [SerializeField]
    private float _time = 0;
    public float time { get { return _time; } }
    public string timeInString
    {
        get
        {
            string mili = Mathf.FloorToInt((_time * 1000f) % 1000).ToString("000");
            string sec = Mathf.FloorToInt(_time % 60).ToString("00");
            string min = Mathf.FloorToInt((_time / 60f) % 60).ToString("00");
            string hour = Mathf.FloorToInt((_time / 3600f)).ToString("00");
            return hour + ":" + min + ":" + sec + "." + mili;
        }
    }

    public IntEvent OnScoreChange = new IntEvent();
    public Vector3Event OnMultiplierChange = new Vector3Event();

    public override void GameAwake()
    {
        if (_instance == null)
            _instance = this;
        else
            KillBehaviour(true);
    }

    public override void GameStart()
    {
        enemyManager = EnemyManager.instance;
        
        Player player = DependencyContainer.GetDependency<Player>() as Player;
        player.OnReceiveDamage.AddListener(ResetMultiplier);

        enemyManager.OnEnemySpawned.AddListener(SubscribeEnemy);
        enemyManager.OnEnemyDied.AddListener(UnsubscribeEnemy);
        enemyManager.OnEnemyDied.AddListener(x => UpdateScoreAndKill(x));
    }

    public override void GameUpdate()
    {
        _time += Time.deltaTime;
    }

    public void UpdateMultiplier(float exp)
    {
        multiplierExp += exp;

        int level = 0;
        float trueThreshold = multiplierThreshold * thresholdAmp[level];
        while (multiplierExp >= trueThreshold)
        {
            level++;
            if (level == thresholdAmp.Length)
                break;
            trueThreshold = multiplierThreshold * thresholdAmp[level];
        }

        scoreMultipler = 1 + 0.5f * level;

        OnMultiplierChange.Invoke(new Vector3(multiplierExp, trueThreshold, scoreMultipler));
    }

    public void ResetMultiplier()
    {
        multiplierExp = 0;
        UpdateMultiplier(0);
    }

    public void SubscribeEnemy(Enemy enemy)
    {
        enemy.OnReceiveDamage.AddListener(UpdateMultiplier);
    }

    public void UnsubscribeEnemy(Enemy enemy)
    {
        enemy.OnReceiveDamage.RemoveListener(UpdateMultiplier);
    }

    public void UpdateScoreAndKill(Enemy enemy)
    {
        if(enemy.property.dieCountAsKill)
        {
            _score += (int)(enemy.property.score * scoreMultipler);
            _killCount++;

            OnScoreChange.Invoke(_score);
        }
    }
}
