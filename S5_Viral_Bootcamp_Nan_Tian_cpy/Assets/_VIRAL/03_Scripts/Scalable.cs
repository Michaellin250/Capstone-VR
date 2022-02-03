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
	[RequireComponent(typeof(Interactable))]
	public class Scalable : MonoBehaviour
	{
        #region EDITOR

        [SerializeField] private float _minScaleFactor = 0.5f;
		[SerializeField] private float _maxScaleFactor = 1.5f;
		[SerializeField] private float _pinchDistanceFactor = 2f;

        #endregion

        #region PRIVATE

        private float _currentScaleFactor = 1f;
		private Vector3 _initialScale;
		private Interactable _interactable;
		private TwoHandedPincher _twoHandedPincher;

		private bool _isScaling = false;
		
		private float _doublePinchStartDistance;
		private float _doublePinchStartValue;

        #endregion


        private void Awake()
		{
			_twoHandedPincher = FindObjectOfType<TwoHandedPincher>();
			_interactable = GetComponent<Interactable>();
			_initialScale = transform.localScale;
		}

		private void Start()
		{
			InitializePincher();
			UpdateRing();
		}
		
		private bool CanBeScaled()
		{
			bool isEnabled = gameObject.activeSelf && enabled;
			bool isFocused = _interactable.Focused;
			bool isCaptured = _interactable.Holdable && _interactable.Holdable.IsCaptured;
			
			return isEnabled && isFocused && !isCaptured;
		}
		
		private void InitializePincher()
		{
			_twoHandedPincher.OnTwoHandedPinchStart.Subscribe(distance =>
			{
				//Debug.Log("DOUBLE PINCH START");
				if (CanBeScaled() && !_isScaling)
				{
					StartScaling(distance);
				}
			}).AddTo(this);

			_twoHandedPincher.OnTwoHandedPinchEnd.Subscribe(_ =>
			{
				//Debug.Log("DOUBLE PINCH END");
				StopScaling();
			}).AddTo(this);

			_twoHandedPincher.TwoHandedPinchDistance.Subscribe(distance =>
			{
				if (_isScaling)
				{
					if (CanBeScaled())
					{
						float pinchDelta = distance - _doublePinchStartDistance;
						AdjustScale(pinchDelta);
					}
					else
					{
						StopScaling();
					}
				}
			}).AddTo(this);
		}

		#region SCALING 

		private void StartScaling(float distance)
		{
			_isScaling = true;
			_doublePinchStartDistance = distance;
			_doublePinchStartValue = _currentScaleFactor;
			_interactable.Ring.ShowScaleRings(true);
		}

		private void AdjustScale(float delta)
		{
			_interactable.Ring.ShowScaleRings(true);
			
			float newScaleFactor = _doublePinchStartValue + (delta * _pinchDistanceFactor);

			if (newScaleFactor > _minScaleFactor && newScaleFactor < _maxScaleFactor)
			{
				_interactable.Ring.Activate(true);
				_currentScaleFactor = newScaleFactor;
				transform.localScale = _initialScale * newScaleFactor;
				
				_interactable.Holdable.RefreshPhysics();

				UpdateRing();
			}
		}
		
		private void UpdateRing()
		{
			_interactable.Ring.SetRingScale(1 + (_currentScaleFactor-_minScaleFactor)/(_maxScaleFactor-_minScaleFactor));
		}
		
		private void StopScaling()
		{
			_isScaling = false;
			_doublePinchStartValue = 0;
			_interactable.Ring.Activate(false);
			_interactable.Ring.ShowScaleRings(false);
		}

		#endregion
		
	}
}