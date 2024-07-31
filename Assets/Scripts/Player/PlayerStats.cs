using UnityEngine;
using TMPro;
using System.Globalization;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private SuperVillain superVillainScript;
        [SerializeField] private GameState gameStateScript;
        [SerializeField] private UserInterface userInterface;
        [SerializeField] private Cheats cheatsScript;

        public float health;
        public int cash;
        public float water;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI waterText;

        private void Start()
        {
            //Set the player's stat values to their default value.
            health = 100;
            cash = 500;
            water = 10;
            healthText.SetText(health.ToString(CultureInfo.InvariantCulture));
            cashText.SetText(cash.ToString());
            waterText.SetText(water.ToString(CultureInfo.InvariantCulture));
        }

        //Called when the player's health needs to be increased. Uses the value of gainHealth.
        public void IncreaseHealth(float gainHealth)
        {
            health += gainHealth;
            healthText.SetText(health.ToString(CultureInfo.InvariantCulture));
        }

        //Called when the player's health needs to be decreased. Uses the value of receivedDamage. Also calls the function to play the anger animation for the Super Villain.
        public void DecreaseHealth(float receivedDamage)
        {
            //IF isInfinitePlayerHealth is false...proceed.
            if (!cheatsScript.isInfinitePlayerHealth)
            {
                health -= receivedDamage;
                healthText.SetText(health.ToString(CultureInfo.InvariantCulture));
                superVillainScript.AngerState();

                //IF health reaches 0...call DeclareEndGame.
                if (health <= 0)
                {
                    gameStateScript.DeclareEndGame();
                }
            }
        }

        //Called when the player's cash value needs to be increased. Uses the value of rewardAmount.
        public void AwardCash(int rewardAmount)
        {
            cash += rewardAmount;
            cashText.SetText(cash.ToString());
            userInterface.AllowPurchasableSelection();
        }

        //Called when the player's cash value needs to be decreased. Uses the value of cost.
        public void SpendCash(int cost)
        {
            //IF isInfiniteCash is false...proceed.
            if (!cheatsScript.isInfiniteCash)
            {
                cash -= cost;
                cashText.SetText(cash.ToString());
            }
            userInterface.AllowPurchasableSelection();
        }

        //Called when the player's cash value needs to be decreased. Uses the value of cost. This version avoids 
        public void SpendCashTower(int cost)
        {
            cash -= cost;
            cashText.SetText(cash.ToString());
        }

        //Called when the amount of water needs to be increased. Uses the value of increaseWaterAmount.
        public void IncreaseWaterSupply(float increaseWaterAmount)
        {
            water += increaseWaterAmount;
            //IF water exceeds 100, bring it down to 100.
            if (water > 100)
            {
                water = 100;
            }

            waterText.SetText(water.ToString(CultureInfo.InvariantCulture));
        }

        //Called when the amount of water needs to be decreased. Uses the value of decreaseWaterAmount.
        public void DecreaseWaterSupply(float decreaseWaterAmount)
        {
            //IF isInfiniteWater is false...proceed.
            if (!cheatsScript.isInfiniteWater)
            {
                water -= decreaseWaterAmount;
                waterText.SetText(water.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
