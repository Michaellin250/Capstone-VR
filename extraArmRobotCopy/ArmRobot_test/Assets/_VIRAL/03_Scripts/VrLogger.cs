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
using TMPro;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class VrLogger : MonoBehaviour
	{
		[SerializeReference] private List<LogType> _logTypes = new List<LogType>();
		[SerializeField] private int _numberOfLines = 10;
		
		private readonly Queue<string> _logQueue = new Queue<string>();
		private TextMeshPro _tmp;

		private void Awake()
		{
			 _tmp = GetComponentInChildren<TextMeshPro>();
			 Application.logMessageReceived += HandleLog;
		}


		private void OnEnable()
		{
			_tmp.text = string.Join("\n", _logQueue);
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= HandleLog;
		}

		void HandleLog(string logString, string stackTrace, LogType type)
		{

			if (!_logTypes.Contains(type)) return;
		
			_logQueue.Enqueue(logString);

			if (_logQueue.Count > _numberOfLines)
			{
				_logQueue.Dequeue();
			}

			if (isActiveAndEnabled)
			{
				_tmp.text = string.Join("\n", _logQueue);
			}
		}
	}
}
	