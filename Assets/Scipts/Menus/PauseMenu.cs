using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    private AudioManager audioManager;
    [SerializeField] private Animator pauseAnims;
    [SerializeField] private GameObject combatUI;
    private CanvasGroup combatCanvas;  

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (combatUI != null)
            combatCanvas = combatUI.GetComponent<CanvasGroup>();
        
        
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
        combatCanvas.alpha = 1f;

        Time.timeScale = 1f;
        GameIsPaused = false;
        if (audioManager != null)
            audioManager.FadeToGameMusic();

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        combatCanvas.alpha = 0f;
        Time.timeScale = 0f;
        GameIsPaused = true;
        if (audioManager != null)
            audioManager.FadeToPauseMusic();


    }

    public void OpenRestartOptins()
    {
        Debug.Log("Restarting Level");

        //Meter nivel actual
        // string mapa = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(mapa, LoadSceneMode.Single);
        pauseAnims.SetBool("restarting", true);

    // Opción B: si tienes una escena persistente “bootstrap” y NO es la que quieres recargar,
        // guarda en una variable el nombre o índice del mapa cuando lo cargas por primera vez.
        // Por ejemplo: GameManager.CurrentMapName = "Mapa Real1";


        //Resume();

    }

    public void RestartWithUnits()
    {
        // Obtener selección actual
        var selector = FindObjectOfType<UnitSelector>();
        if (selector != null)
        {
            RestartData.cachedPlayerTypes = new List<UnitType>(selector.unitsSelected);
        }
        else
        {
            Debug.LogWarning("UnitSelector no encontrado. Se reiniciará sin mantener unidades.");
            RestartData.Clear();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void RestartWithoutUnits()
    {
        RestartData.Clear();  // ← sin mantener selección previa
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    

    public void GoBack()
    {
        Debug.Log("Going Back to Main Menu");
        // Hide options UI, show main menu UI
        // if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        // if (mainMenuUI != null) mainMenuUI.SetActive(true);
        pauseAnims.SetBool("restarting", false);
    }

    public void QuitLevel()
    {
        Debug.Log("Exit Level");
        //Meter nivel inicial
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }
}
