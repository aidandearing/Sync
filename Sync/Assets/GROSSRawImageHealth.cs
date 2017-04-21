using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GROSSRawImageHealth : MonoBehaviour
{
    public RawImage image;
    public Controller controller;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float p = 1.0f;

        for(int i = 0; i < 2; i++)
        {
            if (Blackboard.Global.ContainsKey(Literals.Strings.Tags.Player + i))
                p = (float)(Blackboard.Global[Literals.Strings.Tags.Player + i].Value as PlayerController).controller.statistics["health"].Value / 100.0f;
        }

        image.color = new Color(1, 1, 1, 1.0f - p);// ((float)controller.statistics["health"].Value / 100.0f));
    }
}
