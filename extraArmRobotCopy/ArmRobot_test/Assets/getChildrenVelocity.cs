using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class getChildrenVelocity : MonoBehaviour
{
    public ArticulationBody[] allABs;
    public int currentABIndex;
    //public List<angularVelocity[]> originalVelocities;

    // Start is called before the first frame update
    void Start()
    {
        //allABs = GetComponentsInChildren<ArticulationBody>();
        //originalVelocities = new List<angularVelocity[]>();
        //for(int i = 0; i < allABs.Length; i++)
        /*{
            angularVelocity[] toAdd = new angularVelocity[allABs[i].angularVelocity.length];
            for(int j = 0; j < toAdd.Length; j++)
            {
                toAdd[j] = new angularVelocity(allABs[i].angularVelocity[j]);

            }
            originalVelocities.Add(toAdd);

        }
        //currentABIndex = GameObject.Find("ManualInput").GetComponent<RobotManualInput>().currentJointIndex;*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
