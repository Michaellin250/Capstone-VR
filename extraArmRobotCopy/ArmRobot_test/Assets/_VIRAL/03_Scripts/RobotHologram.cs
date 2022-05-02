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

using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class RobotHologram : MonoBehaviour
	{
		[SerializeField] private Transform _root;
		[SerializeField] private Transform _rotatingBase;
		[SerializeField] private Transform _shoulder;
		[SerializeField] private Transform _elbow;
		[SerializeField] private Transform _wrist;
		[SerializeField] private Transform _slider;
		[SerializeField] private Transform _head;
		[SerializeField] private Transform _mouth;
		[SerializeField] private Transform _pincherLeft;
		[SerializeField] private Transform _pincherRight;
		
		[Space]
		[SerializeField] private Transform _target;

		private RoboticArm _roboticArm;

		private void Awake()
		{
			_roboticArm = FindObjectOfType<RoboticArm>();
		}

		private void Update()
		{
			transform.rotation = _roboticArm.transform.rotation;
			
			_rotatingBase.localRotation = _roboticArm.RotatingBase.localRotation;
			_shoulder.localRotation = _roboticArm.Shoulder.localRotation;
			_elbow.localRotation = _roboticArm.Elbow.localRotation;
			_wrist.localRotation = _roboticArm.Wrist.localRotation;
			_head.localRotation = _roboticArm.Head.localRotation;
			_mouth.localRotation = _roboticArm.Mouth.localRotation;
			
			_slider.localPosition = _roboticArm.Slider.localPosition;
			_pincherLeft.localPosition = _roboticArm.PincherLeft.localPosition;
			_pincherRight.localPosition = _roboticArm.PincherRight.localPosition;

			_target.localPosition = _roboticArm.GetManualTargetLocalPosition();
		}

		public Vector3 GetRelativeTargetPos(Vector3 position)
		{
			return _root.InverseTransformPoint(position);
		}
	}
}