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
using UnityEngine;
using UniRx;

namespace _VIRAL._03_Scripts
{
	public class InventorySlot : Holder
	{
		[SerializeField] protected SwellingRing _ring;
		[SerializeField] protected float _collapsedScaleFactor = 0.3f;
		[SerializeField] protected bool _canAutoCaptureObjects = true;

		private IDisposable _closestReleasedDisposable;
		private Sequence _moveSequence;
		private float _snaptime = 0.3f;

		protected override void Start()
		{
			base.Start();

			_newClosestDetected.Subscribe(holdable =>
			{
				_ring.Show(true);
				_ring.Activate(true);

				Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
				{
					if (_canAutoCaptureObjects && !holdable.Holder)
					{
						Capture();
					}
				});
				
				_closestReleasedDisposable?.Dispose();
				_closestReleasedDisposable = holdable.OnReleased.Subscribe(h =>
				{
					Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
					{
						if (!h.Holder)
						{
							Capture();
						}
					});
				});
			});

			_noMoreObjectDetected.Subscribe(n =>
			{
				if (!_capturedObject)
				{
					_ring.Show(false);
				}
				_ring.Activate(false);
			});

			_onCaptured.Subscribe(_ =>
			{
				Transform tr = _capturedObject.transform;
				
				_ring.Activate(false);
				_moveSequence?.Kill();

				if (_captureKinematically)
				{
					Vector3 localPos = -_capturedObject.Rb.centerOfMass * _collapsedScaleFactor;
					_moveSequence.Append(tr.DOLocalRotateQuaternion(Quaternion.identity, _snaptime));
					_moveSequence.Append(tr.DOLocalMove(localPos, _snaptime));
				}
				else
				{
					_capturedObject.Joint.anchor = _capturedObject.Rb.centerOfMass * _collapsedScaleFactor;
					_capturedObject.Joint.targetRotation = transform.rotation;
				}

				_moveSequence.Join(tr.DOScale(_capturedObject.InitialLocalScale * _collapsedScaleFactor, _snaptime)
					.OnUpdate(() =>
					{
						_capturedObject?.RefreshPhysics();
					}));
			});
			
			_onReleased.Subscribe(_ =>
			{
				_moveSequence?.Kill();
				_moveSequence.Append(_capturedObject.transform.DOScale(_capturedObject.InitialLocalScale, _snaptime)
					.OnUpdate(() =>
					{
						_capturedObject?.RefreshPhysics();
					}));
			});
		}
		
	}
}