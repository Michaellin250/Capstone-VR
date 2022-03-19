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
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class VisualHand : MonoBehaviour
	{
		[SerializeField] private OVRHand.Hand _handType;
		
		[SerializeField] private Transform _indexProximal;
		[SerializeField] private Transform _indexMiddle;
		[SerializeField] private Transform _indexDistal;
		
		private Hand _hand;
		
		private void Awake()
		{
			if (_handType == OVRHand.Hand.HandLeft)
			{
				_hand = FindObjectOfType<HandLeft>().GetComponent<Hand>();
			}
			
			if (_handType == OVRHand.Hand.HandRight)
			{
				_hand = FindObjectOfType<HandRight>().GetComponent<Hand>();
			}
		}

		private void Update()
		{
			MapIndex();
		}

		private void MapIndex()
		{
			if (_indexProximal)
			{
				_indexProximal.localRotation = _hand.IndexProximal.localRotation;
			}
			if (_indexMiddle)
			{
				_indexMiddle.localRotation = _hand.IndexMiddle.localRotation;
			}
			if (_indexDistal)
			{
				_indexDistal.localRotation = _hand.IndexDistal.localRotation;
			}
		}
	}
}