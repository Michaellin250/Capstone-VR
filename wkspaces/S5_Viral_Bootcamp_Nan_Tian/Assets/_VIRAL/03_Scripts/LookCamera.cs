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

using System;
using _VIRAL._03_Scripts;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
	private Transform _centerEye;

	private void Awake()
	{
		_centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
	}

	private void Update()
	{
		transform.LookAt(_centerEye);
	}
}
