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
using DG.Tweening;
using UnityEngine;
using UniRx;

namespace _VIRAL._03_Scripts
{
	public class HandUI : MonoBehaviour
	{
        #region Editor
        [SerializeField] private Transform _target;
        
        [SerializeField] private ViralSettings _viralSettings;

        [Space]
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _panel;
        [SerializeField] private Window _window;

        [Space]
        [SerializeField] private HologramButton _buttonOpen;
        [SerializeField] private HologramButton _buttonClose;
        [SerializeField] private HologramCheckbox _checkboxTeleportation;
        [SerializeField] private HologramCheckbox _checkboxMimotion;
        [SerializeField] private HologramButton _buttonMenu;
        [SerializeField] private HologramButton _buttonMode;
        [SerializeField] private HologramButton _buttonInventory;
        #endregion

        #region Private
        private Transform _centerEyeAnchor;
        private ControlPanel _controlPanel;

        private bool _isFacing = false;
        private bool _panelOpened = false;
        private bool _panelMoving = false;

        private Vector3 _panelInitialScale;
        private Vector3 _buttonOpenInitialScale;

        private Sequence _panelSequence;
        private Tweener _buttonOpenTweener;

        private const string _engineerModeText = "ENGINEER";
        private const string _operatorModeText = "OPERATOR";
        
        private readonly Vector3 _openRotation = new Vector3(20, 0, 0);
        #endregion

        private void Awake()
		{
			_centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;
			_controlPanel = FindObjectOfType<ControlPanel>();
		}

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			InitializePanel();
			InitializeButtons();
			
			transform.parent = _target;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		private void Update()
		{
			CheckFacing();

			if (_isFacing && !_panelOpened && !_panelMoving)
			{
				OpenPanel();
			}

			if (!_isFacing && _panelOpened && !_panelMoving)
			{
				ClosePanel();
			}
		}

		private void CheckFacing()
		{
			Vector3 forward = transform.forward;
			Vector3 toOther = (_centerEyeAnchor.position - transform.position).normalized;

			// check if hand palm is facing towards head
			_isFacing = Vector3.Dot(forward, toOther) > 0.1f;
		}

		private void InitializePanel()
		{
			_panel.gameObject.SetActive(false);
			_pivot.localRotation = Quaternion.Euler(0, 0, 0);
			_panelInitialScale = _panel.localScale;
			_panel.localScale = _panelInitialScale * 0.01f;
			_buttonOpenInitialScale = _buttonOpen.transform.localScale;
			_window.Close();
		}

		private void OpenPanel()
		{
			_panelMoving = true;
			_panel.gameObject.SetActive(true);
			_panelSequence?.Kill();
			_panelSequence = DOTween.Sequence();
			_panelSequence.Append(_pivot.DOLocalRotate(_openRotation, 0.25f))
                .SetEase(Ease.InOutQuad)
				.OnComplete(() =>
				{
					_panelMoving = false;
					_panelOpened = true;
				});
			_panelSequence.Join(_panel.DOScale(_panelInitialScale, 0.2f))
                .SetEase(Ease.InOutQuad);
		}

		private void ClosePanel()
		{
			_window.Close();
			_buttonOpenTweener?.Kill();
			_buttonOpenTweener = _buttonOpen.transform.
                DOScale(_buttonOpenInitialScale, 0.1f).OnComplete(() =>
			{
				_buttonOpen.gameObject.SetActive(true);
			});
			
			_panelMoving = true;
			_panelSequence?.Kill();
			_panelSequence = DOTween.Sequence();
			_panelSequence.Append(
                _pivot.DOLocalRotate(Vector3.zero, 0.1f)).
                SetEase(Ease.InOutQuad).OnComplete(
				() =>
				{
					_panel.gameObject.SetActive(false);
					_panelMoving = false;
					_panelOpened = false;
				});
			_panelSequence.Join(_panel.DOScale(_panelInitialScale * 0.01f, 0.1f))
                .SetEase(Ease.InOutQuad);
		}
		
		private void InitializeButtons()
		{
            #region session 1
            // OPEN MENU
            _buttonOpen.OnPress.Subscribe(b =>
			{
				_window.Open();
				_buttonOpenTweener?.Kill();
				_buttonOpenTweener = _buttonOpen.transform.DOScale(_buttonOpenInitialScale * 0.01f, 0.1f)
					.OnComplete(() => { _buttonOpen.gameObject.SetActive(false); });
				_buttonOpen.DeactivateFor(1);
				Debug.Log("OPEN MENU");
			});

			// CLOSE HAND UI
			_buttonClose.OnPress.Subscribe(b =>
			{
				_window.Close();
				_buttonClose.DeactivateFor(1);
				_buttonOpen.gameObject.SetActive(true);

				Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(t =>
				{
					_buttonOpenTweener?.Kill();
					_buttonOpenTweener = _buttonOpen.transform.DOScale(_buttonOpenInitialScale, 0.1f).OnComplete(() =>
					{
						_buttonOpen.gameObject.SetActive(true);
					});
				});
			});

			_buttonMenu.OnPress.Subscribe(b => { _controlPanel.Activate(true); });

			// TELEPORTATION
			_checkboxTeleportation.SetValue(_viralSettings.TeleportationActive.Value);
			_viralSettings.TeleportationActive.Subscribe(_checkboxTeleportation.SetValue);
			_checkboxTeleportation.OnSwitch.Subscribe(active => { _viralSettings.TeleportationActive.Value = active; });

			// MIMOTION
			_checkboxMimotion.SetValue(_viralSettings.MimotionActive.Value);
			_viralSettings.MimotionActive.Subscribe(_checkboxMimotion.SetValue);
			_checkboxMimotion.OnSwitch.Subscribe(value => { _viralSettings.MimotionActive.Value = value; });
            #endregion
            // MODE SWITCH
            _buttonMode.SetText(_viralSettings.EngineerModeActive.Value? _operatorModeText : _engineerModeText);
			_viralSettings.EngineerModeActive.Subscribe((b =>
			{
				_buttonMode.SetText(b? _operatorModeText :  _engineerModeText);
			}));
			_buttonMode.OnPress.Subscribe(_ =>
			{
				_viralSettings.EngineerModeActive.Value = !_viralSettings.EngineerModeActive.Value;
				Debug.Log("SWITCH MODE: " + (_viralSettings.EngineerModeActive.Value? "ENGINEER" : "OPERATOR"));
			});
		}
	}
}