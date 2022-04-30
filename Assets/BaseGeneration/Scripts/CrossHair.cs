using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class CrossHair : NetworkBehaviour
{
    [Header("Crosshair Panels")]
    [SerializeField] private List<RectTransform> m_crosshairRects = new List<RectTransform>();
    [SerializeField] private List<GameObject> m_crosshairObjects = new List<GameObject>();
    [SerializeField] private GameObject m_dotPanel;
    
    [Header("Crosshair Settings")]
    public float idleSize = 50f;
    public float shootingSize = 100f;
    public float currentSize = 50f;
    public float speed = 10f;
    
    [Header("Hitmarker Panels")]
    [SerializeField] private RectTransform m_hitmarkerRect;
    [SerializeField] private GameObject m_hitmarkerPanel;
    [SerializeField] private CanvasGroup m_hitmarkerCanvasGroup;
    
    [Header("Hitmarker Settings")]
    [SerializeField] private float m_hitMarkerInitialSize = 30;
    [SerializeField] private float m_hitMarkerMaxSize = 50f;
    [SerializeField] private float m_hitMarkerSpeed = 5f;
    [SerializeField] private float m_hitMarkerViewTime = 0.3f;
    
    private int m_currentWeapon;
    private float m_hitMarkerView;
    private float m_hitMarkerCurrentSize;
    
    // do we need to store this in the crosshair?
    public ulong personalID;
    public ulong clientID;
    
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(DelayedStart());
        m_hitMarkerCurrentSize = m_hitMarkerInitialSize;
        m_hitMarkerView = 0;
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);
        WeaponScript.OnCurrentWeaponUsed += OnCurrentWeapon;
    }

    private void OnDisable()
    {
        WeaponScript.OnCurrentWeaponUsed -= OnCurrentWeapon;
    }

    // Update is called once per frame
    private void Update()
    {
        if (InputManager.MECH.FireWeapon.IsPressed())
        {
            currentSize = Mathf.Lerp(currentSize, shootingSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, idleSize, Time.deltaTime * speed);
        }

        m_crosshairRects[m_currentWeapon].sizeDelta = new Vector2(currentSize, currentSize);

        if (InputManager.MECH.ADS.IsPressed())
        {
            m_crosshairObjects[m_currentWeapon].SetActive(false);
        }
        else
        {
            m_crosshairObjects[m_currentWeapon].SetActive(true);
        }
    }
    
    public void SetHitmarker()
    {
        // need to check if there is something already playing?
        // if there is we need to reset hit marker then play from the original
        
        // this doesn't need to be a networked object but this is the only way i can think to get this to work at the moment.
        if (clientID == NetworkManager.LocalClient.ClientId)
        {
            StartCoroutine(FadeHitMarker(m_hitMarkerInitialSize, m_hitMarkerMaxSize, m_hitMarkerSpeed));
        }
    }
    
    private IEnumerator FadeHitMarker(float start, float end, float lerpTime)
    {
        
        m_hitmarkerPanel.SetActive(true);
        
        float timeStartedLerping = Time.time;
        float timeSinceStart = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStart / lerpTime;

        while (true)
        {
            timeSinceStart = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStart / lerpTime;
            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            float alphaValue = Mathf.Lerp(0, 2, percentageComplete);

            m_hitmarkerCanvasGroup.alpha = alphaValue;
            m_hitmarkerRect.sizeDelta = new Vector2(currentValue, currentValue);

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }

        m_hitmarkerPanel.SetActive(false);
        m_hitmarkerCanvasGroup.alpha = 0;
    }


    private void OnCurrentWeapon(string weapon)
    {
        // can we add these to the weapons scriptable objets and pass that as a reference? then we can spawn the correct
        // crosshair panel from there and assign all of things? if the weapon names are ever changed this will always fail.
        
        if(weapon == "Assault Rifle")
        {
            m_currentWeapon = 0;
            m_crosshairObjects[0].SetActive(true);
        }
        else
        {
            m_crosshairObjects[0].SetActive(false);
        }
        if (weapon == "BurstAR")
        {
            m_currentWeapon = 1;

            m_crosshairObjects[1].SetActive(true);
        }
        else
        {
            m_crosshairObjects[1].SetActive(false);
        }
        if (weapon == "Shotgun")
        {
            m_currentWeapon = 2;

            m_crosshairObjects[2].SetActive(true);
        }
        else
        {
            m_crosshairObjects[2].SetActive(false);
        }
        if (weapon == "SniperRifle")
        {
            m_currentWeapon = 3;

            m_crosshairObjects[3].SetActive(true);
        }
        else
        {
            m_crosshairObjects[3].SetActive(false);
        }
        if (weapon == "Pistol")
        {
            m_currentWeapon = 4;

            m_crosshairObjects[4].SetActive(true);
        }
        else
        {
            m_crosshairObjects[4].SetActive(false);
        }

    }
}