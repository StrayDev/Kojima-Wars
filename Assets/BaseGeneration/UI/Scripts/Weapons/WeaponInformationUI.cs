using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInformationUI : MonoBehaviour
{
    [SerializeField] private Image m_weaponImage;
    [SerializeField] private TextMeshProUGUI m_remainingAmmoText;
    [SerializeField] private TextMeshProUGUI m_currentAmmoText;
    [SerializeField] private TextMeshProUGUI m_reloadText;
    [SerializeField] private GameObject m_reloadObject;

    public WeaponStats stats;
    public int magSize;
    private bool blink;
    public Color white = Color.white;
    public Color yellow = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        //magSize = stats.magazineSize;
        // Delaying the start function to allow references to be assigned properly
        // this is a bit of a weird way to do it but you will have reference errors
        // if this is removed.
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // this seems to be the sweet spot for things to catch up
        yield return new WaitForSeconds(0.1f);
        
        // assign to weapon events.
        WeaponScript.OnCurrentBulletsChanged += OnCurrentBulletsChanged;
        WeaponScript.OnWeaponAssigned += OnWeaponAssigned;
        WeaponScript.OnCurrentMagazineChanged += OnCurrentMagazineChanged;
    }

    
    
    private void OnDisable()
    {
        WeaponScript.OnCurrentBulletsChanged -= OnCurrentBulletsChanged; 
    }

    private void OnCurrentBulletsChanged(int amount)
    {
        m_currentAmmoText.text = amount.ToString();

        if(amount <= (magSize/6) && amount !=0)
        {
            m_reloadText.text = "RELOAD";
            m_reloadObject.SetActive(true);
            blink = true;
        }
        else
        {
            m_reloadText.text = " ";
            m_reloadObject.SetActive(false);
            blink = false;
        }
    }
    private void OnCurrentMagazineChanged(int fullMagSize)
    {
        magSize = fullMagSize;
    }

    private void OnWeaponAssigned(Sprite image)
    {
        m_weaponImage.sprite = image;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(blink)
        {
            m_reloadText.color = Lerp(white, yellow, 6);
        }
    }
    public Color Lerp(Color firstColor, Color secondColor, float speed)
    {
        return Color.Lerp(firstColor, secondColor, Mathf.Sin(Time.time * speed));
    }
}
