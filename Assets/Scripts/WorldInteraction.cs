using Towers;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WorldInteraction : MonoBehaviour
{
    [SerializeField] private UserInterface userInterfaceScript;
    [SerializeField] private Player.PlayerStats playerStatsScript;
    [SerializeField] private Cheats cheatsScript;

    public static bool IsClickDisabled;
    [SerializeField] private Transform gridHighlight;
    public SO_Scripts.TowerInfo heldTower;
    private GameObject _lastPlacedTower;
    
    private GameObject _activeHologramTower;

    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private LayerMask towerLayerMask;

    private TileInfo _currentTileInfo;
    private TileInfo _previousTileInfo;

    public SO_Scripts.TowerInfo[] towerInfo; 
    
    private readonly Collider[] _localColliders = new Collider[10];
    private Tower _localTower;

    private Transform _test;
    
    private void Update()
    {
        //IF the pointer is over UI...set _isClickDisabled to true...ELSE set to false. Prevents interaction of elements behind UI.
        
        if(EventSystem.current.IsPointerOverGameObject())
        {
            IsClickDisabled = true;
        }
        else
        {
            IsClickDisabled = false;
        }

        //IsClickDisabled = EventSystem.current.IsPointerOverGameObject();
        //Debug.Log($"Click is disabled? {IsClickDisabled}");
    }

    public void OnLeftMouseButton()
    {
        // For Artillery: IF there is a lastPlacedTower...Try get it's FixedTargetTower component...IF successful...set targeting position.
        if (_lastPlacedTower && _lastPlacedTower.TryGetComponent<FixedTargetTower>(out var fixedTargetTower))
        {
            if (fixedTargetTower && fixedTargetTower.isSelectingTarget)
            {
                fixedTargetTower.ConfirmTarget();
            }
        }
        //IF the pointer is not over UI...then exit function, otherwise continue. 
        if (IsClickDisabled) return;
        
        //Conduct a raycast from the camera to where the player has clicked, and store it.
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //If the raycast hits something on the specified layer mask...
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10000, terrainLayerMask))
        {
            //Get the TileInfo component from the tile's child, and store it.
            var tileInfo = hitInfo.collider.GetComponentInChildren<TileInfo>();
            //IF tileInfo is true...
            if(tileInfo)
            {
                // -- Grid Highlight activation --
                //Ensures the grid highlight object is set to active, and moves it to the build point of the tile.
                gridHighlight.gameObject.SetActive(true);
                gridHighlight.position = tileInfo.transform.position;
                
                
                //ClearLocalTower();
                if (_localTower && _localTower.detectionRadiusCylinder)
                {
                    Debug.Log($"Local Tower called {_localTower.name} has been found. Deactivating cylinder.");
                    _localTower.detectionRadiusCylinder.SetActive(false);
                }
                _localTower = null;
                Debug.Log("Local Tower is null.");

                
                
                // -- Detection Radius Cylinder activation & FixedTargetTower retargeting --
                // Check if a tower is in this spot...IF there is a tower, show the range of the tower by activating the detection radius cylinder.
                if (!tileInfo.isTileAvailable)
                {
                    // for (int i = 0; i < _localColliders.Length; i++)
                    // {
                    //     _localColliders.SetValue(null, i);
                    // }
                    _test = tileInfo.transform;
                    // Physics.OverlapSphereNonAlloc(tileInfo.transform.position, 2, _localColliders, towerLayerMask);
                    // foreach (var localCollider in _localColliders)
                    // {
                    //     if (localCollider)
                    //     {
                    //         if (localCollider.TryGetComponent(out _localTower))
                    //         {
                    //             Debug.Log($"Local Tower: {_localTower.name}");
                    //             if (_localTower.detectionRadiusCylinder)
                    //             {
                    //                 _localTower.detectionRadiusCylinder.SetActive(true);
                    //             }
                    //             else if (_localTower.TryGetComponent(out FixedTargetTower localFixedTargetTower))
                    //             {
                    //                 Debug.Log($"Local Fixed Target Tower: {localFixedTargetTower.name}");
                    //                 localFixedTargetTower.isSelectingTarget = true;
                    //             }
                    //             else
                    //             {
                    //                 Debug.Log("Nada");
                    //             }
                    //         }
                    //     }
                    //     break;
                    // }
                    Physics.OverlapSphereNonAlloc(tileInfo.transform.position, 1, _localColliders, towerLayerMask);
                    for (int i = 0; i < _localColliders.Length; i++)
                    {
                        if (_localColliders[i].TryGetComponent(out _localTower))
                        {
                            Debug.Log($"Local Tower: {_localTower.name}");
                            if (_localTower.detectionRadiusCylinder)
                            {
                                _localTower.detectionRadiusCylinder.SetActive(true);
                            }
                            else if (_localTower.TryGetComponent(out FixedTargetTower localFixedTargetTower))
                            {
                                Debug.Log($"Local Fixed Target Tower: {localFixedTargetTower.name}");
                                localFixedTargetTower.isSelectingTarget = true;
                            }
                        }
                    }

                }

                //IF a tower in the tower menu has been selected...
                if (heldTower)
                {
                    //IF the tile is unoccupied...
                    if (tileInfo.isTileAvailable)
                    {
                        //IF isInfiniteCash is false...and then IF the cost of the tower is less than or equal to the amount of cash available...
                        if (!cheatsScript.isInfiniteCash)
                        {
                            // if (chosenTower.GetComponent<Towers.Tower>().towerCost <= playerStatsScript.cash)
                            // {
                            //     //Call SpendCash with the cost of the tower, and then call Build with the tileInfo component.
                            //     playerStatsScript.SpendCash(chosenTower.GetComponent<Towers.Tower>().towerCost);
                            //     Build(tileInfo);
                            // }
                                
                            //TODO: This
                            // if (towerAndAbilitiesInfo[chosenTower].towerCost <= playerStatsScript.cash)
                            // {
                            //     
                            // }
                            if (heldTower.towerCost <= playerStatsScript.cash)
                            {
                                //Call SpendCash with the cost of the tower, and then call Build with the tileInfo component.
                                playerStatsScript.SpendCash(heldTower.towerCost);
                                Build(tileInfo);
                            }
                        }
                        //ELSE...Instantiate a new tower on the build point, and set chosenTower to null.
                        else
                        {
                            //Call Build with the tileInfo component.
                            Build(tileInfo);
                        }
                    }
                    //ELSE...clear the chosenTower and hologramTower variables, and destroy the _activeHologramTower gameobject.
                    else
                    {
                        heldTower = null;
                        Destroy(_activeHologramTower);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //IF a tower in the tower menu has been selected...
        if (heldTower)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            //IF the cursor is over compatible terrain...
            if (Physics.Raycast(ray, out RaycastHit hitInfo2, 10000, terrainLayerMask))
            {
                //Get the TileInfo component from the tile's child, and store it.
                _currentTileInfo = hitInfo2.collider.GetComponentInChildren<TileInfo>();
                    
                //IF the current tile does not match the previous tile...
                if (_currentTileInfo != _previousTileInfo)
                {
                    //Destroy the previous tile's hologram.
                    if (_activeHologramTower) { Destroy(_activeHologramTower); }
                    SpawnHologramAndSetPreviousTile();
                }
                //ELSE IF the current tile is the same as the previous tile...AND there is no current hologram... 
                else if (_currentTileInfo == _previousTileInfo && !_activeHologramTower)
                {
                    SpawnHologramAndSetPreviousTile();
                }
            }
            else
            {
                //Remove the hologram if hovering over incompatible terrain/space.
                Destroy(_activeHologramTower);
            }
        }
    }

    private void Build(TileInfo tileInfo)
    {
        // Instantiate a new tower on the build point, and set heldTower to null.
        Audio2DManager.Instance.PlayBuildSfx();
        _lastPlacedTower = Instantiate(heldTower.towerPrefab, tileInfo.transform.position, Quaternion.identity);
        _lastPlacedTower.TryGetComponent(out _localTower);
        heldTower = null;
        Destroy(_activeHologramTower);
        //
        tileInfo.isTileAvailable = false;
    }

    private void SpawnHologramAndSetPreviousTile()
    {
        //Spawn the current tile's hologram, and mark the current tile as the new previous tile.
        _activeHologramTower = Instantiate(heldTower.hologramPrefab, _currentTileInfo.transform.position, Quaternion.identity);
        _previousTileInfo = _currentTileInfo;
    }

    public void OnRightMouseButton()
    {
        // Clear grid highlight.
        gridHighlight.gameObject.SetActive(false);
        ClearLocalTower();
    }

    private void ClearLocalTower()
    {
        // Clear localTower's detection radius cylinder...if it exists.
        if (!_localTower) return;
        if (_localTower.detectionRadiusCylinder)
        {
            _localTower.detectionRadiusCylinder.SetActive(false);
        }
        _localTower = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_test)
        {
            Gizmos.DrawSphere(_test.position, 4);
        }
    }
}
