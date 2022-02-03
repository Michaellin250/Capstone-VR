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
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class Teleportation : MonoBehaviour
	{
		[SerializeField] private OVRHand.HandFinger _fingerPinch = OVRHand.HandFinger.Index;
		[SerializeField] private OVRHand.HandFinger _rotationFinger = OVRHand.HandFinger.Pinky;

		[SerializeField] private LaserPointer _laserPointer;
		[SerializeField] private Transform _startPoint;
		[SerializeField] private Transform _target;
		[SerializeField] private Transform _virtualRig;
		[SerializeField] private Transform _virtualPosition;
	
		private LocomotionManager _locomotionManager;
	
		private bool _startAiming = false;
		private bool _isAimingTeleportation = false;
		private bool _canTeleport = false;
		private bool _targetVisible = false;
		private Vector3 _aimingPoint;
		private IDisposable _delayAimDisposable;
		private int _layerMask  = 1 << 4;

		// tweeners
		private Tweener _targetTweener;

		public Subject<Teleportation> OnStartAiming = new Subject<Teleportation>();
		public Subject<Teleportation> OnStopAiming = new Subject<Teleportation>();
		public Subject<Teleportation> OnShowTarget = new Subject<Teleportation>();
		public Subject<Teleportation> OnHideTarget = new Subject<Teleportation>();
		public Subject<Teleportation> OnStartTeleporting = new Subject<Teleportation>();
		public Subject<Teleportation> OnStopTeleporting = new Subject<Teleportation>();

		private void Awake()
		{
			_locomotionManager = GetComponentInParent<LocomotionManager>();
		
			CancelTeleportation();
			_target.localScale = Vector3.one * 0.01f;

			InitializeAudio();
		}
	
		public void HandleTeleportation()
		{
			// Pinching
			if (_locomotionManager.RightHand.IsPinching(_fingerPinch))
			{
				HandlePinch();
			}
		
			// Handle Release
			if (!_locomotionManager.RightHand.IsPinching(_fingerPinch))
			{
				HandleRelease();
			}
		}

		private void HandlePinch()
		{
			// Start aiming
			if (!_startAiming && !_isAimingTeleportation)
			{
				_startAiming = true;
				OnStartAiming.OnNext(this);
				_delayAimDisposable?.Dispose();
				_delayAimDisposable = Observable.Timer(TimeSpan.FromSeconds(0.05f)).Subscribe(c =>
				{
					_isAimingTeleportation = true;
					_startAiming = false;
				});
			}

			if (_isAimingTeleportation)
			{
				_laserPointer.Activate(true);
			}

			// Aiming
			RaycastHit hit;
			if (Physics.Raycast(_startPoint.position, _startPoint.forward, out hit, 10, _layerMask))
			{
				if (_isAimingTeleportation)
				{
					ShowTarget(true);
					_canTeleport = true;
					_aimingPoint = hit.point;
					_target.position = _aimingPoint;
					_laserPointer.DrawCurveValid(_startPoint.position, _target.position);
				}
			}
			else
			{
				_canTeleport = false;
				ShowTarget(false);
				_laserPointer.DrawCurveDenied(_startPoint.position, _startPoint.position + _startPoint.forward*2);
			}
		
			UpdateVirtualRig();
		}

		private void HandleRelease()
		{
			if (_isAimingTeleportation && _canTeleport)
			{
				Teleport(_aimingPoint);
				OnStopAiming.OnNext(this);
			}
		
			Reset();
		}

		public void CancelTeleportation()
		{
			if (_isAimingTeleportation)
			{
				Reset();
			}
		}

		public void Reset()
		{
			_isAimingTeleportation = false;
			_canTeleport = false;
			_laserPointer.Activate(false);
			ShowTarget(false);
		}

		private void Teleport(Vector3 target)
		{
			OnStartTeleporting.OnNext(this);
			_locomotionManager.CameraRig.transform.position = _virtualRig.position;
			OnStopTeleporting.OnNext(this);
		}

		private void UpdateVirtualRig()
		{
			_virtualRig.localPosition = - new Vector3(_locomotionManager.CenterEye.localPosition.x, 0, _locomotionManager.CenterEye.localPosition.z);
			_virtualPosition.localRotation = Quaternion.Euler(0,_locomotionManager.CenterEye.localRotation.eulerAngles.y,0);
		}
	
		private void ShowTarget(bool show)
		{
			if (show && !_targetVisible)
			{
				_laserPointer.gameObject.SetActive(true);
				OnShowTarget.OnNext(this);
				_target.gameObject.SetActive(true);
				_targetTweener?.Kill();
				_targetTweener = _target.DOScale(1, 0.25f).SetEase(Ease.OutBack);
				_targetVisible = true;
			}
		
			if(!show && _targetVisible)
			{
				OnHideTarget.OnNext(this);
				_targetVisible = false;
				_targetTweener?.Kill();
				_targetTweener = _target.DOScale(0.01f, 0.25f)
					.SetEase(Ease.OutBack).OnComplete(() =>
					{
						_target.gameObject.SetActive(false);
					});
			}
		}
	
		[Space] 
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private AudioClip _soundStartAiming;
		[SerializeField] private AudioClip _soundTeleport;

		private void InitializeAudio()
		{
			OnShowTarget.Subscribe(_ =>
			{
				PlaySound(_soundStartAiming, 0.3f);
			});
			OnStartTeleporting.Subscribe(_ =>
			{
				PlaySound(_soundTeleport, 0.7f);
			});
		}

		private void PlaySound(AudioClip clip, float volume)
		{
			_audioSource.Stop();
			_audioSource.volume = volume;
			_audioSource.PlayOneShot(clip);
		}
	}
}
