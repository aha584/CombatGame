using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TestColl : MonoBehaviour
{
    private List<int> numbers = new();
    public IReadOnlyList<int> Numbers => numbers;
    private Dictionary<int,int> keyAndValue = new();
    public IReadOnlyDictionary<int,int> KeyAndValue => keyAndValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //B1
        /*for (int i = 0; i < 10; i++)
        {
            numbers.Add(i + 1);
        }
        numbers.RemoveAt(1);
        foreach (int i in numbers)
        {
            Debug.Log(i);
        }*/

        //B2
        /*int[] numArr = { 1, 2, 3, 4, 5 };
        List<int> numList = new();
        for (int i = 0; i < 5; i++)
        {
            Debug.Log(numArr[i]);
            numList.Add(numArr[i]);
        }
        numList.Add(3);
        numList.Add(9);*/

        //B3
        /*List<string> charClone = new List<string>() { "a", "b", "c", "a", "b" };
        HashSet<string> charNotClone = new();
        for (int i = 0; i < charClone.Count; i++)
        {
            charNotClone.Add(charClone[i]);
        }
        foreach(string charNot in charNotClone)
        {
            Debug.Log(charNot);
        }*/

        //B4
        /*Dictionary<string, int> traCuuDiem = new();
        traCuuDiem.Add("An", 10);
        traCuuDiem.Add("Binh", 8);
        traCuuDiem.Add("Chi", 7);

        if(traCuuDiem.TryGetValue("Tuan",out int diem))
        {
            Debug.Log(diem);
        }
        else
        {
            Debug.Log("Khong ton tai!!");
        }*/

        //B5
        /*Stack<string> userMove = new();
        userMove.Push("Draw");
        userMove.Push("Move");
        userMove.Push("ReSize");

        userMove.Pop();
        foreach(string move in userMove)
        {
            Debug.Log(move);
        }
        userMove.Pop();
        foreach (string move in userMove)
        {
            Debug.Log(move);
        }*/

        //B6
        /*Queue<string> levels = new Queue<string>();
        for(int i = 0; i < 3; i++)
        {
            levels.Enqueue($"Level{i + 1}");
            foreach(string level in levels)
            {
                Debug.Log(level);
            }
        }*/

        //B6.1 + B6.2
        /*int[][] matrix = new int[4][] { new int[4] { 6, 7, 9, 4 }, new int[4] { 2, 3, 4, 5 }, new int[4] { 0, 5, 9, 7 }, new int[4] { 10, 52, 7, 7 } };
        Debug.Log(matrix.Rank);
        Debug.Log(matrix.Length);

        int[,] ma = new int[2, 3] { { 2, 3, 4 }, { 1, 2, 3 } };
        int[,,] maxt = new int[4,5,6];
        Debug.Log(ma.Rank);
        Debug.Log(ma.Length);
        int k = 0;
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; i < matrix.Length; j++)
            {
                matrix[i][j] = i+j;
            }
        }*/

        //B7
        /*List<List<int>> allScoreInEachClass = new();
        int maxAll = int.MinValue;
        for(int i = 0; i < 10; i++)
        {
            allScoreInEachClass[i].Add(i + 1);
        }
        allScoreInEachClass[8].RemoveRange(4, 2);
        allScoreInEachClass[3].RemoveRange(6, 1);
        allScoreInEachClass[6].RemoveRange(3, 3);
        allScoreInEachClass[1].RemoveRange(1, 5);
        for(int i = 0; i < 10; i++)
        {
            int maxInClass = int.MinValue;
            for (int j = 0; j < allScoreInEachClass[i].Count; j++)
            {
                if (allScoreInEachClass[i][j] > maxInClass) maxInClass = allScoreInEachClass[i][j];
            }
            if (maxInClass > maxAll) maxAll = maxInClass;
        }
        Debug.Log(maxAll);*/

        //B8
        /*Dictionary<string, List<string>> classAndName = new();

        //Add
        string classToAdd = "D";
        List<string> nameToAdd = new List<string> {"Long"};
        if (!classAndName.TryAdd(classToAdd, nameToAdd))
        {
            classAndName.Add(classToAdd, nameToAdd);
        }
        else
        {
            classAndName.Add(classToAdd, nameToAdd);
        }

        //Move
        List<string> allMovedStu = classAndName["A"];
        classAndName.Add("B", allMovedStu);

        //Search
        string nameToFind = "An";
        for(int i = 0; i < classAndName.Count; i++)
        {
            if(classAndName.ElementAt(i).Value.Contains(nameToFind))
            {
                Debug.Log(classAndName.ElementAt(i).Key);
            }
        }*/

        //B9
        List<Dictionary<string, int>> bangDiem = new();
        List<string> mon = new();
        List<int> diem = new();
        //mon.Count==diem.Count
        for(int i = 0; i < bangDiem.Count; i++)
        {
            for(int j = 0; j < mon.Count; j++)
            {
                if (bangDiem[i].ContainsKey(mon[j])) diem[j] += bangDiem[i][mon[j]];
            }
        }
        Dictionary<string, int> tongDiemTungMon = new();
        for (int i = 0; i < mon.Count; i++)
        {
            tongDiemTungMon.Add(mon[i], diem[i]);
        }

        //B10
        Dictionary<string, Dictionary<string, int>> departmentScore = new();
        //phongBan<<nhanVien,diemNhanVien>>

        //Search
        int maxScore = int.MinValue;
        for (int i = 0; i < departmentScore.Count; i++)
        {
            Dictionary<string, int> phongBan = departmentScore.ElementAt(i).Value;
            for(int j = 0; j < phongBan.Count; j++)
            {
                if (phongBan.ElementAt(j).Value > maxScore) maxScore = phongBan.ElementAt(j).Value;
            }
        }
        //Average
        List<float> allAverageScore = new();
        for (int i = 0; i < departmentScore.Count; i++)
        {
            Dictionary<string, int> phongBan = departmentScore.ElementAt(i).Value;
            int sum = 0;
            for (int j = 0; j < phongBan.Count; j++)
            {
                sum += phongBan.ElementAt(j).Value;
            }
            allAverageScore.Add(sum / phongBan.Count);
        }
        //PrintAllData
        for (int i = 0; i < departmentScore.Count; i++)
        {
            Dictionary<string, int> phongBan = departmentScore.ElementAt(i).Value;
            Debug.Log("Phong ban " + departmentScore.ElementAt(i).Key + "gom co:");
            for (int j = 0; j < phongBan.Count; j++)
            {
                Debug.Log("Nhan vien: " + phongBan.ElementAt(j).Key + "; Diem: " + phongBan.ElementAt(j).Value);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
