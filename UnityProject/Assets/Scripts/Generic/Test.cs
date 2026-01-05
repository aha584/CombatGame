using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Experimental.GlobalIllumination;

public class Box<T>
{
    private T _value;

    public T Value => _value;

    public void Print()
    {
        Debug.Log(Value);
    }
}

public class ReferenceHolder<T> where T : class
{
    public T Value;

    public void Set(T value)
    {
        Value = value;
    }

    public T Get()
    {
        return Value;
    }
}

public class Creator<T> where T : new()
{
    public T Create()
    {
        return new T();
    }
}

public abstract class BaseMode1 { public int Id; }

public class Repository<T> where T : BaseMode1, new()
{
    private List<T> _tList = new();
    public void Add(T item)
    {
        _tList.Add(item);
    }

    public T FindById(int id)
    {
        return _tList[id];
    }
    public T CreateDefault()
    {
        return new T();
    }
}

public class SimpleCache<T> where T : class
{
    private Dictionary<string, T> _tDict = new();
    public void Add(string key, T value)
    {
        _tDict.Add(key, value);
    }
    public T GetOrDefault(string key)
    {
        if(_tDict.ContainsKey(key))
        {
            return _tDict[key];
        }
        else
        {
            return null;
        }
    } 
}

public class Pool<T> where T : class, new()
{
    private List<T> _inactive = new();
    private List<T> _active = new();
    public T Get(int index)
    {
        if (_inactive[index] == null)
        {
            return new T();
        }
        else
        {
            return _inactive[index];
        }
    }
    public void Release(T item)
    {
        if (_active.Contains(item))
        {
            _active.Remove(item);
            _inactive.Add(item);
        }
    }
}

public class Map<TKey, TValue> 
    where TKey : struct 
    where TValue : class, new()
{
    private List<TKey> _keyList = new();
    private List<TValue> _valueList = new();
    public void Add(TKey key, TValue value)
    {
        _keyList.Add(key);
        _valueList.Add(value);
    }
    public TValue GetOrCreate(TKey key)
    {
        if (_keyList.Contains(key))
        {
            return _valueList[_keyList.IndexOf(key)];
        }
        else
        {
            TValue newValue = new TValue();
            _valueList.Add(newValue);
            return newValue;
        }
    }
}

public interface INamed
{
    string Name { get; set; }
}
public abstract class GameObjects { public int Id; }

public class MultiCreator<T> where T : GameObjects, INamed, new()
{
    public T Create(int id, string name)
    {
        T newObj = new T();
        newObj.Id = id;
        newObj.Name = name;
        return newObj;
        
    }
    public T[] CreateBatch(int count)
    {
        T[] _tArr = new T[count];
        for(int i = 0; i < count; i++)
        {
            _tArr[i] = new T();
        }

        return _tArr;
    }
}

public class Test : MonoBehaviour
{

}
