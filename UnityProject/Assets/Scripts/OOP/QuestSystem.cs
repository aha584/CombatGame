//
// Hoàn thiện tất cả các TODO và thay thế toàn bộ throw exception.
// Yêu cầu sinh viên phải áp dụng OOP: interface, polymorphism, collection, flow logic.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QuestSystem
{
    /// <summary>
    /// Interface đại diện cho một Quest.
    /// Mỗi Quest phải có:
    /// - Nhãn (Label) để hiển thị
    /// - Điều kiện hoàn thành (IsComplete)
    /// - Chỉ tiêu cần đạt (Target)
    /// - Cập nhật tiến độ (UpdateProgress)
    /// - Xuất log thông tin (Log)
    /// 
    /// TODO: Sinh viên sẽ hiện thực các thông tin còn thiếu.
    /// </summary>
    public interface IQuest
    {
        string Label { get; }
        bool IsComplete { get; }
        int Target { get; }

        void UpdateProgress(int amount);
        void Log();
    }

    /// <summary>
    /// Quest: Giết quái.
    /// TODO: Hoàn thiện phần logic bên trong.
    /// </summary>
    public class KillEnemy : IQuest
    {
        // TODO: Thêm biến private lưu tiến độ hiện tại
        private int _currentProcess;
        private int _target;
        // TODO: Thêm constructor nhận vào target
        public KillEnemy(int amountOfEnemy)
        {
            _currentProcess = 0;
            _target = amountOfEnemy;
        }

        public string Label => "Kill Enemy";  // Tên quest hiển thị

        public bool IsComplete => _currentProcess >= _target; // TODO

        public int Target => _target; // TODO

        public void UpdateProgress(int amount)
        {
            // TODO: Tăng tiến độ, giới hạn không vượt Target
            _currentProcess += amount;

        }

        public void Log()
        {
            // TODO: In tiến độ: "Kill Enemy: x / Target"
            Console.WriteLine($"Kill Enemy: {_currentProcess * 1f / Target}");
        }

        /// <summary>
        /// Quest: Thu thập vật phẩm.
        /// TODO: Hoàn thiện phần logic bên trong giống KillEnemy.
        /// </summary>
        public class CollectItem : IQuest
        {
            // TODO: Thêm biến private lưu tiến độ hiện tại
            private int _currentProcess;
            private int _target;

            // TODO: Thêm constructor nhận vào target
            public CollectItem(int amountOfCoin)
            {
                _currentProcess = 0;
                _target = amountOfCoin;
            }

            public string Label => "Collect Coin";

            public bool IsComplete => _currentProcess >= _target;

            public int Target => _target;

            public void UpdateProgress(int amount)
            {
                // TODO: Tăng tiến độ, giới hạn không vượt Target
                _currentProcess += amount;
                if (_currentProcess >= _target)
                {
                    _currentProcess = _target;
                }
            }

            public void Log()
            {
                // TODO: In tiến độ: "Collect Coin: x / Target"
                Console.WriteLine($"Collect Coin: {_currentProcess / Target}");
            }
        }

        /// <summary>
        /// Lớp User đại diện cho người chơi.
        /// Người chơi có danh sách Quest đang làm.
        /// TODO: Sinh viên phải hiện thực toàn bộ logic.
        /// </summary>
        public class User
        {
            private readonly List<IQuest> _quests = new();

            /// <summary>
            /// Thêm Quest cho người chơi.
            /// </summary>
            public void AddQuest(IQuest quest)
            {
                // TODO: Thêm vào danh sách _quests
                _quests.Add(quest);
                // TODO: Check null
                if (quest == null) _quests.Remove(quest);
            }

            /// <summary>
            /// Khi người chơi giết quái → cập nhật tất cả quest có liên quan.
            /// </summary>
            public void KillEnemy()
            {
                // TODO: Duyệt toàn bộ quest, gọi UpdateProgress(1) cho quest dạng KillEnemy
                foreach (IQuest quest in _quests)
                {
                    if (quest is KillEnemy)
                    {
                        quest.UpdateProgress(1);
                    }
                }
            }

            /// <summary>
            /// Khi người chơi nhặt coin → cập nhật tất cả quest có liên quan.
            /// </summary>
            public void CollectCoin(int amount)
            {
                // TODO: UpdateProgress(amount) cho quest dạng CollectItem
                foreach (IQuest quest in _quests)
                {
                    if (quest is CollectItem)
                    {
                        quest.UpdateProgress(amount);
                    }
                }
            }

            /// <summary>
            /// Kiểm tra xem TẤT CẢ quest đã hoàn thành hay chưa.
            /// </summary>
            public bool CheckCompleteAllQuest()
            {
                // TODO: Trả về true nếu tất cả quest trong _quests đều IsComplete
                int _completeCount = 0;
                foreach (IQuest quest in _quests)
                {
                    if (!quest.IsComplete) return false;
                }

                return true;
                if (_completeCount == _quests.Count) return true;
                return false;
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                User user = new User();

                // Tạo quest
                IQuest q1 = new KillEnemy(5);   // giết 5 quái
                IQuest q2 = new CollectItem(10); // nhặt 10 coin

                user.AddQuest(q1);
                user.AddQuest(q2);

                Console.WriteLine("=== INITIAL QUEST STATE ===");
                q1.Log();
                q2.Log();

                Console.WriteLine("\n=== GIẾT 3 QUÁI ===");
                user.KillEnemy();
                user.KillEnemy();
                user.KillEnemy();
                q1.Log();

                Console.WriteLine("\n=== NHẶT 7 COIN ===");
                user.CollectCoin(7);
                q2.Log();

                Console.WriteLine("\n=== GIẾT 2 QUÁI NỮA (đủ 5) ===");
                user.KillEnemy();
                user.KillEnemy();
                q1.Log();

                Console.WriteLine("\n=== NHẶT 5 COIN NỮA (đủ 10) ===");
                user.CollectCoin(5);
                q2.Log();

                Console.WriteLine("\n=== CHECK ALL QUEST COMPLETE ===");
                Console.WriteLine(user.CheckCompleteAllQuest()
                    ? "Tất cả quest đã hoàn thành!"
                    : "Vẫn còn quest chưa hoàn thành.");

                Console.WriteLine("\n=== END TEST QUEST SYSTEM ===");
            }
        }
    }
}
