using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelBackGround : GameBehaviour
{
    [SerializeField]
    private List<BGLoop> loops;
    [SerializeField]
    private float bgwidth;
    private List<float> loopDistance = new List<float>();

    public override void GameAwake()
    {
        for(int i = 0; i < loops.Count; i++)
            loopDistance.Add(0);
    }

    public override void GameUpdate()
    {
        for(int i = 0; i < loops.Count; i++)
        {
            float delta = loops[i].loopSpeed * Time.deltaTime;
            loops[i].left.transform.position += Vector3.left * delta;
            loops[i].right.transform.position += Vector3.left * delta;
            loopDistance[i] += delta;
            while(loopDistance[i] >= bgwidth)
            {
                loops[i].left.transform.position += Vector3.right * 2 * bgwidth;

                loopDistance[i] -= bgwidth;
                GameObject temp = loops[i].left;
                loops[i].left = loops[i].right;
                loops[i].right = temp;
            }
        }
    }

    [System.Serializable]
    private class BGLoop
    {
        public GameObject left;
        public GameObject right;
        public float loopSpeed;
    }
}
