using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvenScript : MonoBehaviour
{
    public Image OvenImage;
    public Vector3 BigSize = new Vector3(16f, 10f, 1f);
    public Vector3 NewPosition = new Vector3(0f, 0f, 0f);
    private Vector3 OriginalSize;
    private Vector3 OriginaPosition;
    private bool IsExpanded = false;

    void Start()
    {
        OriginalSize = OvenImage.transform.localScale;
        OriginaPosition = OvenImage.transform.position;
    }

    public void ToggleScale()
    {
        if (IsExpanded)
        {
            OvenImage.transform.localScale = OriginalSize;
            OvenImage.transform.position = OriginaPosition;
        }
        else
        {
            OvenImage.transform.localScale = BigSize;
            OvenImage.transform.position = NewPosition;
        }
        IsExpanded = !IsExpanded;
    }
}
