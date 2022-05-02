using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointPositionOfFingerA : MonoBehaviour
{
    // Start is called before the first frame update
    private ArticulationBody positionOfFingerA;
    public float smooth = 50.0f;
    void Start()
    {
        positionOfFingerA = this.transform.GetComponent<ArticulationBody>();

    }

    // Update is called once per frame
    void Update()
    {

        //string str = endPoint.velocity.ToString();
        //string str = endPoint.jointVelocity.ToString();
        string str = positionOfFingerA.worldCenterOfMass.ToString();
        print("the worldCenterOfMass of positionOfFingerA:" + str);

    }
}
