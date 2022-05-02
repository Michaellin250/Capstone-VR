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

using DG.Tweening;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class MovingPart : MonoBehaviour
	{
        #region Editor
        [SerializeField] private Vector3 _direction;
        [SerializeField] private float _offset = 0f;
        [SerializeField] private float _multiplier = 0f;
        [SerializeField] private float _delay = 0.0f;
        [SerializeField] private float _time = 0.3f;
        [SerializeField] private Ease _ease = Ease.Linear;
        #endregion

        private Vector3 _initialLocalPosition;
		private Sequence _moveSequence;
		
		private void Awake()
		{
			_initialLocalPosition = transform.localPosition;
		}

		public Sequence GetSequence()
		{
            Vector3 scaledPosition = Vector3.Scale(_initialLocalPosition, _direction * _multiplier);
            Vector3 offsetPosition = _direction * _offset;

            Vector3 newPos = _initialLocalPosition + scaledPosition + offsetPosition;
			_moveSequence = DOTween.Sequence();
			_moveSequence.AppendInterval(_delay);
			_moveSequence.Append(transform.DOLocalMove(newPos, _time).SetEase(_ease));
			
			return _moveSequence;
		}
	}
}