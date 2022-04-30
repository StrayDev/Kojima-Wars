using UnityEngine;

namespace Core
{
    public class QuitGameComponent : MonoBehaviour
    {
        public void ExitGame()
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().ExitGame();
        }

        public void QuitGame()
        {
            if (!Application.isEditor) Application.Quit(); 
            
            #if UNITY_EDITOR //
            UnityEditor.EditorApplication.isPlaying = false;
            #endif // UNITY_EDITOR
        }
    }
}