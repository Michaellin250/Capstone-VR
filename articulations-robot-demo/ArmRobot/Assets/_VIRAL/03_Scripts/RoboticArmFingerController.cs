/************************************************************
 * Copyright (c) Holonautic Ltd. All rights reserved.
 * __________________________________________________
 * 
 * All information contained herein is, and remains
 * the property of Holonautic. The intellectual and technical
 * concepts contained herein are proprietary to Holonautic.
 * Dissemination of this information or reproduction of this
 * material is strictly forbidden unless prior written
 * permission is obtained from Holonautic.
 *
 * *******************************************************/

using _VIRAL._03_Scripts;
using RootMotion.FinalIK;
using UnityEngine;

public class RoboticArmFingerController : MonoBehaviour
{

    [SerializeField] private OVRHand _controlHand;

    [SerializeField] private Transform _wristBase;

    [SerializeField] private Transform _fingerJointLowerPart;
    [SerializeField] private Transform _fingerJointMiddlePart;
    [SerializeField] private Transform _fingerJointTopPart;

    [Space]
    [SerializeField] private Transform _roboticBase;
    [SerializeField] private Transform _roboticJointLowerPart;
    [SerializeField] private Transform _roboticJointMiddlePart;
    [SerializeField] private Transform _roboticJointTopPart;
    [SerializeField] private Transform _roboticHead;

    [SerializeField] private Pincher _pincher;

    public Pincher Pincher => _pincher;


    private RotationLimit _lowerLimitation;
    private RotationLimit _middleLimitation;
    private RotationLimit _topLimitation;


    [Range(0.0f, 1.0f)]
    [SerializeField] private float _speed = 0.1f;

    public Transform RoboticHead => _roboticHead;

    private void Awake()
    {

        _lowerLimitation = _roboticJointLowerPart.GetComponent<RotationLimit>();
        _middleLimitation = _roboticJointMiddlePart.GetComponent<RotationLimit>();
        _topLimitation = _roboticJointTopPart.GetComponent<RotationLimit>();
    }

    private void Update()
    {

        // UpdateArmPoseIteration1();
        // UpdateArmPoseIteration2();
        // UpdateArmPoseIteration3();
        UpdateArmPoseIteration4();
    }

    private void UpdateArmPoseIteration1()
    {
        var newBaseRotation = Quaternion.Euler(_roboticBase.localRotation.eulerAngles.x, _wristBase.localRotation.eulerAngles.y, _roboticBase.localRotation.eulerAngles.z);
        var newLowerRotation = _fingerJointLowerPart.localRotation;
        var newMiddleRotation = _fingerJointMiddlePart.localRotation;
        var newTopRotation = _fingerJointTopPart.localRotation;


        _roboticBase.localRotation = newBaseRotation;
        _roboticJointLowerPart.localRotation = newLowerRotation;
        _roboticJointMiddlePart.transform.localRotation = newMiddleRotation;
        _roboticJointTopPart.transform.localRotation = newTopRotation;
    }

    private void UpdateArmPoseIteration2()
    {

        var lowerRotation = _fingerJointLowerPart.localRotation.eulerAngles;
        var middleRotation = _fingerJointMiddlePart.localRotation.eulerAngles;
        var topRotation = _fingerJointTopPart.localRotation.eulerAngles;


        var newBaseRotation = Quaternion.Euler(_roboticBase.localRotation.eulerAngles.x, _wristBase.localRotation.eulerAngles.y - 180, _roboticBase.localRotation.eulerAngles.z);
        var newLowerRotation = Quaternion.Inverse(Quaternion.Euler(lowerRotation.z, lowerRotation.y, lowerRotation.x));
        var newMiddleRotation = Quaternion.Inverse(Quaternion.Euler(middleRotation.z, middleRotation.y - 90, middleRotation.x));
        var newTopRotation = Quaternion.Inverse(Quaternion.Euler(topRotation));


        _roboticBase.localRotation = newBaseRotation;
        _roboticJointLowerPart.localRotation = newLowerRotation;
        _roboticJointMiddlePart.transform.localRotation = newMiddleRotation;
        _roboticJointTopPart.transform.localRotation = newTopRotation;
    }


    private void UpdateArmPoseIteration3()
    {
        var lowerRotation = _wristBase.localRotation.eulerAngles;
        var middleRotation = _fingerJointMiddlePart.localRotation.eulerAngles;
        var topRotation = _fingerJointTopPart.localRotation.eulerAngles;


        var newBaseRotation = Quaternion.Euler(_roboticBase.localRotation.eulerAngles.x, lowerRotation.y - 180, _roboticBase.localRotation.eulerAngles.z);
        var newLowerRotation = Quaternion.Euler(lowerRotation.z + 90, lowerRotation.y, lowerRotation.x);
        var newMiddleRotation = Quaternion.Inverse(Quaternion.Euler(middleRotation.z + 30, middleRotation.y - 90, middleRotation.x));
        var newTopRotation = Quaternion.Inverse(Quaternion.Euler(topRotation.x, topRotation.y, topRotation.z + 30));


        _roboticBase.localRotation = newBaseRotation;
        _roboticJointLowerPart.localRotation = newLowerRotation;
        _roboticJointMiddlePart.transform.localRotation = newMiddleRotation;
        _roboticJointTopPart.transform.localRotation = newTopRotation;

    }

    private void UpdateArmPoseIteration4()
    {
        var changed = false;

        var lowerRotation = _wristBase.localRotation.eulerAngles;
        var middleRotation = _fingerJointMiddlePart.localRotation.eulerAngles;
        var topRotation = _fingerJointTopPart.localRotation.eulerAngles;


        var newBaseRotation = Quaternion.Euler(_roboticBase.localRotation.eulerAngles.x, lowerRotation.y - 180, _roboticBase.localRotation.eulerAngles.z);
        var newLowerRotation = Quaternion.Euler(lowerRotation.z + 90, lowerRotation.y, lowerRotation.x);
        var newMiddleRotation = Quaternion.Inverse(Quaternion.Euler(middleRotation.z + 30, middleRotation.y - 90, middleRotation.x));
        var newTopRotation = Quaternion.Inverse(Quaternion.Euler(topRotation.x, topRotation.y, topRotation.z + 30));


        var limitedLowerRotation = _lowerLimitation.GetLimitedLocalRotation(newLowerRotation, out changed);
        var limitedMiddleRotation = _middleLimitation.GetLimitedLocalRotation(newMiddleRotation, out changed);
        var limitedTopRotation = _topLimitation.GetLimitedLocalRotation(newTopRotation, out changed);
        _roboticBase.localRotation = Quaternion.Slerp(_roboticBase.localRotation, newBaseRotation, _speed);
        _roboticJointLowerPart.transform.localRotation = Quaternion.Slerp(_roboticJointLowerPart.localRotation, limitedLowerRotation, _speed);


        _roboticJointMiddlePart.transform.localRotation = Quaternion.Slerp(_roboticJointMiddlePart.localRotation, limitedMiddleRotation, _speed);
        _roboticJointTopPart.transform.localRotation = Quaternion.Slerp(_roboticJointTopPart.localRotation, limitedTopRotation, _speed);

        Quaternion rotDown = Quaternion.LookRotation(-Vector3.up, _roboticBase.forward);
        _roboticHead.rotation = rotDown;
    }



}
