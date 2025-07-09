using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Referencias a CanvasGroups")]
    public CanvasGroup rotationGroup;
    public CanvasGroup actionsGroup;
    public CanvasGroup healGroup;
    public CanvasGroup goBack;

    public void SetRotationEnabled(bool canRotate)
    {
        rotationGroup.alpha = canRotate ? 1f : 0.5f;
        // rotationGroup.interactable = canRotate;
        // rotationGroup.blocksRaycasts = canRotate;
    }

    public void SetActionsEnabled(bool hasActions)
    {
        actionsGroup.alpha = hasActions ? 1f : 0.5f;
        // actionsGroup.interactable = hasActions;
        // actionsGroup.blocksRaycasts = hasActions;
    }

    public void SetHealEnabled(bool canHeal)
    {
        healGroup.alpha = canHeal ? 1f : 0.5f;
        // healGroup.interactable = canHeal;
        // healGroup.blocksRaycasts = canHeal;
    }

    public void SetAllZero()
    {
        rotationGroup.alpha = 0f;
        actionsGroup.alpha = 0f;
        healGroup.alpha = 0f;
        goBack.alpha = 1f;
    }

    public void SetAllOne()
    {
        rotationGroup.alpha = 1f;
        actionsGroup.alpha = 1f;
        healGroup.alpha = 1f;
        goBack.alpha = 0f;
    }
    

}
