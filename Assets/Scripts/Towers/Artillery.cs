using System.Collections;
using UnityEngine;

namespace Towers
{
    public class Artillery : FixedTargetTower
    {
        [SerializeField] private GameState gameStateScript;
        [SerializeField] private Vector3 midPoint;

        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float explosionRadius;
        [SerializeField] private LayerMask enemyLayerMask;
        private GameObject _explosion;

        [SerializeField] private GameObject barrel;

        [SerializeField] private Quaternion lookAtMidPoint;
        [SerializeField] private Quaternion limitLookAtMidPoint;

        [SerializeField] private Vector3 targetDir;
        [SerializeField] private Animator artyBarrelAnimator;

        private void Start()
        {
            //Grabs the GameState component from the Manager object and stores it.
            gameStateScript = GameObject.Find("Manager").GetComponent<GameState>();
        }

        private void Update()
        {
            StartCoroutine(ManualUpdate());

            //Calculate the mid-point between the turret and the target, and store as midPoint.
            midPoint = Vector3.Lerp(turret.transform.position, currentTarget, 0.5f);
            //Raise midPoint up to Y: 25.
            midPoint = new Vector3(midPoint.x, 25, midPoint.z);
            //Rotate the barrel to look at the midpoint.
            barrel.transform.LookAt(midPoint);

            //IF isFireRunning is false...start the FireWeapon coroutine.
            if (!isFireRunning)
            {
                StartCoroutine(FireWeapon());
            }
        }

        private IEnumerator FireWeapon()
        {
            //Checks if the game is in between waves. If it is, the weapon will not fire.
            if (!gameStateScript.isInterWave)
            {
                //Sets isFireRunning to true. Prevents constant execution of FireTurret.
                isFireRunning = true;
                //Plays the barrel animation, then wait 1 second.
                artyBarrelAnimator.SetTrigger("trFire");
                yield return new WaitForSeconds(0.5f);
                //Spawn explosion object at current target, and play the explosion sound effect.
                _explosion = Instantiate(explosionPrefab, currentTarget, Quaternion.identity);
                weaponFire.Play();
                //Gathers all colliders within the "explosions" radius...
                int numColliders = Physics.OverlapSphereNonAlloc(_explosion.transform.position, explosionRadius, Colliders, enemyLayerMask);
                //IF the number of colliders is greater than 0...then go through each collider, and find the enemy component.
                if (numColliders > 0)
                {
                    for (int i = 0; i < numColliders; i++)
                    {
                        var enemyToDamage = Colliders[i].GetComponent<Enemies.Enemy>();
                        //IF the enemy component is true...Go through each string within the validTargets array, and check if the enemy tag matches any...
                        if (enemyToDamage != null)
                        {
                            foreach (var validTarget in validTargets)
                            {
                                //IF an enemy tag matches a validTarget string...Deal damage to each enemy.
                                if (enemyToDamage.CompareTag(validTarget))
                                {
                                    //TODO: Alert so defaultDamage influences the damage value used (Could be affected by nearby Intel Centres). Maybe look at this script's parent class (FixedTargetTower) function ManualUpdate.
                                    enemyToDamage.DecreaseHealth(scriptableObject.defaultDamage);
                                }
                            }
                        }
                    }
                }

                //Wait set amount of seconds after spawning...then set isFireRunning to false.
                yield return new WaitForSeconds(RateOfFire);
                isFireRunning = false;
            }
        }
    }
}