using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Threadwork.Networking;
using Unity.Netcode;
using Unity.Collections;

namespace Threadwork.Gameplay.Player
{
    public class Nametag : NetworkBehaviour
    {
        [SerializeField] private Text nametag;

        private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

        private event Action OnNameChange;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;

            PlayerData? playerData = ThreadworkGameNetPortal.Instance.GetPlayerData(OwnerClientId);

            if(playerData.HasValue)
            {
                playerName.Value = playerData.Value.PlayerName;
                OnNameChange.Invoke();
            }
        }

        private void OnEnable()
        {
            OnNameChange += UpdateNametagClientRpc;
        }

        [ClientRpc]
        private void UpdateNametagClientRpc()
        {
            nametag.text = playerName.Value.ToString();
        }

        public void UpdateTitle(NetworkVariable<FixedString64Bytes> pName)
        {
            nametag.text = pName.ToString();
        }

        private void HandleNametagChanged(string oldName, string newName)
        {
            nametag.text = newName;
        }

        private void OnDisable()
        {
            OnNameChange -= UpdateNametagClientRpc;
        }
    }
}
