using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Title Synchronism")]
public class TitleSynchronism : MonoBehaviour
{
    public Synchronism synchronism = new Synchronism();

    public MusicPlayer musicPlayer;

    void Start()
    {
        synchronism.Initialise();
    }

    void Update()
    {
        synchronism.Update();
    }
}
