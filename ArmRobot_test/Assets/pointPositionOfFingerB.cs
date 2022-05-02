using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointPositionOfFingerB : MonoBehaviour
{
    // Start is called before the first frame update
    private ArticulationBody positionOfFingerB;
    public float smooth = 50.0f;
    void Start()
    {
        positionOfFingerB = this.transform.GetComponent<ArticulationBody>();

    }

    // Update is called once per frame
    void Update()
    {

        //string str = endPoint.velocity.ToString();
        //string str = endPoint.jointVelocity.ToString();
        //string str = positionOfFingerB.worldCenterOfMass.ToString();
        //print("the worldCenterOfMass of positionOfFingerB:" + str);
        Vector3 velocityofFingerB= positionOfFingerB.GetPointVelocity(positionOfFingerB.worldCenterOfMass);
        string str= velocityofFingerB.ToString();
        print("the velocity of worldCenterOfMass of positionOfFingerB:" + str);


    }
}
