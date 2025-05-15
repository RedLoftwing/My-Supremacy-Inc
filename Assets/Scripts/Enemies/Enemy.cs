using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected float health;
        private float _defaultHealth;
        public float strength;

        [SerializeField] private bool isExplosive;
        [SerializeField] private GameObject explosionVFX;

        private float _distanceToBase;
        private float _distanceToNextPoint;
        
        private SplineAnimate _splineAnimate;

        private float _timeSinceDamageReceived;

        [SerializeField] private Rigidbody rb;

        private void Start()
        {
            //Sets _defaultHealth value.
            _defaultHealth = health;
            
            //Set up the spline to use and follow along.
            _splineAnimate = GetComponent<SplineAnimate>();
            var splineTag = !CompareTag("Aerial") ? "Spline1" : "Spline2";
            _splineAnimate.Container = GameObject.FindGameObjectWithTag(splineTag).GetComponent<SplineContainer>();
            _splineAnimate.Play();
        }

        private void Update()
        {
            _timeSinceDamageReceived += Time.deltaTime;
        }

        //Called when health needs to be decreased.
        public void DecreaseHealth(float inReceivedDamage)
        {
            // IF enemies are set to invincible in the cheats menu...return.
            if (Cheats.Instance.isInvincibleEnemies) return;
            // IF enemies are NOT set to be instantly killed when hit...proceed.
            if (!Cheats.Instance.isOneHitKill)
            {
                // Only proceed to apply a new dose of damage to the unit when sufficient time has passed (1.35f currently).
                if (!(_timeSinceDamageReceived > 1.35f)) return;
                
                //Reduce the value of health by the value of receivedDamage.
                health -= inReceivedDamage;
                _timeSinceDamageReceived = 0;

                // Only proceed to destroy the unit IF health is below 1.
                if (health > 0) return;

                StartCoroutine(DestroyThisUnit());
            }
            else
            {
                StartCoroutine(DestroyThisUnit());
            }
        }

        private IEnumerator DestroyThisUnit()
        {
            // IF this unit is set to be explosive (i.e. fueled vehicles)...create an explosion at this unit's position.
            if (isExplosive)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(1f);
            
            //Destroy this enemy unit.
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            //On collision enter...get wave component. IF not null, start coroutine.
            other.gameObject.TryGetComponent<Wave>(out var waveComp);
            if (waveComp)
            {
                Debug.Log("Wave collision");
                StartCoroutine(Freeze(4));
            }

            other.gameObject.TryGetComponent<WaterCell>(out var waterCell);
            if (waterCell)
            {
                transform.position = waterCell.gameObject.transform.position;
                StartCoroutine(Freeze(6));
            }
        }

        private IEnumerator Freeze(int duration)
        {
            //Freeze the unit.
            rb.constraints = RigidbodyConstraints.FreezePosition;
            _splineAnimate.Pause();
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
            rb.constraints = RigidbodyConstraints.None;
            _splineAnimate.Play();
        }

        private void OnDestroy()
        {
            //On destruction of this game object, remove 1 from the totalWaveEnemyCount value.
            GameState.Instance.totalWaveEnemyCount--;
        }
    }
}