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
	public class FaceTowardsPlayer : MonoBehaviour
	{
		private Transform _centerEye;
		private Quaternion _rotation;

		void Start()
		{
			_centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
		}

		void Update()
		{
			if (_centerEye)
			{
				Vector3 relativePos = _centerEye.transform.position - transform.position;
				_rotation = Quaternion.Euler(0, Quaternion.LookRotation(relativePos, Vector3.up).eulerAngles.y, 0);
				transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, 0.1f);
			}
		}
	}
}