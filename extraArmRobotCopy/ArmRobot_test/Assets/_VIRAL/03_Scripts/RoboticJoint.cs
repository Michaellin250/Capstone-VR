using System;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class RoboticJoint: MonoBehaviour
	{

		[SerializeField] private Vector3 _axis;
		private Vector3 _initialOffset;
		public Vector3 Axis => _axis;

		public Vector3 InitialOffset => _initialOffset;

		private void Awake()
		{
			_initialOffset = Vector3.Scale(transform.localPosition, transform.lossyScale);  ;
		}

		public float GetAngle()
		{
			if (_axis.x > 0)
			{
				return transform.localRotation.eulerAngles.x;
			}
			else if (_axis.y > 0)
			{
				return transform.localRotation.eulerAngles.y;

			}
			else
			{
				return transform.localRotation.eulerAngles.z;
			}
		}
	
	}
}