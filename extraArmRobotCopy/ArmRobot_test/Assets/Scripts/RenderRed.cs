using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderRed : MonoBehaviour
{
    public Renderer[] allRenderers;
    public int currentRenderIndex;
    public List<Material[]> originalMaterials;

    void Start()
    {
        allRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new List<Material[]>();

        // creates a copy of all the materials and colors of the joints in order to reset the colors later
        for (int i = 0; i < allRenderers.Length; i++)
		{
            Material[] toAdd = new Material[allRenderers[i].materials.Length];
            for(int j = 0; j < toAdd.Length; j++)
			{
                toAdd[j] = new Material(allRenderers[i].materials[j]);
			}
            originalMaterials.Add(toAdd);
		}
        currentRenderIndex = GameObject.Find("ManualInput").GetComponent<RobotManualInput>().currentJointIndex;
    }

    void Update()
	{
        int newRenderIndex = GameObject.Find("ManualInput").GetComponent<RobotManualInput>().currentJointIndex;

        if (newRenderIndex != currentRenderIndex)
		{
            resetOneToOriginalColor(currentRenderIndex);
            changeOneRender(newRenderIndex);
            currentRenderIndex = newRenderIndex;
        }
    }

    void changeOneRender(int index)
	{
        Material [] currentMaterials = allRenderers[index].materials;
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            currentMaterials[i].SetColor("_Color", Color.red);
        }
    }

    void resetOneToOriginalColor(int index)
	{
        Material[] currentMaterials = allRenderers[index].materials;
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            currentMaterials[i].color = originalMaterials[index][i].color;
        }
    }
}