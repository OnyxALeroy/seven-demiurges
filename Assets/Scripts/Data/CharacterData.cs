using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Characters/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName;
    public int level;
    public string characterClass; // Warrior, Mage, etc.

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public int stamina;
    public int maxStamina;
    public int strength;
    public int agility;
    public int intelligence;

    [Header("Position")]
    public Vector3 position; // Where the character is in the world

    [Header("Inventory")]
    public string[] inventoryItems; // Simple example, can expand later

    [Header("Cooldowns & Charging")]
    public float firstSkillCooldown = 0f;
    public float secondSkillCooldown = 0f;
    public float passiveCooldown = 0f;
    public int ultimateCharge = 0;

    [Header("Flags")]
    public bool isAlive;

    [Header("Runtime Link (set automatically)")]
    [HideInInspector] public CharacterBehaviour boundBehaviour;

    /// <summary>
    /// Clone this data at runtime so each instance has its own copy.
    /// </summary>
    public CharacterData Clone() { return Instantiate(this); }
}
