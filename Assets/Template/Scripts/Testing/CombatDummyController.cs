using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatDummyController : MonoBehaviour
{
    private HealthComponent HealthComponent = default;
    
    [SerializeField] private TextMeshProUGUI ShieldsText;
    [SerializeField] private TextMeshProUGUI HealthText;

    private void OnEnable()
    {
        // Setup NetworkedHealthComponent
/*        HealthComponent = GetComponent<HealthComponent>();
        HealthComponent.OnHealthValueChanged(OnNetworkedHealthValueChanged);
        HealthComponent.AddOnShieldsValueChanged(OnNetworkedShieldsValueChanged);
        HealthText.text = "Health =  " + HealthComponent.GetHealthValue();
        ShieldsText.text = "Shields = " + HealthComponent.GetShieldsValue();*/
    }

    private void OnNetworkedHealthValueChanged(float OldValue, float NewValue)
    {
        HealthText.text = "Health = " + NewValue;
    }
    
    private void OnNetworkedShieldsValueChanged(float OldValue, float NewValue)
    {
        ShieldsText.text = "Shields = " + NewValue;
    }

}
