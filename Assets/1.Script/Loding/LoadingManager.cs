using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    public string moveScene;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void beforeSceneSetup(string sceneName)
    {
        // 이동할 씬 대입
        moveScene = sceneName;

        // 로딩 씬 이동
        SceneManager.LoadScene("LoadingScene");
    }
}
