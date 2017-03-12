using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MathHelper
{
    public static class Vector
    {
        public static Vector2 XY (Vector4 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 XY (Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 XZ (Vector4 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 XZ (Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 XW (Vector4 vector)
        {
            return new Vector2(vector.x, vector.w);
        }

        public static Vector2 YZ(Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }

        public static Vector2 YZ (Vector4 vector)
        {
            return new Vector2(vector.y, vector.z);
        }
    }
}
