using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    private AudioManager audioManager;


    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();

            else
            {
                Pause();
            }

        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (audioManager != null)
            audioManager.FadeToGameMusic();

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        if (audioManager != null)
            audioManager.FadeToPauseMusic();


    }

    public void RestartLevel()
    {
        Debug.Log("Restarting Level");
        
        //Meter nivel actual
        string mapa = SceneManager.GetActiveScene().name;

    // Opción B: si tienes una escena persistente “bootstrap” y NO es la que quieres recargar,
    // guarda en una variable el nombre o índice del mapa cuando lo cargas por primera vez.
    // Por ejemplo: GameManager.CurrentMapName = "Mapa Real1";

    SceneManager.LoadScene(mapa, LoadSceneMode.Single);
        //Resume();

    }

    public void QuitLevel()
    {
        Debug.Log("Exit Level");
        //Meter nivel inicial
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }
}
