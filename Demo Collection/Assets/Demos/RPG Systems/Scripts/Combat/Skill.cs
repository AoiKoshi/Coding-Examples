using System.Collections;
using System.Collections.Generic;

public class Skill
{
    public int skillID;
    public string skillName;
    public string skillDescription;
    public enum skillTypes
    {
        melee,
        ranged,
        buff,
        debuff
    }
    public skillTypes skillType;
    public float skillPower;
    public int actionCost; //Can cost 1-3 bars of the action gauge

    public Skill(int ID, string name, string description, skillTypes type, float power, int cost)
    {
        skillID = ID;
        skillName = name;
        skillDescription = description;
        skillType = type;
        skillPower = power;
        actionCost = cost;
    }
}
