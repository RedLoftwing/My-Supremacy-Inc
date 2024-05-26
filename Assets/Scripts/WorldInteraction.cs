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
    [SerializeField] public GameObject chosenTower;
    [SerializeField] public GameObject hologramTower;
    private GameObject _activeHologramTower;

    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private LayerMask towerLayerMask;

    //[SerializeField] private Material transparentMaterial;
    //[SerializeField] private Transform[] childObjects;

    private void Update()
    {
        //IF the pointer is over UI...set _isClickDisabled to true...ELSE set to false. Prevents interaction of elements behind UI.
        
        // if(EventSystem.current.IsPointerOverGameObject())
        // {
        //     _isClickDisabled = true;
        // }
        // else
        // {
        //     _isClickDisabled = false;
        // }

        IsClickDisabled = EventSystem.current.IsPointerOverGameObject();
    }

    public void RayCastClick()
    {
        //IF the pointer is not over UI...
        if (!IsClickDisabled)
        {
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

                    //IF chosenTower is true...
                    if (chosenTower)
                    {
                        //IF the tile is unoccupied...
                        if (tileInfo.isTileAvailable)
                        {
                            //IF isInfiniteCash is false...and then IF the cost of the tower is less than or equal to the amount of cash available...
                            if (!cheatsScript.isInfiniteCash)
                            {
                                if (chosenTower.GetComponent<Towers.Tower>().towerCost <= playerStatsScript.cash)
                                {
                                    //Call SpendCash with the cost of the tower, and then call Build with the tileInfo component.
                                    playerStatsScript.SpendCash(chosenTower.GetComponent<Towers.Tower>().towerCost);
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
                            chosenTower = null;
                            hologramTower = null;
                            Destroy(_activeHologramTower);
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //IF chosenTower is true...
        if (chosenTower)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            //If the raycast hits something on the specified layer mask...
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 10000, terrainLayerMask))
            {
                //Get the TileInfo component from the tile's child, and store it.
                var tileInfo = hitInfo.collider.GetComponentInChildren<TileInfo>();

                if (!_activeHologramTower)
                {
                    _activeHologramTower = Instantiate(hologramTower, tileInfo.transform.position, Quaternion.identity);
                }

                //Show transparent tower.
                _activeHologramTower.transform.position = tileInfo.transform.position;
            }
        }
    }

    private void Build(TileInfo tileInfo)
    {
        //Instantiate a new tower on the build point, and set chosenTower to null.
        Instantiate(chosenTower, tileInfo.transform.position, Quaternion.identity);
        chosenTower = null;
        hologramTower = null;
        Destroy(_activeHologramTower);
        //
        tileInfo.isTileAvailable = false;
    }
}
