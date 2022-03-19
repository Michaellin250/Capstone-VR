using System;
using System.Linq;
using UnityEngine;

namespace _VIRAL._03_Scripts.VisualUtilities
{
	
	[RequireComponent(typeof(LineRenderer))]
	public class TransformLineRenderer: MonoBehaviour
	{
		
		[SerializeField] private Transform[] _transforms;

		private LineRenderer _lineRenderer;
		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.positionCount = _transforms.Length;
			_lineRenderer.SetPositions(_transforms.Select(t => t.position).ToArray());
		}


		private void Update()
		{
			_lineRenderer.SetPositions(_transforms.Select(t => t.position).ToArray());
		}
	}
}