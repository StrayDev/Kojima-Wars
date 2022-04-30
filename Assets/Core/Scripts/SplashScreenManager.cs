using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class SplashScreenManager : MonoBehaviour
    {
        [SerializeField] private string nextScene = "MenuTemp";
        [SerializeField] private float seconds = 3.0f;
        [SerializeField] private List<GameObject> teamIcons;

        public bool skipped {get; private set;} = false;

        private System.Random _rnd;

        public void SkipCutscene() {
            skipped = true;
        }

        private void Start()
        {
            _rnd = new System.Random(Guid.NewGuid().GetHashCode());
            var logoSeed = Guid.NewGuid().GetHashCode();
            
            // shuffle
            var quant = teamIcons.Count;  
            while (quant > 1) 
            {  
                quant--;  
                var ind = _rnd.Next(quant + 1);  
                var value = teamIcons[ind];  
                teamIcons[ind] = teamIcons[quant];  
                teamIcons[quant] = value;  
            }

            for (var i = 0; i < teamIcons.Count; i++)
            {
                teamIcons[i].GetComponent<TeamLogoController>().CreateTeamLogo(i,logoSeed);
            }

            StartCoroutine(waitOrSkip(seconds));
        }

        private async void load() {
            if (!Application.isEditor || Application.isPlaying)
            {
                await SceneLoader.LoadSceneAsync(nextScene);
            }
        }

        private IEnumerator waitOrSkip(float _duration) {
            float e_time = 0f;
            while (e_time < _duration && !skipped) {
                e_time += Time.deltaTime;
                yield return null;
            }
            load();
        }
    }
}
