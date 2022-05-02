using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Robotics.ROSTCPConnector;
//using RosColor = RosMessageTypes.UnityRoboticsDemo.PosRotMsg;

public class RobotController : MonoBehaviour
{
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart;
        // public GameObject robotPart2;
    }
    public Joint[] joints;
    public Quaternion init1;

    //public GameObject cube;

    void Awake()
    {
        /*GameObject robotPart3 = joints[5].robotPart;
        Quaternion init1 = robotPart3.transform.rotation;
        print(init1);*/
        //GameObject robotPart3 = joints[2].robotPart;
        //Quaternion init1 = robotPart3.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        //robotPart3.transform.rotation = Quaternion.Euler(0, 0, 40F);
        //GameObject robotPart2 = joints[4].robotPart;
        //Quaternion init1 = robotPart3.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        //robotPart2.transform.rotation = Quaternion.Euler(0, 0, 40F);
    }

    void Start()
    {

        //Debug.Log("STARTING");
        //GameObject robotPart3 = joints[0].robotPart;
        //Quaternion init1 = robotPart3.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        //robotPart3.transform.rotation = Quaternion.Euler(0, 0, 300F);
        //GameObject robotPart2 = joints[1].robotPart;
        //Quaternion init1 = robotPart2.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        //robotPart2.transform.rotation = Quaternion.Euler(0, 0, 60F);
    }

    void Update()
    {

        //GameObject robotPart3 = joints[4].robotPart;
        //Quaternion init1 = robotPart3.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        //robotPart3.transform.localRotation = Quaternion.Euler(0, 0, 40F);

        //StopAllJointRotations();
        
        //GameObject robotPart2 = joints[2].robotPart;
        //robotPart2.transform.localRotation = Quaternion.Euler(0, 0, 10F);

        //GameObject robotPart1= joints[3].robotPart;
        //robotPart1.transform.localRotation = Quaternion.Euler(0, 20F, 0);



        //ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("color", PosChange);
        //GameObject robotPart1 = joints[3].robotPart;
        /*        float z = 0.766F;
                float w = 0.643F;
                Quaternion cioa = new Quaternion(0, 0, z, w);
                //robotPart1.transform.rotation = new Quaternion(0, 0, z, w);
                GameObject robotPart2 = joints[4].robotPart;
                robotPart2.transform.rotation = cioa;
                GameObject robotPart4 = joints[6].robotPart;
                robotPart4.transform.rotation = new Quaternion(0, 0, 0.871F, 0.996F);*/


        //print(init1);
        /*GameObject robotPart3 = joints[4].robotPart;
        //Quaternion init1 = robotPart3.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        robotPart3.transform.rotation =  Quaternion.Euler(0, 0, 40F);
        GameObject robotPart2 = joints[2].robotPart;
        //Quaternion init1 = robotPart2.transform.rotation;
        //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
        robotPart2.transform.rotation = Quaternion.Euler(0, 0, 60F);
        /* GameObject robotPart5 = joints[3].robotPart;
         //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
         robotPart5.transform.rotation = Quaternion.Euler(0, 0, 20F);
         GameObject robotPart6 = joints[1].robotPart;
         //robotPart3.transform.rotation = initialRot * Quaternion.Euler(0, 0, 45F);
         robotPart6.transform.rotation = Quaternion.Euler(0, 0, 40F);
        print("ciao");*/


    }

    //void poschange(roscolor colormessage)
    //{
    //    gameobject robotpart = joints[2].robotpart;
    //    // robotpart.transform.rotation = new quaternion(0, 0, 90, 0);
    //    debug.log(colormessage);
    //}

    // CONTROL

    public void hello(int i)
    {
        GameObject robotPart1 = joints[i - 1].robotPart;
        float z = 0.766F;
        float w = 0.643F;
        robotPart1.transform.rotation = new Quaternion(0, 0, z, w);
        GameObject robotPart2 = joints[i].robotPart;
        robotPart2.transform.rotation = new Quaternion(0, 0, z, w);
        GameObject robotPart3 = joints[i + 1].robotPart;
        robotPart3.transform.rotation = new Quaternion(0, 0, z, w);

    }

    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart = joints[i].robotPart;
            UpdateRotationState(RotationDirection.None, robotPart);
        }
    }

    public void RotateJoint(int jointIndex, RotationDirection direction)
    {
        //StopAllJointRotations();
        Joint joint = joints[jointIndex];
        UpdateRotationState(direction, joint.robotPart);
        //float hello = CurrentPrimaryAxisRotation();

        //GameObject robotPart = joints[3].robotPart;
        // robotPart.transform.rotation = new Quaternion(0, 0, 90, 0);
    }

    // HELPERS

    static void UpdateRotationState(RotationDirection direction, GameObject robotPart)
    {
        ArticulationJointController jointController = robotPart.GetComponent<ArticulationJointController>();
        jointController.rotationState = direction;


        ArticulationBody jointPos = robotPart.GetComponent<ArticulationBody>();
        float position0 = jointPos.jointPosition[0];
        //print(position0);
    }

    public float CurrentPrimaryAxisRotation()
    {
        GameObject robotPart = joints[3].robotPart;
        ArticulationBody articulation = robotPart.GetComponent<ArticulationBody>();
        float currentRotationRads = articulation.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * currentRotationRads;
        // robotPart.transform.rotation = new Quaternion(0,0,90,0);
        //print(currentRotation);
        return currentRotation;
    }




}
