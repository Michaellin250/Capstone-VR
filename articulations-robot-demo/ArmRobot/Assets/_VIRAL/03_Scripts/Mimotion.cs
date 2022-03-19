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
	public class Mimotion : MonoBehaviour
	{
	
		private Vector3 _cameraRigCurrentPosition;
		private bool _isMovingWithLeftHand = false;
		private bool _isMovingWithRightHand = false;
		private Vector3 _handStart;
		private Vector3 _currentHandPosition;

		private Hand _leftHand;
		private Hand _rightHand;
		private Transform _cameraRig;
		
		private readonly Subject<Mimotion> _onStartMoving = new Subject<Mimotion>();
		private readonly Subject<Mimotion> _onStopMoving = new Subject<Mimotion>();


		public IObservable<Mimotion> OnStartMoving => _onStartMoving;
		public IObservable<Mimotion> OnStopMoving => _onStopMoving;

		private void Awake()
		{
			_leftHand = FindObjectOfType<HandLeft>().GetComponent<Hand>();
			_rightHand = FindObjectOfType<HandRight>().GetComponent<Hand>();
			_cameraRig = FindObjectOfType<OVRCameraRig>().transform;
		}

		private void Start()
		{
			_cameraRigCurrentPosition = _cameraRig.transform.position;

			_leftHand.OnStartGrasping.Subscribe(h =>
			{
				StartMoving(_leftHand.GetPosition(), _leftHand.HandType);
			});
			_leftHand.OnStopGrasping.Subscribe(h =>
			{
				CheckStop();
			});
		
			_rightHand.OnStartGrasping.Subscribe(h =>
			{
				StartMoving(_rightHand.GetPosition(), _rightHand.HandType);
			});
			_rightHand.OnStopGrasping.Subscribe(h =>
			{
				CheckStop();
			});
		}

		private void StartMoving(Vector3 handPosition, OVRHand.Hand handType)
		{
			_cameraRigCurrentPosition = _cameraRig.transform.position;
			_handStart = handPosition;

			_isMovingWithRightHand = (handType == OVRHand.Hand.HandRight);
			_isMovingWithLeftHand = (handType == OVRHand.Hand.HandLeft);
		
			_onStartMoving.OnNext(this);
		}
	
		private void CheckStop()
		{
			if (!_leftHand.IsGrasping && !_rightHand.IsGrasping)
			{
				_isMovingWithLeftHand = _isMovingWithRightHand = false;

				_onStopMoving.OnNext(this);
			}
		}

		public void HandleMimotion()
		{
			if (_isMovingWithLeftHand || _isMovingWithRightHand)
			{
				_currentHandPosition = _isMovingWithLeftHand ? _leftHand.GetPosition() : _rightHand.GetPosition();
			
				Vector3 delta = _handStart - _currentHandPosition;
				Vector3 newPos = _cameraRigCurrentPosition + new Vector3(delta.x, 0, delta.z);
				_cameraRig.transform.position = Vector3.Lerp(_cameraRig.transform.position, newPos, 0.1f);
			}
		}
	}
}
