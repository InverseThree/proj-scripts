using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingUI;
    private string currentSceneName;
    private string targetSceneName;

    public void ReLoad()
    {
        StartCoroutine(Load(SceneManager.GetActiveScene().name, -1));
    }

    public void Return()
    {
        StartCoroutine(Load("title", -1));
    }

    public void LoadScene()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        Debug.Log(currentSceneName);

        switch (currentSceneName)
        {
            case "chapter1":
                targetSceneName = "chapter2";

                PlayerPrefs.SetInt("unlock", 2);
                PlayerPrefs.Save();

                break;

            case "chapter2":
                targetSceneName = "title";

                PlayerPrefs.SetInt("unlock", 3);
                PlayerPrefs.Save();

                break;

            // case "chapter3":
            //     targetSceneName = "title";
            //
            //     PlayerPrefs.SetInt("unlock", 4)
            //     PlayerPrefs.Save();
            //
            //     break;
        }
        StartCoroutine(Load(targetSceneName, -1));
    }

    public IEnumerator Load(string sceneName, int index)
    {
        loadingUI.SetActive(true);

        float minLoadTime = 1.5f;
        float timer = 0f;

        AsyncOperation op;

        if (index != -1)
            op = SceneManager.LoadSceneAsync(index);
        else
            op = SceneManager.LoadSceneAsync(sceneName);

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        while (timer < minLoadTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;

        if (index == 1 || index == 2)
            musicController.Instance.PlayMusic("tut");
        else if (index == 3)
            musicController.Instance.PlayMusic("miniGame");
    }
}
