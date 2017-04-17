using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GROSSRawImageHealth : MonoBehaviour
{
    public RawImage image;
    public LentoControllerSpecial controller;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        image.color = new Color(1, 1, 1, 1 - ((float)controller.controller.statistics["health"].Value / 100.0f));
    }
}
