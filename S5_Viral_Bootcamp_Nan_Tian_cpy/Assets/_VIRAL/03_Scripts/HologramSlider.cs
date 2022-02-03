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
    public class HologramSlider : HologramUiComponent
    {
        [SerializeField] private float _min = 0;
        [SerializeField] private float _max = 10;
        [SerializeField] private float _value = 5;

        [SerializeField] private float _lerpTime = 0.1f;

        [Space] [SerializeField] private Transform _ring;
        [SerializeField] private Transform _minPos;
        [SerializeField] private Transform _maxPos;

        [SerializeField] private TextMeshPro _valueTitle;
        [SerializeField] private TextMeshPro _valueText;
        [SerializeField] private TextMeshPro _minText;
        [SerializeField] private TextMeshPro _maxText;

        private Tweener _sliderTweener;
        private bool _sliding = false;
        private float _ringPosition;

        private readonly Subject<float> _onValueChanged = new Subject<float>();

        public IObservable<float> OnValueChanged => _onValueChanged;

        public float Value => _value;

        protected override void Awake()
        {
            base.Awake();
            UpdateScale();

            SetValue(_value);
            _onValueChanged.OnNext(_value);
        }

        private void Update()
        {
            if (!_sliding && _interactor)
            {
                _sliding = true;
                _icon.transform.DOScale(_iconInitialScale * 1.4f, 0.3f);
            }

            if (_sliding && !_interactor)
            {
                _sliding = false;
                _icon.transform.DOScale(_iconInitialScale, 0.3f);
            }

            if (_interactor)
            {
                CalculateValue(_interactor.transform.position);
            }

            Vector3 currentPos = _ring.localPosition;
            _ring.localPosition = Vector3.Lerp(currentPos, new Vector3(_ringPosition, currentPos.y, currentPos.z),
                _lerpTime);
        }

        private void CalculateValue(Vector3 hitPosition)
        {
            Vector3 hitLocalPosition = transform.InverseTransformPoint(hitPosition);
            SetValue((hitLocalPosition.x - _minPos.localPosition.x) / GetScale() + _min);
            _onValueChanged.OnNext(_value);
        }

        public void SetMin(float min)
        {
            _min = min;
            UpdateScale();
        }

        public void SetMax(float max)
        {
            _max = max;
            UpdateScale();
        }

        private void UpdateScale()
        {
            _value = Mathf.Clamp(_value, _min, _max);
            _onValueChanged.OnNext(_value);
            _minText.text = _min.ToString();
            _maxText.text = _max.ToString();
        }

        public void SetValue(float value)
        {
            _value = Mathf.Clamp(value, _min, _max);
            _valueText.text = Math.Round(_value, 1).ToString();
            AdjustSlider();
        }

        private void AdjustSlider()
        {
            _ringPosition = _minPos.localPosition.x + (_value - _min) * GetScale();
        }

        private float GetScale()
        {
            return GetLength() / (_max - _min);
        }

        private float GetLength()
        {
            return _maxPos.localPosition.x - _minPos.localPosition.x;
            ;
        }

        public override void Click(Vector3 position)
        {
            // do nothing
        }
    }
}
