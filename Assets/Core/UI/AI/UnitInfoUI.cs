using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private AIUnitTypesData m_unitTypesData = default;

    [SerializeField] private Image m_iconImage = default;
    [SerializeField] private TextMeshProUGUI m_nameText = default;
    [SerializeField] private TextMeshProUGUI m_quantityText = default;

    public void UpdateUnit(EUnitTypes unitType, int quantity)
    {
        var unitInfo = m_unitTypesData.GetUnitInfo(unitType);
        m_nameText.text = unitInfo.unitTitle;
        m_iconImage.sprite = unitInfo.unitImage;
        m_quantityText.text = "x" + quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        m_quantityText.text = "x" + quantity.ToString();
    }
}