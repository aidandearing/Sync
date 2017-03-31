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

    protected override void OnTimeBar8()
    {

    }

    protected override void OnTimeBar4()
    {

    }

    protected override void OnTimeBar2()
    {

    }

    protected override void OnTimeBar()
    {

    }

    protected override void OnTimeWhole()
    {

    }

    protected override void OnTimeHalf()
    {

    }

    protected override void OnTimeQuarter()
    {

    }

    protected override void OnTimeEighth()
    {

    }

    protected override void OnTimeSixteenth()
    {

    }

    protected override void OnTimeThirtySecond()
    {

    }
}
