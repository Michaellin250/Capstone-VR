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
using RootMotion.FinalIK;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class RobotControls : MonoBehaviour
	{
		#region EDITOR
		[SerializeField] private Holdable _joystick;
		[SerializeField] private Transform _joystickReference;
		[SerializeField] private Transform _joystickBase;
		
		[SerializeField] private Holdable _lever;
		[SerializeField] private Transform _leverReference;
		[SerializeField] private Transform _leverBase;

		[Space]
		[SerializeField] private HologramButton _buttonMode;
		[SerializeField] private HologramButton _buttonSpawn;
		[SerializeField] private HologramCheckbox _checkboxHandGestures;
		[SerializeField] private HologramSlider _sliderHeight;
		[SerializeField] private HologramSlider2D _sliderPosition;
		
		[Space]
		[SerializeField] private Transform _visualRotator;
		[SerializeField] private Transform _screenRobotHead;
		[SerializeField] private Transform _screenRobotArm;
		[SerializeField] private Transform _screenTarget;
		
		[Space]
		[SerializeField] private RobotHologram _robotHologram;
		[SerializeField] private Rigidbody _hologramTrigger;
		[SerializeField] private RoboticArmFingerController _roboticArmFingerController;
		[SerializeField] private FABRIK _fabrik;

		#endregion

		#region PUBLIC
		public BoolReactiveProperty UseHandGestures = new BoolReactiveProperty(false);
		public RoboticArm RoboticArm => _roboticArm;

		public IObservable<Vector3> OnMove => _onMove;
		public IObservable<Vector3> OnUpdateTarget => _onUpdateTarget;
		public IObservable<float> OnUpdatePincher => _onUpdatePincher;
		public IObservable<RobotControls> OnTriggerDown => _onTriggerDown;
		public IObservable<RobotControls> OnTriggerUp => _onTriggerUp;
		#endregion

		#region PRIVATE
		private readonly Subject<Vector3> _onMove = new Subject<Vector3>();
		private readonly Subject<Vector3> _onUpdateTarget = new Subject<Vector3>();
		private readonly Subject<float> _onUpdatePincher = new Subject<float>();
		
		private readonly Subject<RobotControls> _onTriggerDown = new Subject<RobotControls>();
		private readonly Subject<RobotControls> _onTriggerUp = new Subject<RobotControls>();

		private Hand _joystickHand;
		
		private float _leverX;
		private float _joystickX;
		private float _joystickZ;

		private float _threshold = 1;

		private bool _triggerDown = false;

		private RoboticArm _roboticArm;
		private Spawner _spawner;
		
		private const string _automaticText = "AUTOMATIC";
		private const string _manualText = "MANUAL";

		private Pincher _currentPincher;
		private Pincher _fingerControllerPincher;


		#endregion

		private void Awake()
		{
			_roboticArm = GetComponentInParent<RoboticArm>();
			_spawner = FindObjectOfType<Spawner>();
		}

		private void Start()
		{
			InitializeJoystick();
			InitializeButtons();
			InitializePincherDetection();
		}

		private void Update()
		{
			HandleControls();
			HandleScreen();
			HandleHologramTarget();
			HandleRoboticArmFingerControl();
		}

		private void InitializeJoystick()
		{
			_joystick.OnCaptured.Subscribe(h =>
			{
				_joystickHand = _joystick.Holder.GetComponent<HandGrabber>().Hand;
			});

			_joystick.OnReleased.Subscribe(h =>
			{
				_joystickHand = null;
			});
		}
		
		private void HandleControls()
		{
			_leverX = _leverBase.InverseTransformPoint(_leverReference.transform.position).z;
			_joystickX = _joystickBase.InverseTransformPoint(_joystickReference.transform.position).x;
			_joystickZ = _joystickBase.InverseTransformPoint(_joystickReference.transform.position).z;

			Vector3 movement = new Vector3(_joystickX, _leverX, _joystickZ)*10;
			if (movement.magnitude > 0.05f)
			{
				UseHandGestures.Value = false;
				_onMove.OnNext(movement);
			}

			if (_joystickHand)
			{
				UseHandGestures.Value = false;
				float angle = _joystickHand.IndexMiddle.localRotation.eulerAngles.z;

				if (!_triggerDown && angle > 250 && angle < 300)
				{
					_onTriggerDown.OnNext(this);
					_triggerDown = true;
				}
				
				if (_triggerDown && angle > 320)
				{
					_onTriggerUp.OnNext(this);
					_triggerDown = false;
				}
			}
		}

		private void InitializeButtons()
		{
			// MODE SWITCH (AUTOMATIC / MANUAL)
			_buttonMode.SetText(_roboticArm.Manual.Value ? _automaticText : _manualText);
			_roboticArm.Manual.Subscribe((b =>
			{
				_buttonMode.SetText(b ? _automaticText : _manualText);
			}));
			_buttonMode.OnPress.Subscribe(_ =>
			{
				_roboticArm.SetManualMode(!_roboticArm.Manual.Value);
				UseHandGestures.Value = false;
			});

			// SPAWN
			_buttonSpawn.OnPress.Subscribe(_ =>
			{
				_spawner.Spawn();
			});
			
			// SLIDER HEIGHT Z
			_sliderHeight.OnValueChanged.Subscribe(value =>
			{
				UseHandGestures.Value = false;
				Vector3 currentLocalPos = _roboticArm.GetManualTargetLocalPosition();
				Vector3 newLocalTarget = new Vector3(currentLocalPos.x, value, currentLocalPos.z);
				_onUpdateTarget.OnNext(_roboticArm.transform.TransformPoint(newLocalTarget));
			});
			
			// SLIDER POSITION X/Y
			_sliderPosition.OnValueChanged.Subscribe(vector =>
			{
				UseHandGestures.Value = false;
				Vector3 currentLocalPos = _roboticArm.GetManualTargetLocalPosition();
				Vector3 newLocalTarget = new Vector3(vector.x, currentLocalPos.y, vector.y);
				_onUpdateTarget.OnNext(_roboticArm.transform.TransformPoint(newLocalTarget));
			});
			
			// USE HAND GESTURES
			_checkboxHandGestures.SetValue(UseHandGestures.Value);
			UseHandGestures.Subscribe( use =>
			{
				_fabrik.solver.target.position = _roboticArmFingerController.RoboticHead.position;
				_fabrik.enabled = !use;
				_fingerControllerPincher = use? _roboticArmFingerController.Pincher : null;
				_checkboxHandGestures.SetValue(use);
			});
			_checkboxHandGestures.OnSwitch.Subscribe(b =>
			{
				UseHandGestures.Value = b;
			});
		}
		
		private void HandleScreen()
		{
			Vector3 posTarget = _roboticArm.GetAimPosition();
			_screenTarget.localPosition = new Vector3(posTarget.x, posTarget.z, 0);

			Vector3 posHead = _roboticArm.Head.position;
			posHead = _roboticArm.GetAimPosition();
			
			_screenRobotHead.localPosition = new Vector3(posHead.x, posHead.z, 0);
			
			_screenRobotArm.localRotation = _roboticArm.RotatingBase.localRotation;
			
			_sliderHeight.SetValue(posHead.y);
			_sliderPosition.SetValue(new Vector2(posHead.x, posHead.z));

			_visualRotator.localRotation = _robotHologram.transform.localRotation;
		}

		private void InitializePincherDetection()
		{
			_hologramTrigger.OnTriggerEnterAsObservable().Subscribe(c =>
			{
				if (c.attachedRigidbody)
				{
					Pincher pincher = c.attachedRigidbody.GetComponent<Pincher>();
					if (pincher && !_currentPincher)
					{
						_currentPincher = pincher;
					}
				}
			});
			
			_hologramTrigger.OnTriggerExitAsObservable().Subscribe(c =>
			{
				if (c.attachedRigidbody)
				{
					Pincher pincher = c.attachedRigidbody.GetComponent<Pincher>();
					if (_currentPincher && pincher.GetInstanceID() == _currentPincher.GetInstanceID())
					{
						_currentPincher = null;
					}
				}
			});
		}

		private void HandleRoboticArmFingerControl()
		{
			if (_fingerControllerPincher && UseHandGestures.Value)
			{
				_onUpdatePincher.OnNext(_fingerControllerPincher.GetPinchGap());
				_roboticArm.DrawLaser(true);

			}
		}
		
		private void HandleHologramTarget()
		{
			if (_lever.Holder || _joystick.Holder) return;
			
			if (_currentPincher)
			{

				UseHandGestures.Value = false;
				
				Vector3 newLocalTarget = _robotHologram.GetRelativeTargetPos(_currentPincher.transform.position);
				_onUpdateTarget.OnNext(_roboticArm.transform.TransformPoint(newLocalTarget));
				_onUpdatePincher.OnNext(_currentPincher.GetPinchGap());
			}
		}
	}
}