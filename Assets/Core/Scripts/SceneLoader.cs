using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Core
{
    public class SceneLoader : SingletonComponent<SceneLoader>
    {
        [SerializeField] private SceneTransition _transition = default;
        
        private static AsyncOperation _loadingAsyncOperation = null;
         
        private void Start()
        {
            // move to scene and hide flags
            DontDestroyOnLoad(gameObject);
        }
        
        public static async Task LoadSceneAsync(string sceneName) => await LoadSceneAsync(sceneName, LoadSceneMode.Single);
        public static async Task LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            // wait for animation to complete
            if (Singleton._transition != null)
            {
                await Singleton._transition.TransitionOut();
            }

            // start loading the scene
            _loadingAsyncOperation = SceneManager.LoadSceneAsync(sceneName);

            // dont show the scene automatically
            _loadingAsyncOperation.allowSceneActivation = false;

            // wait for load
            while (_loadingAsyncOperation.progress < .9f)
            {
                await Task.Delay(10);
            }
            
            // enable the scene
            _loadingAsyncOperation.allowSceneActivation = true;
            _loadingAsyncOperation = null;
            
            // fade animation back in
            if (Singleton._transition != null)
            {
                await Singleton._transition.TransitionIn();
            }
        }

        public static IEnumerator NetworkLoadSceneCoroutine(string sceneName) => NetworkLoadSceneCoroutine(sceneName, LoadSceneMode.Single);
        public static IEnumerator NetworkLoadSceneCoroutine(string sceneName, LoadSceneMode loadSceneMode)
        {
            // wait for animation to complete
            if (Singleton._transition != null)
            {
                yield return Singleton._transition.NetTransitionOut();
            }

            // start loading the scene
            var networkSceneManager = NetworkManager.Singleton.SceneManager;
            var sceneEventProgress = networkSceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            
            // wait for 
            while (sceneEventProgress == SceneEventProgressStatus.SceneNotLoaded)
            {
                // delay while scene is loading
                Debug.Log("Loading");
                yield return new WaitForEndOfFrame();
            }
            
            // fade animation back in
            if (Singleton._transition != null)
            {
                yield return Singleton._transition.NetTransitionIn();
            }
        }
    }
}
