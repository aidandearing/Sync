using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GROSSMusicSourceVolumeOnMenu : MonoBehaviour
{
    public MenuController menu;
    public MusicSource source;
    public AnimationCurve volume;

    public float transitionPercentage;
    public float evaluatedVolume;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float p = 0;

        if (menu.isHideTransitioning)
        {
            p = menu.transitionCurrent / menu.transitionDuration;

            if (menu.isHidden)
                p = 1.0f - p;
        }
        else
        {
            if (menu.isHidden)
                p = 1.0f;
            else
                p = 0.0f;
        }

        float e = volume.Evaluate(p);
        source.volume = new AnimationCurve(new Keyframe(0.0f, e), new Keyframe(1.0f, e));

        transitionPercentage = p;
        evaluatedVolume = e;
    }
}