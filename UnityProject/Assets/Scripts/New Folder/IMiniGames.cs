using System;
using UnityEngine;

public interface IMiniGames
{
    Action onWin { get; set; }
    Action onLose { get; set; }
    void Init(LevelData lvData);
    void Win();
    void Lose();
    bool CheckWin();
}
