using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Animator selectorLevels;

    [SerializeField] private CanvasGroup generalSettings;
    [SerializeField] private CanvasGroup audioSettings;


    // public GameObject optionsMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        SetCanvasGroupActive(generalSettings, true);
        SetCanvasGroupActive(audioSettings, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectLevel()
    {
        Debug.Log("Selecting Level");

        //Meter nivel actual
        // SceneManager.LoadScene(1, LoadSceneMode.Single);
        selectorLevels.SetBool("selecting", true);
    }

    public void LoadLevelHard() => SceneManager.LoadScene(1, LoadSceneMode.Single);
    public void LoadLevelEasy() => SceneManager.LoadScene(2, LoadSceneMode.Single);
    public void LoadLevelMedium() => SceneManager.LoadScene(3, LoadSceneMode.Single);

    public void GoBackfromLevels()
    {
        Debug.Log("Going Back to Main Menu");
        // Hide options UI, show main menu UI
        // if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        // if (mainMenuUI != null) mainMenuUI.SetActive(true);
        selectorLevels.SetBool("selecting", false);
    }

    public void QuitGame()
    {
        Debug.Log("Exit Level");
        Application.Quit();
        //Meter nivel inicial
        //SceneManager.LoadScene();

    }

    public void OptionsMenu()
    {
        Debug.Log("Options selected");

        selectorLevels.SetBool("generalSettings", true);
    }

    public void GoBackfromSettings()
    {
        Debug.Log("Going Back to Main Menu");
        // Hide options UI, show main menu UI
        // if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        // if (mainMenuUI != null) mainMenuUI.SetActive(true);
        selectorLevels.SetBool("generalSettings", false);
    }


    public void SelectedAudioSettings()
    {
        SetCanvasGroupActive(generalSettings, false);
        SetCanvasGroupActive(audioSettings, true);

    }
    public void GoBackfromAudioSettings()
    {
        Debug.Log("Going Back to Settings");
        SetCanvasGroupActive(audioSettings, false);
        SetCanvasGroupActive(generalSettings, true);
        Debug.Log("Volviendo a General Settings");
    }

    private void SetCanvasGroupActive(CanvasGroup cg, bool active)
    {
        cg.alpha = active ? 1f : 0f;
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }
    
    
    
}
