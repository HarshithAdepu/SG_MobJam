using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject levelSelectPanel;
    
    public Button playButton;
    public Button backButton;
    public Button quitButton;

    private void OnEnable()
    {
        playButton.onClick.AddListener( OnPlayButtonClick );
        backButton.onClick.AddListener( OnBackButtonClick );
        quitButton.onClick.AddListener( OnQuitButtonClick );
        
        OnBackButtonClick();
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    private void OnPlayButtonClick()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    private void OnBackButtonClick()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }
    
    private void OnQuitButtonClick()
    {
        Application.Quit();
    }
    
    public void OnLevelSelected(int index = 0)
    {
        PersistentGameData.CurrentLevelIndex = index;
        
        SceneManager.LoadScene( "GameScene");
    }
}
