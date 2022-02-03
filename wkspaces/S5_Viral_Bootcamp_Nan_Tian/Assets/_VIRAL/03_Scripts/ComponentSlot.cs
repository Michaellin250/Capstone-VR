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
	public class ComponentSlot : InventorySlot
	{
		[SerializeField] private GameObject _firePrefab;
		[SerializeField] private float _malfunctionDelay = 20f;
		[SerializeField] private Transform _warningSign;
		[SerializeField] private Transform _bluePrint;

		private ParticleSystem _fireEffect;
		private Tweener _warningTweener;
		private Tweener _bluePrintTweener;
		private Tweener _bluePrintYoyoTweener;

		private IDisposable _failureDisposable;

		protected override void Start()
		{
			base.Start();

			Initialize();
			InitializeRandomFailure();
		}

		private void Initialize()
		{
			ShowBluePrint(false, 0);
			ShowWarningSign(false, 0);

			_onCaptured.Subscribe(_ =>
			{
				ShowBluePrint(false);
				InitializeRandomFailure();
			});
			
			_onReleased.Subscribe(_ =>
			{
				ShowBluePrint(true);
				ShowWarningSign(false);
			});
		}

		private void InitializeRandomFailure()
		{
			_failureDisposable?.Dispose();
			
			// Trigger Failure randomly after delay + randome range
			float time = UnityEngine.Random.Range(_malfunctionDelay, _malfunctionDelay + 5);
			_failureDisposable = Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
			{
				Break();
			});
		}
		
		private void Break()
		{
			if (!_capturedObject) return;
			
			StartFire();
			ShowWarningSign(true);
		}

		private void StartFire()
		{
			_fireEffect = Instantiate(_firePrefab).GetComponent<ParticleSystem>();
			_fireEffect.transform.SetPositionAndRotation(_capturedObject.transform.position, Quaternion.identity);
			_fireEffect.transform.parent = _capturedObject.transform;
			_fireEffect.Play();
		}

		private void ShowWarningSign(bool show, float time = 0.3f)
		{
			_warningTweener?.Kill();
			_bluePrintYoyoTweener?.Kill();
			
			if (show)
			{
				_warningSign.gameObject.SetActive(true);
				_warningTweener = _warningSign.DOScale(Vector3.one, time).SetEase(Ease.OutQuad).OnComplete(() =>
				{
					_bluePrintYoyoTweener = _warningSign.DOScale(Vector3.one * 1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);
				});
			}
			else
			{
				_warningTweener = _warningSign.DOScale(Vector3.one * 0.01f, time).SetEase(Ease.OutQuad).OnComplete(() =>
				{
					_warningSign.gameObject.SetActive(false);
				});
			}
		}
		
		private void ShowBluePrint(bool show, float time = 0.3f)
		{
			_bluePrintTweener?.Kill();
			
			if (show)
			{
				_bluePrint.gameObject.SetActive(true);
				_bluePrintTweener = _bluePrint.DOScale(Vector3.one, time).SetEase(Ease.OutQuad);
			}
			else
			{
				_bluePrintTweener = _bluePrint.DOScale(Vector3.one * 0.01f, time).SetEase(Ease.OutQuad).OnComplete(() =>
				{
					_bluePrint.gameObject.SetActive(false);
				});
			}
		}
	}
}