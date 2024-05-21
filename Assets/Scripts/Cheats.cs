using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cheats : MonoBehaviour
{
    [SerializeField] private GameState gameStateScript;
    [SerializeField] private TextMeshProUGUI infinitePlayerHealthActivityText;
    [SerializeField] private TextMeshProUGUI infinitePlayerCashActivityText;
    [SerializeField] private TextMeshProUGUI infiniteWaterActivityText;
    [SerializeField] private TextMeshProUGUI invincibleEnemiesActivityText;
    [SerializeField] private TextMeshProUGUI oneHitKillActivityText;

    public bool isInfinitePlayerHealth;
    public bool isInfiniteCash;
    public bool isInfiniteWater;
    public bool isInvincibleEnemies;
    public bool isOneHitKill;

    public Slider variableEnemyDamageOutputSlider;
    public Slider variableTowerRangeSlider;

    private void Start()
    {
        RestoreDefaults();
    }

    public void RestoreDefaults()
    {
        //Sets default variable values and text.
        isInfinitePlayerHealth = false;
        isInfiniteCash = false;
        isInfiniteWater = false;
        isInvincibleEnemies = false;
        isOneHitKill = false;
        infinitePlayerHealthActivityText.SetText("INACTIVE");
        infinitePlayerCashActivityText.SetText("INACTIVE");
        infiniteWaterActivityText.SetText("INACTIVE");
        invincibleEnemiesActivityText.SetText("INACTIVE");
        oneHitKillActivityText.SetText("INACTIVE");
        variableEnemyDamageOutputSlider.value = 1;
        variableTowerRangeSlider.value = 1;
    }

    public void InfinitePlayerHealth()
    {
        //IF isInfinitePlayerHealth is true, set to false...
        if (isInfinitePlayerHealth)
        {
            isInfinitePlayerHealth = false;
            infinitePlayerHealthActivityText.SetText("INACTIVE");
        }
        //ELSE IF isInfinitePlayerHealth is false, set to true.
        else if (!isInfinitePlayerHealth)
        {
            isInfinitePlayerHealth = true;
            infinitePlayerHealthActivityText.SetText("ACTIVE");
        }
    }

    public void InfinitePlayerCash()
    {
        //IF isInfiniteCash is true, set to false...
        if(isInfiniteCash)
        {
            isInfiniteCash = false;
            infinitePlayerCashActivityText.SetText("INACTIVE");
        }
        //ELSE IF isInfiniteCash is false, set to true.
        else if(!isInfiniteCash)
        {
            isInfiniteCash = true;
            infinitePlayerCashActivityText.SetText("ACTIVE");
        }
    }

    public void InfiniteWater()
    {
        //IF isInfiniteWater is true, set to false...
        if (isInfiniteWater)
        {
            isInfiniteWater = false;
            infiniteWaterActivityText.SetText("INACTIVE");
        }
        //ELSE IF isInfiniteWater is false, set to true.
        else if (!isInfiniteWater)
        {
            isInfiniteWater = true;
            infiniteWaterActivityText.SetText("ACTIVE");
        }
    }

    public void InvincibleEnemies()
    {
        //IF isInvincibleEnemies is true, set to false...
        if (isInvincibleEnemies)
        {
            isInvincibleEnemies = false;
            invincibleEnemiesActivityText.SetText("INACTIVE");
        }
        //ELSE IF isInvincibleEnemies is false...
        else if (!isInvincibleEnemies)
        {
            //IF isOneHitKill true, set to false.
            if (isOneHitKill)
            {
                isOneHitKill = false;
                oneHitKillActivityText.SetText("INACTIVE");
            }

            //Set isInvincibleEnemies to true.
            isInvincibleEnemies = true;
            invincibleEnemiesActivityText.SetText("ACTIVE");
        }
    }

    public void OneHitKillEnemies()
    {
        //IF isOneHitKill is true, set to false...
        if(isOneHitKill)
        {
            isOneHitKill = false;
            oneHitKillActivityText.SetText("INACTIVE");
        }
        //ELSE IF isOneHitKill is false...
        else if(!isOneHitKill)
        {
            //IF isInvincibleEnemies is true, set to false.
            if(isInvincibleEnemies)
            {
                isInvincibleEnemies = false;
                invincibleEnemiesActivityText.SetText("INACTIVE");
            }

            //Set isOneHitKill to true.
            isOneHitKill = true;
            oneHitKillActivityText.SetText("ACTIVE");
        }
    }

    public void CallSpawnInfantry()
    {
        //If a button equipped with this function is pressed, it will call a function that will spawn an Infantry unit.
        gameStateScript.ManualSpawnUnit(0);
    }

    public void CallSpawnRover()
    {
        //If a button equipped with this function is pressed, it will call a function that will spawn a Rover unit.
        gameStateScript.ManualSpawnUnit(1);
    }

    public void CallSpawnAPC()
    {
        //If a button equipped with this function is pressed, it will call a function that will spawn an APC unit.
        gameStateScript.ManualSpawnUnit(2);
    }

    public void CallSpawnTank()
    {
        //If a button equipped with this function is pressed, it will call a function that will spawn a Tank unit.
        gameStateScript.ManualSpawnUnit(3);
    }

    public void CallSpawnJet()
    {
        //If a button equipped with this function is pressed, it will call a function that will spawn a Jet unit.
        gameStateScript.ManualSpawnUnit(4);
    }
}
