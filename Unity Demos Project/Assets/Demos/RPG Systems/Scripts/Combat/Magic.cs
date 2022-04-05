using System;
using System.Collections.Generic;

public class Magic
{
    public int spellID;
    public string spellName;
    public string spellDescription;
    public enum elements
    {
        fire,
        lightning,
        water,
        ice,
        dark,
        light
    }
    public elements element;
    public float spellPower;
    public int tier; //Tiers 1-3, designates casting time and mana cost

    public Magic(int ID, string name, string description, elements type, float power, int tier)
    {
        spellID = ID;
        spellName = name;
        spellDescription = description;
        element = type;
        spellPower = power;
        this.tier = tier;
    }
}
