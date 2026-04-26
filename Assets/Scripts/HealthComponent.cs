using System;
using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public enum DamageType { Normal, Effect, Absolute };
    public event Action<float> HealthChanged;
    public event Action<float, Vector2> DamageTaken;
    public event Action<float> Healed;
    public event Action HealthDepleted;
    public event Action InvincibilityStarted;
    public event Action InvincibilityEnded;

    private int _invincibleEffects = 0;
    public void AddInvincibleEffect()
    {
        _invincibleEffects++;
        if (_invincibleEffects == 1)
        {
            InvincibilityStarted?.Invoke();
        }
    }

    public void RemoveInvincibleEffect()
    {
        if (_invincibleEffects == 1)
        {
            InvincibilityEnded?.Invoke();
        }

        _invincibleEffects = Mathf.Max(0, _invincibleEffects - 1);
    }

    public bool IsInvincible => _invincibleEffects > 0;

    [SerializeField] private float _health = 100f;
    public float Health {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            HealthChanged?.Invoke(_health);

            if (_health == 0)
            {
                HealthDepleted?.Invoke();
            }
        }
    }

    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = Mathf.Max(0, value);
    }

    [SerializeField] private float invincibilityDuration = 1.0f;
    public float InvincibilityDuration => invincibilityDuration;

    public float HealthPercentage => Health / MaxHealth;

    private void Start()
    {
        //Health = MaxHealth;
    }

    public void TakeDamage(float dmg, Vector2 attackDirection, DamageType damageType = DamageType.Normal)
    {
        if (IsInvincible && damageType == DamageType.Normal) return;

        Health -= dmg;

        if (damageType == DamageType.Normal)
        {
            DamageTaken?.Invoke(Health, attackDirection);

            if (Health != 0)
                StartCoroutine(InvincibilityCoroutine());
        }
    }

    public float Heal(float health)
    {
        float healAmount = Mathf.Min(MaxHealth - Health, health);
        if (healAmount == 0f) return 0f;

        Health += healAmount;
        Healed?.Invoke(healAmount);
        return healAmount;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        AddInvincibleEffect();
        yield return new WaitForSeconds(InvincibilityDuration);
        RemoveInvincibleEffect();
    }
}
