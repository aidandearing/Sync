﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public MusicTrack[] tracks;

    void FixedUpdate()
    {
        foreach(MusicTrack track in tracks)
        {
            track.Update();
        }
    }
}
