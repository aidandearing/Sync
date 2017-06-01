using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    public static List<CloudSystem> cloudSystems = new List<CloudSystem>();

    [Serializable]
    public class CloudSystemState
    {
        public enum Form { Cirriform, Cumuliform, Cumulonimbiform, Stratiform, Stratocumuliform };
        public Form form = Form.Cumuliform;
        public enum Level { Low, Mid, High, Vertical, Towering };
        public Level level = Level.Vertical;
        public enum CloudType { None, Altocumulus, Altostratus, Cirrocumulus, Cirrostratus, Cirrus, Cumulonimbus, Cumulus, Nimbostratus, Stratocumulus, Stratus };
        public CloudType type = CloudType.None;
    }

    public CloudSystemState state;

    void Start()
    {
        cloudSystems.Add(this);

        Generate();
    }

    void Generate()
    {

    }

    static List<CloudSystemState> EvaluateCloudStates(EnvironmentWeatherSystem system)
    {
        List<CloudSystemState> states = new List<CloudSystemState>();



        return states;
    }

    public static CloudSystemState.CloudType CloudType(CloudSystemState state)
    {
        switch(state.form)
        {
            case CloudSystemState.Form.Cirriform:
                state.level = CloudSystemState.Level.High;
                return CloudSystemState.CloudType.Cirrus;
            case CloudSystemState.Form.Cumuliform:
                switch(state.level)
                {
                    case CloudSystemState.Level.Low:
                        state.form = CloudSystemState.Form.Cumulonimbiform;
                        return CloudSystemState.CloudType.Stratocumulus;
                    case CloudSystemState.Level.Mid:
                        return CloudSystemState.CloudType.Altocumulus;
                    case CloudSystemState.Level.High:
                        return CloudSystemState.CloudType.Cirrocumulus;
                    case CloudSystemState.Level.Vertical:
                        return CloudSystemState.CloudType.Cumulus;
                    case CloudSystemState.Level.Towering:
                        state.form = CloudSystemState.Form.Cumulonimbiform;
                        return CloudSystemState.CloudType.Cumulonimbus;
                }
                return CloudSystemState.CloudType.Cumulus;
            case CloudSystemState.Form.Cumulonimbiform:
                state.level = CloudSystemState.Level.Towering;
                return CloudSystemState.CloudType.Cumulonimbus;
            case CloudSystemState.Form.Stratiform:
                switch(state.level)
                {
                    case CloudSystemState.Level.Low:
                        return CloudSystemState.CloudType.Stratus;
                    case CloudSystemState.Level.Mid:
                        return CloudSystemState.CloudType.Altostratus;
                    case CloudSystemState.Level.High:
                        return CloudSystemState.CloudType.Cirrostratus;
                    case CloudSystemState.Level.Towering:
                        return CloudSystemState.CloudType.Nimbostratus;
                    case CloudSystemState.Level.Vertical:
                        return CloudSystemState.CloudType.Nimbostratus;
                }
                return CloudSystemState.CloudType.Stratus;
            case CloudSystemState.Form.Stratocumuliform:
                state.level = CloudSystemState.Level.Low;
                return CloudSystemState.CloudType.Stratocumulus;
        }

        return CloudSystemState.CloudType.Cumulus;
    }

    ~CloudSystem()
    {
        cloudSystems.Remove(this);
    }
}