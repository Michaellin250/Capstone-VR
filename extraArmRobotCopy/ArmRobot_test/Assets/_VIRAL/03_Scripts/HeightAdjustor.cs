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
	public class HeightAdjustor : MonoBehaviour
	{
		[SerializeField] private float _heightRatio = 0.6f;
		[SerializeField] public float _threshold = 0.05f;
		[SerializeField] public float _lerpValue = 0.9f;
	
		private Transform _centerEyeAnchor;

		void Awake()
		{
			_centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;
		}
	
		private void Update()
		{
			// approximation of user's height
			float cameraHeight = _centerEyeAnchor.position.y + 0.07f;
			float height = cameraHeight * _heightRatio;
			float distance = Mathf.Abs(transform.position.y - height);
		
			if (distance > _threshold)
			{
				Vector3 newHeight = new Vector3(transform.position.x, height, transform.position.z);
			
				transform.position = Vector3.Lerp(transform.position, newHeight, Time.fixedDeltaTime * _lerpValue);
			}
		}
	}
}
