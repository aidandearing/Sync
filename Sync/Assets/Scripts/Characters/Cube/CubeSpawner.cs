using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public Synchronism.Synchronisations synchronisation = Synchronism.Synchronisations.WHOLE_NOTE;
    public Synchroniser synchroniser;
    public SequencerGameObjects sequencer;

    public bool isInitialised = false;

    public void Initialise()
    {
        if (!isInitialised)
        {
            if ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value != null)
            {
                isInitialised = true;

                synchroniser = ((Synchronism)Blackboard.Global[Literals.Strings.Blackboard.Synchronisation.Synchroniser].Value).synchronisers[synchronisation];
                synchroniser.RegisterCallback(this, Callback);
            }
        }
    }

    void Update()
    {
        Initialise();
    }

    void Callback()
    {
        GameObject inst = sequencer.Evaluate();

        if (inst != null)
        {
            Instantiate(inst, transform.position, new Quaternion());
        }
    }
}