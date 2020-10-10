using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] float yearDuration = 60f;

    int year;
    bool gameOver;

    private void Awake()
    {
        WorkerManager.OnPopulationChanged += GameOverCheck;
    }

    private void OnDestroy()
    {
        WorkerManager.OnPopulationChanged -= GameOverCheck;
    }

    // Start is called before the first frame update
    void Start()
    {
        year = 0;
        gameOver = false;

        StartCoroutine(PassTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameOverCheck(int population)
    {
        if (population < 1 && !gameOver)
        {
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        gameOver = true;
        // Set game over screen active
        // Change time to 0
        Debug.Log("Game over.");
        yield return new WaitForSeconds(5f);
        // SceneManager.LoadScene("MainMenu");
    }

    IEnumerator PassTime()
    {
        while (true)
        {
            year++;
            yield return new WaitForSeconds(yearDuration);
        }
        
    }

}
