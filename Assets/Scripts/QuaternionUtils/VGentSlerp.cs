using System;
using UnityEngine;

public class VGentSlerp
{
    public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
    {
        float dot = Quaternion.Dot(from, to);
        if (Math.Abs(dot) > 1) dot /= dot;
        float theta = Mathf.Acos(dot);

        if (theta < 0.0)
        {
            theta = -theta;
        }

        float st = Mathf.Sin(theta);

        if (st == 0)
        {
            return from;
        }

        float sut = Mathf.Sin(theta * t);
        float sout = Mathf.Sin(theta * (1.0f - t));

        float coeff1 = sout / st;
        float coeff2 = sut / st;

        float x = coeff1 * from.x + coeff2 * to.x;
        float y = coeff1 * from.y + coeff2 * to.y;
        float z = coeff1 * from.z + coeff2 * to.z;
        float w = coeff1 * from.w + coeff2 * to.w;
        Quaternion q = new Quaternion(x, y, z, w);
        return q.normalized;
    }
}
