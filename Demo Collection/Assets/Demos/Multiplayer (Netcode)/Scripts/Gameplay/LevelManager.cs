using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Collections;
using Threadwork.Networking;

namespace Threadwork.Gameplay
{
    public class LevelManager : NetworkBehaviour
    {
        public GameObject playerPrefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                InitialisePlayersServerRpc();
            }
        }

        [ServerRpc]
        private void InitialisePlayersServerRpc()
        {
            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnPlayer(id);
            }
        }

        private void SpawnPlayer(ulong pID)
        {
            Vector3 spawnPos = Vector3.zero;
            Quaternion spawnRot = Quaternion.identity;

            spawnPos = new Vector3(2f, 0f, 0f);
            spawnRot = Quaternion.Euler(0f, 225f, 0f);

            GameObject player = Instantiate(playerPrefab, spawnPos, spawnRot);

            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(pID, true);
        }
    }
}
