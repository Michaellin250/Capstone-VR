using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManualInput : MonoBehaviour
{
    public GameObject robot;
    public int currentJointIndex;
    public CustomController robotController;
    public GameObject handsObject;
    public Vector3 handsPreviousPosition;
    public Quaternion handsPreviousRotation;

    void Start()
	{
        currentJointIndex = 0;
        robotController = robot.GetComponent<CustomController>();
        handsPreviousPosition = handsObject.transform.position;
        handsPreviousRotation = handsObject.transform.localRotation;
    }

    void Update()
    {
        float inputVal = Input.GetAxis("ChangeJoint");

        Vector3 deltaPosition = handsObject.transform.position - handsPreviousPosition;

        if (inputVal > 0 || deltaPosition.y > 0)
        {
            currentJointIndex = (currentJointIndex + 1) % robotController.joints.Length;
        } 
        else if(inputVal < 0 || deltaPosition.y < 0)
		{
            currentJointIndex = (currentJointIndex - 1 + robotController.joints.Length) % robotController.joints.Length;
        }

        //inputVal = Input.GetAxis("MoveJoint");
        //Debug.Log("Previous Rotation Is: ");
        //Debug.Log(handsPreviousRotation);
        Quaternion changeInRotation = handsPreviousRotation * Quaternion.Inverse(handsObject.transform.rotation);
        //Debug.Log(changeInRotation); 
        inputVal = changeInRotation.z;

        if (Mathf.Abs(inputVal) > 0)
        {
            RotationDirection direction = GetRotationDirection(inputVal);

            robotController.RotateJoint(currentJointIndex, direction);
            return;
        }

        robotController.StopAllJointRotations();

        // update state of previous position of hands
        handsPreviousPosition = handsObject.transform.position;
        Debug.Log(handsObject.transform.localRotation);
        handsPreviousRotation = handsObject.transform.localRotation;

    }


    // HELPERS

    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
        {
            return RotationDirection.Positive;
        }
        else if (inputVal < 0)
        {
            return RotationDirection.Negative;
        }
        else
        {
            return RotationDirection.None;
        }
    }
}
