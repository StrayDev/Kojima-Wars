using System.Collections;
using TMPro;
using UnityEngine;

namespace Core
{
    public static class UIHelper
    {
        public static void SetGroupActive(CanvasGroup group, bool value)
        {
            group.alpha = value ? 1 : 0;
            group.interactable = value;
            group.blocksRaycasts = value;
        }
        
        public static void SetGroupActiveFade(CanvasGroup group, bool value, float duration, MonoBehaviour mono)
        {
            var co = SetGroupActiveFadeCoroutine(group, value, duration);
            mono.StartCoroutine(co);
        }
        
        private static bool _inTransition = false;

        private static IEnumerator SetGroupActiveFadeCoroutine(CanvasGroup group, bool value, float duration)
        {
        if (_inTransition) yield return null;

            _inTransition = true;
            
            group.interactable = value;
            group.blocksRaycasts = value;
            
            var elapsedTime = 0f;
            var targetAlpha = value ? 1 : 0;

            while (elapsedTime < duration)
            {
                group.alpha = Mathf.Lerp(group.alpha, targetAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            group.alpha = targetAlpha;

            _inTransition = false;
            yield return null;
        }

        public static void ResetInputField(TMP_InputField field)
        {
            field.text = "";
            field.Select();
            field.ActivateInputField();
        }
    }
}
