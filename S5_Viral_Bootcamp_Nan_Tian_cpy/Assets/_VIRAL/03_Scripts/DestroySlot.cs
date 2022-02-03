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
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class DestroySlot : InventorySlot
	{
		[SerializeField] private ParticleSystem _destroyEffect;
		[SerializeField] private Transform _trashLid;
		[SerializeField] private Transform _trashContainer;

		protected List<Holdable> _markedForDestroy = new List<Holdable>();
		
		private Sequence _lidSequence;

		protected override void Start()
		{
			base.Start();
			_onCaptured.Subscribe(h =>
			{
				if (!h.MarkedForDestruction)
				{
					MarkForDestroy(h);
				}
			});
		}

		protected override void OnTriggerEnter(Collider other)
		{
			if (!other.attachedRigidbody) return;
			Holdable holdable = other.attachedRigidbody.GetComponent<Holdable>();
			
			if (holdable && !holdable.Holder && CheckObjectAllowed(holdable) && CheckIfCanHoldThis(holdable) && !holdable.MarkedForDestruction)
			{
				MarkForDestroy(holdable);
			}
		}

		private void MarkForDestroy(Holdable holdable)
		{
			holdable.Capture(this);
			_capturedObject = holdable;
			
			holdable.CollapseAndDestroy(0.5f);

			_markedForDestroy.Add(holdable);
			OpenLid(true);
			
			_onCaptured.OnNext(holdable);
				
			holdable.OnDestroyAsObservable().Subscribe(h =>
			{
				_destroyEffect.Play();
				_ring.Show(false);

				Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe(_ =>
				{
					_markedForDestroy.RemoveAll(item => item == null);
					if (_markedForDestroy.Count <= 0)
					{
						OpenLid(false);
					}
				});
			});
		}

		private void OpenLid(bool open)
		{
			_lidSequence?.Kill();
			_lidSequence = DOTween.Sequence();

			if (open)
			{
				_lidSequence.Append(_trashLid.DOLocalMoveY(0.05f, 0.3f)).SetEase(Ease.InOutBack);
				_lidSequence.Join(_trashContainer.DOLocalMoveY(-0.05f, 0.3f)).SetEase(Ease.InOutBack);
			}
			else
			{
				_lidSequence.Append(_trashLid.DOLocalMoveY(0.0f, 0.3f)).SetEase(Ease.InOutBack);
				_lidSequence.Join(_trashContainer.DOLocalMoveY(0.0f, 0.3f)).SetEase(Ease.InOutBack);
			}
		}
	}
}