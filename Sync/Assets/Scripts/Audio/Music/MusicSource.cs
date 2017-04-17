using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    public SequencerGradient sequencer;

    public GameObject parent;
    public AnimationCurve volume;
    private float volumeTrack = 1;

    public AudioClip[] clips;
    private int index = 0;
    public AudioSource prefab;
    public bool clipToSynchronisation = true;
    public float clip = 0;
    public List<AudioSource> sources = new List<AudioSource>();

    void Initialise()
    {
        if (!sequencer.isInitialised)
        {
            if (((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value) != null)
            {
                sequencer.Initialise();
                sequencer.callback = Callback;
                Callback();
            }
        }
    }

    public void Update()
    {
        if (!sequencer.isInitialised)
            Initialise();
        else
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i] == null)
                {
                    sources.RemoveAt(i);
                }
            }

            foreach (AudioSource source in sources)
            {
                source.volume = volumeTrack * volume.Evaluate(sequencer.Evaluate());
            }
        }
    }

    void Callback()
    {
        //if (sequencer.Evaluate() > 0.9f)
        //{
            AudioSource instance = Instantiate(prefab, transform);
            instance.clip = clips[(int)(sequencer.Evaluate() * (clips.Length - 1.0f))];
            instance.time = sequencer.synchroniser.Percent * (float)sequencer.synchroniser.Duration;
            instance.Play();
            sources.Add(instance);
            clip = (clipToSynchronisation) ? (float)sequencer.synchroniser.Duration * sequencer.duration : clips[index].length;
            Destroy(instance.gameObject, clip);
        //}
    }

    public void SetVolume(float v)
    {
        volumeTrack = v;
    }
}
