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
	public class HandCursor : MonoBehaviour
	{
		[SerializeField] private OVRHand.HandFinger _fingerPinch = OVRHand.HandFinger.Index;

		[SerializeField] private Interactor _interactor;

		[SerializeField] private Transform _startPoint;
		[SerializeField] private LaserPointer _laserPointer;
		[SerializeField] private Transform _ring;
		[SerializeField] private SpriteRenderer _ringSprite;

		[SerializeField] private float _minDistance = 0.3f;
		[SerializeField] private float _maxDistance = 3;

		[SerializeField] private Color _colorDefault;
		[SerializeField] private Color _colorPinch;

		[SerializeField] private Gradient _gradientDefault;
		[SerializeField] private Gradient _gradientPinch;


		private readonly Subject<HandCursor> _onStartAiming = new Subject<HandCursor>();
		private readonly Subject<HandCursor> _onStopAiming = new Subject<HandCursor>();

		public IObservable<HandCursor> OnStartAiming => _onStartAiming;
		public IObservable<HandCursor> OnStopAiming => _onStopAiming;

		public bool IsAiming => _isAiming;

		private Hand _hand;
		private int _layerMask = 1 << 8; // Hologram Layer

		private Vector3 _direction;

		private bool _isPinching = false;
		private bool _isAiming = false;

		private Vector3 _hitPoint;

		private void Awake()
		{
			_hand = GetComponentInParent<Hand>();
		}

		private void Start()
		{
			_hand.OnPinchStarted(_fingerPinch).Subscribe(_ =>
			{
				HandlePinch();
				_isPinching = true;
			});

			_hand.OnPinchEnded(_fingerPinch).Subscribe(_ =>
			{
				HandleUnpinch();
				_isPinching = false;
			});
		}

		private void Update()
		{
			HandleCursor();

			// Pinching
			if (_hand.IsPinching(_fingerPinch))
			{
				HandlePinching();
			}
		}

		private void HandleCursor()
		{
			_direction = Vector3.Lerp(_direction, _startPoint.forward, 0.025f);

			RaycastHit hit;
			if (Physics.Raycast(_startPoint.position, _direction, out hit, _maxDistance, _layerMask))
			{
				_hitPoint = hit.point;

				if (hit.distance > _minDistance)
				{
					HologramUiComponent uiComponent = hit.collider.GetComponent<HologramUiComponent>();

					if (uiComponent && !uiComponent.CanBeUsedWithPointer) return;

					if (!_isAiming)
					{
						ShowCursor(true);
					}

					_ring.position = hit.point;
					_ring.LookAt(hit.point + hit.normal);

					Gradient gradient = _isPinching ? _gradientPinch : _gradientDefault;
					_laserPointer.DrawLine(_startPoint.position, hit.point, gradient);

					_ringSprite.color = _isPinching ? _colorPinch : _colorDefault;

					if (uiComponent && uiComponent.Enabled)
					{
						if (!_interactor.FocusedComponent ||
						    (_interactor.FocusedComponent && _interactor.FocusedComponent.GetInstanceID() !=
							    uiComponent.GetInstanceID()))
						{
							_interactor.Reset();
							_interactor.FocusedComponent = uiComponent;
							_interactor.FocusedComponent.Focus(true);
						}
					}
					else
					{
						_interactor.Reset();
					}
				}
				else
				{
					if (_isAiming)
					{
						ShowCursor(false);
					}
				}
			}
			else
			{
				if (_isAiming)
				{
					ShowCursor(false);
				}
			}
		}

		private void ShowCursor(bool show)
		{
			_isAiming = show;
			_laserPointer.gameObject.SetActive(show);
			_ring.gameObject.SetActive(show);

			if (show)
			{
				_onStartAiming.OnNext(this);
			}
			else
			{
				_onStopAiming.OnNext(this);
			}
		}

		private void HandlePinch()
		{
			if (_interactor.FocusedComponent && _interactor.FocusedComponent.Enabled)
			{
				_interactor.FocusedComponent.Click(_hitPoint);
			}
		}

		private void HandleUnpinch()
		{
		}

		private void HandlePinching()
		{
			if (_interactor.FocusedComponent)
			{
				//Debug.Log("PINCHING");
				_interactor.FocusedComponent.SetInteractor(_interactor);
				if (_interactor)
				{
					_interactor.FocusedComponent.Activate(true);
				}
			}
		}
	}
}