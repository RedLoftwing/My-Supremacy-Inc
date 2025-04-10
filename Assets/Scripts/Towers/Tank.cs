using System.Collections;
using UnityEngine;

namespace Towers
{
    public class Tank : ProximityTowerMultiTarget
    {
        public ParticleSystem[] particleSystems;
        private Coroutine _attackCannonCoroutine;
        private Coroutine _attackMachineGunCoroutine;
        private Transform _cannonTarget;
        private Transform _machineGunTarget;
        private float _cannonCooldown;
        private float _machineGunCooldown;
        
        private void Start()
        {
            //Sets necessary protected variables.
            TowerSpawned();
        }

        private void Update()
        {
            availableTargets.RemoveAll(target => target == null || !target.activeInHierarchy);
            
            _cannonCooldown -= Time.deltaTime;
            _machineGunCooldown -= Time.deltaTime;
            
            switch (availableTargets.Count)
            {
                case > 0:
                    float singleStep = scriptableObject.rotationSpeed * Time.deltaTime;
                    if(_machineGunTarget) RotateTurret(horizontalTurret[1], singleStep, 'y', _machineGunTarget.gameObject);
                    _attackCannonCoroutine = StartCoroutine(AttackCannon());
                    _attackMachineGunCoroutine = StartCoroutine(AttackMachineGun());
                    break;
                case <= 0 when _attackCannonCoroutine != null && _attackMachineGunCoroutine != null:
                    StopCoroutine(_attackCannonCoroutine);
                    StopCoroutine(_attackMachineGunCoroutine);
                    _attackCannonCoroutine = null;
                    _attackMachineGunCoroutine = null;
                    foreach (var pS in particleSystems) { pS.Stop(); }
                    break;
            }
        }

        private IEnumerator AttackCannon()
        {
            _cannonTarget = FindNewCannonTarget();
            if (_cannonTarget && _cannonCooldown < 0)
            {
                foreach (var pS in particleSystems) 
                {
                    if (!pS.isPlaying)
                    {
                        pS.Play();
                    }
                }
                // NOTE: Replace "2" with scriptableObject damage value.
                _cannonTarget.GetComponent<Enemies.Enemy>().DecreaseHealth(2);
                Debug.Log($"Dealing CANNON damage to: {_cannonTarget.gameObject.name}");
                _cannonCooldown = scriptableObject.defaultRateOfFire;
            }
            else
            {
                foreach (var pS in particleSystems) { pS.Stop(); }
            }
            yield return null;
        }
        
        private IEnumerator AttackMachineGun()
        {
            _machineGunTarget = FindNewMachineGunTarget();
            Debug.Log(_machineGunTarget);
            if (_machineGunTarget && _machineGunCooldown < 0)
            {
                foreach (var pS in particleSystems) 
                {
                    if (!pS.isPlaying)
                    {
                        pS.Play();
                    }
                }
                // NOTE: Replace "2" with scriptableObject damage value.
                _machineGunTarget.GetComponent<Enemies.Enemy>().DecreaseHealth(2);
                Debug.Log($"Dealing MachineGun damage to: {_machineGunTarget.gameObject.name}");
                _machineGunCooldown = scriptableObject.defaultRateOfFire;
            }
            else
            {
                foreach (var pS in particleSystems) { pS.Stop(); }
            }
            yield return null;
        }

        private Transform FindNewCannonTarget()
        {
            // Loop through available targets...IF a target is tagged with "Armoured"...assign its transform to _cannonTarget and exit the loop.
            foreach (var target in availableTargets)
            {
                if (!target.gameObject.CompareTag("Armoured")) continue;
                return target.transform;
            }
            return null;
        }

        private Transform FindNewMachineGunTarget()
        {
            // Loop through available targets...IF a target is NOT tagged with "Armoured"...assign its transform to _machineGunTarget and exit the loop.
            foreach (var target in availableTargets)
            {
                if (target.gameObject.CompareTag("Armoured")) continue;
                return target.transform;
            }
            return null;
        }
    }
}