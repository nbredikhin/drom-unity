using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionText : MonoBehaviour
{
    public Text interactionText;
    private GameObject currentObject;

    public void ShowInteractionSuggestion(GameObject targetObject)
    {
        if (currentObject != null)
        {
            return;
        }
        interactionText.text = "Press E to use "+targetObject.name;
        interactionText.enabled = true;
        currentObject = targetObject;
    }

    public void HideInteractionSuggestion(GameObject targetObject)
    {
        if (targetObject == null || targetObject != currentObject)
        {
            return;
        }
        interactionText.enabled = false;
        currentObject = null;
    }
}
