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

    [SerializeField] public Transform _fingerJointMiddlePartPrev;


    [Space]
    [SerializeField] private Transform _roboticBase;
    [SerializeField] private Transform _roboticJointLowerPart;
    [SerializeField] private Transform _roboticJointMiddlePart;
    [SerializeField] private Transform _roboticJointTopPart;
    [SerializeField] private Transform _roboticJointTopPart2;
    [SerializeField] private Transform _roboticJointTopPart3;
    [SerializeField] private Transform _roboticHead;

    [SerializeField] private Pincher _pincher;

    public Pincher Pincher => _pincher;


    private RotationLimit _lowerLimitation;
    private RotationLimit _middleLimitation;
    private RotationLimit _topLimitation;
    private RotationLimit _topLimitation2;
    private RotationLimit _topLimitation3;

    public GameObject endDefector;
    public int bounded;
    public GameObject cube;
    public int cubeScaleFactor;


    [Range(0.0f, 1.0f)]
    [SerializeField] private float _speed = 0.1f;

    public Transform RoboticHead => _roboticHead;

    private void Awake()
    {

        //endDefector = GameObject.Find("/GradientDescent/UR3/Base/Shoulder/Elbow/Wrist1/Wrist2/Wrist3/HandE");

        /*GameObject emptyGO = new GameObject();
        Transform newTransform = emptyGO.transform;

        _fingerJointMiddlePartPrev = newTransform;*/

        _fingerJointMiddlePartPrev = Instantiate(_fingerJointMiddlePart);
        Debug.Log("FIRST HAND POSITION: ");
        Debug.Log(_fingerJointMiddlePartPrev.position);
        _lowerLimitation = _roboticJointLowerPart.GetComponent<RotationLimit>();
        _middleLimitation = _roboticJointMiddlePart.GetComponent<RotationLimit>();
        _topLimitation = _roboticJointTopPart.GetComponent<RotationLimit>();
        _topLimitation2 = _roboticJointTopPart2.GetComponent<RotationLimit>();
        _topLimitation3 = _roboticJointTopPart3.GetComponent<RotationLimit>();
    }

    private void Update()
    {

        // UpdateArmPoseIteration1();
        // UpdateArmPoseIteration2();
        // UpdateArmPoseIteration3();

        //Debug.Log("Previous Hand Position");
        //Debug.Log(_fingerJointMiddlePartPrev.position);

        UpdateArmPoseIteration5();
        //Debug.Log("Previous Hand Position");
        //Debug.Log(_fingerJointMiddlePartPrev.position);
        //_fingerJointLowerPartPrev = _fingerJointLowerPart;
        //_fingerJointMiddlePartPrev = _fingerJointMiddlePart;

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
        _roboticJointTopPart2.transform.localRotation = newTopRotation;
        _roboticJointTopPart3.transform.localRotation = newTopRotation;
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
        _roboticJointTopPart2.transform.localRotation = newTopRotation;
        _roboticJointTopPart3.transform.localRotation = newTopRotation;
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
        _roboticJointTopPart2.transform.localRotation = newTopRotation;
        _roboticJointTopPart3.transform.localRotation = newTopRotation;
    }

    private void UpdateArmPoseIteration4()
    {
        var changed = false;

        var lowerRotation = _wristBase.localRotation.eulerAngles;
        var middleRotation = _fingerJointMiddlePart.localRotation.eulerAngles;
        var topRotation = _fingerJointTopPart.localRotation.eulerAngles;

        Debug.Log("Lower Rotation is");
        Debug.Log(lowerRotation);
        Debug.Log("Middle Rotation is");
        Debug.Log(middleRotation);
        Debug.Log("Top Rotation is");
        Debug.Log(topRotation);

        // lowerRotation.y - 180 rotates 180 
        var newBaseRotation = Quaternion.Euler(_roboticBase.localRotation.eulerAngles.x, lowerRotation.y - 180, _roboticBase.localRotation.eulerAngles.z);
        var newLowerRotation = Quaternion.Euler(lowerRotation.z + 90, lowerRotation.y, lowerRotation.x);
        // 90 degrees max on y rotation 
        var newMiddleRotation = Quaternion.Inverse(Quaternion.Euler(middleRotation.z + 30, middleRotation.y - 90, middleRotation.x));
        //topRotation.z + 30 uses the wrist to rotate
        var newTopRotation = Quaternion.Inverse(Quaternion.Euler(topRotation.x, topRotation.y, topRotation.z + 30));

        var newTopRotation2 = _fingerJointTopPart.localRotation;
        var newTopRotation3 = Quaternion.Inverse(Quaternion.Euler(topRotation));



        // settings limits of rotation 
        var limitedLowerRotation = _lowerLimitation.GetLimitedLocalRotation(newLowerRotation, out changed);
        var limitedMiddleRotation = _middleLimitation.GetLimitedLocalRotation(newMiddleRotation, out changed);
        var limitedTopRotation = _topLimitation.GetLimitedLocalRotation(newTopRotation, out changed);
        _roboticBase.localRotation = Quaternion.Slerp(_roboticBase.localRotation, newBaseRotation, _speed);
        _roboticJointLowerPart.transform.localRotation = Quaternion.Slerp(_roboticJointLowerPart.localRotation, limitedLowerRotation, _speed);


        _roboticJointMiddlePart.transform.localRotation = Quaternion.Slerp(_roboticJointMiddlePart.localRotation, limitedMiddleRotation, _speed);
        _roboticJointTopPart.transform.localRotation = Quaternion.Slerp(_roboticJointTopPart.localRotation, limitedTopRotation, _speed);
        _roboticJointTopPart2.transform.localRotation = Quaternion.Slerp(_roboticJointTopPart2  .localRotation, limitedTopRotation, _speed);
        _roboticJointTopPart3.transform.localRotation = Quaternion.Slerp(_roboticJointTopPart3.localRotation, limitedTopRotation, _speed);
        
        Quaternion rotDown = Quaternion.LookRotation(-Vector3.up, _roboticBase.forward);
        _roboticHead.rotation = rotDown;
    }

    private void UpdateArmPoseIteration5()
    {

        /*
        
        Debug.Log("Printing Middle Position");
        Debug.Log(_fingerJointMiddlePart.localPosition);

        var deltaPosition = _fingerJointMiddlePart.localPosition - _fingerJointMiddlePartPrev.localPosition;
        Debug.Log("Printing Delta Middle Position");
        Debug.Log(deltaPosition);


        Debug.Log("Printing End Defector Position Before");
        Debug.Log(endDefector.transform.localPosition);
        Debug.Log("Printing End Defector Position After");
        endDefector.transform.localPosition += deltaPosition;
        //Debug.Log(endDefector.transform.localPosition);
        //endDefector.transform.localPosition += new Vector3(5, 0, 0);
        Debug.Log(endDefector.transform.localPosition);*/

        var lowerRotation = _wristBase.localRotation.eulerAngles;
        var middleRotation = _fingerJointMiddlePart.localRotation.eulerAngles;
        var topRotation = _fingerJointTopPart.localRotation.eulerAngles;

        var changed = false;

        //Debug.Log("CURRENT POSITION OF HAND: ");
        //Debug.Log(_fingerJointMiddlePart.position);

        var deltaPosition = _fingerJointMiddlePart.position - _fingerJointMiddlePartPrev.position;




        // lowerRotation.y - 180 rotates 180 
        var newBaseRotation = Quaternion.Euler(_roboticBase.localRotation.eulerAngles.x, lowerRotation.y, _roboticBase.localRotation.eulerAngles.z);

        // 90 degrees max on y rotation 
        var newMiddleRotation = Quaternion.Inverse(Quaternion.Euler(middleRotation.z, middleRotation.y, middleRotation.x));


        // settings limits of rotation 
        var limitedMiddleRotation = _middleLimitation.GetLimitedLocalRotation(newMiddleRotation, out changed);

        //_roboticBase.localRotation = Quaternion.Slerp(_roboticBase.localRotation, newBaseRotation, _speed);
        endDefector.transform.localRotation = Quaternion.Slerp(endDefector.transform.localRotation, newMiddleRotation, _speed);


        //Debug.Log("PRINTING DELTA POSITION");
        //Debug.Log(deltaPosition);

        /*
        if (Mathf.Abs(deltaPosition.x) > bounded || Mathf.Abs(deltaPosition.y) > bounded || Mathf.Abs(deltaPosition.z) > bounded)
        {
            return;
        }*/

        float deltaDistance = Mathf.Sqrt((deltaPosition.x * deltaPosition.x) + (deltaPosition.y * deltaPosition.y) + (deltaPosition.z * deltaPosition.z));
        Debug.Log("PRINTING DELTA DISTANCE");
        Debug.Log(deltaDistance);

        if (Mathf.Abs(deltaDistance) > bounded)
        {
            return;
        }

        /*if (Mathf.Abs(deltaPosition.x) <= bounded || Mathf.Abs(deltaPosition.y) <= bounded || Mathf.Abs(deltaPosition.z) <= bounded)
        {
            endDefector.transform.localPosition += deltaPosition;
        }*/

        Debug.Log("PRINTING DELTA POSITION");
        Debug.Log(deltaPosition);

        if (deltaPosition.x < 0){
            cube.transform.localPosition -= new Vector3(0.1f / cubeScaleFactor, 0, 0);
        } else {
            cube.transform.localPosition += new Vector3(0.1f / cubeScaleFactor, 0, 0);
        }

     
        //endDefector.transform.localPosition += deltaPosition;
    }




}
