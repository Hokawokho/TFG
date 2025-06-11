using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public HitPoints hitPoints;
    public Image meterImage;
    public TextMeshProUGUI hpText;

    void Update()
    {
        if (hitPoints == null)
        {
            meterImage.fillAmount = 0f;
            hpText.text = "0";
        }
        else
        {
            meterImage.fillAmount = (float)hitPoints.hitPoints / hitPoints.maxHitPoints;
            hpText.text = hitPoints.hitPoints.ToString();
        }


    }
    public void SetUnit(UnitEntity unit)
    {
        if (unit == null)
        {
            hitPoints = null;
        }
        else
        {
            hitPoints = unit.hitpoints;
        }

    }
}
