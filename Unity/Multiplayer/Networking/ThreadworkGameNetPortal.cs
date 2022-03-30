using UnityEngine;
using Unity.Collections;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using Sunseekers.Gameplay.Player;
using Threadwork.Lobby.UI;

namespace Threadwork.Networking
{
    public class ThreadworkGameNetPortal : MonoBehaviour
    {
        [Header("Settings")]
        public int minPlayers = 3;
        public int maxPlayers = 4;
        [SerializeField] private int serverLimitPlayers = 8;
        [SerializeField] private string serverPassword;
        [SerializeField] private string joinPassword;
        [SerializeField] private int numberOfPlayersJoined;

        [Header("References")]
        public UNetTransport uNTransport;
        public GameObject playerPrefab;

        public static ThreadworkGameNetPortal Instance => instance;
        private static ThreadworkGameNetPortal instance;

        //Main Portal
        public event Action OnNetworkReadied;
        public event Action<ConnectStatus> OnConnectionFinished;
        public event Action<ConnectStatus> OnDisconnectReasonReceived;
        public event Action<ulong, int> OnClientSceneChanged;
        public event Action OnUserDisconnectRequested;
        public event Action OnNetworkTimedOut;

        public DisconnectReason DisconnectReason { get; private set; } = new DisconnectReason();

        //Data
        private Dictionary<string, PlayerData> clientData;
        private Dictionary<ulong, string> clientIdToGuid;
        private Dictionary<ulong, int> clientSceneMap;
        private bool gameInProgress;

        private const int MaxConnectionPayload = 2048;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
            NetworkManager.Singleton.OnServerStarted += HandleNetworkReadied;
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

            clientData = new Dictionary<string, PlayerData>();
            clientIdToGuid = new Dictionary<ulong, string>();
            clientSceneMap = new Dictionary<ulong, int>();
        }

        private void HandleServerStarted()
        {
            if (!NetworkManager.Singleton.IsHost) { return; }
        }

        private void HandleNetworkReadied()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Scene_Lobby", LoadSceneMode.Single);

            if (NetworkManager.Singleton.IsHost)
            {
                clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
            }
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId) { return; }

            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;
            OnClientSceneChanged += HandleClientSceneChanged;
            numberOfPlayersJoined++;
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId) { return; }

            NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
            OnClientSceneChanged -= HandleClientSceneChanged;
            numberOfPlayersJoined--;

            NetworkManager.Singleton.Shutdown();

            ClearData();

            if (SceneManager.GetActiveScene().name != "Scene_Menu")
            {
                SceneManager.LoadScene("Scene_Menu");
            }
        }

        private void HandleSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

            OnClientSceneChanged?.Invoke(sceneEvent.ClientId, SceneManager.GetSceneByName(sceneEvent.SceneName).buildIndex);
        }

        private void HandleClientSceneChanged(ulong clientId, int sceneIndex)
        {
            clientSceneMap[clientId] = sceneIndex;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnServerStarted -= HandleNetworkReadied;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        #region ServerGameNetPortal
        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(joinPassword);
            NetworkManager.Singleton.StartClient();
        }

        public void Leave()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            }

            HandleClientDisconnect(NetworkManager.Singleton.LocalClientId);
        }

        public void SetJoinPassword(string password)
        {
            joinPassword = password;
        }

        public void ChangeServerPassword(string newPassword)
        {
            serverPassword = newPassword;
        }

        public void ChangePlayerLimit(int count)
        {
            if (count < minPlayers) count = minPlayers;
            else if (count > serverLimitPlayers) count = serverLimitPlayers;
            maxPlayers = count;
        }

        public void SetServerIP()
        {
            //string serverIP = ipAddressField.text;
            //uNTransport.ConnectAddress = serverIP;
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            string password = Encoding.ASCII.GetString(connectionData);
            bool passwordApproved = password == serverPassword;
            Debug.Log(serverPassword + " : " + password);

            ConnectStatus gameReturnStatus = ConnectStatus.Success;

            if (gameInProgress)
            {
                gameReturnStatus = ConnectStatus.GameInProgress;
            }
            else if (clientData.Count >= maxPlayers)
            {
                gameReturnStatus = ConnectStatus.ServerFull;
            }

            bool approveConnection = passwordApproved && gameReturnStatus == ConnectStatus.Success ?
                true : false;

            callback(false, null, approveConnection, null, null);

            if (!approveConnection)
            {
                Debug.Log(gameReturnStatus.ToString());
                StartCoroutine(WaitToDisconnectClient(clientId, gameReturnStatus));
                return;
            }

            string clientGuid = Guid.NewGuid().ToString();
            string playerName = PlayerPrefs.GetString("PlayerName", "No Name");

            clientData.Add(clientGuid, new PlayerData(playerName, clientId));
            clientIdToGuid.Add(clientId, clientGuid);

            HandleClientConnected(clientId);
        }

        private void ClearData()
        {
            clientData.Clear();
            clientIdToGuid.Clear();
            clientSceneMap.Clear();

            gameInProgress = false;
        }

        private IEnumerator WaitToDisconnectClient(ulong clientId, ConnectStatus reason)
        {
            yield return new WaitForSeconds(0);

            KickClient(clientId);
        }

        public PlayerData? GetPlayerData(ulong clientId)
        {
            if (clientIdToGuid.TryGetValue(clientId, out string clientGuid))
            {
                if (clientData.TryGetValue(clientGuid, out PlayerData playerData))
                {
                    return playerData;
                }
                else
                {
                    Debug.LogWarning($"No player data found for client id: {clientId}");
                }
            }
            else
            {
                Debug.LogWarning($"No client guid found for client id: {clientId}");
            }

            return null;
        }

        #endregion

        #region InGameNetPortal

        public void StartGame()
        {
            gameInProgress = true;

            NetworkManager.Singleton.SceneManager.LoadScene("Scene_Main", LoadSceneMode.Single);

            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }

        public void EndRound()
        {
            gameInProgress = false;

            NetworkManager.Singleton.SceneManager.LoadScene("Scene_Lobby", LoadSceneMode.Single);
        }

        private void KickClient(ulong clientId)
        {
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }

            NetworkManager.Singleton.DisconnectClient(clientId);
        }

        private void SpawnPlayer(ulong clientId)
        {
            
        }

        #endregion
    }
}