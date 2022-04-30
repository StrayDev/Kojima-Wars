using UnityEngine;
using UnityEngine.UI;

public class UnitBaseSelector : MonoBehaviour
{
    public Button SelectButton => m_selectButton;

    [SerializeField] private GameTeamData m_gameTeamData = default;
    [SerializeField] private Button m_selectButton = default;
    [SerializeField] private Image m_selectImage = default;

    public void UpdateDisplay(Entity localPlayer, BaseController controller)
    {
        if (controller.TeamOwner == "")
        {
            m_selectImage.color = m_gameTeamData.DefaultTeamColour;
        }
        else
        {
            m_selectImage.color = m_gameTeamData.GetTeamData(controller.TeamOwner).Colour;
        }
    }
}
