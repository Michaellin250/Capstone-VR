using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

/// <summary>
///
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "VRtoRob";

    // The game object
    //public GameObject cube;
    // Publish the cube's position and rotation every N seconds
    public float publishMessageFrequency = 1.0f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;
    // Start is called before the first frame update
    public ArticulationBody HandE;
    public float smooth = 50.0f;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PosRotMsg>(topicName);

        HandE = this.transform.GetComponent<ArticulationBody>();
    }

    private void Update()
    {

        Vector3 velocityofHandE = HandE.GetPointVelocity(HandE.worldCenterOfMass);
        string str = velocityofHandE.ToString();
        Vector3 angVelocity = HandE.angularVelocity;
        string str1 = angVelocity.ToString();
        string res = str + str1;
        //print("the velocity of worldCenterOfMass:" + str + str1);
        //print(velocityofHandE);
        //print(angVelocity);
        float velocity_x = velocityofHandE[0];
        float velocity_y = velocityofHandE[1];
        float velocity_z = velocityofHandE[2];
        float rot_x = angVelocity[0];
        float rot_y = angVelocity[1];
        float rot_z = angVelocity[2];
        float rot_w = 0.0F;

        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {

            PosRotMsg cubePos = new PosRotMsg(
                velocity_x,
                velocity_y,
                velocity_z,
                rot_x,
                rot_y,
                rot_z,
                rot_w
            );

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topicName, cubePos);

            timeElapsed = 0;
        }
    }
}