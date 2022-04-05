using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingText;
    private AsyncOperation gameScene;

    private bool _isLoaded;

    private void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    private void Update()
    {
        if (_isLoaded)
        {
            gameScene.allowSceneActivation = true;
        }
    }

    private IEnumerator LoadGameScene()
    {
        gameScene = SceneManager.LoadSceneAsync(2);
        gameScene.allowSceneActivation = false;
        while (gameScene.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(2);
        loadingText.SetActive(false);
        _isLoaded = true;
    }
}