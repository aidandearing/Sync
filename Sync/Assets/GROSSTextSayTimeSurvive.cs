using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GROSSTextSayTimeSurvive : MonoBehaviour
{
    public Text text;

    // Use this for initialization
    void Start()
    {
        text.text += Mathf.Floor(LentoControllerSpecial.timeAlive) + "s";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
