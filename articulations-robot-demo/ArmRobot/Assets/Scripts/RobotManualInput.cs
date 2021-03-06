using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManualInput : MonoBehaviour
{
    public GameObject robot;
    public int currentJointIndex;
    public CustomController robotController;
    public GameObject handsObject;
   // public Vector3 handsPreviousPosition;
    //public Quaternion handsPreviousRotation;
    public Transform cube;
    public int offSetMovement;

    void Start()
	{
        currentJointIndex = 0;
        robotController = robot.GetComponent<CustomController>();
        //handsPreviousPosition = handsObject.transform.position;
        //handsPreviousRotation = handsObject.transform.localRotation;
    }

    void Update()
    {

        /* pressing W, D or Up and Down Arrow Keys */
        float inputVal = Input.GetAxis("ChangeJoint");

        //Vector3 deltaPosition = handsObject.transform.position - handsPreviousPosition;

        if (inputVal > 0 /*|| deltaPosition.y > 0*/)
        {
            cube.transform.localPosition += new Vector3(0.1f / offSetMovement, 0 , 0);
            //currentJointIndex = (currentJointIndex + 1) % robotController.joints.Length;
        } 
        else if(inputVal < 0 /*|| deltaPosition.y < 0*/)
		{
            cube.transform.localPosition -= new Vector3(0.1f / offSetMovement, 0 ,0 );
            //currentJointIndex = (currentJointIndex - 1 + robotController.joints.Length) % robotController.joints.Length;
        }

        /* Press A and D arrow Keys */
        inputVal = Input.GetAxis("MoveJoint");
        //Quaternion changeInRotation = handsPreviousRotation * Quaternion.Inverse(handsObject.transform.rotation);
        //inputVal = changeInRotation.z;

        if (inputVal > 0 /*|| deltaPosition.y > 0*/)
        {
            cube.transform.localPosition += new Vector3(0, 0, 0.1f / offSetMovement);
            //currentJointIndex = (currentJointIndex + 1) % robotController.joints.Length;
        }
        else if (inputVal < 0 /*|| deltaPosition.y < 0*/)
        {
            cube.transform.localPosition -= new Vector3(0, 0, 0.1f / offSetMovement);
            //currentJointIndex = (currentJointIndex - 1 + robotController.joints.Length) % robotController.joints.Length;
        }


        // update state of previous position of hands
        /*handsPreviousPosition = handsObject.transform.position;
        handsPreviousRotation = handsObject.transform.localRotation;*/

        /*
        if (Mathf.Abs(inputVal) > 0)
        {
            RotationDirection direction = GetRotationDirection(inputVal);
            robotController.RotateJoint(currentJointIndex, direction);
            return;
        }

        robotController.StopAllJointRotations();*/

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
