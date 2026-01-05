using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePopUp : MonoBehaviour
{
    public void Return()
    {
        SceneManager.LoadScene("Emoji Puzzle", LoadSceneMode.Single);
    }
}
