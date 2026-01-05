//
// Hoàn thiện các TODO.
// MỤC TIÊU: Sử dụng Delegate (Func, Action) để tách biệt Logic nhiệm vụ và Dữ liệu game.
// Không dùng if-else cứng để kiểm tra nhiệm vụ. Mọi logic phải được "nhúng" (inject) từ bên ngoài.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestSystem
{
    // ==========================================
    // DATA MODELS
    // ==========================================

    /// <summary>
    /// Class đại diện cho Người chơi.
    /// Chứa các chỉ số để hệ thống nhiệm vụ kiểm tra.
    /// </summary>
    public class Player
    {
        public string Name { get; set; }
        public int Level { get; private set; } = 1;
        public int Gold { get; private set; } = 0;
        public int EnemiesKilled { get; private set; } = 0;
        public List<string> Inventory { get; private set; } = new List<string>();

        // Danh sách ID các nhiệm vụ đã hoàn thành (để check nhiệm vụ chuỗi)
        public List<string> CompletedQuestIds { get; private set; } = new List<string>();

        public Player(string name) => Name = name;

        // Các hàm thay đổi chỉ số (để test)
        public void LevelUp() { Level++; Console.WriteLine($"[Game] Level up! Now level {Level}."); }
        public void AddGold(int amount) { Gold += amount; Console.WriteLine($"[Game] Got {amount} gold. Total: {Gold}."); }
        public void KillEnemy() { EnemiesKilled++; Console.WriteLine($"[Game] Enemy killed. Total: {EnemiesKilled}."); }
        public void AddItem(string item) { Inventory.Add(item); Console.WriteLine($"[Game] Obtained item: {item}."); }
        
        public void MarkQuestCompleted(string questId) 
        {
            if (!CompletedQuestIds.Contains(questId)) CompletedQuestIds.Add(questId);
        }
    }

    // ==========================================
    // CORE DELEGATE SYSTEM
    // ==========================================

    /// <summary>
    /// Class đại diện cho một Nhiệm vụ.
    /// Thay vì viết class con (Inheritance), ta dùng Delegate (Composition) để định nghĩa hành vi.
    /// </summary>
    public class Quest
    {
        public string Id { get; }
        public string Description { get; }
        public bool IsCompleted { get; private set; } = false;

        // -----------------------------------------------------------------------
        // TODO: Định nghĩa Delegate
        // 1. Requirement: Hàm nhận vào Player, trả về bool. (Dùng Func hoặc Predicate)
        //    Dùng để kiểm tra xem Player đã đủ điều kiện xong nhiệm vụ chưa.
        // 2. Reward: Hàm nhận vào Player, không trả về gì (void). (Dùng Action)
        //    Dùng để trao thưởng khi xong nhiệm vụ.
        // -----------------------------------------------------------------------
        
        public Func<Player, bool> Requirement { get; set; }
        public Action<Player> Reward { get; set; }

        // Callback sự kiện: Được gọi ngay sau khi Quest chuyển trạng thái sang Completed.
        // Dùng cho UI, Âm thanh, Log... (Multicast Delegate)
        public Action<string> OnQuestCompletedEvent; 

        public Quest(string id, string description)
        {
            Id = id;
            Description = description;
        }

        /// <summary>
        /// Hàm kiểm tra trạng thái nhiệm vụ.
        /// </summary>
        public void CheckStatus(Player player)
        {
            if (IsCompleted) return; // Đã xong thì không check nữa.

            // TODO: 
            // 1. Kiểm tra delegate Requirement. Nếu Requirement == null, coi như chưa đạt.
            // 2. Gọi Requirement(player). Nếu trả về true:
            //    a. Đánh dấu IsCompleted = true.
            //    b. Gọi delegate Reward(player) (nếu có).
            //    c. Đánh dấu vào player (player.MarkQuestCompleted(Id)).
            //    d. Gọi OnQuestCompletedEvent(Id) (nếu có) để thông báo ra bên ngoài.
            //    e. In log: "[Quest] Completed: {Description}"
            
            if (Requirement(player))
            {
                IsCompleted = true;
                Reward(player);
                player.MarkQuestCompleted(Id);
                OnQuestCompletedEvent(Id);
                Console.WriteLine($"[Quest] Completed: {Description}");
            }
        }
    }

    // ==========================================
    // SYSTEM MANAGER
    // ==========================================

    public class QuestManager
    {
        private List<Quest> _quests = new List<Quest>();

        public void RegisterQuest(Quest quest)
        {
            // TODO: Kiểm tra null và thêm vào list.
            if (quest != null) _quests.Add(quest);
        }

        /// <summary>
        /// Quét toàn bộ danh sách nhiệm vụ xem cái nào đã hoàn thành.
        /// </summary>
        public void Update(Player player)
        {
            Console.WriteLine("\n--- Checking Quests ---");
            // TODO: Duyệt qua danh sách _quests và gọi hàm CheckStatus() của từng quest.
            foreach(var quest in _quests)
            {
                quest.CheckStatus(player);
            }
        }

        /// <summary>
        /// Helper function để in danh sách nhiệm vụ chưa xong.
        /// </summary>
        public void ShowActiveQuests()
        {
            Console.WriteLine("Active Quests:");
            // TODO: Dùng LINQ hoặc foreach in ra Description của các Quest chưa Completed.
            List<Quest> notCompleteQuest = _quests.Where(x => !x.IsCompleted).ToList();
            foreach(var quest in notCompleteQuest)
            {
                Console.WriteLine($"Not Complete Quest: {quest.Description}");
            }
        }
    }

    // ==========================================
    // TEST PROGRAM
    // ==========================================

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== DYNAMIC QUEST SYSTEM WITH DELEGATES ===");

            Player player = new Player("Hero");
            QuestManager questManager = new QuestManager();

            // ---------------------------------------------------------
            // BÀI TẬP 1: Tạo Nhiệm vụ cơ bản
            // "Newbie Killer": Yêu cầu giết 3 quái. Thưởng 100 vàng.
            // ---------------------------------------------------------
            Quest q1 = new Quest("Q01", "Giết 3 con quái vật");
            
            // TODO: Viết Lambda Expression cho Requirement: Trả về true nếu player.EnemiesKilled >= 3
            q1.Requirement = player => player.EnemiesKilled >= 3;

            // TODO: Viết Lambda Expression cho Reward: player.AddGold(100)
            q1.Reward = player => player.AddGold(100);

            // Đăng ký listener cho sự kiện (Multicast)
            q1.OnQuestCompletedEvent += (id) => Console.WriteLine($" >> UI Notification: Nhiệm vụ {id} hoàn tất!");
            q1.OnQuestCompletedEvent += (id) => Console.WriteLine($" >> Audio: Play Sound 'Ding Ding'!");

            questManager.RegisterQuest(q1);


            // ---------------------------------------------------------
            // BÀI TẬP 2: Tạo Nhiệm vụ kết hợp điều kiện
            // "Rich & Strong": Yêu cầu Level >= 2 VÀ Gold >= 150.
            // Thưởng: 1 Item "Sword".
            // ---------------------------------------------------------
            Quest q2 = new Quest("Q02", "Đạt Level 2 và sở hữu 150 vàng");

            // TODO: Viết Lambda Expression phức hợp cho Requirement.
            q2.Requirement = player => player.Level >= 2 && player.Gold >= 150;

            // TODO: Viết Lambda cho Reward: AddItem("Legendary Sword").
            q2.Reward = player => player.AddItem("Legendary Sword");

            questManager.RegisterQuest(q2);


            // ---------------------------------------------------------
            // BÀI TẬP 3: Nhiệm vụ chuỗi (Chain Quest)
            // "Veteran": Phải hoàn thành Q01 rồi VÀ giết thêm 2 quái nữa (Tổng 5).
            // Gợi ý: Kiểm tra player.CompletedQuestIds.Contains("Q01").
            // ---------------------------------------------------------
            Quest q3 = new Quest("Q03", "Hoàn thành nhiệm vụ Q01 và giết tổng 5 quái");

            // TODO: Viết Lambda Expression
            q3.Requirement = player => q1.IsCompleted && player.EnemiesKilled >= 5;

            // TODO: Reward: LevelUp() cho player.
            q3.Reward = player => player.LevelUp();

            questManager.RegisterQuest(q3);


            // =========================================================
            // SIMULATION LOOP
            // =========================================================
            
            questManager.ShowActiveQuests();

            // --- Turn 1: Giết 2 quái ---
            player.KillEnemy();
            player.KillEnemy();
            questManager.Update(player); // Chưa xong cái nào

            // --- Turn 2: Giết thêm 1 quái (Tổng 3) ---
            player.KillEnemy();
            questManager.Update(player); 
            // Mong đợi: Q01 xong. Reward +100 Gold (Total 100).
            // Q02 chưa xong (Gold 100 < 150, Level 1 < 2).
            // Q03 chưa xong (Cần 5 quái).

            // --- Turn 3: Nhặt được vàng ---
            player.AddGold(60); // Total 160.
            player.LevelUp();   // Level 2.
            questManager.Update(player);
            // Mong đợi: Q02 xong (Level 2, Gold 160). Reward +Sword.

            // --- Turn 4: Giết thêm 2 quái (Tổng 5) ---
            player.KillEnemy();
            player.KillEnemy();
            questManager.Update(player);
            // Mong đợi: Q03 xong (Q01 đã xong từ trước + 5 kill). Reward +Level.

            Console.WriteLine("\n=== END SIMULATION ===");
            Console.WriteLine($"Final Stats: Level {player.Level}, Gold {player.Gold}, Items: {string.Join(", ", player.Inventory)}");
            Console.ReadKey();
        }
    }
}