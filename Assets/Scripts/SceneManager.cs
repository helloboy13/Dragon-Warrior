using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject object1;
    [SerializeField] private GameObject object2;
    [SerializeField] private GameObject GameWinScreen;
    [SerializeField] private GameObject GameOverScreen;

    private bool printed = false;

    private void Update()
    {
        if (printed)
            return;

        if (object1 == null && object2 != null)
        {
            GameOverScreen.SetActive(true);
            Debug.Log(object2.tag);
            printed = true;
        }
        else if (object2 == null && object1 != null)
        {
            GameWinScreen.SetActive(true);
            Debug.Log(object1.tag);
            printed = true;
        }
    }


    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScreen");
    }
}
