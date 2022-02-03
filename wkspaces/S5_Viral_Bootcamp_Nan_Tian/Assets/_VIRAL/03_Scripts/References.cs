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
    public class References : MonoBehaviour
    {
        [SerializeField] private Transform _head;

        private Transform _centerEyeAnchor;

        private void Start()
        {
            _centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;
        }

        private void Update()
        {
            _head.position = new Vector3(_centerEyeAnchor.position.x, 0.001f, _centerEyeAnchor.position.z);
            _head.rotation = Quaternion.Euler(0, _centerEyeAnchor.rotation.eulerAngles.y, 0);
        }

        public Transform GetHead()
        {
            return _head;
        }
    }
}
