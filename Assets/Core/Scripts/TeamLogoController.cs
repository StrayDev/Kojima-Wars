using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamLogoController : MonoBehaviour
{

    [SerializeField] private TeamInfoSO teamInformation;
    [SerializeField] private int id;
    [SerializeField] private TMP_Text title;
    [SerializeField] private Image image;
    [SerializeField] private Color iconColour;

    [SerializeField] private bool isPlaywest = false;
    [SerializeField] private Camera playwestCamera; 

    private int seed;
    //private bool shaking = false;

    void start() {
        if (isPlaywest) {
            CreateTeamLogo(-1,1);
        }
    }
 
    public void CreateTeamLogo(int _id, int _seed)
    {
        if (!isPlaywest) {
            id = _id;
            (string, Sprite) data = teamInformation.getNameAndLogo(_id);
            title.text = data.Item1;
            image.sprite = data.Item2;
            seed = _seed; 
        } else {
            title.text = "PlayWest";
        }
    }

    public void hitGround() {
        StartCoroutine(revealNameAndColor(isPlaywest));
    }

    public void jitter() {
        //Debug.Log(gameObject.name);
        StartCoroutine(doJitter(2.5f));
    }

    private IEnumerator doJitter(float _intensity) {
        float duration = 0.35f;
        float e_time = 0f;
        while (e_time < duration) {
            Vector3 new_pos = gameObject.transform.localPosition + (Random.insideUnitSphere * _intensity);
            //Debug.Log($"Test: {gameObject.name} {new_pos}");
            gameObject.transform.localPosition = new_pos;
            e_time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator revealNameAndColor(bool _just_name) {
        float duration = _just_name ? 1f : 2f;
        float e_time = 0f;
        Color current_colour = image.color;
        Color target_colour = current_colour;
        if (!_just_name) {
            target_colour = teamInformation.getUniqueRandomColourPerTeam(id,seed);
        }

        while(e_time < duration) {
            if (!_just_name) {
                Color new_colour = Color.Lerp(current_colour,target_colour,(e_time/duration));
                image.color = new_colour;
            }
            float new_alpha = Mathf.Lerp(0,1,(e_time/duration));
            title.alpha = new_alpha;
            e_time += Time.deltaTime;
            yield return null;
        }
        image.color = target_colour;
        title.alpha = 1;
    }
}
