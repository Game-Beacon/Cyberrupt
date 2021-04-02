using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PropertyManager : GameBehaviour
{
    private static PropertyManager _instance = null;
    public static PropertyManager instance { get { return _instance; } private set { } }

    [SerializeField]
    private PlayerBaseData playerBase;
    private PlayerData _playerData;
    public PlayerData playerData { get { return _playerData; } private set { } }

    [SerializeField]
    private BossBaseData bossBase;
    private BossData _bossData;
    public BossData bossData { get { return _bossData; } private set { } }

    public override void GameAwake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontKillSelfOnLoad();
        }
        else
        {
            KillBehaviour(true);
            return;
        }

        playerBase = Resources.LoadAll<PlayerBaseData>("").FirstOrDefault();
        bossBase = Resources.LoadAll<BossBaseData>("").FirstOrDefault();

        _playerData = new PlayerData(playerBase);
        _bossData = new BossData(bossBase);
    }
}
