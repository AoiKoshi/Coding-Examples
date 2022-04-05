using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private int ID;

    [Serializable]
    public class ItemProperties
    {
        public string name;
        [Tooltip("1 - Items, 2 - Weapons, 3 - Accesories, 4 - Armour, 5 - Loot, 6 - Key Items")]
        [Range(1, 6)]
        public int type = 1;
        [Min(1)]
        public float quantity;
        public bool canSell;
        [Min(0)]
        public int value;
        [TextArea(3, 10)]
        public string description;
    }
    public ItemProperties properties = new ItemProperties();

    [Serializable]
    public class ItemGraphics
    {
        public Sprite overview;
    }
    public ItemGraphics graphics = new ItemGraphics();

    public int GetID()
    {
        return ID;
    }

    public Sprite GetOverviewImage()
    {
        return graphics.overview;
    }

    public virtual void Restore(PartyMember character)
    {
        
    }
}
