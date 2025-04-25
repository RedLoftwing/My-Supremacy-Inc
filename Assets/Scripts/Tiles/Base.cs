using UnityEngine;

public class Base : MonoBehaviour
{
    //On trigger collision enter...
    private void OnTriggerEnter(Collider other)
    {
        //Grab enemy component, and store it as collision.
        Enemies.Enemy collision = other.GetComponent<Enemies.Enemy>();
        //IF collision is not null...Decrease player's health/lives with the strength value multiplied by the slider value, and then destroy the collided unit.
        if (collision != null)
        {
            Player.PlayerStats.Instance.DecreaseHealth(collision.strength * Cheats.Instance.variableEnemyDamageOutputSlider.value);
            Destroy(other.gameObject);
        }
    }
}
