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
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class TwoHandedPincher : MonoBehaviour
	{
		[SerializeField] private OVRHand.HandFinger _pinchFinger = OVRHand.HandFinger.Index;

		private Hand _leftHand;
		private Hand _rightHand;

		private IObservable<float> _twoHandedPinchDistance;

		private IObservable<bool> _onTwoHandedPinch;

		public IObservable<float> TwoHandedPinchDistance
		{
			get
			{
				var leftHandPinch = _leftHand.OnPinch(_pinchFinger);
				var rightHandPinch = _rightHand.OnPinch(_pinchFinger);

				return OnTwoHandedPinchStart
					.SelectMany(p =>
						Observable.EveryUpdate().Select(x => PinchDistance())
							.TakeUntil(leftHandPinch.Merge(rightHandPinch)));
			}
		}

		private void Awake()
		{
			_leftHand = FindObjectOfType<HandLeft>().GetComponent<Hand>();
			_rightHand = FindObjectOfType<HandRight>().GetComponent<Hand>();
		}

		private IObservable<bool> CreateOnTwoHandedPinch()
		{
			var leftHandPinch = _leftHand.OnPinch(_pinchFinger);
			var rightHandPinch = _rightHand.OnPinch(_pinchFinger);
			var combinedPinch = Observable.CombineLatest(leftHandPinch, rightHandPinch);

			return combinedPinch.Select(p => p[0] && p[1]);
		}

		public IObservable<float> OnTwoHandedPinchStart
		{
			get
			{
				if (_onTwoHandedPinch == null)
				{
					_onTwoHandedPinch = CreateOnTwoHandedPinch();
				}

				return _onTwoHandedPinch.Where(p => p).Select(_ => PinchDistance());
			}
		}

		public IObservable<float> OnTwoHandedPinchEnd
		{
			get
			{
				var leftHandPinch = _leftHand.OnPinch(_pinchFinger);
				var rightHandPinch = _rightHand.OnPinch(_pinchFinger);
        
				return OnTwoHandedPinchStart.SelectMany(_ =>
					leftHandPinch.Merge(rightHandPinch)
						.Take(1)
						.Select(p => PinchDistance())
				);
			}
		}

		private float PinchDistance()
		{
			return Vector3.Distance(_leftHand.GetPosition(), _rightHand.GetPosition());
		}
	}
}