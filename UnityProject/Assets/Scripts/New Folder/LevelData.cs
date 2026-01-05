using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Android;
using System;

[Serializable]
public class NodeData
{
    public Sprite Spr;
    public int MatchId;
}

public enum GameType
{
    MatchEmoji,
    MatchTheShadow,
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private GameType _gameType;
    [SerializeField] private List<NodeData> _nodeData;
    

    public GameType GameType => _gameType;
    public List<NodeData> NodeData => _nodeData;

}
