using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public enum GameScreens
    {
        MainMenu,
        SettingsMenu,
        GameUI,
        FailScreen,
        WinScreen
    }

    public GameObject[] gameScreens;
    private int currentActiveScreen;

    public int levelCount;
    public TextMeshProUGUI levelCountText; 
    public TextMeshProUGUI levelScoreText; 
    public TextMeshProUGUI INFOText;
    float deltatime;
    float fps;
    void Start()
    {
        
        instance = this;
        levelCount = 1;
    }
    public void ChangeScreen(int screenIndex)
    {
        gameScreens[currentActiveScreen].SetActive(false);
        gameScreens[screenIndex].SetActive(true);
        currentActiveScreen = screenIndex;
    }

    public void UpdateScore(int currentPoint, int levelRequirement)
    {
        levelScoreText.text = currentPoint + "/" + levelRequirement;
    }

    public void LevelPassed()
    {
        levelCount++;
        levelCountText.text = "Level " + levelCount.ToString();
    }

    public void RestartGame()
    {
        GameManager.instance.ChangeGameState(GameManager.GameState.Idle);
    }
}
