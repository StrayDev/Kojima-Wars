using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class MenuReferenceContainer : MonoBehaviour
    {
        [Header("Main Buttons")] 
        public Button playGameButton = default;
        public Button settingsButton = default;
        public Button creditsButton = default;
        public Button quitGameButton = default;

        [Header("Main Canvas Groups")] 
        public CanvasGroup playGroup = default;
        public CanvasGroup settingsGroup = default;
        public CanvasGroup creditsGroup = default;

        [Header("Canvis Fields")]
        public ScrollRect creditScrollRect = default;

        // Play Game Screen
        [Header("Lobby Canvas Groups")] 
        public CanvasGroup modeSelectGroup = default;
        public CanvasGroup joinGameGroup = default;
        public CanvasGroup inLobbyGroup = default;

        [Header("Lobby Fields")] 
        public TMP_InputField inputField = default;
        
        public Button submitButton = default;
        public Button hostGameButton = default;
        
        public TMP_Text joinCodeText = default;
        public TMP_Text errorMessageText = default;
    }
}