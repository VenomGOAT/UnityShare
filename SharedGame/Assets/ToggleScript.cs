using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    private Toggle toggle;
    private Image image;

    public int CardIndex;

    private Color NormalColor;
    public Color toggledColor = Color.yellow;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        image = GetComponent<Image>();

        NormalColor = image.color;

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        image.color = isOn ? toggledColor : NormalColor;

        if (isOn && !MainGameScript.SelectedCardIndexes.Contains(CardIndex))
        {
            MainGameScript.SelectedCardIndexes.Add(CardIndex);
        }
        else if (!isOn && MainGameScript.SelectedCardIndexes.Contains(CardIndex))
        {
            MainGameScript.SelectedCardIndexes.Remove(CardIndex);
        }

        //Debug.Log("SelectedCardIndexes: " + string.Join(", ",MainGameScript.SelectedCardIndexes));
    }





}
