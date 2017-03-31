using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Scripts/Characters/Nero/NeroSplashBehavior")]
class NeroSplashBehaviour : MonoBehaviour
{
    [Header("References")]
    public Camera camera;

    [Header("Edge Detection")]
    public EdgeDetectionSequencer edgeDetectionSequencer;

    [Header("Camera Control")]
    public SequencerGradient cameraSequencer;
    public Vector3 cameraStartPosition;
    public Vector3 cameraStartEuler;
    public Vector3 cameraEndPosition;
    public Vector3 cameraEndEuler;

    void Update()
    {
        if (!cameraSequencer.isInitialised)
        {
            cameraSequencer.Initialise();
        }

        edgeDetectionSequencer.Update();

        float ct = cameraSequencer.Evaluate();
        camera.transform.position = Vector3.Lerp(cameraStartPosition, cameraEndPosition, ct);
        camera.transform.rotation = Quaternion.Euler(Vector3.Lerp(cameraStartEuler, cameraEndEuler, ct));
    }
}
