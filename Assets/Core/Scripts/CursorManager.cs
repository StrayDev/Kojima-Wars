using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private static List<string> m_visibleActivations = new List<string>();
    private static List<string> m_confineVisibleActivations = new List<string>();
    private static List<string> m_confineInvisibleActivations = new List<string>();

    private static GameObject _virtualCursor;

    public static void SetVirtualCursor(GameObject _cursor)
    {
        _virtualCursor = _cursor;
    }

    public static void EnableCursor(string systemID, bool confine = false)
    {
        if (!m_visibleActivations.Contains(systemID))
        {
            m_visibleActivations.Add(systemID);
        }

        if (m_confineInvisibleActivations.Contains(systemID))
        {
            m_confineInvisibleActivations.Remove(systemID);
        }

        if (confine && !m_confineVisibleActivations.Contains(systemID))
        {
            m_confineVisibleActivations.Add(systemID);
        }

        UpdateCursor();
    }

    public static void DisableCursor(string systemID, bool lockToCenter = true)
    {
        if (m_visibleActivations.Contains(systemID))
        {
            m_visibleActivations.Remove(systemID);
        }

        if (m_confineVisibleActivations.Contains(systemID))
        {
            m_confineVisibleActivations.Remove(systemID);
        }

        if (lockToCenter && m_confineInvisibleActivations.Contains(systemID))
        {
            m_confineInvisibleActivations.Remove(systemID);
        }

        UpdateCursor();
    }

    public static void ConfineCursor(string systemID, bool visible, bool confine)
    {
        if (confine)
        {
            if (visible)
            {
                if (!m_confineVisibleActivations.Contains(systemID))
                {
                    m_confineVisibleActivations.Add(systemID);
                }
            }
            else
            {
                if (!m_confineInvisibleActivations.Contains(systemID))
                {
                    m_confineInvisibleActivations.Add(systemID);
                }
            }
        }
        else
        {
            if (visible)
            {
                if (m_confineVisibleActivations.Contains(systemID))
                {
                    m_confineVisibleActivations.Remove(systemID);
                }
            }
            else
            {
                if (m_confineInvisibleActivations.Contains(systemID))
                {
                    m_confineInvisibleActivations.Remove(systemID);
                }
            }
        }

        UpdateCursor();
    }

    private static void UpdateCursor()
    {
        bool visible = m_visibleActivations.Count >= 1;
        Cursor.visible = visible;
        if(_virtualCursor != null) _virtualCursor.SetActive(visible);

        if (visible)
        {
            Cursor.lockState = m_confineVisibleActivations.Count >= 1 ? CursorLockMode.Confined : CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = m_confineInvisibleActivations.Count >= 1 ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        EnableCursor("main-menu");
        UpdateCursor();
    }

    private void OnApplicationFocus(bool focus)
    {
        UpdateCursor();
    }
}