using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twoPointSTSeparationTest : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private float duration = 2f;
    [SerializeField] Transform target;
    [SerializeField] STAngleLimit targetSTAL;
    [SerializeField] Transform from, to;
    [SerializeField] int splits = 5;
    //[SerializeField] STAngleLimit fromSTAL, toSTAL;
    //[SerializeField] bool isSwing = true;
    //[SerializeField] bool isTwist = true;
    //Quaternion swing, twist;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time / duration, 1);

        Quaternion[] results = new Quaternion[splits + 1];

        STLimitWaypointGenerator(targetSTAL, from.localRotation, to.localRotation, splits, results);

        Quaternion q = STLimitInterpolation(results, splits, t);

        target.localRotation = q;
        /* if (isSwing && isTwist)
        {
            target.localRotation = qs * qt;
        }
        else if (isSwing)
        {
            target.localRotation = qs;
        }
        else if (isTwist)
        {
            target.localRotation = qt;
        }
        else
        {
            target.localRotation = Quaternion.identity;
        } */
        /* Quaternion q = Quaternion.Slerp(from.localRotation, to.localRotation, t);
        target.localRotation = q; */
    }

    void STLimitWaypointGenerator(STAngleLimit targetSTAL, Quaternion from, Quaternion to, int splits, Quaternion[] results)
    {
        Quaternion fromSwing = Quaternion.FromToRotation(targetSTAL.twistAxisRight, from * targetSTAL.twistAxisRight);
        Quaternion fromTwist = Quaternion.Inverse(fromSwing) * from;
        Quaternion toSwing = Quaternion.FromToRotation(targetSTAL.twistAxisRight, to * targetSTAL.twistAxisRight);
        Quaternion toTwist = Quaternion.Inverse(toSwing) * to;

        //ツイストの経由点を生成
        Quaternion twistDiff = fromTwist * Quaternion.Inverse(toTwist);
        if (Mathf.Abs(STInterconversion.Quaternion2SwingTwist(from, targetSTAL.twistAxisRight, targetSTAL.twistAxisUp, targetSTAL.twistAxisForward).z
         - STInterconversion.Quaternion2SwingTwist(to, targetSTAL.twistAxisRight, targetSTAL.twistAxisUp, targetSTAL.twistAxisForward).z) > 180)
        {
            if (twistDiff.w > 0)
            {
                twistDiff.w = -twistDiff.w;
                twistDiff.x = -twistDiff.x;
                twistDiff.y = -twistDiff.y;
                twistDiff.z = -twistDiff.z;
            }
        }
        else
        {
            if (twistDiff.w < 0)
            {
                twistDiff.w = -twistDiff.w;
                twistDiff.x = -twistDiff.x;
                twistDiff.y = -twistDiff.y;
                twistDiff.z = -twistDiff.z;
            }
        }

        //スイングの経由点を生成
        for (int i = 0; i <= splits; i++)
        {
            Quaternion qs = Quaternion.Slerp(fromSwing, toSwing, (float)i / splits);
            //角度制限
            Vector3 STqs = STInterconversion.Quaternion2SwingTwist(qs, targetSTAL.twistAxisRight, targetSTAL.twistAxisUp, targetSTAL.twistAxisForward);
            STqs.x = Mathf.Clamp(STqs.x, 0, targetSTAL.SwingFunction(STqs.y));
            STqs.z = 0;
            //STqs.z = Mathf.Clamp(STqs.z, targetSTAL.twistLimit.x, targetSTAL.twistLimit.y);
            qs = STInterconversion.SwingTwist2Quaternion(STqs, targetSTAL.twistAxisRight, targetSTAL.twistAxisForward);

            Quaternion qt = Quaternion.Slerp(twistDiff, Quaternion.identity, (float)i / splits) * toTwist;
            results[i] = qs * qt;
        }
    }

    Quaternion STLimitInterpolation(Quaternion[] results, int splits, float t)
    {
        float temp = t * splits;
        int index = Mathf.FloorToInt(temp);
        float subT = temp - index;
        return Quaternion.Slerp(results[index], results[index + 1], subT);
    }
}
