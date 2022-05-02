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
using System.IO;
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	[CreateAssetMenu(fileName = "ViralSettings", menuName = "Viral/Settings", order = 1)]
	public class ViralSettings: ScriptableObject
	{
		[Space]
		[SerializeField] public BoolReactiveProperty TeleportationActive = new BoolReactiveProperty(true);
		[SerializeField] public BoolReactiveProperty MimotionActive = new BoolReactiveProperty(true);
		[SerializeField] public BoolReactiveProperty EngineerModeActive = new BoolReactiveProperty(false);
		
		public void Save(string path)
		{
			var json = JsonUtility.ToJson(new SerializableSettings()
			{
				TeleportationActive = TeleportationActive.Value,
				MimotionActive = MimotionActive.Value,
				EngineerModeActive = EngineerModeActive.Value
			});
			
			File.WriteAllText(path, json);
		}

		public void Load(string path)
		{
			if (!File.Exists(path)) return;
			
			var settings = JsonUtility.FromJson<SerializableSettings>(File.ReadAllText(path));
			
			TeleportationActive.Value = settings.TeleportationActive;
			MimotionActive.Value = settings.MimotionActive;
			EngineerModeActive.Value = settings.EngineerModeActive;
		}

		[Serializable]
		private class SerializableSettings
		{
			public bool TeleportationActive;
			public bool MimotionActive;
			public bool EngineerModeActive;
		}

	}
}