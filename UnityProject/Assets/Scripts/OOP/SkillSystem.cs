//
// Hoàn thiện tất cả các TODO và thay thế toàn bộ throw exception.
// Yêu cầu sinh viên áp dụng OOP: interface, trạng thái, collection, flow thời gian (deltaTime).
//

using System;
using System.Collections.Generic;
using TMPro;

namespace SkillSystem
{
    /// <summary>
    /// Interface đại diện cho một Skill.
    /// Mỗi Skill có:
    /// - Tên (Name)
    /// - Thời gian hồi chiêu (Cooldown)
    /// - Trạng thái sẵn sàng (IsReady)
    /// - Hàm Use() để kích hoạt
    /// - Hàm Tick(deltaTime) để cập nhật thời gian hồi
    /// - Hàm Log() để in thông tin skill
    /// </summary>
    public interface ISkill
    {
        string Name { get; }
        float Cooldown { get; }
        bool IsReady { get; }

        void Use();
        void Tick(float deltaTime);
        void Log();
    }

    /// <summary>
    /// Skill: Fireball.
    /// Yêu cầu:
    /// - Có Cooldown cố định (ví dụ do constructor truyền vào).
    /// - Khi Use():
    ///     + Nếu IsReady == false: không cho cast, có thể in cảnh báo.
    ///     + Nếu IsReady == true: thực hiện cast và reset thời gian hồi chiêu.
    /// - Tick(deltaTime):
    ///     + Giảm thời gian hồi hiện tại.
    ///     + Không cho nhỏ hơn 0.
    /// </summary>
    public class FireballSkill : ISkill
    {
        // TODO: Thêm biến private _cooldown, _currentCooldown
        private float _currentCooldown;
        private float _cooldown;

        public string Name => "Fireball";

        public float Cooldown => _cooldown;      // TODO
        public bool IsReady => _currentCooldown <= 0;        // TODO

        public FireballSkill(float cooldown)
        {
            // TODO: Gán giá trị cooldown, kiểm tra cooldown > 0
            if(cooldown > 0)
            {
                _cooldown = cooldown;
            }
        }

        public void Use()
        {
            // TODO:
            // - Nếu chưa sẵn sàng: in thông báo là skill đang hồi chiêu.
            // - Nếu sẵn sàng: in "Cast Fireball", đặt _currentCooldown = _cooldown.
            if(!IsReady)
            {
                Console.WriteLine("Dang hoi chieu");
            }
            else
            {
                Console.WriteLine("Cast Fireball");
                _currentCooldown = _cooldown;
            }
        }

        public void Tick(float deltaTime)
        {
            // TODO:
            // - Giảm _currentCooldown theo deltaTime.
            // - Không để _currentCooldown < 0.
            _currentCooldown -= deltaTime;
            if(_currentCooldown <= 0)
            {
                _currentCooldown = 0;
            }
        }

        public void Log()
        {
            // TODO: In trạng thái: "Fireball - Ready" hoặc "Fireball - Cooldown: x.x s"
            if(!IsReady)
            {
                Console.WriteLine($"Fireball - Cooldown: {_currentCooldown}");
            }
            else
            {
                Console.WriteLine("Fireball - Ready");
            }
        }
    }

    /// <summary>
    /// Skill: Heal.
    /// Logic tương tự Fireball nhưng có thể in log khác khi Use().
    /// </summary>
    public class HealSkill : ISkill
    {
        // TODO: Biến private _cooldown, _currentCooldown
        private float _currentCooldown;
        private float _cooldown;

        public string Name => "Heal";

        public float Cooldown => _cooldown;
        public bool IsReady => _currentCooldown <= 0;

        public HealSkill(float cooldown)
        {
            // TODO: Gán giá trị cooldown, kiểm tra cooldown > 0
            if (cooldown > 0)
            {
                _cooldown = cooldown;
            }
        }

        public void Use()
        {
            // TODO:
            // - Nếu sẵn sàng: in "Cast Heal", reset _currentCooldown.
            // - Nếu không: in "Heal is on cooldown".
            if (!IsReady)
            {
                Console.WriteLine("Heal is on cooldown");
            }
            else
            {
                Console.WriteLine("Cast Heal");
                _currentCooldown = _cooldown;
            }
        }

        public void Tick(float deltaTime)
        {
            // TODO: Giảm _currentCooldown, không cho < 0.
            _currentCooldown -= deltaTime;
            if(_currentCooldown <= 0)
            {
                _currentCooldown = 0;
            }
        }

        public void Log()
        {
            // TODO: In trạng thái tương tự Fireball, nhưng tên skill là Heal.
            if (!IsReady)
            {
                Console.WriteLine($"Heal - Cooldown: {_currentCooldown}");
            }
            else
            {
                Console.WriteLine("Heal - Ready");
            }
        }
    }

    /// <summary>
    /// Lớp Player quản lý các skill.
    /// Yêu cầu:
    /// - Có thể thêm skill mới (AddSkill).
    /// - Tick tất cả skill mỗi frame (TickAllSkills).
    /// - Thử dùng 1 skill cụ thể theo tên (UseSkillByName).
    /// - In trạng thái tất cả skill (LogAllSkills).
    /// </summary>
    public class Player
    {
        private readonly List<ISkill> _skills = new();

        /// <summary>
        /// Thêm một skill vào danh sách.
        /// </summary>
        public void AddSkill(ISkill skill)
        {
            // TODO:
            // - Kiểm tra null.
            // - Thêm skill vào _skills.
            if(skill != null)
            {
                _skills.Add(skill);
            }
        }

        /// <summary>
        /// Cập nhật thời gian hồi chiêu cho tất cả skill.
        /// </summary>
        public void TickAllSkills(float deltaTime)
        {
            // TODO:
            // - Duyệt _skills và gọi skill.Tick(deltaTime).
            foreach(ISkill skill in _skills)
            {
                skill.Tick(deltaTime);
            }
        }

        /// <summary>
        /// Tìm skill theo tên và kích hoạt.
        /// Ví dụ: UseSkillByName("Fireball").
        /// </summary>
        public void UseSkillByName(string skillName)
        {
            // TODO:
            // - Duyệt _skills.
            // - Tìm skill có Name trùng skillName (so sánh chuỗi).
            // - Nếu tìm thấy: gọi Use() và return.
            // - Nếu không tìm thấy: in thông báo không có skill đó.
            foreach(ISkill skill in _skills)
            {
                if(skill.Name == skillName)
                {
                    skill.Use();
                    return;
                }
            }
            Console.WriteLine($"Can't find {skillName} in skill lists!!");
        }

        /// <summary>
        /// In log toàn bộ skill của người chơi.
        /// </summary>
        public void LogAllSkills()
        {
            // TODO:
            // - Duyệt _skills, gọi skill.Log().
            foreach (ISkill skill in _skills)
            {
                skill.Log();
            }
        }

        /// <summary>
        /// Kiểm tra xem có ít nhất một skill đang sẵn sàng hay không.
        /// </summary>
        public bool HasAnyReadySkill()
        {
            // TODO:
            // - Trả về true nếu tồn tại skill.IsReady == true.
            int readyCount = 0;
            foreach (ISkill skill in _skills)
            {
                if (skill.IsReady) readyCount++;
            }
            if (readyCount == _skills.Count) return true;
            return false;
        }
    }

    class Program
{
    static void Main(string[] args)
    {
        // Tạo player và các skill
        Player player = new Player();
        player.AddSkill(new FireballSkill(3f)); // cooldown 3 giây
        player.AddSkill(new HealSkill(5f));     // cooldown 5 giây

        Console.WriteLine("=== INITIAL STATE ===");
        player.LogAllSkills();

        Console.WriteLine("\n=== CAST FIREBALL ===");
        player.UseSkillByName("Fireball");

        Console.WriteLine("\n=== CAST FIREBALL LẦN 2 (đang cooldown) ===");
        player.UseSkillByName("Fireball");

        // Tick 1 giây
        Console.WriteLine("\n=== TICK 1s ===");
        player.TickAllSkills(1f);
        player.LogAllSkills();

        // Tick thêm 3 giây (tổng 4s) → Fireball đã sẵn sàng
        Console.WriteLine("\n=== TICK 3s ===");
        player.TickAllSkills(3f);
        player.LogAllSkills();

        Console.WriteLine("\n=== CAST HEAL ===");
        player.UseSkillByName("Heal");

        Console.WriteLine("\n=== CHECK READY SKILL ===");
        Console.WriteLine(player.HasAnyReadySkill()
            ? "Có skill đang sẵn sàng"
            : "Không có skill nào sẵn sàng");

        Console.WriteLine("\n=== END TEST SKILL SYSTEM ===");
    }
}
}
