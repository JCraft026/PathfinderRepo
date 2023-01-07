using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    public void OpenScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OpenScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
