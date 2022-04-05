using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : Item
{
    [SerializeField]
    private int ID;

    public class ArmourAttributes
    {
        [Range(0, 4)]
        public int boonSlots;
        public List<int> boonsAttached = new List<int>();
    }
    public ArmourAttributes attributes = new ArmourAttributes();

    private void Start()
    {
        properties.type = 4;
    }
}
