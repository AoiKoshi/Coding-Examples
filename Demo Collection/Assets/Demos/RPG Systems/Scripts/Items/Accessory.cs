using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : Item
{
    [SerializeField]
    private int ID;

    public class AccessoryAttributes
    {
        [Range(0,4)]
        public int boonSlots;
        public List<int> boonsAttached = new List<int>();
    }
    public AccessoryAttributes attributes = new AccessoryAttributes();

    private void Start()
    {
        properties.type = 3;
    }
}
