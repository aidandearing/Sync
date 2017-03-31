using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Utility/Transform Transition")]
public class TransformTransition : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve position_x;
    [SerializeField]
    private AnimationCurve position_y;
    [SerializeField]
    private AnimationCurve position_z;
    [SerializeField]
    private bool rotationOriented = false;
    private Vector3 startPosition;
    [SerializeField]
    private AnimationCurve scale_x;
    [SerializeField]
    private AnimationCurve scale_y;
    [SerializeField]
    private AnimationCurve scale_z;
    private Vector3 startScale;
    [SerializeField]
    private AnimationCurve rotation_x;
    [SerializeField]
    private AnimationCurve rotation_y;
    [SerializeField]
    private AnimationCurve rotation_z;
    private Vector3 startRotation;
    [SerializeField]
    private float duration;

    private float time;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        startRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        float percent = time / duration;

        if (!rotationOriented)
            transform.localPosition = startPosition + new Vector3(position_x.Evaluate(percent), position_y.Evaluate(percent), position_z.Evaluate(percent));
        else
            transform.localPosition = startPosition + transform.TransformVector(new Vector3(position_x.Evaluate(percent), position_y.Evaluate(percent), position_z.Evaluate(percent)));

        transform.localScale = startScale + new Vector3(scale_x.Evaluate(percent), scale_y.Evaluate(percent), scale_z.Evaluate(percent));
        transform.rotation = Quaternion.Euler(startRotation.x + rotation_x.Evaluate(percent), startRotation.y + rotation_y.Evaluate(percent), startRotation.z + rotation_z.Evaluate(percent));
    }
}
