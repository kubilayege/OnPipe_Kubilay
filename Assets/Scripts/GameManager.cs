using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Idle,
        Playing,
        CompleteMission,
        Win,
        Fail
    }

    public GameState currentGameState;

    void Start()
    {
        
        instance = this;
        ChangeGameState(GameState.Idle);
    }

    private void Update()
    {
        if ((currentGameState == GameState.Fail || currentGameState == GameState.Win) && Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
        {
            ChangeGameState(GameState.Idle);
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Idle:
                LevelManager.instance.StartLevel();
                LevelManager.instance.player.gameStarted = false;
                UIManager.instance.ChangeScreen((int)UIManager.GameScreens.MainMenu);
                currentGameState = GameState.Idle;

                break;
            case GameState.Playing:
                currentGameState = GameState.Playing;
                LevelManager.instance.player.gameStarted = true;
                UIManager.instance.ChangeScreen((int)UIManager.GameScreens.GameUI);
                break;
            case GameState.CompleteMission:
                LevelManager.instance.EndLevel();
                currentGameState = GameState.CompleteMission;
                break;
            case GameState.Win:
                Handheld.Vibrate();
                LevelManager.instance.WinLevel();
                UIManager.instance.ChangeScreen((int)UIManager.GameScreens.WinScreen);
                currentGameState = GameState.Win;
                break;
            case GameState.Fail:
                LevelManager.instance.ResetLevel();
                UIManager.instance.ChangeScreen((int)UIManager.GameScreens.FailScreen);
                currentGameState = GameState.Fail;
                break;
            default:
                break;
        }
    }
}
