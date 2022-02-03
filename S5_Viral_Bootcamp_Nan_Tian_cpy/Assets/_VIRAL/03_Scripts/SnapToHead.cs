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

using UnityEngine;

namespace _VIRAL._03_Scripts
{
    public class SnapToHead : MonoBehaviour
    {
        private References _references;

        [Space]
        [SerializeField] private Transform _translocator;
    
        [SerializeField] private float _lerpSpeedPosition = 0.05f;
        [SerializeField] private float _lerpSpeedRotation = 0.1f;
    
        [SerializeField] private float _lerpSpeedDistance = 0.01f;

        private float _distance;
    
        private void Awake()
        {
            _references = FindObjectOfType<References>();
        }

        public void SetDistance(float distance)
        {
            _distance = distance;
        }
    
        private void Update()
        {
            var currentPos = transform.position;
            var newPos = _references.GetHead().position;
            newPos = Vector3.Lerp(currentPos, new Vector3(newPos.x, currentPos.y, newPos.z), _lerpSpeedPosition);
        
            Quaternion newRot = Quaternion.Euler(0, _references.GetHead().rotation.eulerAngles.y, 0);
            newRot = Quaternion.Lerp(transform.rotation, newRot, _lerpSpeedRotation);
        
            transform.SetPositionAndRotation(newPos, newRot);
        
            Vector3 pos = _translocator.localPosition;
            _translocator.localPosition = Vector3.Lerp(pos,new Vector3(0, 0, _distance), _lerpSpeedDistance);
        }
    }
}
