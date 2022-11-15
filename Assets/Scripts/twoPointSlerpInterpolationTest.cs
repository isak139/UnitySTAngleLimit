using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twoPointSlerpInterpolationTest : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private float duration = 2f;
    [SerializeField] Transform target;
    [SerializeField] Transform from, to;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time / duration, 1);
        Quaternion q = Quaternion.Slerp(from.localRotation, to.localRotation, t);
        target.localRotation = q;

    }
}
