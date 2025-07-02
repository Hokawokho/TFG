using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;



public class UnitSelector : MonoBehaviour
{

    public List <UnitType> unitsSelected;

    [SerializeField] private List<Image> spritesSelected;
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var img in spritesSelected)
        {
            img.enabled = false;
            img.sprite = null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void selectMeleeUnit()
    {
        AddUnitImage(senderButton: UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 
                     scale: new Vector3(3.59124f, 3.59124f, 3.59124f));

    }

    public void selectRangeUnit()
    {
        AddUnitImage(senderButton: UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject, 
                     scale: new Vector3(3.591713f, 3.591713f, 3.591713f));

    }

    private void AddUnitImage(GameObject senderButton, Vector3 scale)
    {
        if (currentIndex >= spritesSelected.Count) return;

        Image sourceImage = senderButton.GetComponent<Image>();
        if (sourceImage == null || sourceImage.sprite == null) return;

        Image targetImage = spritesSelected[currentIndex];
        targetImage.sprite = sourceImage.sprite;
        targetImage.enabled = true;
        targetImage.transform.localScale = scale;

        currentIndex++;
    }


    void createUnitList()
    {
        // unitsSelected.Add();
    }

    void discardUnits()
    {
        currentIndex = 0;

        foreach (var img in spritesSelected)
        {
            img.sprite = null;
            img.enabled = false;
        }


    }


    public void GoBack() => SceneManager.LoadScene(0, LoadSceneMode.Single);

    public void startGame()
    {


        //poner la escena que se cargue
        // SceneManager.LoadScene(0, LoadSceneMode.Single);

        //PONER ESTO EN EL TURNMANAGER COMO UN NUEVO ESTADO
    }


}
