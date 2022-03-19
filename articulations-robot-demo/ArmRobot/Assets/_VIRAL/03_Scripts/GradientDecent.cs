using System.Linq;
using UnityEngine.Serialization;

namespace _VIRAL._03_Scripts
{
	using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientDecent : MonoBehaviour
{
	private RoboticJoint[] _joints;
	[SerializeField] private float _delta;
	[SerializeField] private float _learningRate;
	[SerializeField] private Transform _target;
	[SerializeField] private float _distanceThreshold = 0.01f;
	[SerializeField] private bool _automaticUpdate = false;
	[SerializeField] private int _maxIterations = 10;

	
	private void Awake()
	{
		_joints = GetComponentsInChildren<RoboticJoint>();
	}

	private void Update()
	{
		if (_joints.Length <= 0 || _delta <= 0) return;

		var sqrDistance = (_joints.Last().transform.position - _target.transform.position).sqrMagnitude;

		if (sqrDistance <= _distanceThreshold * _distanceThreshold)
		{
			return;
		}

		if (!_automaticUpdate) return;
		
		
		for (int i = 0; i < _maxIterations; i++)
		{
			ApplyInverseKinematics();
			sqrDistance = (_joints.Last().transform.position - _target.transform.position).sqrMagnitude;

			if (sqrDistance <= _distanceThreshold * _distanceThreshold)
			{
				return;
			}
		}
		
	

	}

	private void ApplyInverseKinematics()
	{
		var angles = new float[_joints.Length];

		for (var i = 0; i < _joints.Length; i++)
		{
			angles[i] = _joints[i].GetAngle();
		}



		InverseKinematics(_target.transform.position, angles);
		
		
		for (var i = 0; i < _joints.Length; i++)
		{
			_joints[i].transform.localRotation = Quaternion.AngleAxis(angles[i], _joints[i].Axis);
		}		
	}

	public float DistanceFromTarget(Vector3 target, float[] angles)
	{
		var endPoint = ForwardKinmatics(angles);

		return Vector3.Distance(endPoint, target);
	}

	public Vector3 ForwardKinmatics(float[] angles)
	{
		var previousPoint = _joints[0].transform.position;

		var rotation = Quaternion.identity;


		for (int i = 1; i < _joints.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(angles[i - 1], _joints[i - 1].Axis);

			var nextPoint = previousPoint + rotation * _joints[i].InitialOffset;

			previousPoint = nextPoint;
		}

		return previousPoint;
	}

	public float PartialGradient(Vector3 target, float[] angles, int i)
	{
		var angle = angles[i];

		var currentDistance = DistanceFromTarget(target, angles);

		angles[i] += _delta;

		var nextDistance = DistanceFromTarget(target, angles);

		//if closer negative, if farther away positive
		var gradient = (nextDistance - currentDistance) / _delta;


		//Restore initial state
		angles[i] = angle;

		return gradient;
	}

	public void InverseKinematics(Vector3 target, float[] angles)
	{

		if (DistanceFromTarget(target, angles) < _distanceThreshold)
		{
			return;
		}
		
		for (var i = _joints.Length - 1; i >= 0; i--)
		{
			var gradient = PartialGradient(target, angles, i);
			angles[i] -= _learningRate * gradient;

			if (DistanceFromTarget(target, angles) < _distanceThreshold)
			{
				return;
			}
		}
	}
}
}