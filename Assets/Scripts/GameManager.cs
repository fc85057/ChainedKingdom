﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static event Action<int> OnYearChanged = delegate { };

    [SerializeField] float yearDuration = 60f;
    [SerializeField] int winYear;

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject winScreen;

    int year;
    bool gameOver;
    bool paused;

    private void Awake()
    {
        WorkerManager.OnPopulationChanged += GameOverCheck;
    }

    private void OnDestroy()
    {
        WorkerManager.OnPopulationChanged -= GameOverCheck;
    }

    void Start()
    {
        year = 0;
        gameOver = false;

        StartCoroutine(PassTime());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    void GameOverCheck(int population)
    {
        if (population < 1 && !gameOver)
        {
            //StartCoroutine(GameOver());
            GameOver();
        }
    }

    IEnumerator PassTime()
    {
        while (true)
        {
            year++;
            if (year == winYear)
            {
                WinGame();
            }
            else
            {
                OnYearChanged(year);
                yield return new WaitForSeconds(yearDuration);
            }
            
        }

    }

    void WinGame()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0;
    }

    void GameOver()
    {
        gameOver = true;
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Game over");
    }

    /*
    IEnumerator GameOver()
    {
        gameOver = true;
        gameOverScreen.SetActive(true);
        Debug.Log("Game over.");
        yield return new WaitForSeconds(5f);
        LoadMainMenu();
    }
    */

    public void Pause()
    {
        if (gameOver)
            return;

        if (!paused)
        {
            paused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            paused = false;
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
        
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
