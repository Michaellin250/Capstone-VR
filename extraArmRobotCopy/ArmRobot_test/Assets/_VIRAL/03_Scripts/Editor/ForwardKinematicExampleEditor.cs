using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace _VIRAL._03_Scripts.Editor
{
	[CustomEditor(typeof(ForwardKinematicExample))]
	public class ForwardKinematicExampleEditor: UnityEditor.Editor
	{
		
		public override void OnInspectorGUI()
		{
			
			DrawDefaultInspector();

			ForwardKinematicExample fk = (ForwardKinematicExample)target;
			
			
		
			if(GUILayout.Button("Apply Angle"))
			{
				fk.ApplyAngleToPart();

			}
			

			if (GUILayout.Button("Reverse Angle"))
			{
				fk.ReverseAngle();
			}
			
			if (GUILayout.Button("Reset"))
			{
				fk.Reset();
			}
		}
	}
}