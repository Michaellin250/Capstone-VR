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
	public class KinematicsEstimator : MonoBehaviour
	{
        #region private vars
        private IDisposable _velocityEstimatorDisposable;
		private Rigidbody _rigidbody;

		private Vector3 _referencePosition;
		
		private int sampleCount;
		private Vector3[] velocitySamples;
		private Vector3[] angularVelocitySamples;

		private Vector3 _previousPosition;
		private Quaternion _previousRotation;

		private int _velocitySampleCount = 5;
		private int _angularVelocitySampleCount = 5;
        #endregion

        public int VelocitySampleCount
		{
			get => _velocitySampleCount;
			set
			{
				angularVelocitySamples = new Vector3[value];
				_velocitySampleCount = value;
			}
		}

		public int AngularVelocitySampleCount
		{
			get => _angularVelocitySampleCount;
			set
			{
				velocitySamples = new Vector3[value];
				_angularVelocitySampleCount = value;
			}
		}

		private void Awake()
		{
			velocitySamples = new Vector3[_velocitySampleCount];
			angularVelocitySamples = new Vector3[_angularVelocitySampleCount];
		}

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void StartEstimatingVelocity()
		{
			_velocityEstimatorDisposable?.Dispose();
			
			sampleCount = 0;
			_previousPosition = _referencePosition;
			_previousRotation = transform.rotation;
			
			_velocityEstimatorDisposable = Observable.EveryUpdate().Subscribe(_ =>
			{
				EstimateVelocity();
			}).AddTo(this);
		}

		public void StopEstimatingVelocity()
		{
			_velocityEstimatorDisposable?.Dispose();
			
		}

		private void EstimateVelocity()
		{
            int v = sampleCount % velocitySamples.Length;
            int w = sampleCount % angularVelocitySamples.Length;

            sampleCount = Mathf.Max(0, sampleCount + 1);

            #region linear velocity estimation 
            _referencePosition = _rigidbody == null ? transform.position : (transform.position + _rigidbody.centerOfMass);

			float velocityFactor = 1.0f / Time.deltaTime;

			
			
			// Estimate linear velocity
			velocitySamples[v] = velocityFactor * (_referencePosition - _previousPosition);
            #endregion
            #region angular velocity
            #region Complicated Math to get angular vecolicty

            // Get rotation difference
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(_previousRotation);
			
			// Get angle difference
			
			float theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));
			
			//Safety check if > 180
			if (theta > Mathf.PI)
			{
				theta -= 2.0f * Mathf.PI;
			}
			
			//Direction of the rotation
			Vector3 angularVelocity = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
			
			if (angularVelocity.sqrMagnitude > 0.0f)
			{
				angularVelocity = theta * velocityFactor * angularVelocity.normalized;
			}

			#endregion
	

			angularVelocitySamples[w] = angularVelocity;
            #endregion

            _previousPosition = _referencePosition;
			_previousRotation = transform.rotation;
		}
		
		public Vector3 GetEstimatedVelocity()
		{
			Vector3 velocity = Vector3.zero;
			
			int velocitySampleCount = Mathf.Min(sampleCount, velocitySamples.Length);
			
			// Compute average velocity
			if (velocitySampleCount != 0)
			{
				for (int i = 0; i < velocitySampleCount; i++)
				{
					velocity += velocitySamples[i];
				}
				velocity *= (1.0f / velocitySampleCount);
			}

			if (velocity == Vector3.negativeInfinity || velocity == Vector3.positiveInfinity || float.IsNaN(velocity.x ) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
			{
				return Vector3.zero;
			}

			return velocity;
		}
		
		public Vector3 GetEstimatedAngularVelocity()
		{
			
			Vector3 angularVelocity = Vector3.zero;
			int angularVelocitySampleCount = Mathf.Min(sampleCount, angularVelocitySamples.Length);
			
			// Compute average angular velocity
			if (angularVelocitySampleCount != 0)
			{
				for (int i = 0; i < angularVelocitySampleCount; i++)
				{
					angularVelocity += angularVelocitySamples[i];
				}
				angularVelocity *= (1.0f / angularVelocitySampleCount);
			}

			return angularVelocity;
		}
	}
}