using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class ForwardKinematicExample : MonoBehaviour
	{
        #region properties
        [SerializeField] private List<Part> _parts;

		[SerializeField] private LineRenderer _lineRenderer;

		public LineRenderer LineRenderer => _lineRenderer;
		public List<Part> Parts => _parts;


		private int _currentIndex = 0;

		public int CurrentIndex => _currentIndex;

		private Tween _rotationTween;

		public Tween RotationTween => _rotationTween;

        #endregion

        public void ApplyAngleToPart()
		{
			_rotationTween?.Kill();

			if (_currentIndex >= _parts.Count)
			{
				return;
			}

			if (_currentIndex > 0)
			{
				_parts[_currentIndex - 1].Transform.localRotation = Quaternion.Euler(_parts[_currentIndex - 1].Angle);
			}
			

			var part = _parts[_currentIndex];


			var index = _currentIndex;
			_rotationTween = part.Transform.DOLocalRotate(part.Angle, 2.0f);
			_rotationTween.onUpdate += () =>
			{

				for (int i = 0; i < Parts.Count; i++)
				{
					LineRenderer.SetPosition(i, Parts[i].Transform.position);
				}	
			};
			

			if (_currentIndex < Parts.Count - 1)
			{
				_currentIndex++;
			}
			
		}

		public void ReverseAngle()
		{
			_rotationTween.Kill();

			if (_currentIndex < 0)
			{
				return;
			}

			if ( _currentIndex < _parts.Count - 1)
			{
				_rotationTween = null;
				_parts[_currentIndex + 1].Transform.localRotation = Quaternion.identity;
			}

			var part = _parts[_currentIndex];
			
			_rotationTween = part.Transform.DOLocalRotate(Vector3.zero, 2.0f);

			_rotationTween.onUpdate += () =>
			{

				for (int i = 0; i < Parts.Count; i++)
				{
					LineRenderer.SetPosition(i, Parts[i].Transform.position);
				}	
			};

			

			if (_currentIndex > 0)
			{
				_currentIndex--;
			}
		}


		public void Reset()
		{
			foreach (var part in _parts)
			{
				part.Transform.localRotation = Quaternion.identity;
			}

			for (int i = 0; i < Parts.Count; i++)
			{
				LineRenderer.SetPosition(i, Parts[i].Transform.position);
			}	
			_rotationTween?.Kill();
			_rotationTween = null;
			_currentIndex = 0;
		}

		[Serializable]
		public struct Part
		{
			public Transform Transform;
			public Vector3 Angle;
		}
	}
}