using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class MechUIWorldHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider, shieldSlider;
    [SerializeField] private Image fill;
    [SerializeField] private GameObject nameObj;

    private TextMeshProUGUI nameText;
    private Entity entity;
    private CS_PlayerStats playerStats;

    private ulong localPlayerId = default;

    // Start is called before the first frame update
    void Start()
    {
        localPlayerId = NetworkManager.Singleton.LocalClientId;

        entity = gameObject.GetComponent<Entity>();
        playerStats = gameObject.GetComponent<CS_PlayerStats>();
        nameText = nameObj.GetComponent<TextMeshProUGUI>();

        nameText.text = entity.Name;
        nameText.color = Color.red;

        shieldSlider.maxValue = playerStats.maxShields;
        shieldSlider.value = playerStats.maxShields;

        healthSlider.maxValue = playerStats.maxHealth;
        healthSlider.value = playerStats.maxHealth;

        if(TryGetComponent(out HealthComponent hc))
        {
            hc.GetCallback().AddListener(OnHealthBarValueChanged);
        }

        if (TryGetComponent(out ShieldComponent sp))
        {
            sp.GetCallback().AddListener(OnShieldBarValueChanged);
        }
    }

    private void OnHealthBarValueChanged(int oldValue, int newValue)
    {
        healthSlider.value = newValue;
    }
    private void OnShieldBarValueChanged(int oldValue, int newValue)
    {
        shieldSlider.value = newValue;
    }
}
