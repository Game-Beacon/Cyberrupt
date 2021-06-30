using UnityEngine;

public class Bot_Death : MonoBehaviour
{
    [SerializeField]
    private float deathDelay;

    void Start()
    {
        Destroy(gameObject, this.deathDelay);
    }
}