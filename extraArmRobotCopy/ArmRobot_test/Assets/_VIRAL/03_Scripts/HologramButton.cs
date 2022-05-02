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
	public class HologramButton : HologramUiComponent
	{
		private readonly Subject<HologramButton> _onButtonPress = new Subject<HologramButton>();

		public IObservable<HologramButton> OnPress => _onButtonPress;

		public override void EnterAction(Vector3 point)
		{
			base.EnterAction(point);
			transform.DOScale(_initialScale * 1.1f, 0.1f);
		}

		public override void ExitAction(Vector3 point)
		{
			base.ExitAction(point);
			PressButton();
		}

		public void PressButton()
		{
			if (_enabled)
			{
				_onButtonPress.OnNext(this);

				PlaySound();

				if (_deactivateAfterActionTime > 0)
				{
					DeactivateFor(_deactivateAfterActionTime);
				}
			}
		}
	}
}

