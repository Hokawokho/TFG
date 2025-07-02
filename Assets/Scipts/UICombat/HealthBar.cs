using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public HitPoints hitPoints;

    public UnitEntity unitEntity;
    public Image meterImage;
    public TextMeshProUGUI hpText;

    void Update()
    {
        // if (hitPoints == null)
        // {
        //     meterImage.fillAmount = 0f;
        //     hpText.text = "0";
        // }
        // else
        // {
        //     meterImage.fillAmount = (float)hitPoints.hitPoints / hitPoints.maxHitPoints;
        //     hpText.text = hitPoints.hitPoints.ToString();
        // }

        if (unitEntity == null)
        {
            meterImage.fillAmount = 0f;
            hpText.text = "0";
        }
        else
        {
            float maxHP = unitEntity.unitType.initialHitPoints;
            float curHP = unitEntity.currentHealth;
            meterImage.fillAmount = curHP / maxHP;
            hpText.text = curHP.ToString();
        }

    }
    public void SetUnit(UnitEntity unit)
    {
        // if (unit == null)
        // {
        //     hitPoints = null;
        // }
        // else
        // {
        //     hitPoints = unit.hitpoints;   
        // }

        unitEntity = unit;

    }
}
