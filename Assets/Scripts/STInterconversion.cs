using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STInterconversion : MonoBehaviour
{
    public static Vector3 Quaternion2SwingTwist(Quaternion q, Vector3 twistAxisRight, Vector3 twistAxisUp, Vector3 twistAxisForward)
    {
        Vector3 swingTwist;

        Quaternion swing = Quaternion.FromToRotation(twistAxisRight, q * twistAxisRight);
        Quaternion twist = Quaternion.Inverse(swing) * q;
        Vector3 r = Vector3.Project(new Vector3(q.x, q.y, q.z), twistAxisRight);

        if (r.sqrMagnitude < Mathf.Epsilon)
        {
            swingTwist.z = 0f;
        }
        else
        {
            if (Vector3.Dot(r, twistAxisRight) < 0) swingTwist.z = -Mathf.Acos(twist.w) * 2.0f * Mathf.Rad2Deg;
            else swingTwist.z = Mathf.Acos(twist.w) * 2.0f * Mathf.Rad2Deg;
        }

        Vector3 p = Vector3.ProjectOnPlane(swing * twistAxisRight, twistAxisRight).normalized;


        if (Vector3.Dot(p, twistAxisForward) < 0) swingTwist.y = 360.0f - Mathf.Acos(Vector3.Dot(p, twistAxisUp)) * Mathf.Rad2Deg;
        else swingTwist.y = Mathf.Acos(Vector3.Dot(p, twistAxisUp)) * Mathf.Rad2Deg;

        /* if (Vector3.Dot(p, twistAxisForward) < 0) swingTwist.y = -Mathf.Acos(Vector3.Dot(p, twistAxisUp)) * Mathf.Rad2Deg;
        else swingTwist.y = Mathf.Acos(Vector3.Dot(p, twistAxisUp)) * Mathf.Rad2Deg; */

        swingTwist.x = Vector3.Angle(q * twistAxisRight, twistAxisRight);

        // -180 ~ 180
        if (swingTwist.z > 180.0f) swingTwist.z -= 360.0f;
        else if (swingTwist.z < -180.0f) swingTwist.z += 360.0f;

        return swingTwist;
    }

    public static Quaternion SwingTwist2Quaternion(Vector3 swingTwist, Vector3 twistAxisRight, Vector3 twistAxisForward)
    {
        //一致しないときがある．
        Vector3 direction = Quaternion.AngleAxis(swingTwist.y, twistAxisRight) * Quaternion.AngleAxis(swingTwist.x, twistAxisForward) * twistAxisRight;
        Quaternion swing = Quaternion.AngleAxis(swingTwist.x, Vector3.Cross(twistAxisRight, direction));
        Quaternion twist = Quaternion.AngleAxis(swingTwist.z, twistAxisRight);
        return swing * twist;
    }

}
