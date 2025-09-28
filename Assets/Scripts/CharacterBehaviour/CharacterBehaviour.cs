using UnityEngine;

public abstract class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterData characterTemplate;   // Assigned in Inspector
    protected CharacterData data;                               // The cloned runtime instance
    public CharacterData Data => data;

    protected virtual void Awake()
    {
        if (characterTemplate != null)
        {
            data = characterTemplate.Clone();
            data.boundBehaviour = this;
        }
    }

    protected virtual void Start()
    {
        Debug.Log($"Character {data.characterName} spawned with {data.health}/{data.maxHealth} HP.");
    }

    protected virtual void Update()
    {
        if (data.firstSkillCooldown > 0f) { Mathf.Max(0f, data.firstSkillCooldown - Time.deltaTime); }
        if (data.secondSkillCooldown > 0f) { Mathf.Max(0f, data.secondSkillCooldown - Time.deltaTime); }
        if (data.ultimateCharge < 100) { Mathf.Min(100, data.ultimateCharge + (int)(GetUltimateChargingRate() * Time.deltaTime)); }
        if (data.passiveCooldown > 0f) { Mathf.Max(0f, data.passiveCooldown - Time.deltaTime); }
    }

    // --------------------------------------------------------------------------------------------

    public virtual void TakeDamage(int amount)
    {
        data.health = Mathf.Max(0, data.health - amount);
        Debug.Log($"{data.characterName} took {amount} damage → {data.health} HP left!");

        if (data.health <= 0) { Die(); }
    }

    protected abstract void Die();

    // --------------------------------------------------------------------------------------------

    public void AskForFirstSkill(SkillContext ctx)
    {
        if (data.firstSkillCooldown > 0f) { return; }

        data.firstSkillCooldown = GetFirstSkillDefaultCooldown();
        TriggerFirstSkill(ctx);
    }
    protected abstract float GetFirstSkillDefaultCooldown();
    protected abstract void TriggerFirstSkill(SkillContext ctx);

    public void AskForSecondSkill(SkillContext ctx)
    {
        if (data.secondSkillCooldown > 0f) { return; }

        data.secondSkillCooldown = GetSecondSkillDefaultCooldown();
        TriggerSecondSkill(ctx);
    }
    protected abstract float GetSecondSkillDefaultCooldown();
    protected abstract void TriggerSecondSkill(SkillContext ctx);

    public void AskForUltimate(SkillContext ctx)
    {
        if (data.ultimateCharge < 100) { return; }

        data.ultimateCharge = 0;
        TriggerUltimate(ctx);
    }
    protected abstract float GetUltimateChargingRate();
    protected abstract void TriggerUltimate(SkillContext ctx);

    public void AskForPassive(SkillContext ctx)
    {
        if (data.passiveCooldown > 0f) { return; }

        data.passiveCooldown = GetPassiveDefaultCooldown();
        TriggerPassive(ctx);
    }
    protected abstract float GetPassiveDefaultCooldown();
    protected abstract void TriggerPassive(SkillContext ctx);
}
