using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Unity.Netcode;

namespace Sunseekers.Gameplay.Tasks
{
    public class TaskManager : NetworkBehaviour
    {
        public List<Task> listOfTasks = new List<Task>();

        [SerializeField]
        private NetworkVariable<int> targetProgress = new NetworkVariable<int>(100);
        [SerializeField]
        private NetworkVariable<int> currentProgress = new NetworkVariable<int>(0);

        public event Action OnProgressUpdate;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;

            int totalProgress = 0;
            foreach (Task task in listOfTasks)
            {
                totalProgress += task.GetTaskWeight();
            }
            targetProgress.Value = totalProgress;
            OnProgressUpdate.Invoke();
        }

        private void OnEnable()
        {
            OnProgressUpdate += UpdateProgressClientRpc;
        }

        private void OnDisable()
        {
            OnProgressUpdate -= UpdateProgressClientRpc;
        }

        public void AddTask(Task newTask)
        {
            listOfTasks.Add(newTask);
        }

        public void RemoveTask(int taskID)
        {
            int index = 0;
            bool found = false;

            foreach (Task task in listOfTasks)
            {
                if (task.taskID == taskID)
                {
                    found = true;
                    break;
                }
                index++;
            }

            if (!found) return;
            listOfTasks.RemoveAt(index);
        }

        public void OnTaskComplete(Task.TaskData taskData)
        {
            OnTaskCompleteServerRpc(taskData);
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnTaskCompleteServerRpc(Task.TaskData task)
        {
            currentProgress.Value += task.taskWeight;
            OnProgressUpdate.Invoke();
            RemoveTask(task.taskID);
        }

        [ClientRpc]
        public void UpdateProgressClientRpc()
        {
            //Update HUDs or world elements
        }
    }
}
