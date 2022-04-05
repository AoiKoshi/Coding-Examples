using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    [SerializeField]
    private int ID;
    public GameObject weaponModel;

    public class WeaponAttributes
    {
        public enum weaponType
        { 
            sword,
            rod,
            spear,
            greatsword,
            rifle,
            glove
        }
        public weaponType type;

        [Range(0, 4)]
        public int boonSlots;
        public List<int> boonsAttached = new List<int>();
    }
    public WeaponAttributes attributes = new WeaponAttributes();

    private void Start()
    {
        properties.type = 2;
    }
}
