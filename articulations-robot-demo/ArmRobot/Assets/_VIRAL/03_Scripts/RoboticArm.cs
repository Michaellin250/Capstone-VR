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
using System.Linq;
using System.Text;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
	public class RoboticArm : MonoBehaviour
	{
		#region EDITOR

		[SerializeField] public BoolReactiveProperty Manual = new BoolReactiveProperty(true);
		
		[Space]
		[SerializeField] private Transform _manualTarget;
		[SerializeField] private Transform _dropTarget;
		[SerializeField] private Transform _ikTarget;
		[SerializeField] private RoboticMouthSlot _roboticMouthSlot;
		[SerializeField] private LayerMask _layerMask;
		
		[Space]
		[SerializeField] private Transform _rotatingBase;
		[SerializeField] private Transform _shoulder;
		[SerializeField] private Transform _elbow;
		[SerializeField] private Transform _wrist;
		[SerializeField] private Transform _slider;
		[SerializeField] private Transform _head;
		[SerializeField] private Transform _mouth;
		[SerializeField] private Transform _pincherLeft;
		[SerializeField] private Transform _pincherRight;
		
		[SerializeField] private Transform _pincherLeftTarget;
		[SerializeField] private Transform _pincherRightTarget;
		
		[Space]
		//[SerializeField] private ViralSettings _viralSettings;

		// Robot Settings
		[Space]
		[SerializeField] private float _speedMultiplier = 1;
		[Space]
		[SerializeField] private float _minRadius = 0.4f;
		[SerializeField] private float _maxRadius = 2.2f;
		[Space]
		[SerializeField] private float _rotationTime = 0.5f;
		[SerializeField] private float _bendArmTime = 0.5f;
		[SerializeField] private float _pinchTime = 0.5f;
		
		// Debug
		[Space]
		[SerializeField] private bool _showDebugState = false;
		[SerializeField] private Transform _debugContainer;
		[SerializeField] private TextMeshPro _textMeshPro;
		
		// Laser
		[Space] 
		[SerializeField] private LineRenderer _laser;
		[SerializeField] private Transform _laserStart;
		[SerializeField] private LayerMask _layerMaskLaser;
		
		#endregion

		#region PRIVATE
		
		// References
		private Transform _centerEye;

		// States
		private bool _mouthFull = false;
		private bool _isMoving = false;

		// Tweeners & Sequences
		private Tweener _rotationTweener;
		private Tweener _bendArmTweener;
		private Tweener _adjustHeadTweener;
		private Tweener _pinchLeftTweener;
		private Tweener _pinchRightTweener;
		
		// Caches values
		private Vector3 _aimPosition;
		private Vector3 _restTargetLocalPosition = new Vector3(0, 1, 1);
		private Vector3 _adjustedTargetLocalPosition;
		private Holdable _currentTarget;
		private List<Holdable> _targets = new List<Holdable>();
		
		private RaycastHit[] _captureZoneHits = new RaycastHit[100];

		private RobotControls _robotControls;

		private bool _faceDown = false;
		private Vector3 _manualInput = Vector3.zero;
		
		private float _pincherOpenedGap = 0.18f;
		private float _pincherClosedGap = 0.0f;

		private IDisposable _checkRobotAutomaticMovement;
		private IDisposable _checkCaptureZone;
		
		#endregion

		#region PUBLIC

		public Transform RotatingBase => _rotatingBase;
		public Transform Shoulder => _shoulder;
		public Transform Elbow => _elbow;
		public Transform Wrist => _wrist;
		public Transform Slider => _slider;
		public Transform Head => _head;
		public Transform Mouth => _mouth;
		public Transform PincherLeft => _pincherLeft;
		public Transform PincherRight => _pincherRight;
		
		#endregion
		
		private void Awake()
		{
			_centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
			_robotControls = FindObjectOfType<RobotControls>();
		}

		private void Start()
		{
			InitializeRobot();
			InitializeDebugState();
		}

		private void InitializeRobot()
		{
			CheckMode();
			CheckMouthState();

			Manual.Subscribe(m =>
			{
				SetManualMode(m);
			});

			if (!Manual.Value)
			{
				InitializeAutomaticMode();
			}

			InitializeManualMode();
			InitializePincher();
		}

		private void InitializeAutomaticMode()
		{
			DrawLaser(false);
			_checkRobotAutomaticMovement = Observable.Interval(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
			{

				/*
				if (!_viralSettings.EngineerModeActive.Value && !Manual.Value)
				{
					HandleAutomaticMovement();
				}*/
			}).AddTo(this);

			_checkCaptureZone = Observable.Interval(TimeSpan.FromSeconds(0.5f)).Subscribe(t =>
			{
				/*
				if (!_viralSettings.EngineerModeActive.Value)
				{
					CheckCaptureZone();
				}*/
			}).AddTo(this);
		}

		private void CheckMode()
		{

			/*
			_viralSettings.EngineerModeActive.Subscribe(active =>
			{
				if (active)
				{
					Abort();
				}
			});*/
		}
		
		private void CheckMouthState()
		{
			_roboticMouthSlot.OnCaptured.Subscribe(h =>
			{
				_mouthFull = true;
			});
			
			_roboticMouthSlot.OnReleased.Subscribe(h =>
			{
				_mouthFull = false;
			});
		}

		private void CheckCaptureZone()
		{
			Physics.SphereCastNonAlloc(transform.position, _maxRadius, transform.up, _captureZoneHits, 5, _layerMask.value);

			_captureZoneHits.ToList().ForEach(h =>
			{
				if (h.rigidbody)
				{
					Holdable holdable = h.rigidbody.GetComponent<Holdable>();

					if (holdable)
					{
						bool isNew = !_targets.Contains(holdable);
						bool isFree = !holdable.IsCaptured;
						bool notAPart = !holdable.GetComponentInParent<RoboticArm>();
						bool canBeCaptured = _roboticMouthSlot.CheckObjectAllowed(holdable) &&
						                     _roboticMouthSlot.CheckIfCanHoldThis(holdable);

						if (isNew && isFree && notAPart && canBeCaptured)
						{
							_targets.Add(holdable);
						}
					}
				}
			});
		}

		#region CHOOSE THE TARGET
		private void HandleAutomaticMovement()
		{
			if (!_currentTarget)
			{
				_currentTarget = GetNextTarget();
			}
			
			if (_currentTarget)
			{
				CheckState();
				CheckAbort();
			}

			_manualTarget.position = _aimPosition;
		}
		
		private Holdable GetNextTarget()
		{
			// make sure to remove null entities, captured entities, and moving entities
			_targets.RemoveAll(x => x == null);
			_targets.RemoveAll(x => x.IsCaptured);
			_targets.RemoveAll(x => x.Rb.velocity.magnitude > 0.05f);

			Holdable holdable = _targets.Count > 0 ? _targets[0] : null;
			
			return holdable;
		}

		private void CheckState()
		{
			if (!_isMoving && _currentTarget)
			{
				if (!_mouthFull)
				{
					_aimPosition = _currentTarget.transform.position;
					_faceDown = true;
					GoToTarget(_aimPosition, false);
				}
				else
				{
					_aimPosition = _dropTarget.position;
					_faceDown = false;
					GoToTarget(_aimPosition, true);
				}
			}
		}

		#endregion
		
		#region GO AUTOMATICALLY TO TARGET

		private void GoToTarget(Vector3 point, bool dropOnTarget)
		{
			//Debug.Log("GO TO TARGET " + point + ( dropOnTarget? " & DROP" : " & CAPTURE "));
			
			_isMoving = true;
			
			_adjustedTargetLocalPosition = _restTargetLocalPosition;
			
			BendArm(_bendArmTime / _speedMultiplier);
			RotateBase(point, _rotationTime / _speedMultiplier).OnComplete(() =>
			{
				ComputeAdjustedTargetPosition(point);
				BendArm(_bendArmTime / _speedMultiplier);

				if (!dropOnTarget)
				{
					Pinch(true, _pinchTime / _speedMultiplier);
				}
				
				AdjustHead(point, _bendArmTime / _speedMultiplier).OnComplete(() =>
				{
					if (dropOnTarget)
					{
						Pinch(true, _pinchTime / _speedMultiplier).OnComplete(() =>
						{
							_isMoving = false;
						});
						DropObject();
						_targets.Remove(_currentTarget);
						_currentTarget = null;
					}
					else
					{
						Pinch(false, _pinchTime / _speedMultiplier).OnComplete(() =>
						{
							_isMoving = false;
						});
						CaptureObject();
					}
				});
			});
		}
		
		private void ComputeAdjustedTargetPosition(Vector3 position)
		{
			if (_faceDown)
			{
				position += Vector3.up * 0.3f;;
			}
			else
			{
				position += Vector3.up * 0.17f;
				Vector3 direction = position - transform.position;
				direction = direction.normalized;
				position -= direction * Vector3.Distance(_head.position, _roboticMouthSlot.transform.position);
			}
			
			_adjustedTargetLocalPosition = _rotatingBase.InverseTransformPoint(position);
			_adjustedTargetLocalPosition.x = 0;
		}

		#endregion
		
		#region ANIMATE AND CONTROL ARM
		
		private Tweener RotateBase(Vector3 point, float time)
		{
			Vector3 relativePos = point - transform.position;
			Quaternion rot = Quaternion.Euler(0, Quaternion.LookRotation(relativePos, Vector3.up).eulerAngles.y, 0);
			
			_rotationTweener?.Kill();
			_rotationTweener = _rotatingBase.DORotateQuaternion(rot, time);
			_rotationTweener.SetEase(Ease.InOutQuad);

			return _rotationTweener;
		}

		private Tweener BendArm(float time)
		{
			_bendArmTweener?.Kill();
			_bendArmTweener.SetEase(Ease.InOutQuad);
			_bendArmTweener = _ikTarget.DOLocalMove(_adjustedTargetLocalPosition, time);
			return _bendArmTweener;
		}

		private Tweener AdjustHead(Vector3 point, float time)
		{
			Quaternion rotForward = Quaternion.LookRotation(_rotatingBase.forward, Vector3.up);
			Quaternion rotDown = Quaternion.LookRotation(-Vector3.up, _rotatingBase.forward);
			Quaternion newRot = _faceDown ? rotDown : rotForward;
			
			_adjustHeadTweener?.Kill();
			_adjustHeadTweener.SetEase(Ease.InOutQuad);
			_adjustHeadTweener = _head.DORotateQuaternion(newRot, time);

			return _adjustHeadTweener;
		}

		private Tweener Pinch(bool open, float time)
		{
			float gap = open ? _pincherOpenedGap : _pincherClosedGap;
			
			_pinchLeftTweener?.Kill();
			_pinchRightTweener?.Kill();

			_pinchLeftTweener = _pincherLeftTarget.DOLocalMoveX(-gap/2, time);
			_pinchRightTweener = _pincherRightTarget.DOLocalMoveX(gap/2, time);

			return _pinchLeftTweener;
		}
		
		private void DropObject()
		{
			if (_mouthFull)
			{
				_roboticMouthSlot.Release();
			}
		}

		private void CaptureObject()
		{
			if (!_mouthFull)
			{
				if (_currentTarget)
				{
					_roboticMouthSlot.Capture(_currentTarget);
				}
				else
				{
					_roboticMouthSlot.Capture();
				}
			}
		}

		#endregion
		
		#region CHECK ABORT

		private void CheckAbort()
		{
			if (!_currentTarget) return; 
			
			float distance = Vector3.Distance(_currentTarget.transform.position, _aimPosition);
			bool targetMoving = _currentTarget.Rb.velocity.magnitude > 0.05f;
			
			if ((!_mouthFull && distance > 0.01f) || targetMoving)
			{
				Abort();
				GetNextTarget();
			}
		}
		
		private void Abort()
		{
			Debug.Log("ABORT");

			_currentTarget = null;
			
			// kill all the tweeners
			_rotationTweener?.Kill();
			_bendArmTweener?.Kill();
			_adjustHeadTweener?.Kill();
			_pinchLeftTweener?.Kill();
			_pinchRightTweener?.Kill();
			
			Pinch(true, _pinchTime / _speedMultiplier);
			DropObject();
			_mouthFull = false;
			_isMoving = false;
		}

		#endregion

		#region DEBUG

		private void InitializeDebugState()
		{
			Observable.Interval(TimeSpan.FromSeconds(0.2f)).Subscribe(t =>
			{
				if (_showDebugState)
				{
					UpdateDebugState();
				}
					
				_debugContainer.gameObject.SetActive(_showDebugState);
			});
		}
		
		private void UpdateDebugState()
		{
			// Used for visual debugging
			
			StringBuilder str = new StringBuilder();
			str.Append("TARGETS: ").Append(_targets.Count).Append("\n").Append("\n");
			str.Append("CURRENT: ").Append(_currentTarget ? _currentTarget.name : "NONE").Append("\n");
			str.Append("TARGET POSITION: ").Append(_currentTarget ? _currentTarget.transform.position.ToString() : "-").Append("\n");
			str.Append("AIM POSITION: ").Append(_aimPosition).Append("\n");
			str.Append("CAPTURED: ").Append(_roboticMouthSlot.CapturedObject ? _roboticMouthSlot.CapturedObject.name : "NONE").Append("\n");
			str.Append("---------").Append("\n");
			str.Append("MANUAL INPUT: ").Append(_manualInput.ToString()).Append("\n");

			_textMeshPro.text = str.ToString();
		}

		#endregion

		private void InitializePincher()
		{
			// ignore colliders of pinchers to prevent glitches and blocked situations
			List<Collider> _collidersLeft = _pincherLeft.GetComponentsInChildren<Collider>().ToList();
			List<Collider> _collidersRight = _pincherRight.GetComponentsInChildren<Collider>().ToList();
			
			_collidersLeft.ForEach(c =>
			{
				_collidersRight.ForEach(d =>
				{
					Physics.IgnoreCollision(c,d,true);
				});
			});
		}
		
		public void DrawLaser(bool draw)
		{
			if (!draw)
			{
				_laser.gameObject.SetActive(false);
				return;
			}
			
			_laser.gameObject.SetActive(true);

			RaycastHit hit;
			bool didHit = Physics.Raycast(_laserStart.position, _laserStart.forward, out hit, 5, _layerMaskLaser.value);

			if (didHit)
			{
				_laser.SetPosition(0, _laserStart.position);
				_laser.SetPosition(1, hit.point);
			}
		}
		
		#region MANUAL CONTROLS
		
		public void SetManualMode(bool manual)
		{
			Manual.Value = manual;

			Debug.Log("SET MODE: " + (manual ? "MANUAL" : "AUTOMATIC")); 
			if (manual)
			{
				_checkCaptureZone?.Dispose();
				_checkRobotAutomaticMovement?.Dispose();
				Abort();
			}
			else
			{
				InitializeAutomaticMode();
			}
		}
		
		private void InitializeManualMode()
		{
			_robotControls.OnMove.Subscribe(vector =>
			{
				_manualInput = vector;
				float x = _manualTarget.localPosition.x - vector.x / 30;
				float y = Mathf.Clamp(_manualTarget.localPosition.y - vector.y/30, 0, 2);
				float z = _manualTarget.localPosition.z - vector.z / 30;

				Vector2 flatPos = Vector2.ClampMagnitude(new Vector2(x, z), _maxRadius);

				flatPos = flatPos.magnitude < _minRadius ? flatPos.normalized * _minRadius : flatPos;
				Vector3 newPos = new Vector3(flatPos.x, y, flatPos.y);
				
				_manualTarget.localPosition = newPos;

				HandleManualMovement();
			});
			
			_robotControls.OnTriggerDown.Subscribe(_ =>
			{
				Pinch(false, _pinchTime / _speedMultiplier);
				CaptureObject();
			});
			_robotControls.OnTriggerUp.Subscribe(_ =>
			{
				Pinch(true, _pinchTime / _speedMultiplier);
				DropObject();
			});

			_robotControls.OnUpdateTarget.Subscribe(position =>
			{
				_manualTarget.position = new Vector3(position.x, Math.Max(0,position.y), position.z);
				HandleManualMovement();
			});

			_robotControls.OnUpdatePincher.Subscribe(gap =>
			{
				float adjustedGap = Mathf.Clamp(gap, 0, _pincherOpenedGap) * 2f;

				if (adjustedGap < 0.05f)
				{
					CaptureObject();
				}

				if (adjustedGap > 0.1f)
				{
					DropObject();
				}
				
				Vector3 newLeftPos = new Vector3(-adjustedGap/2, 0, 0);
				Vector3 newRightPos = new Vector3(adjustedGap/2, 0, 0);

				_pincherLeftTarget.localPosition = Vector3.Lerp(_pincherLeftTarget.localPosition, newLeftPos, 0.1f);
				_pincherRightTarget.localPosition = Vector3.Lerp(_pincherRightTarget.localPosition, newRightPos, 0.1f);
			});
		}

		private void HandleManualMovement()
		{
			// cannot move in engineer mode
			//if (_viralSettings.EngineerModeActive.Value) return;
			
			// if controls touched => go to manual mode
			if (!Manual.Value)
			{
				SetManualMode(true);
			}
			
			_faceDown = true;
			GoManuallyToTarget(_manualTarget.position);
			DrawLaser(_faceDown);
		}
		
		private void GoManuallyToTarget(Vector3 point)
		{
			_aimPosition = point;
			Vector3 relativePos = point - transform.position;
			Quaternion rot = Quaternion.Euler(0, Quaternion.LookRotation(relativePos, Vector3.up).eulerAngles.y, 0);

			_rotatingBase.rotation = Quaternion.Slerp(_rotatingBase.rotation, rot, 0.05f);
			ComputeAdjustedTargetPosition(point);
			_ikTarget.localPosition = Vector3.Lerp(_ikTarget.localPosition,_adjustedTargetLocalPosition, 0.05f);
			
			Quaternion rotForward = Quaternion.LookRotation(_rotatingBase.forward, Vector3.up);
			Quaternion rotDown = Quaternion.LookRotation(-Vector3.up, _rotatingBase.forward);
			_head.rotation = _faceDown ? rotDown : rotForward;
		}

		#endregion

		public Vector3 GetTargetPosition()
		{
			if (_currentTarget)
			{
				return _currentTarget.transform.position;
			}
			else
			{
				return Vector3.zero;
			}
		}

		public Vector3 GetAimPosition()
		{
			return _aimPosition;
		}

		public Vector3 GetAimLocalPosition()
		{
			return transform.InverseTransformPoint(_aimPosition);
		}

		public Vector3 GetManualTargetLocalPosition()
		{
			return transform.InverseTransformPoint(_manualTarget.position);
		}
	}
}

