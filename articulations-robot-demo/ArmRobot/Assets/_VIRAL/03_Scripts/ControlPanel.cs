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
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class ControlPanel : MonoBehaviour
	{
		[SerializeField] private ViralSettings _viralSettings;

		[Space] [SerializeField] private SnapToHead _snapToHead;
		[SerializeField] private HeightAdjustor _heightAdjustor;

		[Space] [SerializeField] private HologramButton _buttonClose;
		[SerializeField] private HologramButton _hologramButtonSelect;
		[SerializeField] private HologramButton _hologramButtonDeselect;

		[Space] [SerializeField] private HologramCheckbox _hologramCheckboxFollowPlayer;
		[SerializeField] private HologramCheckbox _hologramCheckboxTeleportation;
		[SerializeField] private HologramCheckbox _hologramCheckboxMimotion;

		[Space] [SerializeField] private HologramSlider _sliderScale;

		[Space] [SerializeField] private Transform _container;

		private Tweener _panelTweener;

		private float _uiScale = 1;
		private bool _isActive;

		private const string SAVE_FILE_NAME = "ViralSettings.json";
		private string _savePath => $"{Application.persistentDataPath}/{SAVE_FILE_NAME}";

		private void Awake()
		{
			_snapToHead.SetDistance(_sliderScale.Value);
			_sliderScale.OnValueChanged.Subscribe(value =>
			{
				_snapToHead.SetDistance(value);

				// adjusting the size of the UI relative to the distance
				_uiScale = value;
			});

			AdjustScale(_sliderScale.Value);
		}

		private void Start()
		{
			_viralSettings.Load(_savePath);
			Initialize();
			Activate(false);
		}

		private void Update()
		{
			_container.localScale = Vector3.Lerp(_container.localScale, Vector3.one * _uiScale, 0.01f);
		}

		private void Initialize()
		{
			if (_buttonClose)
			{
				_buttonClose.OnPress.Subscribe(b => { Activate(false, 0.2f); });
			}
			
			_hologramButtonSelect.OnPress.Subscribe(_ =>
			{
				_hologramCheckboxTeleportation.PhysicalCheck(true);
				_hologramCheckboxMimotion.PhysicalCheck(true);
			});

			_hologramButtonDeselect.OnPress.Subscribe(_ =>
			{
				_hologramCheckboxTeleportation.PhysicalCheck(false);
				_hologramCheckboxMimotion.PhysicalCheck(false);
			});
			
			
			_hologramCheckboxFollowPlayer.OnSwitch.Subscribe(value =>
			{
				_hologramCheckboxFollowPlayer.SetValue(value);
				_snapToHead.enabled = value;
				_heightAdjustor.enabled = value;
			});

			// TELEPORTATION
			_hologramCheckboxTeleportation.SetValue(_viralSettings.TeleportationActive.Value);
			_viralSettings.TeleportationActive.Subscribe(_hologramCheckboxTeleportation.SetValue);
			_hologramCheckboxTeleportation.OnSwitch.Subscribe(active =>
			{
				_viralSettings.TeleportationActive.Value = active;
			});

			// MIMOTION
			_hologramCheckboxMimotion.SetValue(_viralSettings.MimotionActive.Value);
			_viralSettings.MimotionActive.Subscribe(_hologramCheckboxMimotion.SetValue);
			_hologramCheckboxMimotion.OnSwitch.Subscribe(value => { _viralSettings.MimotionActive.Value = value; });
		}

		public void Switch()
		{
			var o = gameObject;
			Activate(!o.activeSelf, o.activeSelf ? 0.1f : 0.2f);
		}

		public void Activate(bool activate, float time = 0.5f)
		{
			_panelTweener?.Kill();

			if (activate)
			{
				gameObject.SetActive(true);
				_isActive = true;
				_panelTweener = _container.DOScale(Vector3.one * _uiScale, time).SetEase(Ease.InOutQuad);
			}
			else
			{
				_panelTweener = _container.DOScale(Vector3.one * _uiScale * 0.01f, time).SetEase(Ease.InOutQuad)
					.OnComplete(() =>
					{
						_isActive = false;
						gameObject.SetActive(false);
					});
			}
		}

		private void AdjustScale(float value)
		{
			// adjusting the size of the UI relative to the distance
			_snapToHead.SetDistance(value);
			_uiScale = value;
		}

		private void OnDisable()
		{
			_viralSettings.Save(_savePath);
		}
	}
}