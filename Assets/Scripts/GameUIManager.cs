using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public TMP_Text levelNameText;
    public TMP_Text timeRemainingText;
    public Button restartButton;
    public Button homeButton;
    public Slider towerHealthSlider;   
    public TMP_Text towerHealthText;   
    public TMP_Text guardsCountText;   
    public TMP_Text npcCountText;      
    
    public TMP_Text exitModifierTextPrefab;
    private Dictionary<string, TMP_Text> exitModifierTexts = new Dictionary<string, TMP_Text>();
    public GameObject levelCompletePanel;  
    public Button nextLevelButton;
    public Button levelCompleteHomeButton;
    public GameObject timeUpPanel;
    public Button timeUpRestartButton;
    public Button timeUpHomeButton;

    private GameManager gameManagerRef;
    
    public void InitializeUI(GameManager gameManager)
    {
        gameManagerRef = gameManager;

        restartButton.onClick.AddListener(OnRestartButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        levelCompleteHomeButton.onClick.AddListener(OnHomeButtonClicked);
        timeUpRestartButton.onClick.AddListener(OnRestartButtonClicked);
        timeUpHomeButton.onClick.AddListener(OnHomeButtonClicked);

        levelNameText.text = gameManager.CurrentLevelData.levelName;
        levelCompletePanel.SetActive(false);
        timeUpPanel.SetActive(false);

        towerHealthSlider.maxValue = gameManager.CurrentLevelData.towerHealth;
        towerHealthSlider.value = gameManager.CurrentLevelData.towerHealth;
    }
    
    void Update()
    {
        float timeRemaining = gameManagerRef.GetTimeRemaining();
        if (timeRemaining >= 0)
        {
            timeRemainingText.text = FormatTime(timeRemaining);
        }

        guardsCountText.text = gameManagerRef.towerManager.GetGuards().Count.ToString();

        npcCountText.text = gameManagerRef.npcManager.GetNPCCount().ToString();

        UpdateTowerHealth( gameManagerRef.towerManager.GetTowerHealth() );
    }

    public void UpdateTowerHealth( float health )
    {
        towerHealthSlider.value = health;
        towerHealthText.text = $"Tower Health: {health:N0}";
    }

    public void UpdateExitValues( Dictionary<string, GameObject> exitGroups, Dictionary<string, int> exitModifiers )
    {
        foreach ( KeyValuePair<string, GameObject> exitGroup in exitGroups )
        {
            if ( !exitModifierTexts.ContainsKey( exitGroup.Key ) )
            {
                GameObject parent = exitGroup.Value;
                Vector3 averagePosition = Vector3.zero;

                foreach ( Transform child in parent.transform )
                {
                    averagePosition += child.position + Vector3.back * 0.5f + Vector3.up * 1.5f;
                }
                
                averagePosition /= parent.transform.childCount;
                
                TMP_Text spawnedText = Instantiate( exitModifierTextPrefab );
                spawnedText.name = exitGroup.Key;
                spawnedText.transform.position = averagePosition;
                exitModifierTexts[exitGroup.Key] = spawnedText;
            }

            int modifierValue = exitModifiers[ exitGroup.Key ];
            exitModifierTexts[exitGroup.Key].text = modifierValue < 0 ? modifierValue.ToString() : "+" + modifierValue;
        }
    }
    
    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public void ShowLevelCompletePanel()
    {
        levelCompletePanel.SetActive(true);
    }

    public void ShowTimeUpPanel()
    {
        timeUpPanel.SetActive(true);
    }

    void OnRestartButtonClicked()
    {
        gameManagerRef.RestartLevel();
    }

    void OnHomeButtonClicked()
    {
        gameManagerRef.GoToMainMenu();
    }

    void OnNextLevelButtonClicked()
    {
        gameManagerRef.LoadNextLevel();
    }
}