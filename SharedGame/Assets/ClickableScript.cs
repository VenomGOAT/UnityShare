using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ClickableScript;


public class ClickableScript : MonoBehaviour
{
    public Button BakeButton;
    public static bool BakeCanInteract;

    public Button DrawButton;
    public static bool DrawCanInteract;


    void Start()
    {
        BakeButton.interactable = false;
        DrawButton.interactable = true;
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
    }
}
