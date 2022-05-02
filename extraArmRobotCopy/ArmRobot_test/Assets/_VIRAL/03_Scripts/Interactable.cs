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
	[RequireComponent(typeof(Rigidbody))]
	public class Interactable : MonoBehaviour
	{
		[SerializeField] private ObjectType _objectType = ObjectType.Undefined;
		[SerializeField] private GameObject _ringPrefab;
		[SerializeField] private float _ringScale = 0.1f;
		[SerializeField] private bool _canBeFocused = true;

        #region PRIVATE

        private SwellingRing _ring;
        private CenterEyeAnchor _centerEye;
        private Rigidbody _rb;
        private Holdable _holdable;
        private Scalable _scalable;

        private bool _focused = false;
        private Tweener _ringScaleTweener;

        #endregion

        public Holdable Holdable => _holdable;
		public Scalable Scalable => _scalable;
		public ObjectType ObjectType => _objectType;

		public bool Focused => _focused;

		public SwellingRing Ring => _ring;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
			_ring = Instantiate(_ringPrefab, transform.position, transform.rotation).GetComponent<SwellingRing>();
			_ring.transform.localScale = Vector3.one * _ringScale;
			_centerEye = FindObjectOfType<CenterEyeAnchor>();
			_holdable = GetComponent<Holdable>();
			_scalable = GetComponent<Scalable>();
			
			_ring.gameObject.SetActive(_canBeFocused);
		}

		private void Start()
		{
			Focus(false);
			_ring.ShowScaleRings(false);

			_holdable?.OnCaptured.Subscribe(h =>
			{
				_ring.gameObject.SetActive(false);
			});
			
			_holdable?.OnReleased.Subscribe(h =>
			{
				_ring.gameObject.SetActive(_canBeFocused);
			});
		}

		private void Update()
		{
			_ring.transform.position = _rb.worldCenterOfMass;
			CheckGaze();
		}

		private void CheckGaze()
		{
			if (!_canBeFocused) return;
			
			if (_centerEye.IsLookingAt(transform, 2f, 0.05f))
			{
				if (!_focused)
				{
					Focus(true);
				}
			}
			else
			{
				if (_focused)
				{
					Focus(false);
				}
			}
		}

		private void Focus(bool focus)
		{
			_focused = focus;
			_ring.Show(focus);
			if (!focus)
			{
				_ring.Activate(false);
			}
		}

		public void AdjustRingScale(float multiplier)
		{
			_ringScaleTweener?.Kill();
			_ringScaleTweener = _ring.transform.DOScale(Vector3.one * _ringScale * multiplier, 0.2f)
				.SetEase(Ease.OutBack);
		}

		public void OnDisable()
		{
			if (_ring)
			{
				_ring.gameObject.SetActive(false);
			}
		}
		
		public void OnEnable()
		{
			if (_ring)
			{
				_ring.gameObject.SetActive(true);
			}
		}
	}
}