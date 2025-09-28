using UnityEngine;

public class Hugo : CharacterBehaviour
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("Hugo is ready for battle!");
    }

    protected override void Die()
    {
        Debug.Log("Hugo falls dramatically and drops his sword!");
        // Add custom animation, cutscene, etc.
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        Debug.Log("Hugo grunts loudly!");
    }

    // --------------------------------------------------------------------------------------------

    protected override float GetFirstSkillDefaultCooldown() { return 100f; }
    protected override void TriggerFirstSkill(SkillContext ctx) { }

    protected override float GetSecondSkillDefaultCooldown() { return 100f; }
    protected override void TriggerSecondSkill(SkillContext ctx) { }

    protected override float GetUltimateChargingRate() { return 100f; }
    protected override void TriggerUltimate(SkillContext ctx) { }

    protected override float GetPassiveDefaultCooldown() { return 100f; }
    protected override void TriggerPassive(SkillContext ctx) { }
}
