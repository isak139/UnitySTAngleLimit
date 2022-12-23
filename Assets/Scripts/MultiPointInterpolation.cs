using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPointInterpolation : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    [SerializeField][Range(0, 10)] private float duration = 2f;
    [SerializeField] bool slerpOrSquad = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time / duration, 1);
        Quaternion q = Quaternion.identity;
        if (slerpOrSquad)
        {
            // waypointsの値をSLERPで補間する
            float temp = 1f / (waypoints.Count - 1);
            int index = Mathf.FloorToInt(t / temp);
            float t2 = (t - temp * index) / temp;
            q = Quaternion.Slerp(waypoints[index].rotation, waypoints[index + 1].rotation, t2);
        }
        else
        {
            Quaternion[] quaternions = new Quaternion[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
            {
                quaternions[i] = waypoints[i].rotation;
            }
            q = Squad.Spline(quaternions, t);
        }

        transform.localRotation = q;
    }
}
