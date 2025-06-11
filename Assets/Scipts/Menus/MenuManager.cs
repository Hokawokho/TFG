using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectLevel()
    {
        Debug.Log("Selecting Level");

        //Meter nivel actual
        //SceneManager.LoadScene();

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
        Debug.Log("Opening Options");
        //Meter nivel inicial
        //SceneManager.LoadScene();

    }
}
