using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour
{
    // Start is called before the first frame update

    private ArticulationBody ab;
    public float smooth = 50.0f;

    void Start()
    {
        ab = this.transform.GetComponent<ArticulationBody>();
      
        
        
    }

    void Update()
    {
        string str = ab.angularVelocity.ToString();
        //print(ab.angularVelocity);
        print("the angular velocity of wrist 03:"+str);
    }

 
}
