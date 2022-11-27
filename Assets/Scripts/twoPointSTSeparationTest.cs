using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twoPointSTSeparationTest : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private float duration = 2f;
    [SerializeField] Transform target;
    [SerializeField] STAngleLimit targetSTAL;
    [SerializeField] Transform from, to;
    [SerializeField] STAngleLimit fromSTAL, toSTAL;
    [SerializeField] bool isSwing = true;
    [SerializeField] bool isTwist = true;
    Quaternion swing, twist;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time / duration, 1);

        Quaternion fromSwing = Quaternion.FromToRotation(targetSTAL.twistAxisRight, from.localRotation * targetSTAL.twistAxisRight);
        Quaternion fromTwist = Quaternion.Inverse(fromSwing) * from.localRotation;
        Quaternion toSwing = Quaternion.FromToRotation(targetSTAL.twistAxisRight, to.localRotation * targetSTAL.twistAxisRight);
        Quaternion toTwist = Quaternion.Inverse(toSwing) * to.localRotation;

        STLimitInterpolation(fromSwing, fromTwist, toSwing, toTwist);

        Quaternion qs = Quaternion.Slerp(fromSwing, toSwing, t);
        Quaternion qt = Quaternion.Slerp(Quaternion.identity, twist, t) * toTwist;
        Debug.Log("swing: " + swing + " twist: " + twist);
        if (isSwing && isTwist)
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
        }
        /* Quaternion q = Quaternion.Slerp(from.localRotation, to.localRotation, t);
        target.localRotation = q; */
    }

    void STLimitInterpolation(/* Quaternion[] results, int splits,  */Quaternion fromSwing, Quaternion fromTwist, Quaternion toSwing, Quaternion toTwist)
    {
        //ツイスト
        twist = fromTwist * Quaternion.Inverse(toTwist);
        Debug.Log(Mathf.Abs(fromSTAL.currentSwingTwist.z - toSTAL.currentSwingTwist.z));
        if (Mathf.Abs(fromSTAL.currentSwingTwist.z - toSTAL.currentSwingTwist.z) > 180)
        {
            if (twist.w > 0)
            {
                twist.w = -twist.w;
                twist.x = -twist.x;
                twist.y = -twist.y;
                twist.z = -twist.z;
            }
        }
        else
        {
            if (twist.w < 0)
            {
                twist.w = -twist.w;
                twist.x = -twist.x;
                twist.y = -twist.y;
                twist.z = -twist.z;
            }
        }

    }
}
