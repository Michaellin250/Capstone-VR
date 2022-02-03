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
using TMPro;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class HologramHint : MonoBehaviour
	{
		[SerializeField] private Transform _container;
		[SerializeField] private TextMeshPro _textHint;

		private HologramUiComponent _uiComponent;
		private bool _hintVisible = true;
		private Tweener _hintTweener;
		private Transform _centerEye;
		private float _scaleMultiplier;
		
		private void Awake()
		{
			_centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
		}

		private void Start()
		{
			ShowHint(false);
		}

		private void Update()
		{
            HandleOrientation();
        }

        private void HandleOrientation()
        {
            Vector3 forward = (transform.position - _centerEye.position).normalized;
            Vector3 offset = new Vector3(0, 0.02f, 0);
            Vector3 newPos = _uiComponent.transform.position + offset - forward.normalized * 0.02f;

            Vector3 relativePos = _centerEye.position - transform.position;
            float yAngle = Quaternion.LookRotation(relativePos, Vector3.up).eulerAngles.y;
            Quaternion newRot = Quaternion.Euler(0, yAngle, 0);

            transform.SetPositionAndRotation(newPos, newRot);

            // adjust scale based on distance from camera
            float distance = Vector3.Distance(transform.position, _centerEye.position);
            _container.localScale = Vector3.one * Math.Max(0.3f, distance);
        }

        public void ShowHint(bool show)
		{
			if (!_hintVisible && show)
			{
				_hintTweener?.Kill();
				gameObject.SetActive(true);
				_hintVisible = true;
				_hintTweener = transform.DOScale(Vector3.one * 1f, 0.1f);
			}
			
			if(_hintVisible && !show)
			{
				_hintTweener?.Kill();
				_hintVisible = false;
				_hintTweener = transform.DOScale(Vector3.one * 0.01f, 0.1f).
                    OnComplete(() =>
				{
					gameObject.SetActive(false);
				});
			}
		}

		public void SetUiComponent(HologramUiComponent component)
		{
			_uiComponent = component;
		}
		
		public void SetText(string text)
		{
			_textHint.text = text;
		} 
		
	}
}