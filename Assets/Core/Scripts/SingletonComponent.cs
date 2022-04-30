using UnityEngine;

namespace Core
{
    public abstract class SingletonComponent<T> : MonoBehaviour where T : Component
    {
        private static T _singleton;

        public static T Singleton => _singleton;

        private void Awake()
        {
            if (_singleton == null)
            {
                _singleton = this as T;
                return;
            }
           
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_singleton == this) _singleton = null;
        }
    }
}
