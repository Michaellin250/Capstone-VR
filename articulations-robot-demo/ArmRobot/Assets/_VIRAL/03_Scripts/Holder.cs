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
using System.Linq;
using TMPro;
using UnityEngine;
using UniRx;

namespace _VIRAL._03_Scripts
{
	public class Holder : MonoBehaviour
	{
		[SerializeField] protected HolderType _holderType = HolderType.Undefined;
		[SerializeField] protected bool _captureKinematically = false;
		[SerializeField] protected Transform _textContainer;
		[SerializeField] protected TextMeshPro _tmp;
		[SerializeField] protected float _releaseDistance = 0.1f;
		[SerializeField] private bool _restrictAllowedObjects = false;
		[SerializeField] private ObjectType[] _allowedObjectTypes;

        #region PUBLIC

        public IObservable<Holdable> OnCaptured => _onCaptured;
        public IObservable<Holdable> OnReleased => _onReleased;
        public Rigidbody Rigidbody => _rb;
        public bool CaptureKinematically => _captureKinematically; 
        public float ReleaseDistance => _releaseDistance;

        public ObjectType[] AllowedObjectTypes => _allowedObjectTypes;

        public Holdable CapturedObject => _capturedObject;

        public HolderType HolderType => _holderType;

        #endregion

        #region PRIVATE

        protected Holdable _capturedObject;
        protected Holdable _closestHoldable;

        protected List<Holdable> _reachableHoldables = new List<Holdable>();

        protected readonly Subject<Holdable> _onCaptured = new Subject<Holdable>();
        protected readonly Subject<Holdable> _onReleased = new Subject<Holdable>();

        protected readonly Subject<Holdable> _newClosestDetected = new Subject<Holdable>();
        protected readonly Subject<Holdable> _noMoreObjectDetected = new Subject<Holdable>();

        private Rigidbody _rb;
        private CenterEyeAnchor _centerEye;
        
        #endregion

        protected void Awake()
        {
	        _rb = GetComponent<Rigidbody>();
	        _centerEye = FindObjectOfType<CenterEyeAnchor>();
        }

        protected virtual void Start()
		{
			// do nothing
		}

        protected void Update()
        {
	        if (_textContainer)
	        {
		        _textContainer.LookAt(_centerEye.transform);
	        }
        }

        protected virtual void OnTriggerEnter(Collider other)
		{
			if (_capturedObject) return;
			if (!other.attachedRigidbody) return;
			
			Holdable holdable = other.attachedRigidbody.GetComponent<Holdable>();

			bool allowedToHold = CheckObjectAllowed(holdable) && CheckIfCanHoldThis(holdable);
			
			if (holdable && !holdable.MarkedForDestruction && !_reachableHoldables.Contains(holdable) && allowedToHold)
			{
				_reachableHoldables.Add(holdable);
				CheckClosest();
			}
		}
        
		private void OnTriggerExit(Collider other)
		{
			if (!other.attachedRigidbody) return;
			Holdable holdable = other.attachedRigidbody.GetComponent<Holdable>();
			
			if (holdable && _reachableHoldables.Contains(holdable))
			{
				_reachableHoldables.Remove(holdable);
				holdable.Highlight(false);
				CheckClosest();
			}
		}

		protected void CheckClosest()
		{
			float closestDistance = 1000;
			_closestHoldable = null;
			
			_reachableHoldables.ForEach(holdable =>
			{
				if (holdable)
				{
					holdable.Highlight(false);
				
					float distance = Vector3.Distance(holdable.transform.position, transform.position);

					if (distance < closestDistance)
					{
						closestDistance = distance;
						_closestHoldable = holdable;
					}
				}
			});

			if (_closestHoldable == null)
			{
				_noMoreObjectDetected.OnNext(null);
			}
            else
            {
                _closestHoldable.Highlight(true);
                _newClosestDetected.OnNext(_closestHoldable);
            }
        }

		#region CAPTURE & RELEASE

		public void Capture(Holdable holdable)
		{
			_closestHoldable = holdable;
			Capture();
		}
		
		public void Capture()
		{
			if (_capturedObject) return;

			if (_closestHoldable)
			{
				_closestHoldable.Capture(this);
				_capturedObject = _closestHoldable;
				UpdateText(_capturedObject.name);
				_onCaptured.OnNext(_capturedObject);
				
				//Debug.Log(name + " CAPTURED " + _capturedObject.name);
			}
		}

		public void Release()
		{
			if (_capturedObject)
			{
				_capturedObject.Release();
				_onReleased.OnNext(_capturedObject);
				UpdateText("");
				//Debug.Log(name + " RELEASED " + _capturedObject.name);
				_capturedObject = null;

				Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
				{
					CheckClosest();
				});
			}
		}
		
		#endregion

		#region CHECK TYPES
		public bool CheckIfCanHoldThis(Holdable holdable)
		{
			if (!holdable.RestrictHolderTypes) return true;
			return holdable.AllowedHolderTypes.Contains(_holderType);
		}
        
		public bool CheckObjectAllowed(Holdable holdable)
		{
			if (!_restrictAllowedObjects) return true;
			return _allowedObjectTypes.Contains(holdable.Interactable.ObjectType);
		}
		#endregion
		
		private void UpdateText(string text)
		{
			if (_tmp)
			{
				_tmp.text = text;
			}
		}
	}
}