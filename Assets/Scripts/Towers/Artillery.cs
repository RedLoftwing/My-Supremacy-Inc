using Enemies;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Towers.Tower;

namespace Towers {
    public class Artillery : Tower {
        [Header("Artillery Specific Variables")]
        [SerializeField] private LayerMask terrainLayerMask;
        [SerializeField] private LayerMask enemyLayerMask;
        [HideInInspector] public bool isSelectingTarget;

        public GameObject targetHighlight;
        [SerializeField] private Animator artyBarrelAnimator;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float explosionRadius;
        private Collider[] enemyColliders = new Collider[100];

        protected override void TowerSpawned() {
            isSelectingTarget = true;
            base.TowerSpawned();
        }

        public void ConfirmTarget() {
            isSelectingTarget = false;
            WorldInteraction.Instance.isClickDisabled = false;
        }

        protected override void UpdateSystems() {
            if (isSelectingTarget) SetTargetLocation();
            if (!GameState.Instance.IsInterWave) {
                FireWeapon(aimPoints[0], nearbyTargets[0]);
            }
        }

        protected override void FireWeapon(AimPoint inAimPoint, GameObject inGO) {
            aimPoints[0].timeSinceLastFired += Time.deltaTime;
            if (aimPoints[0].timeSinceLastFired > RateOfFire) StartCoroutine(FireProcedure());
        }

        private IEnumerator FireProcedure() {
            // Visualise the firing process. Playing the barrel animation, and each of the particle systems.
            aimPoints[0].timeSinceLastFired = 0;
            artyBarrelAnimator.SetTrigger("trFire");
            foreach (var pS in aimPoints[0].pS) pS.Play();
            yield return new WaitForSeconds(0.5f);

            // Create explosion at the target point, get all colliders within the radius, and go through each to find the enemy component.
            Instantiate(explosionPrefab, nearbyTargets[0].transform.position, Quaternion.identity);
            var numColliders = Physics.OverlapSphereNonAlloc(nearbyTargets[0].transform.position, explosionRadius, enemyColliders, enemyLayerMask);
            if (numColliders > 0) {
                for (int i = 0; i < numColliders; i++) {
                    enemyColliders[i].TryGetComponent<Enemy>(out var enemyToDamage);
                    if (!enemyToDamage) yield return null;
                    // Check to ensure that the detected enemy type is a valid target for this tower.
                    foreach (var targetType in scriptableObject.targetTypes) {
                        if (enemyToDamage.gameObject.CompareTag(targetType.ToString())) {
                            // Calculate damage based on if the enemy is "Armoured", then deal damage to it.
                            var damageToDeal = Damage;
                            if (enemyToDamage.CompareTag("Armoured")) damageToDeal *= 2f;
                            enemyToDamage.DecreaseHealth(damageToDeal);
                        }
                    }
                }
            }

            //Wait set amount of seconds after spawning...then set isFireRunning to false.
            yield return new WaitForSeconds(RateOfFire);
        }

        protected override void UpdateParticleSystem() {
            foreach (var pS in aimPoints[0].pS) {
                if (!GameState.Instance.IsInterWave) pS.Play();
                else pS.Stop();
            }
        }

        protected override void CheckTurretRotations() {
            if (isSelectingTarget) {
                foreach (var turretAxis in turretAxes) {
                    if (turretAxis.turretAxisType is not (TurretAxisTypes.Horizontal or TurretAxisTypes.Vertical)) continue;
                    turretAxis.turretAxisPiece.transform.LookAt(nearbyTargets[0].transform);
                }
            }
        }

        private void SetTargetLocation() {
            if (!Camera.main) return;
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 2000, terrainLayerMask)) {
                //Determines the required terrain Y axis offset.
                Vector3 terrainOffset;
                switch (hitInfo.collider.gameObject.tag) {
                    case "Height-0.0":
                        terrainOffset = Vector3.zero;
                        break;
                    case "Height-2.5":
                        terrainOffset = new Vector3(0.0f, 2.5f, 0.0f);
                        //_terrainOffset = Vector3.up * 2.5f;
                        break;
                    case "Height-5.0":
                        terrainOffset = new Vector3(0.0f, 5f, 0.0f);
                        break;
                    default:
                        terrainOffset = Vector3.zero;
                        break;
                }

                // Ensures the target highlight object is set to active.
                targetHighlight.gameObject.SetActive(true);

                // Set targetHighlight and the nearbyTarget to the impact point of the raycast.
                var tilePos = hitInfo.collider.gameObject.transform.position;
                targetHighlight.transform.position = tilePos + terrainOffset;
                nearbyTargets[0].transform.position = tilePos + terrainOffset;

                CheckTurretRotations();
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(nearbyTargets[0].transform.position, explosionRadius);
        }
    }
}