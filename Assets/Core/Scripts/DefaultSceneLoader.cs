// Stephen Rayment 11/02/2022

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        
        private const string RunDefaultScene = "Debug/Default Scene On Play";
        private const string DisableWarnings = "Debug/Disable Warnings";
        
        private static bool _isActive;
        private static bool _isWarningActive;
        
        private const string Warning = "Debug / Default Scene On Play, is currently active";

        static DefaultSceneLoader()
        {
            _isActive = EditorPrefs.GetBool(RunDefaultScene, false);
            _isWarningActive = EditorPrefs.GetBool(DisableWarnings, false);
            
            EditorApplication.delayCall += () =>
            {
                if (!_isActive || !Application.isPlaying) return;

                if (SceneManager.GetActiveScene().buildIndex == 0) return;
                
                if(!_isWarningActive) UnityEngine.Debug.LogWarning(Warning);
                
                SceneManager.LoadScene(0);
            };
        }
        
        [MenuItem(RunDefaultScene)]
        private static void Toggle()
        {
            _isActive = !_isActive;
            SetActive(RunDefaultScene, _isActive);
        }
        
        [MenuItem(DisableWarnings)]
        private static void ToggleWarning()
        {
            _isWarningActive = !_isWarningActive;
            SetActive(DisableWarnings, _isWarningActive);
        }

        private static void SetActive(string name, bool active)
        {
            Menu.SetChecked(name, active);
            EditorPrefs.SetBool(name, active);
        }
    }
    
} // CORE

#endif // UNITY_Editor
