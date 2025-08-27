using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    private Toggle toggle;
    private Image image;

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
    }





}
