using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CookieToogleUI : MonoBehaviour
{
    private Toggle toggle;
    private Image image;

    public int CookieIndex;

    private Color NormalColor;
    public Color toggledColor = Color.green;

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

        if (isOn && !MainGameScript.SelectedCookiesIndexes.Contains(CookieIndex))
        {
            MainGameScript.SelectedCookiesIndexes.Add(CookieIndex);
        }
        else if (!isOn && MainGameScript.SelectedCookiesIndexes.Contains(CookieIndex))
        {
            MainGameScript.SelectedCookiesIndexes.Remove(CookieIndex);
        }

        //Debug.Log("SelectedCardIndexes: " + string.Join(", ",MainGameScript.SelectedCardIndexes));
    }
}
