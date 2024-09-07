using System;
using System.Collections;
using SO_Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class UserInterface : MonoBehaviour
{
    private readonly string[] _potentialNames = {"Tower Button", "Tower Button (1)", "Tower Button (2)", "Tower Button (3)", "Tower Button (4)", "Tower Button (5)", "Tower Button (6)"};
    [SerializeField] private GameObject[] purchasableOptions;
    private UIHover[] _buttonUIHoverComp = new UIHover[10];
    private Button[] _buttonComp = new Button[10];
    [Header("Script Instance References")]
    [SerializeField] private WorldInteraction worldInteractionScript;
    [SerializeField] private GameState gameStateScript;
    [SerializeField] private Player.PlayerStats playerStatsScript;
    private Scene _currentScene;
    [SerializeField] private LayerMask uiLayerMask;
    private GameObject _buttonObj;
    [SerializeField] private GameObject buttonHighlightObj;

    [Header("-Information Panels-")]
    [Header("Tower Info Panel")]
    public GameObject towerInfoPanel;
    public TextMeshProUGUI towerInfoPanelTitle;
    public TextMeshProUGUI towerInfoPanelCost;
    public TextMeshProUGUI towerInfoPanelAttackInfo;
    public TextMeshProUGUI towerInfoPanelDescription;
    private Vector3[] _towerPanelPoint = new Vector3[2];
    [HideInInspector] public bool isCursorOverTowerButton;
    private float _towerCursorHoverTimer;
    [Header("Ability Info Panel")]
    public GameObject abilityInfoPanel;
    public TextMeshProUGUI abilityInfoPanelTitle;
    public TextMeshProUGUI abilityInfoPanelCost;
    public UnityEngine.UI.Image abilityInfoPanelCostIcon;
    public TextMeshProUGUI abilityInfoPanelDescription;
    private Vector3[] _abilityPanelPoint = new Vector3[2];
    [HideInInspector] public bool isCursorOverAbilityButton;
    private float _abilityCursorHoverTimer;
    [SerializeField] private AnimationCurve informationPanelPopUpCurve;
    
    [Header("Other UI Stuff")]
    [SerializeField] private TextMeshProUGUI gameTimerText;
    private float _gameTime;

    public GameObject activeWavePanel;
    public TextMeshProUGUI waveText;

    public GameObject endGamePanel;
    public TextMeshProUGUI endGameTextPanel;
    public TextMeshProUGUI endGameBackTextPanel;
    public TextMeshProUGUI finalWaterValue;
    public TextMeshProUGUI finalCashValue;
    public TextMeshProUGUI finalHealthValue;

    public GameObject[] panelArray;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mapSelectPanel;
    public GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject enemySpawnSection;

    [SerializeField] private GameObject reversedSpline;
    [SerializeField] private GameObject waterCellObj;

    [SerializeField] private AbilityInfo[] abilitySO;

    private void Start()
    {
        //Finds the active scene and stores it as currentScene.
        _currentScene = SceneManager.GetActiveScene();
        //
        for (int i = 0; i < purchasableOptions.Length; i++)
        {
            _buttonComp[i] = purchasableOptions[i].GetComponent<Button>();
            _buttonUIHoverComp[i] = purchasableOptions[i].GetComponent<UIHover>();
        }
        
        //IF the active scene's name is NOT "MainMenu"...then proceed with setting gameplay variables.
        if (_currentScene.name != "MainMenu")
        {
            //Game Timer values set to default. Calls coroutine to start timer.
            _gameTime = 0;
            gameTimerText.SetText(System.TimeSpan.FromSeconds(_gameTime).ToString("mm':'ss"));
            StartCoroutine(GameTimer());
            //Set active state for the "Active Wave" UI object.
            activeWavePanel.SetActive(false);
            //Set Wave text values.
            waveText.SetText("Wave: " + gameStateScript.waveNumber + "/10");
        }
        //ELSE IF the active scene's name IS "MainMenu"...set anything that needs to be disabled as such.
        else if (_currentScene.name == "MainMenu")
        {
            //Gets the Button components from each of the children objects, and stores them in the childrenComponents array.
            var childrenComponents = enemySpawnSection.GetComponentsInChildren<Button>();
            //For each button in the childrenComponents array... 
            foreach (var button in childrenComponents)
            {
                //Sets spawn buttons as not interactable.
                button.interactable = false;
            }
        }
        
        //Set panel position points.
        _towerPanelPoint[0] = towerInfoPanel.transform.position;
        _towerPanelPoint[1] = new Vector3(_towerPanelPoint[0].x - 214.5f, _towerPanelPoint[0].y, _towerPanelPoint[0].z);
        _abilityPanelPoint[0] = abilityInfoPanel.transform.position;
        _abilityPanelPoint[1] = new Vector3(_abilityPanelPoint[0].x, _abilityPanelPoint[0].y + 165, _abilityPanelPoint[0].z);
        
        //
        AllowPurchasableSelection();
    }

    private void Update()
    {
        PanelSlide(isCursorOverTowerButton, ref _towerCursorHoverTimer, towerInfoPanel, _towerPanelPoint);
        PanelSlide(isCursorOverAbilityButton, ref _abilityCursorHoverTimer, abilityInfoPanel, _abilityPanelPoint);
    }

    private void PanelSlide(bool isCursorOverButton, ref float timer, GameObject panel, Vector3[] panelPoints)
    {
        timer += isCursorOverButton ? Time.deltaTime : -Time.deltaTime;
        timer = Mathf.Clamp(timer, 0f, 1f);
        float tValue = timer / 1f;
        float curveValue = informationPanelPopUpCurve.Evaluate(tValue);
        panel.transform.position = Vector3.Lerp(panelPoints[0], panelPoints[1], curveValue);

        if (Vector3.Distance(panel.transform.position, panelPoints[0]) < 0.01f && !isCursorOverButton)
        {
            panel.SetActive(false);
            panel.transform.position = panelPoints[0];
        }
    }

    //Called whenever one of the Tower Menu buttons are pressed.
    public void TowerButtonUsed()
    {
        //Grabs the gameobject (button) that was selected by the player, and stores it as buttonObj.
        _buttonObj = EventSystem.current.currentSelectedGameObject;
        //Debug.Log(_buttonObj);
        
        //IF the name of the button matches with any of the values stored in the potentialNames array...proceed with the true path...
        if(_buttonObj.name == _potentialNames[0])
        {
            AssignTower(0);
        }
        else if(_buttonObj.name == _potentialNames[1])
        {
            AssignTower(1);
        }
        else if (_buttonObj.name == _potentialNames[2])
        {
            AssignTower(2);
        }
        else if (_buttonObj.name == _potentialNames[3])
        {
            AssignTower(3);
        }
        else if (_buttonObj.name == _potentialNames[4])
        {
            AssignTower(4);
        }
        else if (_buttonObj.name == _potentialNames[5])
        {
            AssignTower(5);
        }
        else if (_buttonObj.name == _potentialNames[6])
        {
            AssignTower(6);
        }
        //ELSE IF...do nothing.
        else
        {
            Debug.Log("No name match for TowerButtonUsed..." + _buttonObj);
        }
    }

    private void AssignTower(int towerValue)
    {
        //Sets the object's activity to true, and then moves its position to the selected tower.
        buttonHighlightObj.SetActive(true);
        buttonHighlightObj.transform.position = _buttonObj.transform.position;
        //Assign the appropriate tower scriptable object (defined by towerValue) to the heldTower value.
        worldInteractionScript.heldTower = worldInteractionScript.towerInfo[towerValue];
    }

    private IEnumerator GameTimer()
    {
        //While gameTime is greater than or equal to 0...
        while (_gameTime >= 0)
        {
            //Wait 1 second...then set text to the value of gameTime (Converted from float value to minutes and seconds), then increase the value of gameTime by 1.
            yield return new WaitForSeconds(1);
            gameTimerText.SetText(System.TimeSpan.FromSeconds(_gameTime).ToString("mm':'ss"));
            _gameTime++;
        }
    }

    public void PlayButton()
    {
        //IF the game is currently in between waves...
        if(gameStateScript.isInterWave)
        {
            //Advance to next wave and set isInterWave to false.
            gameStateScript.waveNumber++;
            gameStateScript.isInterWave = false;
        }
    }

    public void WaterAbility1()
    {
        //Sell water for cash.
        //IF water supply is greater than or equal to 5...call DecreaseWaterSupply and AwardCash.
        if(playerStatsScript.water >= abilitySO[0].abilityExpenditureAmount)
        {
            playerStatsScript.DecreaseWaterSupply(abilitySO[0].abilityExpenditureAmount);
            playerStatsScript.AwardCash(abilitySO[0].abilityIncomeAmount);
            //AllowPurchasableSelection();
        }
    }

    public void WaterAbility2()
    {
        //IF cash is greater than or equal to 70...Instantiate waveObj at spawn point AND call SpendCash.
        if(playerStatsScript.cash >= abilitySO[1].abilityExpenditureAmount)
        {
            Instantiate(abilitySO[1].abilityPrefab, reversedSpline.transform.position, Quaternion.identity);
            playerStatsScript.SpendCash(abilitySO[1].abilityExpenditureAmount);
        }
    }

    public void WaterAbility3()
    {
        //IF cash is greater than or equal to 70...Instantiate waterCellObj at a random location on the path AND call SpendCash.
        if (playerStatsScript.cash >= abilitySO[2].abilityExpenditureAmount)
        {
            var paths = GameObject.Find("Enemy Path Group").GetComponentsInChildren<Transform>();
            var randomNum = Random.Range(10, paths.Length - 10);
            Instantiate(abilitySO[2].abilityPrefab, paths[randomNum].position, Quaternion.identity);
            playerStatsScript.SpendCash(abilitySO[2].abilityExpenditureAmount);
        }
    }

    public void SwitchPanel(string requestedPanel)
    {
        //Used to switch between UI elements.
        //Called from a function that is called by a button (e.g. GoToNewGame is called by the NewGame button. Which then calls this function.)
        
        //FOR each gameObject in the panel array...
        foreach (GameObject panel in panelArray)
        {
            //IF the gameObject's name matches the inputted string...Set it to active.
            panel.SetActive(panel.name == requestedPanel);
        }
    }
    
    public void AllowPurchasableSelection()
    {
        for (int i = 0; i < purchasableOptions.Length; i++)
        {
            if (_buttonUIHoverComp[i].purchasableType == UIHover.PurchasableType.Tower)
            {
                var activeState = _buttonUIHoverComp[i].towerInfo.towerCost > playerStatsScript.cash;
                _buttonComp[i].interactable = !activeState;
                _buttonUIHoverComp[i].buttonGreyOutFilter.SetActive(activeState);
            }
            else
            {
                if (_buttonUIHoverComp[i].abilityInfo.abilityExpenditureCurrencyType == AbilityInfo.CurrencyTypes.Water)
                {
                    _buttonUIHoverComp[i].buttonGreyOutFilter.SetActive(_buttonUIHoverComp[i].abilityInfo.abilityExpenditureAmount > playerStatsScript.water);
                }
                else
                {
                    _buttonUIHoverComp[i].buttonGreyOutFilter.SetActive(_buttonUIHoverComp[i].abilityInfo.abilityExpenditureAmount > playerStatsScript.cash);
                }
            }
        }
    }
    
    public void GoToNewGame()
    {
        //Called by a NewGame button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("MapSelect Panel");
    }

    public void LoadMap1()
    {
        //Called by a LoadMap1 button. Loads the Map1 Scene.
        SceneManager.LoadScene("Map1");
    }

    public void LoadMap2()
    {
        //Called by a LoadMap2 button. Loads the Map2 Scene.
        SceneManager.LoadScene("Map2");
    }

    public void LoadMap3()
    {
        //Called by a LoadMap3 button. Loads the Map3 Scene.
        SceneManager.LoadScene("Map3");
    }

    public void LoadGame()
    {
        //Called by a LoadGame button. Calls SwitchPanel with the appropriate string.
    }

    public void GoToTutorial()
    {
        //Called by a Tutorial button. Loads the Tutorial Scene.
        SceneManager.LoadScene("Tutorial");
    }

    public void GoToSettings()
    {
        //Called by a Settings button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("Settings Panel");
    }

    public void GoToCheats()
    {
        //Called by a Cheats button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("Cheats Panel");
    }

    public void ReturnToPauseMenu()
    {
        //Called by a Return to Pause Menu button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("Pause Menu");
    }

    public void ReturnToMainMenu()
    {
        //Finds the active scene and stores it as currentScene.
        _currentScene = SceneManager.GetActiveScene();
        //IF the active scene's name is NOT "MainMenu"...then proceed with setting gameplay variables.
        if (_currentScene.name != "MainMenu")
        {
            //Load the MainMenu scene.
            SceneManager.LoadScene("MainMenu");
        }
        //ELSE...set the main menu panel to true and hide all other panels/menus.
        else
        {
            SwitchPanel("MainMenu Panel");
        }
    }

    public void QuitGame()
    {
        //Quit the application.
        Application.Quit();
    }
}
