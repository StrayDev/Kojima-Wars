using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseController : NetworkBehaviour
{
    [SerializeField] int BaseId;

    public event Action<EBaseState> OnStateChanged;
    public event Action<string> OnOwnerChanged;

    public BaseCaptureZone CaptureZone => m_baseCaptureZone;

    public BaseDefencesController DefencesController => m_baseDefencesController;

    public Transform PlayerStartPosition => m_playerStartingPositions[Random.Range(0, m_playerStartingPositions.Length)];

    public EBaseGenerationType GenerationType => m_generationType;

    public EBaseRank Rank => m_rank;

    public EBaseState State { get; private set; } = EBaseState.IDLE;

    public string TeamOwner { get; protected set; } = "";

    [Header("Script References")]
    [SerializeField] BaseCaptureZone m_baseCaptureZone = default;
    [SerializeField] BaseDefencesController m_baseDefencesController = default;
    [SerializeField] Transform[] m_playerStartingPositions = default;
    public DefenceLocationManager m_defenceLocationManager = default;

    [Header("Base Values")]
    [SerializeField] private EBaseGenerationType m_generationType = EBaseGenerationType.CURRENCY;
    [SerializeField] private EBaseRank m_rank = EBaseRank.MEDIUM;

    [Tooltip("The amount of score this base generates.")]
    [SerializeField] private int m_scoreAmount;

    [Tooltip("The amount of time between each new increase in the owners score. E.g. increase every 5 seconds.")]
    [SerializeField] private float m_scoreDelay;

    private float m_scoreTimer = 0;
    private GameController m_gameController = default;

    public int GetBaseId()
    {
        return BaseId;
    }

    public void ChangeState(EBaseState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(State);
    }

    public void ChangeTeamOwner(string newTeamOwnerName, bool invokeEvent = true)
    {
        TeamOwner = newTeamOwnerName;
        m_scoreTimer = 0;

        if (invokeEvent)
        {
            OnOwnerChanged?.Invoke(TeamOwner);
        }

        m_baseDefencesController.DestroyAllDefences();
    }

    protected virtual void Awake()
    {
        m_gameController = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        if (IsServer && m_gameController.GameTimer.IsCounting && TeamOwner != "")
        {
            m_scoreTimer += Time.deltaTime;

            if (m_scoreTimer >= m_scoreDelay)
            {
                m_scoreTimer = 0;
                m_gameController.AddScoreToTeamServerRpc(TeamOwner, m_scoreAmount);
            }
        }
    }
}