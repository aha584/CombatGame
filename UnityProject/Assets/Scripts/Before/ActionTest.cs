using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ActionTest : MonoBehaviour
{
    public Action<string> actionTest;
    public Action<int, int> sum;
    public Action<List<int>> listOfNumber;
    public List<int> number = new List<int>() {5,6,8,9,3,4};

    public Func<int,int> doubleN;
    public Func<string, string, bool> isLonger;
    public Func<List<int>, int> sumOfList;
    public Func<List<int>, int> maxInList;
    public UnityEngine.Events.UnityEvent<string> unityEvent;
    public UnityEngine.Events.UnityEvent<int, int> unityintEvent;
    public UnityEngine.Events.UnityEvent<List<int>> unitymathEvent;

    

    public void Start()
    {
        //actionTest = Say;
        //actionTest("Thanh");
        //sum = Add;
        //sum(3, 5);
        //listOfNumber = Numbers;
        //listOfNumber(number);

        doubleN = DoubleNumber;
        doubleN(300);
        isLonger = IsLonger;
        IsLonger(";osh;j;slkdgj", "uhsglijhljsgho;iasjdg;");
        sumOfList = SumOfList;
        sumOfList(number);
        maxInList = MaxInList;
        maxInList(number);
        /*
        unityEvent.AddListener(Say);
        unityEvent?.Invoke("Thanh");
        unityintEvent.AddListener(Add);
        unityintEvent?.Invoke(3, 5);
        unitymathEvent.AddListener(Numbers);
        unitymathEvent?.Invoke(number);
        */

        unityEvent.AddListener((string a) =>
        {
            Debug.Log("Hello" + a);
        });
        unityintEvent.AddListener((int a, int b) =>
        {
            Debug.Log(a + b);
        });
        actionTest += (string a) =>
        {
            Debug.Log("Hello" + a);
        };

    }

    public void Say(string name)
    {
        Debug.Log("Hello" + name);
    }
    public void Add(int a, int b)
    {
        Debug.Log(a + b);
    }
    public void Numbers(List<int> list)
    {
        foreach(int i in list)
        {
            Debug.Log(i);
        }
    }
    public int DoubleNumber(int a)
    {
        return a * a;
    }

    public bool IsLonger(string a, string b)
    {
        return a.Length > b.Length;
    }

    public int SumOfList(List<int> a)
    {
        int sum = 0;
        foreach(int i in a)
        {
            sum += i;
        }
        return sum;
    }
    public int MaxInList(List<int> a)
    {
        int ma = a.Max();
        return ma;
    }
}
