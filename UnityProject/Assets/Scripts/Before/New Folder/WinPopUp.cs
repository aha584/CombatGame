using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPopUp : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadScene("Emoji Puzzle", LoadSceneMode.Single);
    }
}
