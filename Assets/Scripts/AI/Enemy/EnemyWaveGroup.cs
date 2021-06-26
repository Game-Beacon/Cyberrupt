using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyWaveGroup")]
public class EnemyWaveGroup : ScriptableObject
{
    [System.Serializable]
    private struct SpecialWave
    {
        public EnemyWave wave;
        public int spawnIndex;
    }

    [SerializeField]
    private int waveCount;

    [SerializeField]
    private List<EnemyWave> waveCandidates = new List<EnemyWave>();

    [SerializeField]
    private List<SpecialWave> specials = new List<SpecialWave>();

    public EnemyWave[] GetWaves()
    {
        EnemyWave[] waves = new EnemyWave[waveCount];
        Dictionary<int, List<EnemyWave>> difficultyDict = new Dictionary<int, List<EnemyWave>>();

        //把關卡根據難度分類
        foreach(EnemyWave w in waveCandidates)
        {
            if (difficultyDict.ContainsKey(w.difficulty))
                difficultyDict[w.difficulty].Add(w);
            else
            {
                difficultyDict.Add(w.difficulty, new List<EnemyWave>());
                difficultyDict[w.difficulty].Add(w);
            } 
        }

        //取得所有難度，並排序
        List<int> difficulties = new List<int>(difficultyDict.Keys);
        difficulties.Sort();

        //根據難度數量來決定每種難度需要幾關
        //比如：需要40波敵人，總共有5種難度
        //則每種難度會挑8波出來（有可能會有重複的關卡）
        int[] levelCount = new int[difficulties.Count];
        int countBase = waveCount / difficulties.Count;
        int extra = waveCount % difficulties.Count;
        for (int i = 0; i < difficulties.Count; i++)
            levelCount[i] = (i < extra) ? countBase + 1 : countBase;

        //填入關卡
        int index = 0;
        for(int i = 0; i < difficulties.Count; i++)
        {
            int rand = -1;
            int previousRand = -1;
            for (int j = 0; j < levelCount[i]; j++)
            {
                while(rand == previousRand)
                    rand = Random.Range(0, difficultyDict[difficulties[i]].Count);
                previousRand = rand;
                waves[index] = difficultyDict[difficulties[i]][rand];
                index++;
            }
        }

        //把特殊關卡覆蓋上去
        foreach (SpecialWave s in specials)
            waves[s.spawnIndex] = s.wave;

        return waves;
    }

    public void SetCandidates(List<EnemyWave> candidates)
    {
        waveCandidates = candidates;
    }
}
