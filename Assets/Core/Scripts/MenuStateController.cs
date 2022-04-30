using System;
using UnityEngine;

namespace Core
{
    public enum MenuState
    {
        Main,
        Play,
        Settings,
        Credits
    }

    public class MenuStateController : MonoBehaviour
    {
        [SerializeField] private MenuReferenceContainer container = default;

        private CanvasGroup _currentGroup = default; 
       
        private void Awake()
        {
            UIHelper.SetGroupActive(container.playGroup, false);
            UIHelper.SetGroupActive(container.settingsGroup, false);
            UIHelper.SetGroupActive(container.creditsGroup, false);
        }
        
        private void SetMainButtonsActive(bool value)
        {
            container.playGameButton.interactable = value;
            container.settingsButton.interactable = value;
            container.creditsButton.interactable = value;
            container.quitGameButton.interactable = value;
        }
        
        public void OnPlayButtonPressed()
        {
            SetMainButtonsActive(false);
            _currentGroup = container.playGroup;
            
            UIHelper.SetGroupActiveFade(_currentGroup, true, .25f, this);
        }
        
        public void OnSettingsButtonPressed()
        {
            SetMainButtonsActive(false);
            _currentGroup = container.settingsGroup;
            UIHelper.SetGroupActiveFade(_currentGroup, true, .25f, this);
        }
        
        public void OnCreditsButtonPressed()
        {
            SetMainButtonsActive(false);
            _currentGroup = container.creditsGroup;
            container.creditScrollRect.content.anchoredPosition = new Vector2(0,0);
            UIHelper.SetGroupActiveFade(_currentGroup, true, .25f, this);
        }
        
        public void OnBackButtonPressed()
        {
            SetMainButtonsActive(true);
            UIHelper.SetGroupActiveFade(_currentGroup, false, .25f, this);
            _currentGroup = null;
        }
        
    }
}