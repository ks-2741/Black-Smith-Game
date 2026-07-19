using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillEntry
{
    public SkillType type;
    public int level = 0;
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [Header("Skill Levels (set starting values here, or leave at 0)")]
    public List<SkillEntry> skills = new List<SkillEntry>();

    [Header("Settings")]
    public int maxLevel = 10;

    public event System.Action OnSkillsChanged;

    void Awake()
    {
        Instance = this;
    }

    public int GetLevel(SkillType type)
    {
        SkillEntry entry = skills.Find(s => s.type == type);
        return entry != null ? entry.level : 0;
    }

    // Call this from your skill tree UI when the player spends points to upgrade a skill.
    public bool UpgradeSkill(SkillType type)
    {
        SkillEntry entry = skills.Find(s => s.type == type);

        if (entry == null)
        {
            entry = new SkillEntry { type = type, level = 0 };
            skills.Add(entry);
        }

        if (entry.level >= maxLevel)
        {
            Debug.Log("SkillManager: " + type + " is already at max level.");
            return false;
        }

        entry.level++;
        Debug.Log("SkillManager: " + type + " leveled up to " + entry.level);

        OnSkillsChanged?.Invoke();
        return true;
    }
}