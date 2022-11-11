using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST2PointInterpolationTest : MonoBehaviour
{
    //[SerializeField] GameObject from, to;
    [SerializeField][Range(0, 10)] private float duration = 2f;
    [SerializeField] STAngleLimit target;
    [SerializeField] DisplaySTAngle from, to;
    STAngleLimit STAngleLimitFrom;
    STAngleLimit STAngleLimitTo;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 STFrom = from.currentSwingTwist;
        Vector3 STTo = to.currentSwingTwist;
        float t = Mathf.PingPong(Time.time / duration, 1);

        float x = Mathf.Lerp(STFrom.x, STTo.x, t);

        float y = Mathf.LerpAngle(STFrom.y, STTo.y, t);

        float z = Mathf.Lerp(STFrom.z, STTo.z, t);

        //Vector3 lerpSTAngle = Vector3.Lerp(STFrom, STTo, t);
        Vector3 lerpSTAngle = new Vector3(x, y, z);
        target.SetSTAngle(lerpSTAngle);
        //Debug.Log(slerpSTAngle);
    }
}
