using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
public class TestMind2 : MonoBehaviour
{
    public object[] objects = new object[5] { (object)1, (object)4.78f, (object)89370983709834, (object)637, (object)4689748794.484265389035f };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string str1 = "abc";
        string str2 = "bc";
        string str = str1.Replace(str2, "");

        Debug.Log(SumObjects(objects));
        string str3 = SumWithBigNumber("23584769512547852478569581254", "25417958462458725168954758632");
        Debug.Log(str3);
        Debug.Log(str3.Length);

    }

    public object SumObjects(object[] arr)
    {
        double sum = 0;
        foreach(object obj in arr)
        {
            if (obj is int) sum += (int)obj;
            else if (obj is float) sum += (float)obj;
            else if (obj is double) sum += (double)obj;
        }
        return (object)sum;
    }

    public bool TryParsePositive(string str, out int result)
    {
        if(int.TryParse(str, out int intx))
        {
            if(intx >= 0)
            {
                result = intx;
            }
            else
            {
                result = 0;
                return false;
            }
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }

    public void FindMinMax(int[] arr, out int min, out int max)
    {
        min = int.MaxValue;
        max = int.MinValue;
        foreach(int i in arr)
        {
            if (i < min) min = i;
            else if (i > max) max = i;
        }
    }

    public void AvarageValue(int?[] arr, out float value)
    {
        int count = 0;
        int sum = 0;
        foreach(int? i in arr)
        {
            if(i.HasValue)
            {
                count++;
                sum += i.Value;
            }
        }
        if (count == 0)
        {
            value = 0;
        }
        else
        {
            value = sum / count;
        }
    }

    public string SumWithBigNumber(string num1, string num2)
    {
        int remNumber = 0;
        string sum = "";
        char[] allNumbersNum1 = num1.Reverse().ToArray();
        char[] allNumbersNum2 = num2.Reverse().ToArray();
        Dictionary<int, int> multiplierAndDigitNum1 = new();
        Dictionary<int, int> multiplierAndDigitNum2 = new();
        Dictionary<int, int> multiplierAndDigitNewNum = new();


        for (int i = 0; i < allNumbersNum1.Length; i++)
        {
            multiplierAndDigitNum1.Add(i, int.Parse(allNumbersNum1[i].ToString())); 
        }
        for (int i = 0; i < allNumbersNum2.Length; i++)
        {
            multiplierAndDigitNum2.Add(i, int.Parse(allNumbersNum2[i].ToString()));
        }

        int maxLength = Mathf.Max(num1.Length, num2.Length);
        for(int i=0; i<maxLength;i++)
        {
            int heso = 0;
            if (multiplierAndDigitNum1.ContainsKey(i))
                heso += multiplierAndDigitNum1[i];
            if (multiplierAndDigitNum2.ContainsKey(i))
                heso += multiplierAndDigitNum2[i];
            multiplierAndDigitNewNum.Add(i, heso);
        }
        for (int i = 0; i < multiplierAndDigitNewNum.Count - 1; i++)
        {
            if (multiplierAndDigitNewNum[i] > 9)
            {
                multiplierAndDigitNewNum[i] -= 10;
                multiplierAndDigitNewNum[i + 1] += 1;
            }
            sum += multiplierAndDigitNewNum[i].ToString();
        }
        if (multiplierAndDigitNewNum[multiplierAndDigitNewNum.Count-1] > 9)
        {
            multiplierAndDigitNewNum[multiplierAndDigitNewNum.Count-1] -= 10;
            sum += multiplierAndDigitNewNum[multiplierAndDigitNewNum.Count - 1];
            sum += "1";
        }
        else
        {
            sum += multiplierAndDigitNewNum[multiplierAndDigitNewNum.Count - 1];
        }

        var r = sum.Reverse().ToArray();
        string x = string.Empty;
        foreach (var i in r)
            x += i;

        return x;
    }
}
