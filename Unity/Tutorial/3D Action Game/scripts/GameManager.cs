using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character PlayerCharacter;
    public GameUIManager _GameUIManager;
    
    private bool isGameOver;

    private void Awake()
    {
        PlayerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    public void GameOver()
    {
        Debug.Log("Game is Over"); 
        _GameUIManager.ShowGameOverUI();
    }

    public void GameIsFinished()
    {
        Debug.Log("Game is Finished"); 
        _GameUIManager.ShowGameIsFinishedUI();
    }
    

    private void Update()
    {
        if (isGameOver)
            return;

        if (PlayerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            isGameOver = true;
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _GameUIManager.TogglePauseUI();
        }
        
    }

    
    
    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
