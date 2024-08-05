using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private GameState gameStateScript;
        [SerializeField] private Cheats cheatsScript;
        [SerializeField] protected float health;
        private float _defaultHealth;
        public float strength;

        [SerializeField] private bool isExplosive;
        [SerializeField] private GameObject explosionVFX;

        private float _distanceToBase;
        private float _distanceToNextPoint;
        
        private SplineAnimate _splineAnimate;

        private void Start()
        {
            //Grab components and stores them as their own variables.
            gameStateScript = GameObject.Find("Manager").GetComponent<GameState>();
            cheatsScript = GameObject.Find("Manager").GetComponent<Cheats>();

            //Sets _defaultHealth value.
            _defaultHealth = health;

            //Set up the spline to use and follow along.
            _splineAnimate = this.GetComponent<SplineAnimate>();
            var splineTag = !this.CompareTag("Aerial") ? "Spline1" : "Spline3";
            _splineAnimate.Container = GameObject.FindGameObjectWithTag(splineTag).GetComponent<SplineContainer>();
            _splineAnimate.Play();
        }

        //Called when health needs to be decreased.
        public void DecreaseHealth(float receivedDamage)
        {
            //IF isInvincibleEnemies is false...proceed.
            if (!cheatsScript.isInvincibleEnemies)
            {
                //IF isOneHitKill is false...proceed.
                if (!cheatsScript.isOneHitKill)
                {
                    //Reduce the value of health by the value of receivedDamage.
                    health -= receivedDamage;

                    //IF health is less than or equal to 0...destroy this enemy unit.
                    if (health <= 0)
                    {
                        //IF isExplosive is true AND application is still playing...spawn an explosion on this position.
                        if (isExplosive)
                        {
                            Instantiate(explosionVFX, transform.position, Quaternion.identity);
                        }

                        Destroy(this.gameObject);
                    }
                }
                //ELSE
                else
                {
                    //Destroy this enemy unit.
                    Destroy(this.gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //On collision enter...get wave component. IF not null, start coroutine.
            var waveComp = other.gameObject.GetComponent<Wave>();
            if (waveComp)
            {
                StartCoroutine(Freeze(4));
            }

            var waterCell = other.gameObject.GetComponent<WaterCell>();
            if (waterCell)
            {
                transform.position = waterCell.gameObject.transform.position;
                StartCoroutine(Freeze(6));
            }
        }

        private IEnumerator Freeze(int duration)
        {
            //Freeze the unit.
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            //IF duration is less than or equal to 4...
            if (duration <= 4)
            {
                //Wait 4 seconds...
                yield return new WaitForSeconds(duration);
            }
            //ELSE...
            else
            {
                //Wait 2 seconds.
                yield return new WaitForSeconds(2);
                //Take a fraction of max health away from the unit.
                DecreaseHealth(_defaultHealth / 10);
                //Wait 2 seconds.
                yield return new WaitForSeconds(2);
                //Take a fraction of max health away from the unit.
                DecreaseHealth(_defaultHealth / 10);
                //Wait 2 seconds.
                yield return new WaitForSeconds(2);
                //Take a fraction of max health away from the unit.
                DecreaseHealth(_defaultHealth / 10);

            }

            //Unfreeze the unit.
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        private void OnDestroy()
        {
            //On destruction of this game object, remove 1 from the thisWavesEnemies value.
            gameStateScript.thisWavesEnemies--;
        }
    }
}