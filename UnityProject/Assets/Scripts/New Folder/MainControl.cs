using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainControl : MonoBehaviour
{
    [SerializeField] private Image _sliderHandle;
    [SerializeField] private float _duration;

    private IEnumerator Start()
    {
        float value = 0;
        while (value < 1)
        {
            value += Time.deltaTime / _duration;
            _sliderHandle.fillAmount = value;

            yield return null;
        }
        Debug.Log("Done Loading!!!");
        SceneManager.LoadScene("Emoji Puzzle");
    }
}
