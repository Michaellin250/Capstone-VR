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

using DG.Tweening;
using UnityEngine;

public class Window : MonoBehaviour
{
	[SerializeField] private Transform _background;
	[SerializeField] private Transform _header;
	[SerializeField] private Transform _content;

	private Sequence _sequence;

	private readonly Vector3 _closedBackgroundScale = new Vector3(0.01f, 1, 1);
	
	public void Open()
	{
		_sequence?.Kill();
		_sequence = DOTween.Sequence();

		_background.gameObject.SetActive(true);
		_sequence.Append(_background.DOScale(Vector3.one, 0.2f).OnComplete(() =>
		{
			_header.gameObject.SetActive(true);
			_content.gameObject.SetActive(true);
		}));
		_sequence.Append(_header.DOScale(Vector3.one, 0.2f));
		_sequence.Join(_content.DOScale(Vector3.one, 0.2f));
	}
	
	public void Close()
	{
		_sequence?.Kill();
		_sequence = DOTween.Sequence();

		_sequence.Append(_content.DOScale(Vector3.one * 0.01f, 0.2f).OnComplete(() =>
		{
			_content.gameObject.SetActive(false);
		}));
		_sequence.Join(_header.DOScale(Vector3.one * 0.01f, 0.2f).OnComplete(() =>
		{
			_header.gameObject.SetActive(false);
		}));
		_sequence.Append(_background.DOScale(_closedBackgroundScale , 0.2f).OnComplete(() =>
		{
			_background.gameObject.SetActive(false);
		}));
	}
}
