using Towers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WorldInteraction : MonoBehaviour
{
    public static WorldInteraction Instance { get; private set; }

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
    private FixedTargetTower _localFixedTargetTower;
    
    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    private void Update()  {
        // Prevents interaction of elements behind UI. IsPointerOverGameObject? (UI), if true, marks click disabled as true.
        IsClickDisabled = EventSystem.current.IsPointerOverGameObject();
    }

    public void OnLeftMouseButton() {
        if (HandleArtilleryTargetPlacement()) return;

        // Ensure the pointer is not over UI before continuing.
        if (IsClickDisabled) return;
        // Ensure there is a tile hit by the ray before continuing. 
        if (!FireRayAtClickPoint(out var tileInfo)) return;
        
        // Places the highlight gameObject AND activates any relevant VFX if a tower is found too.
        HighlightTile(tileInfo);

        // IF there is a tower selected from the tower purchase menu...continue.
        if (heldTower) {
            TryBuildHeldTower(tileInfo);
        }
    }
    
    private bool HandleArtilleryTargetPlacement() {
        // IF there is a lastPlacedTower...Try get it's FixedTargetTower component...IF successful...set targeting position.
        if (!_lastPlacedTower || !_lastPlacedTower.TryGetComponent<FixedTargetTower>(out var fixedTargetTower)) return false;
        
        // IF the fixed target type tower is selecting a target...confirm its target.
        if (!fixedTargetTower.isSelectingTarget) return false;
        fixedTargetTower.ConfirmTarget();
        return true;
    }

    private bool FireRayAtClickPoint(out TileInfo tileInfo) {
        tileInfo = null;

        // Ensure there is a main camera (to fire a ray from) before continuing.
        if (!Camera.main) return false;
        
        // Conduct a raycast from the camera to where the player has clicked, and store it.
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        // IF the raycast hits something on the specified layer mask...continue.
        if (!Physics.Raycast(ray, out var hitInfo, 10000, terrainLayerMask)) return false;
        
        // Get the TileInfo component from the tile's child, and store it.
        tileInfo = hitInfo.collider.GetComponentInChildren<TileInfo>();
        return true;
    }

    private void HighlightTile(TileInfo inTileInfo) {
        //Ensures the grid highlight object is set to active, and moves it to the build point (top surface) of the tile.
        gridHighlight.gameObject.SetActive(true);
        gridHighlight.position = inTileInfo.transform.position;
        
        // Reset local tower variables and deactivate any tower specific vfx (i.e. detection radius cylinder).
        ClearLocalTower();
        
        // -- Detection Radius Cylinder activation & FixedTargetTower retargeting --
        // Check if a tower is in this spot...IF there is a tower, show the range of the tower by activating the detection radius cylinder.
        if (inTileInfo.isTileAvailable) return;
        
        // IF the tower in this spot is a fixed target tower...ensure it is not actively selecting a target.
        var towerIsSelecting = _localFixedTargetTower && _localFixedTargetTower.isSelectingTarget;
        if (towerIsSelecting) return;
            
        // Get local tower colliders.
        Physics.OverlapSphereNonAlloc(inTileInfo.transform.position, 1, _localColliders, towerLayerMask);
            
        // Find the closest tower.
        Collider closestCollider = null;
        var closestDistance = float.MaxValue;
        foreach (var localCollider in _localColliders) {
            if (!localCollider) continue;
            var distance = Vector3.Distance(inTileInfo.transform.position, localCollider.transform.position);
            if (distance < closestDistance) {
                closestCollider = localCollider;
                closestDistance = distance;
            }
        }
            
        // Ensures a tower is found && that the closest one is determined before continuing.
        if (!closestCollider || !closestCollider.TryGetComponent(out _localTower)) return;
        // IF the tower has detection radius cylinder...activate the cylinder...ELSE IF it's a fixed target tower...allow it to select a new target.
        if (_localTower.detectionRadiusCylinder) {
            _localTower.detectionRadiusCylinder.SetActive(true);
        }
        else if (_localTower.TryGetComponent(out _localFixedTargetTower)) {
            _localFixedTargetTower.isSelectingTarget = true;
            _localFixedTargetTower.targetHighlight.SetActive(true);
        }
    }

    private void TryBuildHeldTower(TileInfo inTileInfo) {
        //IF the tile is unoccupied...continue.
        if (inTileInfo.isTileAvailable) {
            // Ensures there is either...Infinite Cash OR an appropriate amount of cash available.
            if (!Cheats.Instance.isInfiniteCash && (Player.PlayerStats.Instance.cash < heldTower.towerCost)) return;
            
            // Spend the cash if Infinite Cash is disabled.
            if (!Cheats.Instance.isInfiniteCash) {
                Player.PlayerStats.Instance.SpendCash(heldTower.towerCost);
            }
            // Build the tower with the tile information.
            Build(inTileInfo);
        }
        //ELSE...clear the chosenTower and hologramTower variables, and destroy the _activeHologramTower gameobject.
        else {
            heldTower = null;
            Destroy(_activeHologramTower);
        }
    }
    
    private void FixedUpdate() {
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
        _lastPlacedTower.TryGetComponent(out _localFixedTargetTower);
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
        // Clear localTower's detection radius cylinder or the target highlight...if it exists.
        if (!_localTower) return;
        if (_localTower.detectionRadiusCylinder) {
            Debug.Log($"Clearing the radius cylinder of: {_localTower.gameObject.name}");
            _localTower.detectionRadiusCylinder.SetActive(false);
        }
        else if (_localFixedTargetTower) { _localFixedTargetTower.targetHighlight.SetActive(false); }
        _localTower = null;
        _localFixedTargetTower = null;
    }
}
