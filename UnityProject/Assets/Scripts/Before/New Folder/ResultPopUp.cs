using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultPopUp : MonoBehaviour
{
    [SerializeField] private Image _slide;
    [SerializeField] private TMP_Text _percent;
    [SerializeField] private float _duration;
    [SerializeField] private GameObject _doneBut;

    private IEnumerator Start()
    {

        float start = User.Percent - 0.5f;
        float end = User.Percent;

        float timer = 0;
        float delta = (end - start)/_duration;
        _slide.fillAmount = start;
        while (timer < _duration)
        {
            _slide.fillAmount += delta * Time.deltaTime;
            timer += Time.deltaTime;
            int percent = (int)(_slide.fillAmount * 100f);
            _percent.text = $"{percent}";
            yield return null;
        }
        Debug.Log("Done Load");
        
        _slide.fillAmount = start;
        

        //_slide.DOFillAmount(User.Percent, _duration).From(User.Percent - 0.5f);
        //percent = _slide.fillAmount;
        //_percent.text = $"{percent}";
        
        _doneBut.SetActive(true);
    }

    public void ClickDone()
    {
        SceneManager.LoadScene("Emoji Puzzle");
    }

}
