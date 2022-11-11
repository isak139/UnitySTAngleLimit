using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAngleLimit : MonoBehaviour
{
    public Vector3 twistAxis = new Vector3(1, 0, 0);
    public float swingLimit = 45.0f;
    public Vector2 twistLimit = new Vector2(-45.0f, 45.0f);
    [Header("Debug tools")]
    public Quaternion initialRotation;
    [SerializeField] float gizmoSize = 1.5f;
    Vector3 twistAxisRight;
    Vector3 twistAxisUp;
    Vector3 twistAxisForward;
    bool isStart = false;
    Quaternion currentRotation;
    Quaternion currentRotationDiff;
    public Vector3 currentSwingTwist;
    private void OnValidate()
    {
        //入力制限
        swingLimit = Mathf.Clamp(swingLimit, 0, 180);
        twistLimit.x = Mathf.Clamp(twistLimit.x, -180, 0);
        twistLimit.y = Mathf.Clamp(twistLimit.y, 0, 180);
        gizmoSize = Mathf.Clamp(gizmoSize, 0.1f, 2);
    }

    //これを変更することで方位角によって制限角度を変更する．inspectorから制御できるようにする．
    float SwingFunction(float phi)
    {
        float theta = (Mathf.Sin(Mathf.Deg2Rad * phi * 3) + 2) * swingLimit;
        return swingLimit;
    }

    void OnDrawGizmos()
    {
        //Gizmoの表示が有効であるとき，自動で初期化する．実行後は行わない．
        if (!isStart)
        {
            Initialize();
        }

        Quaternion parentRotation;
        if (transform.parent != null) parentRotation = transform.parent.rotation;
        else parentRotation = Quaternion.identity;


        // スイングツイスト系の基準軸を表示する．赤がツイスト軸
        Debug.DrawRay(transform.position, parentRotation * twistAxisRight * gizmoSize, Color.red);
        Debug.DrawRay(transform.position, parentRotation * twistAxisUp * gizmoSize, Color.green);
        Debug.DrawRay(transform.position, parentRotation * twistAxisForward * gizmoSize, Color.blue);
        // スイングツイスト系の制限範囲を表示する．
        // スイングの制限範囲を表示する．
        Vector3 drawVec = parentRotation * Quaternion.AngleAxis(0, twistAxisRight) * Quaternion.AngleAxis(SwingFunction(0), twistAxisForward) * twistAxisRight;
        Debug.DrawRay(transform.position, drawVec * gizmoSize, Color.yellow);
        for (int phi = 12; phi <= 360; phi += 12)
        {
            Debug.DrawRay(transform.position, drawVec * gizmoSize, Color.yellow);
            Vector3 nextVec = parentRotation * Quaternion.AngleAxis(phi, twistAxisRight) * Quaternion.AngleAxis(SwingFunction(phi), twistAxisForward) * twistAxisRight;
            Debug.DrawLine(transform.position + drawVec * gizmoSize, transform.position + nextVec * gizmoSize, Color.yellow);
            drawVec = nextVec;
        }
    }

    [ContextMenu("Initialize")]
    void Initialize()
    {
        twistAxisRight = (transform.rotation * twistAxis).normalized;
        twistAxisUp = (Vector3.Cross(twistAxisRight, -transform.forward)).normalized;
        twistAxisForward = (Vector3.Cross(twistAxisRight, twistAxisUp)).normalized;
        if (!isStart)
        {
            //実行後は変更しない．
            initialRotation = transform.localRotation;
        }
    }
    void Start()
    {
        Initialize();
        isStart = true;
    }
    void Update()
    {
        /* Quaternion parentRotation;
        if (transform.parent != null) parentRotation = transform.parent.rotation;
        else parentRotation = Quaternion.identity;

        twistAxisRight = (transform.rotation * twistAxis).normalized;
        twistAxisUp = (Vector3.Cross(twistAxisRight, -transform.forward)).normalized;
        twistAxisForward = (Vector3.Cross(twistAxisRight, twistAxisUp)).normalized; */


        currentRotation = transform.localRotation;
        currentRotationDiff = currentRotation * Quaternion.Inverse(initialRotation);

        currentSwingTwist = STInterconversion.Quaternion2SwingTwist(currentRotationDiff, twistAxisRight, twistAxisUp, twistAxisForward);
    }
    void LateUpdate()
    {
        // 回転制限
        currentSwingTwist.x = Mathf.Clamp(currentSwingTwist.x, 0, SwingFunction(currentSwingTwist.y));
        currentSwingTwist.z = Mathf.Clamp(currentSwingTwist.z, twistLimit.x, twistLimit.y);
        Quaternion q = STInterconversion.SwingTwist2Quaternion(currentSwingTwist, twistAxisRight, twistAxisForward);
        transform.localRotation = initialRotation * q;
    }
    public void SetSTAngle(Vector3 swingTwist)
    {
        currentSwingTwist = swingTwist;
        Debug.Log(currentSwingTwist);
        Quaternion q = STInterconversion.SwingTwist2Quaternion(currentSwingTwist, twistAxisRight, twistAxisForward);
        transform.localRotation = initialRotation * q;
    }
}
