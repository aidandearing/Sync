using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    public static List<CloudSystem> cloudSystems = new List<CloudSystem>();

    public enum SystemCircumference { Circle, Square, Sphere, Cube };
    public SystemCircumference circumferenceMode = SystemCircumference.Sphere;

    public GameObject cloudPrefab;

    [Header("Cloud Profiling")]
    [Tooltip("Defines the shape the top of the cloud will approximate")]
    public AnimationCurve cloudSystemTopProfile;
    [Tooltip("Defines the shape the bottom of the cloud will approximate ")]
    public AnimationCurve cloudSystemBottomProfile;

    [Header("Cloud Attributes")]
    [Tooltip("Defines the maximum height (scale) of any one cloud node in the system based on the value returned by the cloud system profile")]
    [Range(1, 5000)]
    public float cloudSystemHeight = 1000;
    [Tooltip("Defines the variation in cloud height experienced, as a value treated as a percentage of the total cloud system height when used by profiling")]
    [Range(0, 2500)]
    public float cloudSystemHeightDelta = 150;
    [Tooltip("Defines the factor of each cloud node's radius the next ones will generate from")]
    [Range(1.0f, 0.1f)]
    public float cloudSystemCascadeRatio = 0.9f;
    [Tooltip("Defines the number of times new cloud node layers will be generated branching from existing ones")]
    [Range(0, 32)]
    public int cloudSystemCascades = 4;
    [Tooltip("Defines the number of new cloud nodes generated per previous generated cycle on each of the last generated nodes")]
    [Range(0, 32)]
    public int cloudSystemDensity = 4;
    [Tooltip("Defines the chance on any one decorator node that it has a decorator pass generated on it as well")]
    [Range(0.0f,1.0f)]
    public float cloudSystemDensitySubPassChance = 0.5f;
    [Tooltip("Defines the minimum size as a percentage of the parent cloud node each decorator node added by cloudSystemDensity will be")]
    [Range(0.1f, 1.0f)]
    public float cloudSystemDensityScaleMin = 0.2f;
    [Tooltip("Defines the maximum size as a percentage of the parent cloud node each decorator node added by cloudSystemDensity will be")]
    [Range(0.2f, 1.5f)]
    public float cloudSystemDensityScaleMax = 0.8f;

    void Start()
    {
        cloudSystems.Add(this);

        Generate();
    }

    void Generate()
    {
        // Cloud Generation follows some basic principles.
        // A primary cloud node is constructed right in the middle
        // 2 child nodes are spawned on opposite poles with some variance, 
        // and this process is repeated off each side the specified number of times, 
        // with a certain number of random cloud nodes spawned along some ratio of the radius of any primary cloud node at each cascade
        float profile = 0;
        GameObject node = GenerateNode(profile);

        // Quick decorator pass for some random cloud nodes along a ratio of the radius
        DecorateNode(node);

        Vector3 location = Vector3.zero;// UnityEngine.Random.insideUnitSphere;

        for (int i = 0; i < cloudSystemCascades; i++)
        {
            profile = i / cloudSystemCascades;
            for (int j = 0; j < cloudSystemDensity; j++)
            {
                location = UnityEngine.Random.insideUnitSphere;
                location.Set(location.x * cloudSystemHeight * 0.2f, location.y * cloudSystemHeight * 0.1f, location.z * cloudSystemHeight * 1.0f);

                node = GenerateNode(profile);
                node.transform.localScale *= (node.transform.localPosition - location).magnitude / (cloudSystemHeight);
                node.transform.localPosition = location;
            }
        }

        // Next let's generate one pole at a time
        //GeneratePole(node, new Vector3(0, 0, 1));

        //GeneratePole(node, new Vector3(0, 0, -1));
    }

    void DecorateNode(GameObject node)
    {
        // Based on the decoration mode
        switch (circumferenceMode)
        {
            // Sphere randomly distributes cloud nodes along a sphere made the size of the parent node factored by the cloudSystemCascadeRatio
            case SystemCircumference.Sphere:
                float radius = node.transform.localScale.x * cloudSystemCascadeRatio;

                for (int i = 0; i < cloudSystemDensity; i++)
                {
                    GameObject decoratorNode = Instantiate(cloudPrefab, transform, false);
                    decoratorNode.transform.localScale = node.transform.localScale * UnityEngine.Random.Range(cloudSystemDensityScaleMin, cloudSystemDensityScaleMax);
                    decoratorNode.transform.localPosition = node.transform.localPosition + UnityEngine.Random.onUnitSphere * cloudSystemCascadeRatio * node.transform.localScale.x / 2;

                    if (UnityEngine.Random.value < cloudSystemDensitySubPassChance)
                    {
                        DecorateNode(decoratorNode);
                    }
                }
                break;
        }
    }

    GameObject GenerateNode(float evaluate)
    {
        GameObject node = Instantiate(cloudPrefab, transform, false);
        node.transform.localScale = Vector3.one * EvaluateProfile(evaluate) * (cloudSystemHeight + UnityEngine.Random.Range(-cloudSystemHeightDelta, cloudSystemHeightDelta));
        return node; 
    }

    void GeneratePole(GameObject node, Vector3 direction)
    {
        // Calculate a new random direction to start the pole from
        Vector3 dirActual = (direction + UnityEngine.Random.onUnitSphere * 0.1f).normalized;

        int cascades = 1;
        float profile = cascades / cloudSystemCascades;

        GameObject newNode = GenerateNode(profile);
        GameObject lastNode = newNode;
        newNode.transform.localPosition = node.transform.localPosition + dirActual * node.transform.localScale.x / 2.0f * cloudSystemCascadeRatio;

        DecorateNode(newNode);

        for (cascades = 1; cascades < cloudSystemCascades; cascades++)
        {
            dirActual = (direction + UnityEngine.Random.onUnitSphere * 0.1f).normalized;
            profile = cascades / cloudSystemCascades;
            newNode = GenerateNode(profile);
            newNode.transform.localPosition = lastNode.transform.localPosition + dirActual * lastNode.transform.localScale.x / 2.0f * cloudSystemCascadeRatio - EvaluateProfileOffset(profile, newNode);
            lastNode = newNode;
            DecorateNode(newNode);
        }
    }

    float EvaluateProfile(float evaluate)
    {
        return (1.0f - cloudSystemBottomProfile.Evaluate(evaluate) + cloudSystemTopProfile.Evaluate(evaluate)) / 2.0f;
    }

    Vector3 EvaluateProfileOffset(float evaluate, GameObject node)
    {
        return new Vector3(0, (cloudSystemBottomProfile.Evaluate(evaluate) + cloudSystemTopProfile.Evaluate(evaluate)) / 2.0f * cloudSystemHeight, 0);
    }

    ~CloudSystem()
    {
        cloudSystems.Remove(this);
    }
}