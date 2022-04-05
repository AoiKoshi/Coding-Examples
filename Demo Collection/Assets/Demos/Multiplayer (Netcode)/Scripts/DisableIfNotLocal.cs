using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableIfNotLocal : NetworkBehaviour
{
    [SerializeField] private bool destroy;

    private void Start()
    {
        if (!IsLocalPlayer)
        {
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
