using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitMapUI : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private GameObject m_gameobjectHolder = default;
    [SerializeField] private Camera m_miniMapCamera = default;
    [SerializeField] private RectTransform m_miniMapImage = default;
    [SerializeField] private UnitBaseSelector m_baseSelectorPrefab = default;
    [SerializeField] private Image m_selectedIcon = default;
    [SerializeField] private RectTransform m_selectedIconTransform = default;

    [Header("Unit Info")]
    [SerializeField] private Transform m_unitInfoHolder = default;
    [SerializeField] private TextMeshProUGUI m_unitGroupText = default;
    [SerializeField] private UnitInfoUI m_infantryInfoUI = default;
    [SerializeField] private UnitInfoUI m_tankInfoUI = default;
    [SerializeField] private UnitInfoUI m_helicopterInfoUI = default;

    [SerializeField] private PlayerDataSO playerDataSO = default;

    [Header("Unit Buttons")]
    [SerializeField] private List<Button> m_unitGroupButtons = default;
    [SerializeField] private Color m_defaultColour = new Color(59, 65, 75, 255);
    [SerializeField] private Color m_highlightedColour = new Color(255, 204, 51, 255);

    private PauseManager m_pauseManager = null;
    private Entity m_playerController = null;

    private Dictionary<BaseController, UnitBaseSelector> m_baseSelectorMap = new Dictionary<BaseController, UnitBaseSelector>();

    private int m_selectedUnitGroup = -1;
    private bool m_isMapOpen = false;

    private void Awake()
    {
        m_gameobjectHolder.SetActive(false);
        m_selectedIcon.enabled = false;

        m_pauseManager = FindObjectOfType<PauseManager>();
    }

    private void OnEnable()
    {
        InputManager.MECH.UnitMap.performed += OnToggleMap;
    }

    private void OnDisable()
    {
        InputManager.MECH.UnitMap.performed -= OnToggleMap;
    }

    private void OnToggleMap(InputAction.CallbackContext context)
    {
        if (m_isMapOpen)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }

        m_isMapOpen = !m_isMapOpen;
    }

    private void OpenMap()
    {
        foreach (Button button in m_unitGroupButtons)
        {
            button.onClick.AddListener(() => OnUnitGroupSelected(m_unitGroupButtons.IndexOf(button), NetworkManager.Singleton.LocalClientId));
        }

        FindObjectOfType<PauseManager>().Pause(false);
        InputManager.MECH.UnitMap.Enable();
        m_pauseManager.SetCanPause(false);
        
        if (m_playerController == null)
        {
            foreach (MechCharacterController mech in MechCharacterController.List)
            {
                if (mech.OwnerClientId == mech.NetworkManager.LocalClientId)
                {
                    m_playerController = mech.GetComponent<Entity>();
                    SetupBaseLocations();
                    break;
                }
            }
        }

        foreach (KeyValuePair<BaseController, UnitBaseSelector> selector in m_baseSelectorMap)
        {
            selector.Value.SelectButton.interactable = false;
        }

        int count = 0;
        foreach(Button button in m_unitGroupButtons)
        {
            button.image.color = m_defaultColour;
            button.interactable = playerDataSO.GetAIGroup(NetworkManager.Singleton.LocalClientId, count).agents.Count > 0;
            count++;
        }

        m_selectedUnitGroup = -1;
        m_selectedIcon.enabled = false;

        CursorManager.EnableCursor("ai-ui");

        m_gameobjectHolder.SetActive(true);
        m_unitInfoHolder.gameObject.SetActive(false);
    }

    private void CloseMap()
    {
        foreach (Button button in m_unitGroupButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        CursorManager.DisableCursor("ai-ui");

        m_gameobjectHolder.SetActive(false);

        foreach (Button button in m_unitGroupButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        FindObjectOfType<PauseManager>().Unpause();
        m_pauseManager.SetCanPause(true);
    }

    private void Update()
    {
        if (m_gameobjectHolder.activeSelf)
        {
            foreach (KeyValuePair<BaseController, UnitBaseSelector> map in m_baseSelectorMap)
            {
                map.Value.UpdateDisplay(m_playerController, map.Key);
            }

            for (int i = 0; i < m_unitGroupButtons.Count; i++)
            {
                AIGroup aiGroup = playerDataSO.GetAIGroup(NetworkManager.Singleton.LocalClientId, i);
                m_unitGroupButtons[i].interactable = aiGroup.agents.Count != 0;

                if (aiGroup.agents.Count == 0 && m_selectedUnitGroup == i)
                {
                    m_selectedUnitGroup = -1;
                    m_selectedIcon.enabled = false;
                    m_unitGroupButtons[i].image.color = m_defaultColour;
                }
            }
        }
    }

    private void SetupBaseLocations()
    {
        BaseController[] baseControllers = FindObjectsOfType<BaseController>();
        m_baseSelectorMap = new Dictionary<BaseController, UnitBaseSelector>();

        foreach (BaseController controller in baseControllers)
        {
            var cameraSize = new Vector2(m_miniMapCamera.pixelWidth, m_miniMapCamera.pixelHeight);
            var imageSize = new Vector2(m_miniMapImage.rect.width, m_miniMapImage.rect.height);

            Vector3 screenPos = m_miniMapCamera.WorldToScreenPoint(controller.transform.position);
            screenPos.x -= 35;
            screenPos.y += 90;

            UnitBaseSelector selector = Instantiate(m_baseSelectorPrefab, m_miniMapImage);
            selector.SelectButton.onClick.RemoveAllListeners();
            selector.GetComponent<RectTransform>().anchoredPosition = screenPos;
            selector.UpdateDisplay(m_playerController.GetComponent<Entity>(), controller);
            selector.SelectButton.onClick.AddListener(() => OnSelectBase(controller, NetworkManager.Singleton.LocalClientId));

            m_baseSelectorMap.Add(controller, selector);
        }
    }

    private void OnSelectBase(BaseController controller, ulong localPlayerId)
    {
        int baseId = controller.GetBaseId();

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SelectBaseAIMapServerRpc(localPlayerId, baseId, m_selectedUnitGroup);
        Debug.Log("TEST");
        OnUnitGroupSelected(m_selectedUnitGroup, localPlayerId);
    }

    private void OnUnitGroupSelected(int index, ulong localPlayerId)
    {
        if (playerDataSO.GetAIGroup(localPlayerId, index).agents.Count > 0)
        {
            m_selectedUnitGroup = index;

            int count = 0;
            foreach (Button button in m_unitGroupButtons)
            {
                button.image.color = count == index ? m_highlightedColour : m_defaultColour;
                count++;
            }

            SetUnitInfoUI(index);
            m_unitInfoHolder.gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SelectAIGroupServerRpc(localPlayerId, index);

            foreach (KeyValuePair<BaseController, UnitBaseSelector> selector in m_baseSelectorMap)
            {
                selector.Value.SelectButton.interactable = playerDataSO.GetAIGroup(localPlayerId, index).agents.Count > 0;
            }
        }
        else
        {
            foreach (KeyValuePair<BaseController, UnitBaseSelector> selector in m_baseSelectorMap)
            {
                selector.Value.SelectButton.interactable = false;
            }

            m_selectedIcon.enabled = false;
        }
    }

    public void SetUnitInfoUI(int index)
    {
        var numUnits = new Dictionary<EUnitTypes, int>() { { EUnitTypes.INFANTRY, 0}, { EUnitTypes.TANK, 0}, {EUnitTypes.HELICOPTER, 0} };
        foreach (GameObject unit in playerDataSO.GetAIGroup(NetworkManager.Singleton.LocalClientId, index).agents)
        {
            EUnitTypes type = unit.GetComponent<AI_AgentController>().UnitType;

            if (numUnits.ContainsKey(type))
            {
                numUnits[type] += 1;
            }
            else
            {
                numUnits.Add(type, 1);
            }
        }

        m_infantryInfoUI.gameObject.SetActive(numUnits[EUnitTypes.INFANTRY] != 0);
        m_infantryInfoUI.UpdateQuantity(numUnits[EUnitTypes.INFANTRY]);

        m_tankInfoUI.gameObject.SetActive(numUnits[EUnitTypes.TANK] != 0);
        m_tankInfoUI.UpdateQuantity(numUnits[EUnitTypes.TANK]);

        m_helicopterInfoUI.gameObject.SetActive(numUnits[EUnitTypes.HELICOPTER] != 0);
        m_infantryInfoUI.UpdateQuantity(numUnits[EUnitTypes.HELICOPTER]);
    }

    public void SetBaseSelectionIcons(ulong localPlayerId, int baseId)
    {
        BaseController selectedBaseController = null;
        BaseController[] baseControllers = GameObject.FindObjectsOfType<BaseController>();
        foreach(BaseController baseController in baseControllers)
        {
            if(baseId == baseController.GetBaseId())
            {
                selectedBaseController = baseController;
                break;
            }
        }

        m_baseSelectorMap[selectedBaseController].SelectButton.interactable = false;
        
        m_selectedIcon.transform.parent = m_baseSelectorMap[selectedBaseController].transform;
        m_selectedIconTransform.anchoredPosition = new Vector3(0, 0, 50);
        m_selectedIcon.enabled = true;
    }
}