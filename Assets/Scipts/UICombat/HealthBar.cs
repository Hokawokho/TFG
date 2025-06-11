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
        meterImage.fillAmount = (float)hitPoints.hitPoints / hitPoints.maxHitPoints;
        hpText.text = hitPoints.hitPoints.ToString();
    }
}
