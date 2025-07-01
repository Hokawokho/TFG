using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SelectUnit()
    {
        

    }

    public void GoBack() => SceneManager.LoadScene(0, LoadSceneMode.Single);

    public void startGame()
    {
        
        
        //poner la escena que se cargue
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    

}
