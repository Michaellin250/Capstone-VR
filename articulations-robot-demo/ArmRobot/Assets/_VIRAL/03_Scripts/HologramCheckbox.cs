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
public class HologramCheckbox : HologramUiComponent
{
		[SerializeField] private SpriteRenderer _iconCheckmark;

	private bool _isChecked = false;

	private Tweener _checkmarkTweener;
	
	private Vector3 _iconCheckmarkInitialScale;
	

		private readonly Subject<bool> _onSwitch = new Subject<bool>();

		public IObservable<bool> OnSwitch => _onSwitch;

	protected override void Awake()
	{
		base.Awake();
			
			//accessing transforms is expensive, so store them into local var
			var checkmarkTransform = _iconCheckmark.transform;
			
			_iconCheckmarkInitialScale = checkmarkTransform.localScale;
			checkmarkTransform.localScale = Vector3.one * 0.01f;
		_iconCheckmark.gameObject.SetActive(false);
	}
	
	public override void ExitAction(Vector3 point)
	{
		base.ExitAction(point);
		_icon.transform.DOScale(_iconInitialScale, 0.1f);
	}
	
	public override void EnterAction(Vector3 point)
	{
		base.EnterAction(point);
		_icon.transform.DOScale(_iconInitialScale*1.2f, 0.1f);
		
		PlaySound();
		PhysicalSwitch();
	}

	public void PhysicalSwitch()
	{
		PhysicalCheck(!_isChecked);
	}

	public void PhysicalCheck(bool value)
	{
		_isChecked = value;
			_onSwitch.OnNext(value);
		
		_checkmarkTweener?.Kill();
		if (value)
		{
			_iconCheckmark.gameObject.SetActive(true);
			_checkmarkTweener = _iconCheckmark.transform.DOScale(_iconCheckmarkInitialScale, 0.25f).SetEase(Ease.OutBack);
		}
		else
		{
			_checkmarkTweener = _iconCheckmark.transform.DOScale(_iconCheckmarkInitialScale * 0.01f, 0.25f)
				.SetEase(Ease.InBack).OnComplete(() =>
				{
					_iconCheckmark.gameObject.SetActive(false);
				});
		}
	}

	public void SetValue(bool value)
	{
		_isChecked = value;
		if (value)
		{
			_iconCheckmark.gameObject.SetActive(true);
			_iconCheckmark.transform.localScale = _iconCheckmarkInitialScale;
		}
		else
		{
			_iconCheckmark.gameObject.SetActive(false);
			_iconCheckmark.transform.localScale = _iconCheckmarkInitialScale * 0.01f;
		}
	}
}
}
