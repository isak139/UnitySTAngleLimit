using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twoPointSTSeparationTest : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private float duration = 2f;
    [SerializeField] Transform target;
    //[SerializeField] STAngleLimit targetSTAL;
    [SerializeField] Transform from, to;
    [SerializeField] STAngleLimit fromSTAL, toSTAL;
    [SerializeField] bool isSwing = true;
    [SerializeField] bool isTwist = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time / duration, 1);
        Quaternion fromSwing = Quaternion.FromToRotation(fromSTAL.twistAxisRight, from.localRotation * fromSTAL.twistAxisRight);
        Quaternion fromTwist = Quaternion.Inverse(fromSwing) * from.localRotation;
        Quaternion toSwing = Quaternion.FromToRotation(toSTAL.twistAxisRight, to.localRotation * toSTAL.twistAxisRight);
        Quaternion toTwist = Quaternion.Inverse(toSwing) * to.localRotation;
        Quaternion swing = Quaternion.Slerp(fromSwing, toSwing, t);
        Quaternion twist = Quaternion.Slerp(fromTwist, toTwist, t);
        if (isSwing && isTwist)
        {
            target.localRotation = swing * twist;
        }
        else if (isSwing)
        {
            target.localRotation = swing;
        }
        else if (isTwist)
        {
            target.localRotation = twist;
        }
        else
        {
            target.localRotation = Quaternion.identity;
        }
        /* Quaternion q = Quaternion.Slerp(from.localRotation, to.localRotation, t);
        target.localRotation = q; */
    }
}
