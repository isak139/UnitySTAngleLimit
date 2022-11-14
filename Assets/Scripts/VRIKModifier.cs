using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using RootMotion;

// VRIKModifyer.csにはsh-akira氏が作成したVirtualMotionCaptureからコピー&ペーストしたコードが含まれています
// Copyright (c) 2018 sh-akira
// Released under the MIT license

// https://github.com/sh-akira/VirtualMotionCapture/blob/master/LICENSE

// スクリプトからVRIKをAddComponentする場合、VRIK.InitiateSolverが呼ばれる前に実行される必要があるため、そのフレームでVRIKModifyerはAddComponentされる必要がある
[DefaultExecutionOrder(1)] // GameoObjectToPHBallJointTracerより後
public class VRIKModifier : MonoBehaviour
{
    public bool useVRoidSetting = true; // VRoidのモデル用パラメータを設定
    public bool useToe = false; // Trueであればつま先立ちする
    public bool useWristRotationFix = true; // 手首のねじれを解消するWristRotationFix.csを使用するか
    private VRIK vrik;

    private const float LeftLowerArmAngle = -30f;
    private const float RightLowerArmAngle = -30f;
    private const float LeftUpperArmAngle = -30f;
    private const float RightUpperArmAngle = -30f;
    private const float LeftHandAngle = -30f;
    private const float RightHandAngle = -30f;

    void Start()
    { // VRIK.InitiateSolverが呼ばれる前に実行される必要があるため、Awakeを使用
        SetVRIK();
    }
    bool isFirst = true;
    private void FixedUpdate()
    {
        if (isFirst)
        {
            SetVRIK();
            isFirst = false;
        }
    }
    public void SetVRIK()
    {
        SetVRIK(this.gameObject);
    }
    public void SetVRIK(GameObject model)
    {
        vrik = model.GetComponent<VRIK>();
        if (vrik == null)
        {
            vrik = model.AddComponent<VRIK>();
        }
        if (!useToe && vrik.references.isEmpty)
        { // 別のスクリプトでVRIKをAddComponentする場合がある
            vrik.AutoDetectReferences(); // ここでやらないとStartでReferencesが設定されてToeのみ消すことが出来ない
        }
        var preIKPositionWeight = vrik.solver.IKPositionWeight;
        vrik.solver.IKPositionWeight = 0f;
        if (useVRoidSetting)
        {
            // VRIKパラメータ設定
            vrik.solver.spine.maintainPelvisPosition = 0.08f;
            vrik.solver.leftLeg.swivelOffset = -38;
            vrik.solver.rightLeg.swivelOffset = 38;
            vrik.solver.locomotion.footDistance = 0.07f;
            vrik.solver.locomotion.stepThreshold = 0.2f;
            vrik.solver.locomotion.angleThreshold = 45f;
            vrik.solver.leftArm.stretchCurve = new AnimationCurve();
            vrik.solver.rightArm.stretchCurve = new AnimationCurve();
        }
        if (!useToe)
        {
            // つま先立ちしないように
            vrik.references.leftToes = null;
            vrik.references.rightToes = null;
        }
        // 再度IKの計算が実行
        vrik.UpdateSolverExternal(); // ここでVRIK.InitiateSolverが実行される，その後もう一度ExecutionOrderが9998であるVRIK.StartでVRIK.InitiateSolverが実行される
        vrik.solver.IKPositionWeight = preIKPositionWeight;

        if (useWristRotationFix)
        {
            var animator = model.GetComponent<Animator>();
            animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).eulerAngles = new Vector3(LeftLowerArmAngle, 0, 0);
            animator.GetBoneTransform(HumanBodyBones.RightLowerArm).eulerAngles = new Vector3(RightLowerArmAngle, 0, 0);
            animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).eulerAngles = new Vector3(LeftUpperArmAngle, 0, 0);
            animator.GetBoneTransform(HumanBodyBones.RightUpperArm).eulerAngles = new Vector3(RightUpperArmAngle, 0, 0);
            var wristRotationFix = model.GetComponent<WristRotationFix>();
            if (wristRotationFix == null)
            {
                wristRotationFix = model.AddComponent<WristRotationFix>();
            }
            wristRotationFix.SetVRIK(vrik);
        }
    }
}