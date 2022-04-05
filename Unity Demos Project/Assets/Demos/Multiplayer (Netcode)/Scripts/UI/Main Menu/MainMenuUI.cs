using Threadwork.Networking;
using TMPro;
using UnityEngine;

namespace Threadwork.UI.Titlescreen
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Play")]
        [SerializeField] private TMP_InputField displayNameInputField;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject playMenuPanel;
        [SerializeField] private GameObject hostMenuPanel;
        [SerializeField] private TMP_InputField serverPlayerLimitInputField;
        [SerializeField] private TMP_InputField serverPasswordInputField;
        [SerializeField] private GameObject joinMenuPanel;
        [SerializeField] private TMP_InputField joinPasswordInputField;

        [Header("Other Panels")]
        [SerializeField] private GameObject howToPlayPanel;
        [SerializeField] private GameObject profilePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        private void Start()
        {
            PlayerPrefs.GetString("PlayerName");
        }

        public void OnHostClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            try
            {
                ThreadworkGameNetPortal.Instance.ChangePlayerLimit(int.Parse(serverPlayerLimitInputField.text));
            }
            catch
            {
                ThreadworkGameNetPortal.Instance.ChangePlayerLimit(4);
            }
            ThreadworkGameNetPortal.Instance.ChangeServerPassword(serverPasswordInputField.text);

            ThreadworkGameNetPortal.Instance.StartHost();
        }

        public void OnClientClicked()
        {
            if (displayNameInputField.text.Length == 0) displayNameInputField.text = "No Name";
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            ThreadworkGameNetPortal.Instance.SetJoinPassword(joinPasswordInputField.text);

            ThreadworkGameNetPortal.Instance.StartClient();
        }

        public void TogglePlayScreen(bool toggle)
        {
            playMenuPanel.SetActive(toggle);
            mainMenuPanel.SetActive(!toggle);
        }

        public void ToggleHostScreen(bool toggle)
        {
            hostMenuPanel.SetActive(toggle);
            playMenuPanel.SetActive(!toggle);
        }

        public void ToggleJoinScreen(bool toggle)
        {
            joinMenuPanel.SetActive(toggle);
            playMenuPanel.SetActive(!toggle);
        }

        public void ToggleHowToPlayScreen(bool toggle)
        {
            howToPlayPanel.SetActive(toggle);
            mainMenuPanel.SetActive(!toggle);
        }

        public void ToggleProfileScreen(bool toggle)
        {
            profilePanel.SetActive(toggle);
            mainMenuPanel.SetActive(!toggle);
        }

        public void ToggleSettingsScreen(bool toggle)
        {
            settingsPanel.SetActive(toggle);
            mainMenuPanel.SetActive(!toggle);
        }

        public void ToggleCreditsScreen(bool toggle)
        {
            creditsPanel.SetActive(toggle);
            mainMenuPanel.SetActive(!toggle);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OnServerLimitValueChanged()
        {
            try
            {
                int count = int.Parse(serverPlayerLimitInputField.text);
                if (count <= 2) count = 3;
                else if (count > 8) count = 8;
                serverPlayerLimitInputField.text = count.ToString();
            }
            catch
            {
                serverPlayerLimitInputField.text = "4";
            }
        }

        public void OnNameValueChanged()
        {
            if (displayNameInputField.text.Length > 12)
            {
                string revision = displayNameInputField.text.Substring(0, 12);
                displayNameInputField.text = revision;
            }
            else if (displayNameInputField.text.Length == 0)
            {
                displayNameInputField.text = "No Name";
            }
        }
    }
}
