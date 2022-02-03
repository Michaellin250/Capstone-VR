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
using UniRx;

namespace _VIRAL._03_Scripts
{
	
	public class HandGrabber : Holder
	{
		[SerializeField] private Hand _hand;
		
		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			_hand.OnStartGrasping.Subscribe(h =>
			{
				//Debug.Log("GRAB START");
				Capture();
			});
			_hand.OnStopGrasping.Subscribe(h =>
			{
				//Debug.Log("GRAB END");
				Release();
			});

			_onCaptured.Subscribe(_ =>
			{
				_hand.MakeTransparent(true);
			});
			
			_onReleased.Subscribe(_ =>
			{
				_hand.MakeTransparent(false);
			});
		}
	}
}