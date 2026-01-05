
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Match2Game : MonoBehaviour, IMiniGames
{
    [SerializeField] private List<EP_Node> _row1;
    [SerializeField] private List<EP_Node> _row2;
    public Action onWin { get; set; }
    public Action onLose { get; set; }

    private EP_Node _node1, _node2;

    public void Init(LevelData lvData)
    {
        int x = 0;
        for (int i = 0; i < (lvData.NodeData.Count / 2); i++)
        {
            _row1[i].SetUp(lvData.NodeData[i]);
            _row1[i].onClick += ClickNode;
        }
        for (int i = lvData.NodeData.Count - 1; i >= (lvData.NodeData.Count / 2); i--)
        {
            //Debug.Log(i);
            _row2[x].SetUp(lvData.NodeData[i]);
            _row2[x].onClick += ClickNode;
            x++;
        }
    }

    public void ClickNode(EP_Node node)
    {
        if (_node1 == null)
        {
            _node1 = node;
            _node1.SetUpNode1();
        }
        else if(node.RowId !=  _node1.RowId)
        {
            _node2 = node;
            _node1.ConnectNode2(_node2);
            _node2._oppNode = _node1;
            if(CheckConnectAll())
            {
                if(CheckWin())
                {
                    Win();
                }
                else
                {
                    Lose();
                }
            }
            _node1 = null;
            _node2 = null;
        }
    }

    private bool CheckConnectAll()
    {
        foreach (var node in _row1)
        {
            if (!node.IsConnect())
                return false;
        }
        foreach (var node in _row2)
        {
            if (!node.IsConnect())
                return false;
        }
        return true;
    }

    public bool CheckWin()
    {
        foreach(var node in _row1)
        {
            if (!node.IsCorrect())
                return false;
        }
        foreach(var node in _row2)
        {
            if (!node.IsCorrect())
                return false;
        }
        return true;
    }

    public void Win()
    {
        ShowResult();
        StartCoroutine(Co_DelayShowPopup(onWin));
    }

    private IEnumerator Co_DelayShowPopup(Action onComplete)
    {
        yield return new WaitForSeconds(1.5f);
        onComplete?.Invoke();
    }

    public void Lose()
    {
        ShowResult();
        StartCoroutine(Co_DelayShowPopup(onLose));
    }

    private void ShowResult()
    {
        foreach(var node in _row1)
        {
            node.ShowResult();
        }
    }
}
