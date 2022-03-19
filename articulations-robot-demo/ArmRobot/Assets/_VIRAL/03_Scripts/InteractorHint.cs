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
	public class InteractorHint : MonoBehaviour
	{
		private HologramUiComponent _hintedComponent;
		public HologramUiComponent HintedComponent => _hintedComponent;

		public void SetComponent(HologramUiComponent component)
		{
            bool notNullAndDifferent = false;

            if (_hintedComponent && component)
            {
                notNullAndDifferent = _hintedComponent.GetInstanceID() != component.GetInstanceID();
            }

            if (!_hintedComponent || !component || notNullAndDifferent)
			{
				if (_hintedComponent)
				{
					_hintedComponent.Focus(false);
				}

				if (component)
				{
                    component.Focus(true);
				}

                _hintedComponent = component;
            }
		}
    }
}