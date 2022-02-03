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
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	[RequireComponent(typeof(Interactable))]
	public class Holdable : MonoBehaviour
	{
	    [SerializeField] protected bool _captureKinematically = false;
		[SerializeField] protected List<HolderType> _allowedHolderTypes = new List<HolderType>();
		[SerializeField] protected List<Holdable> _subParts = new List<Holdable>();
		[SerializeField] protected Joint _linkedJoint;

        #region PUBLIC

        public Vector3 InitialLocalScale => _initialLocalScale;
        public Holder Holder => _holder;
        public Rigidbody Rb => _rb;
        public ConfigurableJoint Joint => _joint;
        public Interactable Interactable => _interactable;
        public bool IsCaptured => _isCaptured;
        public bool MarkedForDestruction => _markedForDestruction;
        public IObservable<Holdable> OnCaptured => _onCaptured;
        public IObservable<Holdable> OnReleased => _onReleased;

        public List<HolderType> AllowedHolderTypes => _allowedHolderTypes;

        #endregion

        #region PRIVATE

        private Interactable _interactable;
        private Holder _holder;
        
        private Rigidbody _rb;
        private ConfigurableJoint _joint;

        private Vector3 _capturePoint;

        private bool _markedForDestruction = false;
        private bool _isCaptured = false;

        private Transform _initialParent;
        private Vector3 _initialLocalScale = Vector3.one;
        private Vector3 _initialLocalPosition;

        private bool _initialKinematicState;
        private bool _initialGravityState;
        
        private Vector3 _linkedJointConnectedAnchor;

        private KinematicsEstimator _kinematicsEstimator;

        private readonly Subject<Holdable> _onCaptured = new Subject<Holdable>();
        private readonly Subject<Holdable> _onReleased = new Subject<Holdable>();

        #endregion

        private void Awake()
        {
	        _interactable = GetComponent<Interactable>();
	        _rb = GetComponent<Rigidbody>();
	        _kinematicsEstimator = gameObject.AddComponent<KinematicsEstimator>();
	        Initialize();
        }

        private void Start()
		{
			
		}

        private void Update()
        {
	        CheckDetachJoint();
        }

        private void Initialize()
		{
			_initialLocalScale = transform.localScale;
			_initialLocalPosition = transform.localPosition;
			_initialKinematicState = _rb.isKinematic;
			_initialGravityState = _rb.useGravity;
			
			if (_linkedJoint)
			{
				_linkedJointConnectedAnchor = _linkedJoint.connectedAnchor;
			}
		}

        public void Highlight(bool highlight) 
		{
			_interactable.AdjustRingScale(highlight ? 1.3f : 1f);
		}
		
		#region CAPTURE & RELEASE

		public void Capture(Holder holder)
		{
			if (_holder)
			{
				Debug.Log(name + "TAKE OVER");
				_holder.Release();
			}

			if (_captureKinematically || holder.CaptureKinematically)
			{
				// GRAB KINEMATICALLY
				_rb.isKinematic = true;
				_rb.useGravity = false;
				
				_kinematicsEstimator.StartEstimatingVelocity();
				transform.parent = holder.transform;
				_rb.interpolation = RigidbodyInterpolation.None;
			} 
			else
			{
				// GRAB PHYSICALLY
				_rb.useGravity = false;
				CreateJoint(holder);
			}

			_isCaptured = true;
			_holder = holder;
			_onCaptured.OnNext(this);
			//Debug.Log("CAPTURED: " + name );
		}

		public void Release()
		{
			if (!_holder) return;
			
			if (_captureKinematically || _holder.CaptureKinematically)
			{
				_rb.isKinematic = _initialKinematicState;
				transform.parent = _initialParent;
				
				if (_rb.useGravity)
				{
					_rb.velocity = _kinematicsEstimator.GetEstimatedVelocity();
					_rb.angularVelocity = _kinematicsEstimator.GetEstimatedAngularVelocity();
				}

				_kinematicsEstimator.StopEstimatingVelocity();
			} 
			else
			{
				// release physics
				Destroy(_joint);
				_joint = null;
			}
			
			_rb.useGravity = _initialGravityState;
			_rb.interpolation = RigidbodyInterpolation.Interpolate;

			_isCaptured = false;
			_holder = null;
			_onReleased.OnNext(this);
			//Debug.Log("RELEASED: " + name );
		}
		
		#endregion
		
		#region PHYSICS

		private void CreateJoint(Holder holder)
		{
			_joint = gameObject.AddComponent<ConfigurableJoint>();
			_joint.connectedBody = holder.Rigidbody;

			float spring, angularSpring, damper, maximumForce = 0;

			if (holder.GetComponent<HandGrabber>())
			{
				// HAND GRABBER
				_joint.autoConfigureConnectedAnchor = true;
				spring = 3000;
				angularSpring = 1000;
				damper = 50;
				maximumForce = 10000;
				_capturePoint = transform.InverseTransformPoint(holder.transform.position);
			}
			else
			{
				// SLOT
				_joint.autoConfigureConnectedAnchor = false;
				_joint.anchor = holder.transform.position;
				_joint.connectedAnchor = Vector3.zero;
				spring = 1000;
				angularSpring = 100;
				damper = 100;
				maximumForce = 1000;
				_capturePoint = _rb.centerOfMass;
			}
			
			ConfigurableJointMotion motion = ConfigurableJointMotion.Free;
			
			_joint.xMotion = _joint.yMotion = _joint.zMotion = motion;
			_joint.angularXMotion = _joint.angularYMotion = _joint.angularZMotion = motion;

			JointDrive motionDrive = new JointDrive();
			motionDrive.positionSpring = spring;
			motionDrive.positionDamper = damper;
			motionDrive.maximumForce = maximumForce;

			JointDrive angularDrive = new JointDrive();
			angularDrive.positionSpring = angularSpring;
			angularDrive.positionDamper = damper;
			angularDrive.maximumForce = maximumForce;

			_joint.xDrive = _joint.yDrive = _joint.zDrive = motionDrive;
			_joint.angularXDrive = _joint.angularYZDrive = angularDrive;

			_joint.breakForce = 5000;
			_joint.breakTorque = 5000;
		}

		private void CheckDetachJoint()
		{
			if (!_joint || !_holder) return;
			
			if (!_captureKinematically || !_holder.CaptureKinematically)
			{
				float distance = Vector3.Distance(transform.TransformPoint(_capturePoint),
					_holder.transform.position);

				if(distance > _holder.ReleaseDistance)
				{
					//Debug.Log("DISTANCE: " + distance);
					_holder.Release();
				}
			}
		}

		private void OnJointBreak(float breakForce)
		{
			//Debug.Log("JOINT BREAK");
			if (!_captureKinematically || (_holder && !_holder.CaptureKinematically))
			{
				_holder.Release();
			}
		}

		public void RefreshPhysics()
		{
			RefreshSubJoints();
		}
		
		public void RefreshSubJoints()
		{
			_subParts.ForEach(p =>
			{
				p.RefreshJoint();
				p.RefreshSubJoints();
			});
		}

		public void RefreshJoint()
		{
			if (_linkedJoint)
			{
				_linkedJoint.connectedAnchor = _linkedJointConnectedAnchor;
			}
		}
		
		#endregion
		
		public void CollapseAndDestroy(float time)
		{
			_markedForDestruction = true;
			transform.DOScale(InitialLocalScale * 0.01f, time).OnUpdate(() =>
			{
				RefreshPhysics();

			}).OnComplete(() =>
			{
				DestroyImmediate(gameObject);
			});
		}
	}
}