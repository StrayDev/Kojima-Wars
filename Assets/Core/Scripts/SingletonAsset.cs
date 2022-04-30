using UnityEngine;

namespace Core
{
    public abstract class SingletonAsset<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _singleton = null;

        public static T Singleton => _singleton != null ? _singleton : CreateNewSingleton();

        private static T CreateNewSingleton()
        {
            var results = Resources.FindObjectsOfTypeAll<T>();
            Debug.Log(results.Length);
            if (results.Length < 1)
            {
                UnityEngine.Debug.LogError($"SingletonAsset of type { typeof(T) } does not exist");
                // todo create asset in Core/Resources
                return null;
            }

            _singleton = results[0];
            return _singleton;
        }
    }
}
