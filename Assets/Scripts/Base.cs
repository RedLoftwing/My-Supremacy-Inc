using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStatsScript;
    private Cheats cheatsScript;

    private void Start()
    {
        //Grab cheats component and store it as cheatsScript.
        cheatsScript = GameObject.Find("Manager").GetComponent<Cheats>();
    }

    //On trigger collision enter...
    private void OnTriggerEnter(Collider other)
    {
        //Grab enemy component, and store it as collision.
        Enemy collision = other.GetComponent<Enemy>();
        //IF collision is not null...Decrease player's health/lives with the strength value multiplied by the slider value, and then destroy the collided unit.
        if (collision != null)
        {
            playerStatsScript.DecreaseHealth(collision.strength * cheatsScript.variableEnemyDamageOutputSlider.value);
            Destroy(other.gameObject);
        }
    }
}
