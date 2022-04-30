using UnityEngine;

public class BaseTeamLaser : MonoBehaviour
{
    [SerializeField] private BaseController m_baseController = default;
    [SerializeField] private GameTeamData m_gameTeamData = default;
    [SerializeField] private MeshRenderer m_meshRenderer = default;
    [SerializeField] private float m_flashTimeDelay = 0.3f;

    private bool m_flashingTeamColour = false;
    private bool m_fade = false;
    private float m_flashTimer = 0;

    private string m_contestingTeam = "";
    private Color m_startColour = Color.white;

    private void OnEnable()
    {
        m_baseController.OnOwnerChanged += OnTeamChanged;
        m_baseController.OnStateChanged += OnStateChanged;

        Material materialInstance = Instantiate(m_meshRenderer.material);
        m_meshRenderer.material = materialInstance;

        m_flashTimer = m_flashTimeDelay;

        OnTeamChanged(m_baseController.TeamOwner);
    }

    private void Update()
    {
        if (m_baseController.State != EBaseState.IDLE)
        {
            Color teamColour = m_baseController.State == EBaseState.CONTESTED ? m_gameTeamData.GetTeamData(m_contestingTeam).Colour : m_gameTeamData.DefaultTeamColour;
            m_flashTimer += Time.deltaTime;
            if (m_flashingTeamColour)
            {
                m_meshRenderer.material.color = Color.Lerp(teamColour, m_startColour, m_flashTimer / m_flashTimeDelay);
            }
            else
            {
                m_meshRenderer.material.color = Color.Lerp(m_startColour, teamColour, m_flashTimer / m_flashTimeDelay);
            }

            if (m_flashTimer >= m_flashTimeDelay)
            {
                m_flashTimer = 0;
                m_flashingTeamColour = !m_flashingTeamColour;
            }
        }
        else if (m_fade)
        {
            m_flashTimer += Time.deltaTime;

            Color flashColour = m_baseController.TeamOwner == "" ? Color.white : m_gameTeamData.GetTeamData(m_baseController.TeamOwner).Colour;
            m_meshRenderer.material.color = Color.Lerp(m_startColour, flashColour, m_flashTimer / m_flashTimeDelay);

            if (m_flashTimer >= m_flashTimeDelay)
            {
                m_flashTimer = 0;
                m_fade = false;
            }
        }
    }

    private void OnDisable()
    {
        m_baseController.OnOwnerChanged -= OnTeamChanged;
        m_baseController.OnStateChanged -= OnStateChanged;
    }


    private void OnStateChanged(EBaseState state)
    {
        if (state == EBaseState.CONTESTED)
        {
            m_contestingTeam = m_baseController.CaptureZone.ContestingTeam;
            m_startColour = m_meshRenderer.material.color;
        }
        else if (state == EBaseState.IDLE)
        {
            m_fade = true;
            m_contestingTeam = "";

            if (m_baseController.TeamOwner == "")
            {
                m_meshRenderer.material.color = Color.white;
            }
            else
            {
                m_meshRenderer.material.color = m_gameTeamData.GetTeamData(m_baseController.TeamOwner).Colour;
            }
        }
    }


    private void OnTeamChanged(string teamOwner)
    {
        if (teamOwner != "")
        {
            Color newColour = m_gameTeamData.GetTeamData(teamOwner).Colour;
            newColour.a = 0.5f;
            m_meshRenderer.material.color = newColour;
        }
    }
}