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
using TMPro;
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class HologramUiComponent : MonoBehaviour
	{
        #region Editor
        [SerializeField] protected bool _enableOnStart = true;
		[SerializeField] protected bool _canBeUsedWithPointer = true;
		[SerializeField] protected bool _showHint = true;
		[SerializeField] protected float _showHintDelay = 0.5f;

		[SerializeField] protected TextMeshPro _text;
		[SerializeField] protected SpriteRenderer _icon;

		[SerializeField] protected float _actionDelay = 0f;
		[SerializeField] protected float _deactivateAfterActionTime = 0f;

		[SerializeField] protected Color _colorDisabled = new Color(1.000f, 1.000f, 1.000f, 0.3f);
		[SerializeField] protected Color _colorEnabled = new Color(1.000f, 1.000f, 1.000f, 0.7f);
		[SerializeField] protected Color _colorFocus = new Color(1.000f, 1.000f, 1.000f, 1.000f);
		[SerializeField] protected Color _colorActive = new Color(0.000f, 0.779f, 1.000f, 1.000f);

		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private AudioClip _buttonClip;

		[SerializeField] protected GameObject _hintPrefab;
		[SerializeField] protected string _hintText;
        #endregion

        #region Private
        public bool CanBeUsedWithPointer => _canBeUsedWithPointer;
		public bool Enabled => _enabled;
		public bool Focused => _focus;
		public bool Active => _active;

		protected Vector3 _iconInitialScale;
		protected Vector3 _initialScale;

		protected bool _enabled = true;
		protected bool _focus = false;
		protected bool _active = false;

		protected HologramHint _hologramHint;
		protected Interactor _interactor;

		protected IDisposable _hintDisposable;
        #endregion
        protected virtual void Awake()
		{
			_initialScale = transform.localScale;
			_iconInitialScale = _icon.transform.localScale;
			Enable(_enableOnStart);

			InitializeHint();
		}

        private void InitializeHint()
        {
            // instantiating hint
            var hint = Instantiate(_hintPrefab, transform.position, transform.rotation);
            _hologramHint = hint.GetComponent<HologramHint>();

            // assigning text - title by default
            _hologramHint.SetUiComponent(this);
            _hologramHint.SetText(_hintText.Length > 0 ? _hintText : _text.text);
        }

        public void DeactivateFor(float seconds = 0.4f)
		{
			Enable(false);
			Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe(t => { Enable(true); }).AddTo(this);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_enabled) return;

			Interactor newInteractor = other.gameObject.GetComponent<Interactor>();
			InteractorHint interactorHint = other.gameObject.GetComponent<InteractorHint>();

			if (newInteractor && !_interactor)
			{
				SetInteractor(newInteractor);
				EnterAction(other.transform.position);
			}

			if (interactorHint)
			{
				interactorHint.SetComponent(this);
				Focus(true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!_enabled) return;

			Interactor newInteractor = other.gameObject.GetComponent<Interactor>();
			InteractorHint interactorHint = other.gameObject.GetComponent<InteractorHint>();

			if (newInteractor && _interactor)
			{
				SetInteractor(null);
				ExitAction(other.transform.position);
			}

			if (interactorHint)
			{
				Focus(false);
			}
		}

		public virtual void EnterAction(Vector3 point)
		{
			Activate(true);
			ShowHint(false);
		}

		public virtual void ExitAction(Vector3 point)
		{
			transform.DOScale(_initialScale, 0.1f);

			Observable.Timer(TimeSpan.FromSeconds(_actionDelay)).Subscribe(t => { Activate(false); });
		}

		public virtual void Click(Vector3 position)
		{
			EnterAction(position);

			Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => { ExitAction(position); });
		}

		public void SetInteractor(Interactor interactor)
		{
			_interactor = interactor;
		}

		public void Enable(bool enable)
		{
			_enabled = enable;
			UpdateColor();
		}

		public void Activate(bool active)
		{
			_active = active;
			UpdateColor();
		}

		public void Focus(bool focus)
		{
			if (focus == _focus) return;

			_focus = focus;
			UpdateColor();

			if (focus)
			{
				ShowHint(true);
			}
			else
			{
				ShowHint(false);
			}
		}

		private void UpdateColor()
		{
			SetColor(_active ? _colorActive : _focus ? _colorFocus : _enabled ? _colorEnabled : _colorDisabled);
		}

		private void SetColor(Color color)
		{
			_text.color = color;
			if (_icon)
			{
				_icon.color = color;
			}
		}

		public void SetText(string text)
		{
			_text.text = text;
		}

		protected virtual void PlaySound()
		{
			if (_audioSource != null && _buttonClip != null)
			{
				_audioSource.PlayOneShot(_buttonClip, 1.0f);
			}
		}
		
		private void ShowHint(bool show)
		{
			if (show && _showHint)
			{
				_hintDisposable?.Dispose();
				_hintDisposable = Observable.Timer(TimeSpan.FromSeconds(_showHintDelay)).Subscribe(_ =>
				{
					if (_focus)
					{
						_hologramHint.ShowHint(true);
					}
				}).AddTo(this);
			}
			else
			{
				_hologramHint.ShowHint(false);
			}
		}

		private void OnDisable()
		{
			ShowHint(false);
		}

		private void OnDestroy()
		{
			Destroy(_hologramHint);
		}
	}
}