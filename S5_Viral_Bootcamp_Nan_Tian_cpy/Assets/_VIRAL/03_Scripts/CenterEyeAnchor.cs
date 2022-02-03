/************************************************************
 * Copyright (c) Holonautic Ltd. All rights reserved.
 * __________________________________________________
 * 
 * All information contained herein is, and remains
 * the property of Holonautic. The intellectual and technical
 * concepts contained herein are proprietary to Holonautic.
 * Dissemination of this information or reproduction of this
 * material is strictly forbidden unless prior written
 * permission is obtained from Holonautic.
 *
 * *******************************************************/

using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class CenterEyeAnchor : MonoBehaviour
	{
		public bool IsLookingAt(Transform other, float distance = 10f, float fov = 0.1f)
		{
			Vector3 direction = (transform.position - other.transform.position).normalized;
			
			float dot = Vector3.Dot(direction, transform.forward);
			float dist = Vector3.Distance(transform.position, other.position);
			
			return (dot < (-1 + fov) && dist < distance);
		}
	}
}
