using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ClickableScript;


public class ClickableScript : MonoBehaviour
{
    public Button BakeButton;
    public static bool BakeCanInteract;

    public Button DrawButton;
    public static bool DrawCanInteract;

    public Button UseAbilityButton;
    public static bool UseAbilityCanInteract;


    void Start()
    {
        BakeButton.interactable = false;
        DrawButton.interactable = true;
        UseAbilityButton.interactable = true;
    }

    void Update()
    {
        if (BakeCanInteract)
        {
            BakeButton.interactable = true;
        }
        else
        {
            BakeButton.interactable = false;
        }

        if (DrawCanInteract)
        {
            DrawButton.interactable = true;
        }
        else
        {
            DrawButton.interactable = false;
        }

        if (UseAbilityCanInteract)
        {
            UseAbilityButton.interactable = true;
        }
        else
        {
            UseAbilityButton.interactable = false;

        }
    }
}
