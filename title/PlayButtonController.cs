using UnityEngine;
using UnityEngine.UI;

public class PlayButtonController : MonoBehaviour
{
    public Toggle[] toggles;
    public Button playButton;

    public SceneLoader sceneLoader;

    private int unlock;

    private int index;

    private void Start()
    {
        playButton.onClick.AddListener(LoadScene);

        unlock = PlayerPrefs.GetInt("unlock", 1);

        for (int i = 0; i < toggles.Length; i++)
            toggles[i].interactable = (i < unlock);
    }
    private void Update()
    {
        bool anyOn = false;

        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                index = i + 1;

                anyOn = true;
                break;
            }
        }

        playButton.interactable = anyOn;
    }

    private void LoadScene()
    {
        StartCoroutine(sceneLoader.Load(null, index));
    }
}
