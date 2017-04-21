using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PrismaticoProjectileController : MonoBehaviour
{
    new public Renderer renderer;
    public SynchronisedProjectileBehaviour projectile;
    public PrismaticoControllerSpecial prismatico;

    public float delay = 2.0f;
    public bool isDelaying = true;
    public float lifetime = 2.0f;
    public float time;

    public float forwardOffset = 2.5f;

    public Color colour;
    public Color edgeColour;
    public AnimationCurve edgeSharpness;
    public AnimationCurve cutoffRange;

    public AnimationCurve damageOverLife;

    void Start()
    {
        time = Time.time;
        projectile.enabled = false;

        transform.position = prismatico.transform.position + prismatico.transform.forward * forwardOffset + new Vector3(0,1,0);

        renderer.material.SetColor("_Colour", colour);
        renderer.material.SetColor("_ColourEdge", edgeColour);
        renderer.material.SetFloat("_EdgeSharpness", edgeSharpness.Evaluate(0));
        renderer.material.SetFloat("_CutoffRange", cutoffRange.Evaluate(0));

        prismatico.colour = prismatico.materials[0].GetColor("_OutlineColor");
        prismatico.light.color = prismatico.colour;
    }

    void FixedUpdate()
    {
        float deltaTime = Time.time - time;
        if (deltaTime > delay)
        {
            if (isDelaying)
            {
                isDelaying = false;
                projectile.enabled = true;
                projectile.origin = prismatico.transform.position + prismatico.transform.forward * forwardOffset + new Vector3(0, 1, 0);
                projectile.direction = prismatico.player.cameraRayQuery.rayHitLast.point - projectile.origin;

                renderer.material.SetFloat("_EdgeSharpness", edgeSharpness.Evaluate(1));
                renderer.material.SetFloat("_CutoffRange", cutoffRange.Evaluate(1));

                prismatico.light.color = edgeColour;
                prismatico.colour = edgeColour;

                foreach (Material mat in prismatico.materials)
                {
                    mat.SetColor("_RimColour", edgeColour);
                    mat.SetColor("_OutlineColor", edgeColour);
                }
            }
            else
            {
                if (deltaTime > delay + lifetime)
                {
                    
                }
                else
                {
                    float p = 1 - ((deltaTime - delay) / lifetime);
                    renderer.material.SetFloat("_EdgeSharpness", edgeSharpness.Evaluate(p));
                    renderer.material.SetFloat("_CutoffRange", cutoffRange.Evaluate(p));
                    projectile.hostileProperties[0].value = damageOverLife.Evaluate(1.0f - p);
                }
            }
        }
        else
        {
            float p = deltaTime / delay;
            renderer.material.SetFloat("_EdgeSharpness", edgeSharpness.Evaluate(p));
            renderer.material.SetFloat("_CutoffRange", cutoffRange.Evaluate(p));
            transform.position = Vector3.MoveTowards(transform.position, prismatico.transform.position + prismatico.transform.forward * forwardOffset + new Vector3(0, 1, 0), 10.0f * Time.fixedDeltaTime);

            prismatico.light.color = Color.Lerp(prismatico.colour, edgeColour, p);
            foreach (Material mat in prismatico.materials)
            {
                mat.SetColor("_RimColour", Color.Lerp(prismatico.colour, edgeColour, p));
                mat.SetColor("_OutlineColor", Color.Lerp(prismatico.colour, edgeColour, p));
            }
        }
    }

    public void Reset()
    {
        time = Time.time;
    }
}