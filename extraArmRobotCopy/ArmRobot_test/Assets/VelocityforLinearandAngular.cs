using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityforLinearandAngular : MonoBehaviour
{
    // Start is called before the first frame update
    // private ArticulationBody HandE;
    // public float smooth = 50.0f;

    Vector3 rotationLast;
    Vector3 rotationDelta;

    Vector3 pos, velocity;

    void Start()
    {
        // HandE = this.transform.GetComponent<ArticulationBody>();
        rotationLast = transform.rotation.eulerAngles;
        pos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        rotationDelta = transform.rotation.eulerAngles - rotationLast;
        rotationLast = transform.rotation.eulerAngles;

        velocity = (transform.position - pos) / Time.deltaTime;
        pos = transform.position;

        /*Vector3 velocityofHandE = HandE.GetPointVelocity(HandE.worldCenterOfMass);
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
        print(velocity_x);*/

        print(velocity);


    }
}
