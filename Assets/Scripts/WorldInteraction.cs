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
    }

    public void OnLeftMouseButton()
    {
        if (_lastPlacedTower)
        {
            var fixedTargetTower = _lastPlacedTower.GetComponent<FixedTargetTower>();
            if (fixedTargetTower.isSelectingTarget)
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
                //Ensures the grid highlight object is set to active, and moves it to the build point of the tile.
                gridHighlight.gameObject.SetActive(true);
                gridHighlight.position = tileInfo.transform.position;

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
        //Instantiate a new tower on the build point, and set heldTower to null.
        _lastPlacedTower = Instantiate(heldTower.towerPrefab, tileInfo.transform.position, Quaternion.identity);
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
}
