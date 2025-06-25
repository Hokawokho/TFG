using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject mainMenuUI;

    [SerializeField] private Animator selectorLevels;
    

    // public GameObject optionsMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        // Set initial UI state
        if (mainMenuUI != null) mainMenuUI.SetActive(true);
        // if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // if (optionsMenuUI != null && optionsMenuUI.activeSelf)
            //     GoBack();
        }
    }

    public void SelectLevel()
    {
        Debug.Log("Selecting Level");

        //Meter nivel actual
        // SceneManager.LoadScene(1, LoadSceneMode.Single);
        selectorLevels.SetBool("selecting", true);
    }

    public void LoadLevelHard() =>  SceneManager.LoadScene(1, LoadSceneMode.Single);
    public void LoadLevelEasy() =>  SceneManager.LoadScene(2, LoadSceneMode.Single);
    public void LoadLevelMedium() =>  SceneManager.LoadScene(3, LoadSceneMode.Single);

    public void QuitGame()
    {
        Debug.Log("Exit Level");
        Application.Quit();
        //Meter nivel inicial
        //SceneManager.LoadScene();

    }

    public void OptionsMenu()
    {
        Debug.Log("Opening Options");

        // if (optionsMenuUI != null) optionsMenuUI.SetActive(true);
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        //Meter nivel inicial
        //SceneManager.LoadScene();

    }

    public void GoBack()
    {
        Debug.Log("Going Back to Main Menu");
        // Hide options UI, show main menu UI
        // if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        // if (mainMenuUI != null) mainMenuUI.SetActive(true);
        selectorLevels.SetBool("selecting", false);
    }
    
}
