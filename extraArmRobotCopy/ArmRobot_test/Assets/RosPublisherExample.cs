using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;



/// <summary>
///
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "pos_rot";

    // The game object
    public GameObject RightHand; 
    // Publish the cube's position and rotation every N seconds
    public float publishMessageFrequency = 0.5f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;


    private Vector3 rotationLast;
    private Vector3 rotationDelta;

    private Vector3 pos, velocity;

    // to save to txt
    //private int i = 0;
    //private List<double[]> Allarr = new List<double[]>();

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PosRotMsg>(topicName);

        rotationLast = RightHand.transform.rotation.eulerAngles;
        pos = RightHand.transform.position;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        rotationDelta = RightHand.transform.rotation.eulerAngles - rotationLast;
        rotationLast = RightHand.transform.rotation.eulerAngles;

        velocity = (RightHand.transform.position - pos) / Time.deltaTime;
        pos = RightHand.transform.position;

        //to save to txt
        /*i++;
        double[] arr = new double[6];
        arr[0] = velocity[0];
        arr[1] = velocity[1];
        arr[2] = velocity[2];
        arr[3] = rotationDelta[0];
        arr[4] = rotationDelta[1];
        arr[5] = rotationDelta[2];
        Allarr.Add(arr);*/

        if (timeElapsed > publishMessageFrequency)
        {
            PosRotMsg cubePos = new PosRotMsg(
                velocity[0],
                velocity[1],
                velocity[2],
                rotationDelta[0],
                rotationDelta[1],
                rotationDelta[2],
                0.0F

            );

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topicName, cubePos);

            timeElapsed = 0;
        }


        //to save to txt
        /*if (i == 8000)
        {
            if (!System.IO.File.Exists("e:\\testtxt.txt"))
            {
                //Create a file if there is no file
                FileStream fs1 = new FileStream("C:\\testtxt.txt", FileMode.Create, FileAccess.Write);//Create the file  
                StreamWriter sw = new StreamWriter(fs1);
                string str8000 = transferListToString(Allarr, i);
                sw.WriteLine(str8000);//Write
                sw.Close();
                fs1.Close();
            }
            else
            {
                FileStream fs = new FileStream("e:\\testtxt.txt", FileMode.Open, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fs);
                string str8000 = transferListToString(Allarr, i);
                sr.WriteLine(str8000);//Write
                sr.Close();
                fs.Close();
            }

        }*/
    }
}