using UnityEngine;
using UnityEngine.UI;

public class Tail : MonoBehaviour
{
    public DragonEnemy dragonEnemy;
    public DragonMovement dragon;

    public Slider healthSlider;
    public Slider enemyHealthSlider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            dragonEnemy.Health -= 5;
            enemyHealthSlider.value = dragonEnemy.Health;
            if (dragonEnemy.Health <= 0)
            {
                // Handle enemy death
                Destroy(dragonEnemy.gameObject);
            }
        }
        if (other.CompareTag("Player"))
        {
            dragon.Health -= 5;
            healthSlider.value = dragon.Health;
            if (dragon.Health <= 0)
            {
                // Handle player death
                Destroy(dragon.gameObject);
            }
        }
    }
}
