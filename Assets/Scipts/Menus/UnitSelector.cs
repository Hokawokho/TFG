using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;



public class UnitSelector : MonoBehaviour
{

    public readonly List<UnitType> unitsSelected = new List<UnitType>(3);

    [SerializeField] private List<Image> spritesSelected;

    [SerializeField] private UnitType meleeUnitType;   // arrastra meleeUnit.asset
    [SerializeField] private UnitType rangeUnitType;
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        unitsSelected.Clear();
        currentIndex = 0;

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

    public void selectMeleeUnit(GameObject buttonGO)
    {
        AddUnit(senderButton: buttonGO, size: new Vector2(19f, 27f), meleeUnitType);

    }

    public void selectRangeUnit(GameObject buttonGO)
    {
        AddUnit(senderButton: buttonGO, size: new Vector2(19f, 21f), rangeUnitType);

    }

    private void AddUnit(GameObject senderButton, Vector3 size, UnitType unitType)
    {
        if (currentIndex >= spritesSelected.Count) return;

        Image sourceImage = senderButton.GetComponent<Image>();
        if (sourceImage == null || sourceImage.sprite == null) return;

        Image targetImage = spritesSelected[currentIndex];
        targetImage.sprite = sourceImage.sprite;
        targetImage.enabled = true;

        RectTransform rt = targetImage.rectTransform;
        rt.sizeDelta = size;


        if (unitsSelected.Count > currentIndex)     // AÇÒ PAL FUTUR SOBREESCRIURE UNA UNITAT ANTERIOR PER UNA NOVA
            unitsSelected[currentIndex] = unitType;
        else                                        // slot nuevo
            unitsSelected.Add(unitType);

        currentIndex++;
    }


    void createUnitList()
    {
        // unitsSelected.Add();
    }

    public void discardUnits()
    {
        currentIndex = 0;
        unitsSelected.Clear();

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

        var tm = FindObjectOfType<TurnManager>();
        if (tm != null)
            tm.OnUnitsSelectionConfirmed(unitsSelected);
        
    }
    
    public void PrintSelectedUnits()
{
    Debug.Log("=== Unidades seleccionadas ===");

    for (int i = 0; i < unitsSelected.Count; i++)
    {
        UnitType unit = unitsSelected[i];
        if (unit != null)
            Debug.Log($"Posición {i}: {unit.name}");
        else
            Debug.Log($"Posición {i}: (vacía)");
    }
}


}
