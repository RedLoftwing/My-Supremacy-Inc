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
        
        private enum Particles
        {
            CannonFire = 0,
            CannonSmoke = 1,
            MgFire = 2
        }
        
        private void Start()
        {
            //Sets necessary protected variables.
            TowerSpawned();
        }

        // private void Update()
        // {
        //     availableTargets.RemoveAll(target => target == null || !target.activeInHierarchy);
        //     
        //     _cannonCooldown -= Time.deltaTime;
        //     _machineGunCooldown -= Time.deltaTime;
        //     
        //     switch (availableTargets.Count)
        //     {
        //         case > 0:
        //             float singleStep = scriptableObject.rotationSpeed * Time.deltaTime;
        //             if(_cannonTarget) RotateTurret(horizontalTurret[0], singleStep, 'y', _cannonTarget.gameObject);
        //             if (_machineGunTarget && _machineGunTarget.gameObject.CompareTag("Aerial"))
        //             {
        //                 RotateTurret(horizontalTurret[1], singleStep, 'y', _machineGunTarget.gameObject);
        //                 RotateTurret(verticalTurret[1], singleStep, 'x', _machineGunTarget.gameObject);
        //             }
        //             else if (_machineGunTarget)
        //             {
        //                 RotateTurret(horizontalTurret[1], singleStep, 'y', _machineGunTarget.gameObject);
        //             }
        //             _attackCannonCoroutine = StartCoroutine(AttackCannon());
        //             _attackMachineGunCoroutine = StartCoroutine(AttackMachineGun());
        //             break;
        //         case <= 0 when _attackCannonCoroutine != null && _attackMachineGunCoroutine != null:
        //             StopCoroutine(_attackCannonCoroutine);
        //             StopCoroutine(_attackMachineGunCoroutine);
        //             _attackCannonCoroutine = null;
        //             _attackMachineGunCoroutine = null;
        //             foreach (var pS in particleSystems) { pS.Stop(); }
        //             break;
        //     }
        // }

        private IEnumerator AttackCannon()
        {
            _cannonTarget = FindNewCannonTarget();
            if (_cannonTarget)
            {
                if(!(_cannonCooldown < 0)) yield break;
                if (!particleSystems[(int)Particles.CannonFire].isPlaying)
                {
                    particleSystems[(int)Particles.CannonFire].Stop();
                    particleSystems[(int)Particles.CannonFire].Play();
                }
                if (!(particleSystems[(int)Particles.CannonSmoke].isPlaying))
                {
                    particleSystems[(int)Particles.CannonSmoke].Stop();
                    particleSystems[(int)Particles.CannonSmoke].Play();
                }
                _cannonTarget.GetComponent<Enemies.Enemy>().DecreaseHealth(scriptableObject.defaultDamage);
                _cannonCooldown = scriptableObject.defaultRateOfFire;
            }
            yield return null;
        }
        
        private IEnumerator AttackMachineGun()
        {
            _machineGunTarget = FindNewMachineGunTarget();
            if (_machineGunTarget)
            {
                if(!(_machineGunCooldown < 0))
                if (!particleSystems[(int)Particles.MgFire].isPlaying)
                {
                    particleSystems[(int)Particles.MgFire].Stop();
                    particleSystems[(int)Particles.MgFire].Play();
                }
                _machineGunTarget.GetComponent<Enemies.Enemy>().DecreaseHealth(2);
                _machineGunCooldown = 1.5f;
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