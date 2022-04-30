
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Core
{
    public interface ISceneTransition
    {
        public abstract Task TransitionIn();
        public abstract Task TransitionOut();
    }

    public class SceneTransition : NetworkBehaviour, ISceneTransition
    {
        [SerializeField] private float transitionTime = 1;

        [SerializeField] private Animator animator;

        [SerializeField] private string fadeInTrigger = "FadeIn";
        [SerializeField] private string fadeOutTrigger = "FadeOut";

        public async Task TransitionIn() => await TransitionAsync(fadeInTrigger);
        public async Task TransitionOut() => await TransitionAsync(fadeOutTrigger);

        private async Task TransitionAsync(string trigger)
        {
            animator.SetTrigger(trigger);
            await Task.Delay((int)(1000 * transitionTime));
        }

        public IEnumerator NetTransitionIn() => NetTransitionAsync(fadeInTrigger);
        public IEnumerator NetTransitionOut() => NetTransitionAsync(fadeOutTrigger);

        private IEnumerator NetTransitionAsync(string trigger)
        {
            TransitionClientRpc(trigger);
            yield return new WaitForSeconds(transitionTime);
        }

        [ClientRpc]
        private void TransitionClientRpc(string trigger)
        {
            animator.SetTrigger(trigger);
        }
    }
}