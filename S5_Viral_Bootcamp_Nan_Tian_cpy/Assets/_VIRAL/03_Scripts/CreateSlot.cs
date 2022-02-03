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
using UnityEngine;
using UniRx;

namespace _VIRAL._03_Scripts
{
	public class CreateSlot : InventorySlot
	{
		[SerializeField] private GameObject _objectToCreate;
		[SerializeField] private ParticleSystem _createEffect;

		protected override void Start()
		{
			base.Start();

			Create();
			
			_noMoreObjectDetected.Subscribe(_ =>
			{
				Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(t =>
				{
					Create();
				});
			});
		}

		private void Create()
		{
			GameObject newObj = Instantiate(_objectToCreate);
			
			_closestHoldable = newObj.GetComponent<Holdable>();
			_closestHoldable.transform.position = transform.position;
			_closestHoldable.transform.rotation = transform.rotation;
			
			_closestHoldable.transform.localScale = _closestHoldable.InitialLocalScale * 0.01f;
			_closestHoldable.RefreshPhysics();

			Capture();
			_createEffect.Play();
			
			//Debug.Log("CREATE " + _objectToCreate.name);
		}
	}
}