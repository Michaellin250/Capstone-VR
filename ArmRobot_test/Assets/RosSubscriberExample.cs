using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosColor = RosMessageTypes.UnityRoboticsDemo.PosRotMsg;
using System.Collections;

public class RosSubscriberExample : MonoBehaviour
{

    // public GameObject cube;
    //ROS THINGS 
    float timeElapsed;
    public float angle0 = 0F;
    public float angle1 = -1.57F;
    public float angle2 = 0F;
    public float angle3 = -1.57F;
    public float angle4 = 0F;
    public float angle5 = -1.57F;
    //ROSConnection ros;

    // moving joints
    [System.Serializable]
    public struct Joint
    {
        public string inputAxis;
        public GameObject robotPart;
        // public GameObject robotPart2;
    }
    public Joint[] joints;
   // public Quaternion init1;


    void Start()
    {
        //ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("color", PosChange);
        print("hello");
        //ros = ROSConnection.GetOrCreateInstance().Subscribe<RosColor>;
        Debug.Log("connecting");
    }

    void Update()
    {

        GameObject robotPart0 = joints[0].robotPart;
        robotPart0.transform.localRotation = Quaternion.Euler(0, -angle0 * Mathf.Rad2Deg, 0);
        GameObject robotPart1 = joints[1].robotPart;
        robotPart1.transform.localRotation = Quaternion.Euler(0, 0, (angle1 * Mathf.Rad2Deg) + 90F);
        GameObject robotPart2 = joints[2].robotPart;
        robotPart2.transform.localRotation = Quaternion.Euler(0, 0, angle2 * Mathf.Rad2Deg);

        /*
        Changed Joint From 3 to 4 @Sanam. Pengwei's joint 4 is not the same as our joint 4.
        Robotics robot is missing a rotation axis... 
         */
        GameObject robotPart3 = joints[4].robotPart;
        robotPart3.transform.localRotation = Quaternion.Euler(0, 0, (angle3 * Mathf.Rad2Deg) + 90F);
        GameObject robotPart4 = joints[5].robotPart;
        robotPart4.transform.localRotation = Quaternion.Euler(0, -angle4 * Mathf.Rad2Deg, 0);

        /*
        End-Effector Grasper Rotation doesn't work. The active line just puts the grasper in the proper orientation. 
        The commented line is supposed to rotate the grasper. But the grasper never rotates about the correct axis.
         */
        GameObject robotPart5 = joints[6].robotPart;
        robotPart5.transform.localRotation = Quaternion.Euler(-1.57F * Mathf.Rad2Deg, 0, 0);
        //robotPart5.transform.localRotation = Quaternion.Euler(-1.57F * Mathf.Rad2Deg, angle5 * Mathf.Rad2Deg,0);


        // Original Code
        /*
        GameObject robotPart0 = joints[0].robotPart;
        robotPart0.transform.localRotation = Quaternion.Euler(0, -angle0 * Mathf.Rad2Deg, 0);
        GameObject robotPart1 = joints[1].robotPart;
        robotPart1.transform.localRotation = Quaternion.Euler(0, 0, (angle1 * Mathf.Rad2Deg) + 90F);
        GameObject robotPart2 = joints[2].robotPart;
        robotPart2.transform.localRotation = Quaternion.Euler(0, 0, angle2 * Mathf.Rad2Deg);
        GameObject robotPart3 = joints[3].robotPart;
        robotPart3.transform.localRotation = Quaternion.Euler(0, (angle3 * Mathf.Rad2Deg) + 90F, 0);
        GameObject robotPart4 = joints[4].robotPart;
        robotPart4.transform.localRotation = Quaternion.Euler(0, 0, angle4 * Mathf.Rad2Deg);
        GameObject robotPart5 = joints[5].robotPart;
        robotPart5.transform.localRotation = Quaternion.Euler(0, angle5 * Mathf.Rad2Deg,0);
        */

        timeElapsed += 1;

        if (timeElapsed > 0.5f)
        {


            // Finally send the message to server_endpoint.py running in ROS
            //ros.Publish(topicName, cubePos);
            //Debug.Log("in the for loop");
            //ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("color", PosChange);
            ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("RobtoVR", PosChange);

            timeElapsed = 0;
        }



    }

    void PosChange(RosColor colorMessage)
    {
        Debug.Log("calling the function");
        Debug.Log(colorMessage);

        angle0 = colorMessage.pos_x;
        angle1 = colorMessage.pos_y;
        angle2 = colorMessage.pos_z;
        angle3 = colorMessage.rot_x;
        angle4 = colorMessage.rot_y;
        angle5 = colorMessage.rot_z;



        //GameObject robotPart3 = joints[1].robotPart;
        //Quaternion init1 = robotPart3.transform.rotation;
        //robotPart3.transform.localRotation = Quaternion.Euler(0, 0, 45F);

        //StopAllJointRotations();

        //GameObject robotPart2 = joints[2].robotPart;
        //robotPart2.transform.localRotation = Quaternion.Euler(0, 0, 10F);

        //GameObject robotPart1 = joints[3].robotPart;
        //robotPart1.transform.localRotation = Quaternion.Euler(0, 20F, 0);
    }
}