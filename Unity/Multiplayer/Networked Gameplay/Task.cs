using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Unity.Netcode;

namespace Sunseekers.Gameplay.Tasks
{
    public class Task : NetworkBehaviour
    {
        [SerializeField] private TaskManager taskManager;

        [Tooltip("Unique task ID")]
        public int taskID;
        [Tooltip("How much completing the task will contribute towards total progress")]
        public int taskWeight = 10;
        [Tooltip("Visual marker to indicate task on the map screen")]
        [SerializeField] private GameObject mapIcon;

        private NetworkVariable<bool> playerOccupied = new NetworkVariable<bool>(false);

        public class TaskData : INetworkSerializable
        {
            public int taskID, taskWeight;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref  taskID);
                serializer.SerializeValue(ref taskWeight);
            }
        }
        public TaskData data = new TaskData();

        private void Start()
        {
            data.taskID = taskID;
            data.taskWeight = taskWeight;
        }

        //Begin interaction
        public virtual void OnInitiateTask()
        {
            if (!playerOccupied.Value)
            {
                OccupyTaskServerRpc();
            }
        }

        //Exit before completion
        public virtual void OnExitTask()
        {
            UnOccupyTaskServerRpc();
        }

        public virtual void OnCompleteTask()
        {
            //Send over a serialised task data container to the server
            taskManager.OnTaskComplete(data);
            DeactivateTaskClientRpc();
        }

        [ServerRpc]
        public virtual void OccupyTaskServerRpc()
        {
            playerOccupied.Value = true;
        }

        [ServerRpc]
        public virtual void UnOccupyTaskServerRpc()
        {
            playerOccupied.Value = false;
        }

        [ClientRpc]
        public virtual void DeactivateTaskClientRpc()
        {
            //mapIcon.SetActive(false);
            this.enabled = false;
        }

        public int GetTaskWeight()
        {
            return taskWeight;
        }
    }
}
