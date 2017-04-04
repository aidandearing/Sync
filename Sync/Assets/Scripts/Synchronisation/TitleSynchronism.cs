using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Synchronisation/Title Synchronism")]
public class TitleSynchronism : Synchronism
{
    public MusicPlayer musicPlayer;

    protected override void Start()
    {
        Initialise();
    }

    protected override void Update()
    {
        base.Update();
    }
}
