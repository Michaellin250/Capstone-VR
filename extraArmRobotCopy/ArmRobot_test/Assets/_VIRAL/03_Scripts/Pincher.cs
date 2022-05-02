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
using System.Collections.Specialized;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class Pincher : MonoBehaviour
	{
		[SerializeField] private Hand _hand;
		[SerializeField] private Transform _indexTip;
		[SerializeField] private Transform _thumbTip;

		public Hand Hand => _hand;

		private void Update()
		{
			transform.position = _thumbTip.position + (_indexTip.position - _thumbTip.position)/2;
		}

		public float GetPinchGap()
		{
			return Vector3.Distance(_indexTip.position, _thumbTip.position);
		}
	}
}