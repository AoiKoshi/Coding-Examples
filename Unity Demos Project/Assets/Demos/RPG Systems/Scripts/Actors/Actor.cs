using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [Serializable]
    public class CharacterProfile
    {
        public string firstName;
        public string lastName;
        public int age;
        [TextArea(1, 20)]
        public string report;
    }
    public CharacterProfile profile = new CharacterProfile();

    public NPCStatSheet stats
    {
        get
        {
            return gameObject.GetComponent<NPCStatSheet>();
        }
    }
}
