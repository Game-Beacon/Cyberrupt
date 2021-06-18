using UnityEngine;

public class BotDeathSubscriber : MonoBehaviour
{
    [SerializeField]
    private GameObject deathVFX;

    void Start()
    {
        var bot = GetComponent<Bot>();            
        bot.OnDeath += () => this.createVFX(bot.transform);
    }

    private void createVFX(Transform bot)
    {
        Instantiate(this.deathVFX, bot.position, Quaternion.identity);
    }
}
