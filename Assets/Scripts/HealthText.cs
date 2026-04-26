using UnityEngine;
using TMPro;
public class HealthText : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    [SerializeField] private HealthComponent hc;
    
    private void Awake()
    {
        healthText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        healthText.text = $"{hc.Health} / {hc.MaxHealth}";
    }
}
