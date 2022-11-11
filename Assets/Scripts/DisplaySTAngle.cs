using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySTAngle : MonoBehaviour
{
    public Vector3 twistAxis = new Vector3(1, 0, 0);
    [Header("Debug tools")]
    public Quaternion initialRotation;
    Vector3 twistAxisRight;
    Vector3 twistAxisUp;
    Vector3 twistAxisForward;
    bool isStart = false;
    Quaternion currentRotation;
    Quaternion currentRotationDiff;
    public Vector3 currentSwingTwist;

    void OnDrawGizmos()
    {
        //Gizmoの表示が有効であるとき，自動で初期化する．実行後は行わない．
        if (!isStart)
        {
            Initialize();
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
        currentRotation = transform.localRotation;
        currentRotationDiff = currentRotation * Quaternion.Inverse(initialRotation);

        currentSwingTwist = STInterconversion.Quaternion2SwingTwist(currentRotationDiff, twistAxisRight, twistAxisUp, twistAxisForward);
    }
}
