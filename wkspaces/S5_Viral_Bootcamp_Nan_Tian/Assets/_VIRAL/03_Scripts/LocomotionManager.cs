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
	public class LocomotionManager : MonoBehaviour
	{

		[SerializeField] private ViralSettings _viralSettings;
		[Space] [SerializeField] private Teleportation _teleportation;
		[SerializeField] private Mimotion _mimotion;

		// References
		public Transform CameraRig => _cameraRig;
		public Transform CenterEye => _centerEyeAnchor;
		public Hand LeftHand => _leftHand;
		public Hand RightHand => _rightHand;

		private Transform _cameraRig;
		private Transform _centerEyeAnchor;
		private Hand _leftHand;
		private Hand _rightHand;
		private HandCursor _handCursor;

		// Disposables
		private IDisposable _handleTeleportationDisposable;
		private IDisposable _handleMimotionDisposable;

		private void Awake()
		{
			_cameraRig = FindObjectOfType<OVRCameraRig>().transform;
			_centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;
			_leftHand = FindObjectOfType<HandLeft>().GetComponent<Hand>();
			_rightHand = FindObjectOfType<HandRight>().GetComponent<Hand>();
			_handCursor = FindObjectOfType<HandCursor>();
		}

		private void Start()
		{
			Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(t =>
			{
				StartCheckingTeleportation();
				StartCheckingMimotion();
				InitializeMimotionEvents();

				InitializeCursorEvents();
			}).AddTo(this);
			
			
		}

		private void StartCheckingTeleportation()
		{
			_handleTeleportationDisposable?.Dispose();
			_handleTeleportationDisposable = Observable.EveryUpdate().Subscribe(_t =>
			{
				if (_viralSettings.TeleportationActive.Value && _rightHand.IsTrackingGood())
				{
					_teleportation.HandleTeleportation();
				}
				else
				{
					_teleportation.CancelTeleportation();
				}

				if ((_viralSettings.MimotionActive.Value && _leftHand.IsGrasping || _rightHand.IsGrasping) || _handCursor.IsAiming)
				{
					_teleportation.CancelTeleportation();
					_handleTeleportationDisposable?.Dispose();
				}
			});
		}

		private void StartCheckingMimotion()
		{
			_handleMimotionDisposable?.Dispose();
			_handleMimotionDisposable = Observable.EveryUpdate().Subscribe(_t =>
			{
				if (_viralSettings.MimotionActive.Value && _leftHand.IsTrackingGood() && _rightHand.IsTrackingGood())
				{
					_mimotion.HandleMimotion();
				}
			});
		}

		private void InitializeMimotionEvents()
		{
			_mimotion.OnStartMoving.Subscribe(_m =>
			{
				if (_viralSettings.MimotionActive.Value)
				{
					_handleTeleportationDisposable?.Dispose();
					_teleportation.CancelTeleportation();
				}
			});

			_mimotion.OnStopMoving.Subscribe(_m =>
			{
				Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_t => { StartCheckingTeleportation(); });
			});
		}

		private void InitializeCursorEvents()
		{
			_handCursor.OnStartAiming.Subscribe(_ =>
			{
				_handleTeleportationDisposable?.Dispose();
				_teleportation.CancelTeleportation();
			});

			_handCursor.OnStopAiming.Subscribe(_ => { StartCheckingTeleportation(); });
		}

	}
}