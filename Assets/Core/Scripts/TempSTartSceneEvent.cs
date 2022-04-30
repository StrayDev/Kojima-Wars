using Core;
using Unity.Netcode;
using UnityEngine;

public class TempSTartSceneEvent : NetworkBehaviour
{
    [SerializeField] private string m_sceneName = "TemplateTestScene";

    public void DoThing()
    {
        var coroutine = SceneLoader.NetworkLoadSceneCoroutine(m_sceneName);
        StartCoroutine(coroutine);
    }
}
