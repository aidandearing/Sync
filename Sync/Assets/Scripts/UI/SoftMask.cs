using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("Scripts/UI/Soft Mask")]
public class SoftMask : MonoBehaviour
{
    public Material material;
    public Canvas canvas;

    [Tooltip("The mask area this UI element is to use as its mask")]
    public RectTransform maskArea;
    RectTransform rect;

    [Tooltip("Texture to be used for the alpha map")]
    public Texture alphaMask;

    [Tooltip("Controls the minimum alpha to apply the mask at")]
    [Range(0.0f,1.0f)]
    public float cutOff = 0;

    Vector2 AlphaUV;

    Vector2 min;
    Vector2 max = Vector2.one;
    Vector2 p;
    Vector2 size;

    Rect maskRect;
    Rect contentRect;

    Vector2 centre;

    bool isText = false;

    void Start()
    {
        if (GetComponent<Text>())
        {
            isText = true;

            if (transform.parent.GetComponent<Mask>() == null)
                transform.parent.gameObject.AddComponent<Mask>();

            transform.parent.GetComponent<Mask>().enabled = false;
        }
    }
}
