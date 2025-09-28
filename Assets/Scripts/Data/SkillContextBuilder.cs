using UnityEngine;
using System.Collections.Generic;

public class SkillContextBuilder
{
    private CharacterBehaviour caster;
    private Vector3 target;
    private Vector3 castPosition;
    private Vector3 castDirection;
    private float holdDuration;

    public SkillContextBuilder WithCaster(CharacterBehaviour caster)
    {
        this.caster = caster;
        return this;
    }

    public SkillContextBuilder WithTarget(Vector3 target)
    {
        this.target = target;
        return this;
    }

    public SkillContextBuilder AtPosition(Vector3 position)
    {
        this.castPosition = position;
        return this;
    }

    public SkillContextBuilder InDirection(Vector3 dir)
    {
        this.castDirection = dir.normalized;
        return this;
    }

    public SkillContextBuilder WithHoldDuration(float duration)
    {
        this.holdDuration = duration;
        return this;
    }

    public SkillContext Build()
    {
        return new SkillContext(
            caster,
            target,
            castPosition,
            castDirection,
            holdDuration
        );
    }
}
