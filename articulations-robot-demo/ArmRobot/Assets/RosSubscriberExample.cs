using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosColor = RosMessageTypes.UnityRoboticsDemo.PosRotMsg;
using System.Collections;

public class RosSubscriberExample : MonoBehaviour
{
    public GameObject cube;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<RosColor>("color", PosChange);
    }

    void PosChange(RosColor colorMessage)
    {
        Debug.Log(colorMessage);
    }
}