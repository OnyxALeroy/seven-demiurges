using System;
using UnityEngine;

[System.Serializable]
public class SkillContext
{
    public CharacterBehaviour caster;
    public Vector3 target; // Target coordinates

    // Where the skill was cast & its direction
    public Vector3 castPosition;
    public Vector3 castDirection;

    // How long button was held before release
    public float holdDuration;

    // --------------------------------------------------------------------------------------------

    public SkillContext(
        CharacterBehaviour caster,
        Vector3 target,
        Vector3 castPosition,
        Vector3 castDirection,
        float holdDuration)
    {
        this.caster = caster;
        this.target = target;
        this.castPosition = castPosition;
        this.castDirection = castDirection;
        this.holdDuration = holdDuration;
    }
}
