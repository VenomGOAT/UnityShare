using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardUI : MonoBehaviour
{
    private MainGameScript.Card CardData;
    private bool isSelected = false;

    public Image HighlightBorder;

    public void Init(MainGameScript.Card card)
    {
        CardData = card;
        name = card.name;
        Deselect();
    }

    public void OnClick()
    {
        isSelected = !isSelected;
        if (isSelected)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }

    public MainGameScript.Card GetCardData()
    {
        return CardData;
    }

    private void Select()
    {
        if (HighlightBorder != null)
        {
            HighlightBorder.enabled = true;
        }
    }

    private void Deselect()
    {
        if (HighlightBorder != null)
        {
            HighlightBorder.enabled = false;
        }
    }



}
