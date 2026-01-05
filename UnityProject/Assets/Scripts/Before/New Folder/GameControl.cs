using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    private IMiniGames _miniGame;

    void Start()
    {
        LevelData levelData = Resources.Load<LevelData>($"ScriptableObjects/Level_{User.levelId}");
        if(levelData.GameType == GameType.MatchTheShadow)
        {
            MatchTheShadow prefab = Resources.Load<MatchTheShadow>("MatchTheShadow");
            _miniGame = Instantiate(prefab);
            _miniGame.Init(levelData);
        }
        else if (levelData.GameType == GameType.MatchEmoji)
        {
            Match2Game prefab = Resources.Load<Match2Game>("MatcthEmoji");
            _miniGame = Instantiate(prefab);
            _miniGame.Init(levelData);
        }
        if(_miniGame != null)
        {
            _miniGame.onWin += Win;
            _miniGame.onLose += Lose;
        }
    }

    public void Win()
    {
        User.levelId++;
        SceneManager.LoadScene("WinPopUp", LoadSceneMode.Additive);
    }
    
    public void Lose()
    {
        SceneManager.LoadScene("LosePopUp", LoadSceneMode.Additive);
    }
}
