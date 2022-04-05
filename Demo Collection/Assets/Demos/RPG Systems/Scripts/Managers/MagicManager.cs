using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicManager : MonoBehaviour
{
    public static MagicManager Instance { get; private set; }
    public List<Magic> spells;

    //Spells
    public static Magic SIZZLE
    {
        get
        {
            return new Magic(0, "Sizzle", "Casts a wave of heat at the enemy which sometimes causes confusion.", Magic.elements.fire, 15f, 1);
        }
    }
    public static Magic FIREBALL
    {
        get
        {
            return new Magic(1, "Fireball", "Casts a ball of fire at the enemy.", Magic.elements.fire, 40f, 2);
        }
    }
    public static Magic SCORCH
    {
        get
        {
            return new Magic(2, "Scorch", "Casts a searing flame at the enemy.", Magic.elements.fire, 90f, 3);
        }
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Add spells
        spells.Add(SIZZLE);
        spells.Add(FIREBALL);
        spells.Add(SCORCH);
    }
}
