using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class SkillSheet : MonoBehaviour
{
    public List<Skill> skills = new List<Skill>();

    public void UnlockSkill(Skill skill)
    {
        skills.Add(skill);
    }

    public bool IsSkillUnlocked(Skill skill)
    {
        return skills.Contains(skill);
    }
}