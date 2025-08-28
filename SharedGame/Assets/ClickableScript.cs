using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ClickableScript;


public class ClickableScript : MonoBehaviour
{
    public Button button;
    public static bool CanInteract;

    
    void Start()
    {
        button.interactable = false;
    }

    void Update()
    {
        if (CanInteract)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
