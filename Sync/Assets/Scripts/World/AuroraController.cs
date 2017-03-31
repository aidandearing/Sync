using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class AuroraController : MonoBehaviour
{
    public LineRenderer line;

    public Vector2 offset = new Vector2(5000, 2000);

    void Update()
    {
        List<Vector3> vectors = new List<Vector3>();

        float theta = Mathf.PI * 2 / (line.numPositions - 1);

        for (int i = 0; i < line.numPositions; i++)
        {
            float x = Mathf.Cos(theta * i) * offset.x;
            float z = Mathf.Sin(theta * i) * offset.x;

            vectors.Add(new Vector3(x, offset.y, z));
        }

        line.SetPositions(vectors.ToArray());
    }
}
