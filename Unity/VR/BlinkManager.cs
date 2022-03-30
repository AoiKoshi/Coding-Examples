using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkManager : MonoBehaviour
{
    private Blink blink;
    public Action OnTeleport;

    public static BlinkManager Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            this.enabled = false;
        }
        blink = FindObjectOfType<Blink>();
    }

    public void OnBlink()
    {
        PlayerManager.Instance.TriggerHaptic(0.3f, 0.3f, PlayerManager.handTypes.both);
        blink.OnBlink();
        if(OnTeleport != null)
        {
            OnTeleport.Invoke();
        }
    }
}
