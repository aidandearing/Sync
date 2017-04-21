using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GROSSSceneJump : MonoBehaviour
{
    public string[] inputs = {
        Literals.Strings.Input.Controller.ButtonA,
        Literals.Strings.Input.Controller.ButtonB,
        Literals.Strings.Input.Controller.ButtonX,
        Literals.Strings.Input.Controller.ButtonY,
        Literals.Strings.Input.Controller.ButtonMenu,
        Literals.Strings.Input.Controller.ButtonView,
    };

    public string scene = "survival_levelup";

    public float loadTime = 2.0f;
    public float loadCurrent = 0.0f;
    public AsyncOperation load;
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
        if (load == null)
        {
            if (loadCurrent <= 0)
            {
                foreach (string str in inputs)
                {
                    if (Input.GetButton(str))
                    {
                        if (load == null)
                            load = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                    }
                }
            }
        }
        else
        {
            loadCurrent += Time.deltaTime;

            float p = Mathf.Min(load.progress, loadCurrent / loadTime);

            float e = volume.Evaluate(p);
            source.volume = new AnimationCurve(new Keyframe(0.0f, e), new Keyframe(1.0f, e));

            if (p >= 1)
            {
                if (load != null)
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
                    load = null;
                }
            }

            transitionPercentage = p;
            evaluatedVolume = e;
        }
    }
}
