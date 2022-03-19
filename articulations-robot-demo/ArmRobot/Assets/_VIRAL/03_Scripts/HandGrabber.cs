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

		public Hand Hand => _hand;

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

			_onCaptured.Subscribe(h =>
			{
				bool useSnapHandRight = h.VisualSnapHandRight && _hand.HandType == OVRHand.Hand.HandRight;
				bool useSnapHandLeft = h.VisualSnapHandLeft && _hand.HandType == OVRHand.Hand.HandLeft;
				
				if (useSnapHandRight || useSnapHandLeft)
				{
					h.ShowVisualSnapHand(true, _hand.HandType);
					_hand.ShowHand(false);
				}
				else
				{
					_hand.MakeTransparent(true);
					_hand.ShowHand(true);
				}
			});
			
			_onReleased.Subscribe(h =>
			{
				h.ShowVisualSnapHand(false, _hand.HandType);
				_hand.ShowHand(true);
				_hand.MakeTransparent(false);
			});
		}
	}
}