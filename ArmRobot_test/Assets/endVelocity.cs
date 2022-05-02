using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endVelocity : MonoBehaviour
{
    // Start is called before the first frame update
    private ArticulationBody endPoint;
    public float smooth = 50.0f;
    void Start()
    {
        endPoint = this.transform.GetComponent<ArticulationBody>();

    }

    // Update is called once per frame
    void Update()
    {

        //string str = endPoint.velocity.ToString();
        //string str = endPoint.jointVelocity.ToString();
        string str = endPoint.worldCenterOfMass.ToString();
        print("the worldCenterOfMass of endPoint:" + str);

    }
}
