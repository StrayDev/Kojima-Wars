using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IconsTransition : MonoBehaviour
{
    public GameObject mechIcon;
    public GameObject vtolicon;

    public JetShoot jetShoot;
    public VTOLCharacterController vtol;

    public GameObject holder;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI altText;

    public Slider slider;

    void Start()
    {
        mechIcon.SetActive(true);
        vtolicon.SetActive(false);

        holder.SetActive(false);
      
    }

    void Update()
    {
        if(vtol!= null && jetShoot!= null)
        { 
        if (InputManager.MECH.Transform.triggered)
        {
            mechIcon.SetActive(false);
            vtolicon.SetActive(true);
            holder.SetActive(true);
        }
        if (InputManager.VTOL.Transform.triggered || vtol.collided)
        {
            vtolicon.SetActive(false);
            mechIcon.SetActive(true);
            holder.SetActive(false);
            vtol.collided = false;
        }
            if (holder.activeSelf)
            {
                if (vtol.rb.velocity.magnitude > 95)
                {
                    speedText.text = "SPD: " + 100;
                }
                else
                {
                    speedText.text = "SPD: " + Mathf.RoundToInt(vtol.rb.velocity.magnitude).ToString();
                }

                altText.text = "ALT: " + Mathf.RoundToInt(vtol.altitudRef).ToString();
                slider.value = jetShoot.overheatTime;
            }
        }
    }
}
