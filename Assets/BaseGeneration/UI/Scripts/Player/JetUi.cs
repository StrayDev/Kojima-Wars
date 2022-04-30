using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JetUi : MonoBehaviour
{

    public JetShoot jetShoot;
    public VTOLCharacterController vtol;

    public GameObject holder;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI altText;

    public Slider slider;

    private void Start()
    {
        holder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
