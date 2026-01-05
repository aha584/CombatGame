using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqExercises
{
    // --- 1. ĐỊNH NGHĨA MODEL ---
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double GPA { get; set; }
        public int ClassId { get; set; }
        public List<string> Skills { get; set; } = new List<string>();

        // Override ToString để in kết quả cho đẹp
        public override string ToString()
        {
            return $"[ID: {Id}, Name: {Name}, Age: {Age}, GPA: {GPA}, ClassId: {ClassId}, Skills: {string.Join(", ", Skills)}]";
        }
    }

    public class Classroom
    {
        public int Id { get; set; }
        public string Names { get; set; }

        public override string ToString()
        {
            return $"[ID: {Id}, Class: {Names}]";
        }
    }

    public class Program
    {
        public static void Main()
        {
            // --- 2. KHỞI TẠO DỮ LIỆU MẪU ---
            var classrooms = new List<Classroom>
            {
                new Classroom { Id = 1, Names = "CNTT" },
                new Classroom { Id = 2, Names = "Kinh Tế" }
            };

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "An", Age = 20, GPA = 8.5, ClassId = 1, Skills = new List<string> { "C#", "SQL" } },
                new Student { Id = 2, Name = "Binh", Age = 19, GPA = 6.0, ClassId = 1, Skills = new List<string> { "Java" } },
                new Student { Id = 3, Name = "Chi", Age = 21, GPA = 9.2, ClassId = 2, Skills = new List<string> { "Excel", "Word" } },
                new Student { Id = 4, Name = "Dung", Age = 20, GPA = 7.5, ClassId = 2, Skills = new List<string> { "PowerPoint" } },
                new Student { Id = 5, Name = "Eva", Age = 22, GPA = 5.5, ClassId = 1, Skills = new List<string> { "HTML", "CSS", "JS" } },
                new Student { Id = 6, Name = "Phong", Age = 20, GPA = 8.0, ClassId = 99, Skills = new List<string>() }
            };

            Console.WriteLine("=== BÀI TẬP LINQ (SKELETON) ===\n");

            // ==========================================
            // CẤP ĐỘ 1: CƠ BẢN (Where, Select, OrderBy)
            // ==========================================

            Console.WriteLine("--- Bài 1: Tìm sinh viên có tuổi > 20 ---");
            // TODO: Viết code
            // var result1 = ...
            // PrintList(result1); 
            var result1 = students.Where(student => student.Age > 20);
            PrintList(result1);
            Console.WriteLine();


            Console.WriteLine("--- Bài 2: Tìm SV có GPA >= 8.0 và sắp xếp theo tên (A-Z) ---");
            // TODO: Viết code
            var result2 = students.Where(student => student.GPA >= 8.0).OrderBy(student => student.Name);
            PrintList(result2);
            Console.WriteLine();


            Console.WriteLine("--- Bài 3: Chỉ lấy 'Tên' và 'GPA' (Kiểu ẩn danh/Anonymous) ---");
            // TODO: Viết code
            var result3 = students.Select(student => new { student.Name, student.GPA }); 
            PrintList(result3);
            Console.WriteLine();


            // ==========================================
            // CẤP ĐỘ 2: THAO TÁC DỮ LIỆU (Count, Max, Skip/Take, Any)
            // ==========================================

            Console.WriteLine("--- Bài 4: Có bao nhiêu sinh viên thuộc lớp CNTT (ClassId = 1)? ---");
            // TODO: Viết code
            // Console.WriteLine("Kết quả: " + ...);
            List<Student> result4 = students.Where(student => student.ClassId == 1).ToList();
            Console.WriteLine($"ket qua: {result4.Count}");
            Console.WriteLine();


            Console.WriteLine("--- Bài 5: Tìm sinh viên có GPA cao nhất trường ---");
            // TODO: Viết code
            double maxGPA = students.Max(students => students.GPA);
            var StuMaxGPA = students.Where(student => student.GPA == maxGPA);
            PrintList(StuMaxGPA);
            Console.WriteLine();


            Console.WriteLine("--- Bài 6: Bỏ qua 2 sinh viên đầu, lấy 3 sinh viên tiếp theo ---");
            // TODO: Viết code
            var result6 = students.Skip(2).Take(3);
            PrintList(result6);
            
            Console.WriteLine();


            Console.WriteLine("--- Bài 7: Kiểm tra xem có sinh viên nào tên là 'Nam' không? ---");
            // TODO: Viết code
            bool result7 = students.Any(student => student.Name == "Nam");
            if (result7)
            {
                Console.WriteLine("Ton tai");
            }
            else
            {
                Console.WriteLine("Khong ton tai");
            }
            
            Console.WriteLine();


            // ==========================================
            // CẤP ĐỘ 3: FLAT & SET (SelectMany)
            // ==========================================

            Console.WriteLine("--- Bài 8: Lấy danh sách tất cả kỹ năng (không trùng lặp) ---");
            // TODO: Viết code

            //Linq
            var test = students.SelectMany(student => student.Skills);
            PrintList(test);

            Console.WriteLine();
            //HashSet
            var result8 = students.Select(student => student.Skills);
            HashSet<string> allSkills = new HashSet<string>();
            foreach (var listSkills in result8)
            {
                foreach (var skill in listSkills)
                {
                    allSkills.Add(skill);
                }
            }
            PrintList(allSkills);
            
            Console.WriteLine();


            Console.WriteLine("--- Bài 9: Tìm SV có tuổi = 20 VÀ GPA > 8.0 ---");
            // TODO: Viết code
            var result9 = students.Where(student => student.Age == 20 && student.GPA > 8.0);
            PrintList(result9);
            
            Console.WriteLine();


            // ==========================================
            // CẤP ĐỘ 4: GROUPING (GroupBy)
            // ==========================================

            Console.WriteLine("--- Bài 10: Nhóm SV theo Tuổi (In ra: Tuổi - Số lượng) ---");
            // TODO: Viết code
            //sai
            var result10 = students.GroupBy(student => student.Age);
            foreach(var groupAge in result10)
            {
                foreach(var age in groupAge)
                {
                    Console.WriteLine(age.Age + "_" + groupAge.Count());
                    break;
                }
            }
            
            Console.WriteLine();


            Console.WriteLine("--- Bài 11: Nhóm SV theo ClassId, chỉ lấy nhóm có GPA TB > 7.0 ---");
            // TODO: Viết code
            var result11 = students.GroupBy(student => student.ClassId);
            foreach(var groupClass in result11)
            {
                double avg;
                double sum = 0;
                foreach(var studentInClass in groupClass)
                {
                    sum += studentInClass.GPA;
                }
                avg = sum / groupClass.Count();
                if (avg > 7.0) PrintList(groupClass);
                //hoac co the chi in lop
            }

            Console.WriteLine();


            // ==========================================
            // CẤP ĐỘ 5: JOIN & NÂNG CAO
            // ==========================================

            Console.WriteLine("--- Bài 12: Inner Join (Tên SV - Tên Lớp) ---");
            // TODO: Viết code
            var result12 = students.Join(classrooms, i => i.ClassId, o => o.Id, (i, o) => new {Student = i.Name, Classroom = o.Names });
            PrintList(result12);
            
            Console.WriteLine();


            Console.WriteLine("--- Bài 13: Left Join (Tên SV - Tên Lớp, nếu null thì ghi 'Chưa phân lớp') ---");
            // TODO: Viết code
            
            
            Console.WriteLine();


            Console.WriteLine("--- Bài 14: Tìm lớp học có tổng điểm GPA cao nhất ---");
            // TODO: Viết code
            var result14 = students.GroupBy(student => student.ClassId);
            double sum14 = 0;
            foreach (var groupClass in result14)
            {
                double currSum = 0;
                foreach (var studentInClass in groupClass)
                {
                    currSum += studentInClass.GPA;
                }
                if (currSum > sum14)
                {
                    sum14 = currSum;
                }
            }
            foreach (var groupClass in result14)
            {
                double currSum = 0;
                foreach (var studentInClass in groupClass)
                {
                    currSum += studentInClass.GPA;
                }
                if (currSum == sum14)
                {
                    PrintList(groupClass);
                }
            }


            Console.WriteLine();

            // Giữ màn hình console (nếu chạy local)
            Console.ReadLine();
        }

        // --- HÀM HỖ TRỢ IN KẾT QUẢ ---
        public static void PrintList<T>(IEnumerable<T> source)
        {
            if (source == null || !source.Any())
            {
                Console.WriteLine("(Empty List)");
                return;
            }

            foreach (var item in source)
            {
                Console.WriteLine(item);
            }
        }
    }
}