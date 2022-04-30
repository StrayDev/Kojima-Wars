using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UWELogoAnimation : MonoBehaviour
    {

        [SerializeField] private float duration = .5f;
        [SerializeField] private CanvasGroup image = default; 
        
        private Vector3 _visiblePosition = default;
        private Vector3 HiddenPosition => _visiblePosition + new Vector3(0, -250, 0);

        private void Awake()
        {
            _visiblePosition = transform.position;
        }

        public void HideLogo(bool value)
        {
            StartCoroutine(MoveLogoCoroutine(value));
        }
        
        private IEnumerator MoveLogoCoroutine(bool value)
        {
            var elapsedTime = 0f;
            var targetPos = value ? HiddenPosition : _visiblePosition;
            var targetAlpha = value ? 0 : 1;
            var current = transform.position;
            
            while (elapsedTime < duration)
            {
                transform.position = Vector3.Slerp(current, targetPos, elapsedTime / duration);
                image.alpha = Mathf.InverseLerp(image.alpha, targetAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            image.alpha = targetAlpha;
            transform.position = targetPos;
        }
    }
}
