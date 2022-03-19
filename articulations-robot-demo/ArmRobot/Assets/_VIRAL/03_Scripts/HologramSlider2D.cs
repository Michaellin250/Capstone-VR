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
using UnityEngine.Serialization;

namespace _VIRAL._03_Scripts
{
	public class HologramSlider2D : HologramSlider
	{
		[SerializeField] private float _range = 1;
		[SerializeField] private Vector2 _vector2Value = Vector2.zero;
		
		[SerializeField] private Transform _minPosY;
		[SerializeField] private Transform _maxPosY;
		
		private new readonly Subject<Vector2> _onValueChanged = new Subject<Vector2>();

		
		private Vector2 _ringPosition2d = Vector2.zero;

		public new IObservable<Vector2> OnValueChanged => _onValueChanged;
		public new Vector2 Value => _vector2Value;

		protected override void Awake()
		{
			base.Awake();
			UpdateScale();

			SetValue(_vector2Value);
			_onValueChanged.OnNext(_vector2Value);
		}
		
		protected override void CalculateValue(Vector3 hitPosition)
		{
			Vector3 hitLocalPosition = transform.InverseTransformPoint(hitPosition);

			Vector2 scale = GetScale();
			
			//(percentage * 2 * range) - range
			//(((hitLocalPosition.x - _minPos.localPosition.x) / (_maxPos.localPosition.x - _minPos.localPosition.x)) / (2* range)) - range;
			float xPos = (hitLocalPosition.x - _minPos.localPosition.x) / scale.x - _range;
			float yPos = (hitLocalPosition.y - _minPosY.localPosition.y) / scale.y - _range;
			
			SetValue(new Vector2(xPos, yPos));

			_onValueChanged.OnNext(_vector2Value);
		}
		
		protected override void UpdateRingPosition()
		{
			Vector3 currentPos = _ring.localPosition;
			Vector3 newPos = new Vector3(_ringPosition2d.x, _ringPosition2d.y, currentPos.z);
			
			_ring.localPosition = Vector3.Lerp(currentPos, newPos, _lerpTime);
		}
		
		protected override void UpdateScale()
		{
			_vector2Value = Vector2.ClampMagnitude(_vector2Value, _range);
			_onValueChanged.OnNext(_vector2Value);
		}
		
		public void SetValue(Vector2 value)
		{
			_vector2Value = Vector2.ClampMagnitude(value, _range);

			_valueText.text = _vector2Value.ToString();
			AdjustSlider();
		}

		protected override void AdjustSlider()
		{
			Vector2 scale = GetScale();
			
			float xPos = _minPos.localPosition.x + (_vector2Value.x + _range) * scale.x;
			float yPos = _minPosY.localPosition.y + (_vector2Value.y + _range) * scale.y;

			_ringPosition2d = new Vector2(xPos, yPos);
		}
		
		private new Vector2 GetScale()
		{
			return GetLength() / (2 * _range);
		}
		
		private new Vector2 GetLength()
		{
			float xLength = _maxPos.localPosition.x - _minPos.localPosition.x;
			float yLength = _maxPosY.localPosition.y - _minPosY.localPosition.y;

			return new Vector2(xLength, yLength);
		}
	}
}