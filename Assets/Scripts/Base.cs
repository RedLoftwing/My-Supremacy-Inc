using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Player.PlayerStats playerStatsScript;
    private Cheats _cheatsScript;

    private void Start()
    {
        //Grab cheats component and store it as cheatsScript.
        _cheatsScript = GameObject.Find("Manager").GetComponent<Cheats>();
    }

    //On trigger collision enter...
    private void OnTriggerEnter(Collider other)
    {
        //Grab enemy component, and store it as collision.
        Enemies.Enemy collision = other.GetComponent<Enemies.Enemy>();
        //IF collision is not null...Decrease player's health/lives with the strength value multiplied by the slider value, and then destroy the collided unit.
        if (collision != null)
        {
            playerStatsScript.DecreaseHealth(collision.strength * _cheatsScript.variableEnemyDamageOutputSlider.value);
            Destroy(other.gameObject);
        }
    }
}
