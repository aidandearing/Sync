using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Utility/Standard Material Transition")]
public class StandardMaterialTransition : MonoBehaviour
{
    private Renderer render;

    [SerializeField]
    private Gradient colour;
    [SerializeField]
    private AnimationCurve cutoff;
    [SerializeField]
    private AnimationCurve smoothness;
    [SerializeField]
    private AnimationCurve metallic;
    [SerializeField]
    private AnimationCurve bumpScale;
    [SerializeField]
    private AnimationCurve heightScale;
    [SerializeField]
    private Gradient emission;
    [SerializeField]
    private float duration;

    private float time;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        float percent = time / duration;

        render.material.shader = Shader.Find("Standard");
        render.material.SetColor("_Color", colour.Evaluate(percent));
        render.material.SetFloat("_Cutoff", Mathf.Clamp(cutoff.Evaluate(percent), 0, 1));
        render.material.SetFloat("_Glossiness", Mathf.Clamp(smoothness.Evaluate(percent), 0, 1));
        render.material.SetFloat("_Metallic", Mathf.Clamp(metallic.Evaluate(percent), 0, 1));
        render.material.SetFloat("_BumpScale", bumpScale.Evaluate(percent));
        render.material.SetFloat("_Parallax", heightScale.Evaluate(percent));
        render.material.SetColor("_EmissionColor", emission.Evaluate(percent));
    }
}
