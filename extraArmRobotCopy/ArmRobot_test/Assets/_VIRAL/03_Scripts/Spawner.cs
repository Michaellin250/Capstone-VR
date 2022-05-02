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
using Random = System.Random;

namespace _VIRAL._03_Scripts
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private bool _spawnAutomatically = false;
		
		[SerializeField] private GameObject _objectToCreate;
		[SerializeField] private float _spawnDelay = 2f;

		[SerializeField] private float _spawnHeight = 0.5f;
		[SerializeField] private float _innerRadius = 1;
		[SerializeField] private float _outerRadius = 2;

		private int iterator;
		private IDisposable _timer;
		
		private void Start()
		{
			if (_spawnAutomatically)
			{
				InitializeSpawner();
			}
		}

		private void Update()
		{
			if (!gameObject.activeSelf)
			{
				_timer?.Dispose();
				_timer = null;
			}

			if (_spawnAutomatically && gameObject.activeSelf && _timer == null)
			{
				InitializeSpawner();
			}
		}

		private void InitializeSpawner()
		{
			_timer = Observable.Interval(TimeSpan.FromSeconds(_spawnDelay)).Subscribe(_ =>
			{
				Spawn();
			}).AddTo(this);
		}

		public void Spawn()
		{
			iterator++;
			Vector2 flatpos = UnityEngine.Random.insideUnitCircle;
			flatpos = flatpos.normalized * UnityEngine.Random.Range(_innerRadius, _outerRadius);
			Vector3 pos = new Vector3(flatpos.x, _spawnHeight, flatpos.y);
			
			GameObject newObj = Instantiate(_objectToCreate, pos, Quaternion.identity);
			newObj.name = newObj.name + " " + iterator;
		}
	}
}