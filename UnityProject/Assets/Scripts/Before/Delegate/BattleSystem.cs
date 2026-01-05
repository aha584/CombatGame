//
// Hoàn thiện các TODO.
// MỤC TIÊU: Sử dụng Delegate (Func) để biến đổi dữ liệu qua nhiều bước (Pipeline Pattern).
// Hiểu cách luân chuyển giá trị từ Delegate này sang Delegate khác.
//

using System;
using System.Collections.Generic;

namespace BattleSystem
{
    // ==========================================
    // DATA MODELS
    // ==========================================

    public class Fighter
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int BaseAttack { get; set; }
        public int Defense { get; set; }
        
        // Thuộc tính để kiểm tra logic (VD: Có đang bị choáng, có buff không)
        public bool IsStunned { get; set; } = false;
        public bool IsPoweredUp { get; set; } = false; // Buff tăng sát thương

        public Fighter(string name, int hp, int atk, int def)
        {
            Name = name;
            Health = hp;
            BaseAttack = atk;
            Defense = def;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
            Console.WriteLine($"   -> {Name} bị trừ {amount} HP. Còn lại: {Health}");
        }
    }

    /// <summary>
    /// Context chứa thông tin về cuộc tấn công hiện tại.
    /// Delegate sẽ dùng thông tin này để quyết định xem có tăng/giảm damage không.
    /// </summary>
    public class AttackContext
    {
        public Fighter Attacker { get; }
        public Fighter Defender { get; }
        
        // Có thể thêm random để tính xác suất Crit
        public Random Rand { get; } = new Random(); 

        public AttackContext(Fighter attacker, Fighter defender)
        {
            Attacker = attacker;
            Defender = defender;
        }
    }

    // ==========================================
    // CORE DELEGATE SYSTEM
    // ==========================================

    /// <summary>
    /// Delegate dùng để biến đổi sát thương.
    /// Input: 
    ///   - currentDamage: Sát thương hiện tại (sau khi đã qua các bước xử lý trước).
    ///   - ctx: Thông tin ngữ cảnh (ai đánh ai).
    /// Output:
    ///   - Sát thương mới (int).
    /// </summary>
    public delegate int DamageModifier(int currentDamage, AttackContext ctx);

    /// <summary>
    /// Delegate dùng để gây hiệu ứng phụ sau khi đánh (Hút máu, Choáng...).
    /// </summary>
    public delegate void OnHitEffect(int finalDamage, AttackContext ctx);

    public class DamagePipeline
    {
        // Danh sách các bước tính toán sát thương (Chạy tuần tự)
        private List<DamageModifier> _modifiers = new List<DamageModifier>();

        // Danh sách các hiệu ứng xảy ra sau khi đánh
        private Action<int, AttackContext> _onHitEvents;

        /// <summary>
        /// Đăng ký một bước tính toán sát thương mới vào quy trình.
        /// </summary>
        public void RegisterModifier(DamageModifier modifier)
        {
            // TODO: Kiểm tra null và thêm vào list _modifiers.
            if(modifier != null)
            {
                _modifiers.Add(modifier);
            }
        }

        /// <summary>
        /// Đăng ký hiệu ứng sau đòn đánh (dùng Multicast Delegate).
        /// </summary>
        public void RegisterOnHitEffect(OnHitEffect effect)
        {
            // TODO:
            // - Chuyển đổi OnHitEffect thành Action<int, AttackContext> nếu cần,
            // - Hoặc dùng toán tử += để gán vào _onHitEvents.
            // Lưu ý: Cần xử lý trường hợp _onHitEvents đang null.
            if(_onHitEvents != null)
            {
                _onHitEvents += (int finalDamage, AttackContext ctx) => effect(finalDamage, ctx);
            }
            else
            {
                _onHitEvents(0, new AttackContext(new Fighter("",0,0,0), new Fighter("", 0, 0, 0)));
            }
        }

        /// <summary>
        /// Hàm tính toán và thực thi đòn đánh.
        /// Đây là trái tim của hệ thống.
        /// </summary>
        public void ExecuteAttack(Fighter attacker, Fighter defender)
        {
            Console.WriteLine($"\n[BATTLE] {attacker.Name} tấn công {defender.Name}!");

            // 1. Tạo Context
            var ctx = new AttackContext(attacker, defender);

            // 2. Sát thương khởi điểm = BaseAttack của người đánh
            int damage = attacker.BaseAttack;

            // TODO: PIPELINE PROCESS
            // - Duyệt qua từng modifier trong danh sách _modifiers.
            // - Với mỗi modifier, gọi hàm: damage = modifier(damage, ctx);
            // - Ý nghĩa: Output của bước trước là Input của bước sau.
            // VD: 100 -> (Trừ giáp 10) -> 90 -> (Crit x2) -> 180.
            
            // Code here...
            foreach(var modifier in _modifiers)
            {
                damage = modifier(damage, ctx);
            }

            // Đảm bảo damage không âm
            if (damage < 0) damage = 0;

            Console.WriteLine($"   -> Sát thương cuối cùng: {damage}");

            // 3. Trừ máu nạn nhân
            defender.TakeDamage(damage);

            // TODO: TRIGGER ON-HIT EVENTS
            // - Gọi _onHitEvents (nếu khác null), truyền vào damage cuối và ctx.

            // Code here...
            if (_onHitEvents != null)
            {
                _onHitEvents(damage, ctx);
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
            // Setup đấu sĩ
            Fighter hero = new Fighter("Hero", hp: 500, atk: 50, def: 10);
            Fighter boss = new Fighter("Demon King", hp: 1000, atk: 80, def: 20);

            // Setup hệ thống tính damage
            DamagePipeline pipeline = new DamagePipeline();

            // ---------------------------------------------------------
            // BÀI TẬP 1: Đăng ký các quy tắc tính Sát Thương (Logic)
            // ---------------------------------------------------------

            // 1. Quy tắc Giáp: Damage giảm đi bằng Defense của đối thủ.
            pipeline.RegisterModifier((dmg, ctx) => 
            {
                // TODO: Tính dmg - ctx.Defender.Defense. Trả về kết quả mới.
                // Log ra console: "   [Logic] Trừ giáp..."
                dmg -= ctx.Defender.Defense;
                Console.WriteLine("[Logic] Trừ giáp...");
                return dmg; // Sửa dòng này
            });

            // 2. Quy tắc Buff sức mạnh: Nếu Attacker.IsPoweredUp == true thì Damage + 50%.
            pipeline.RegisterModifier((dmg, ctx) => 
            {
                // TODO: Kiểm tra ctx.Attacker.IsPoweredUp.
                // Nếu true: trả về dmg * 1.5 (ép kiểu int).
                // Nếu false: giữ nguyên dmg.
                if (ctx.Attacker.IsPoweredUp)
                {
                    return dmg = (int)(dmg * 1.5f);
                }
                else
                {
                    return dmg; // Sửa dòng này
                }
            });

            // 3. Quy tắc Chí mạng (Critical): Có 30% cơ hội x2 Damage.
            pipeline.RegisterModifier((dmg, ctx) => 
            {
                // TODO: Dùng ctx.Rand.Next(0, 100) < 30.
                // Nếu trúng: In ra "   [CRITICAL!]" và trả về dmg * 2.
                if(ctx.Rand.Next(0,100) < 30)
                {
                    Console.WriteLine("   [CRITICAL!]");
                    return dmg *= 2;
                }
                else
                {
                    return dmg; // Sửa dòng này
                }
            });

            // ---------------------------------------------------------
            // BÀI TẬP 2: Đăng ký hiệu ứng sau đòn đánh (Side Effects)
            // ---------------------------------------------------------

            // 1. Hút máu (Lifesteal): Hồi phục cho người đánh bằng 10% sát thương gây ra.
            pipeline.RegisterOnHitEffect((finalDmg, ctx) => 
            {
                // TODO: Tính healAmount = finalDmg * 0.1.
                // Cộng máu cho ctx.Attacker.Health.
                // In ra: "   [Effect] Hút máu..."

                float healAmount = finalDmg * 0.1f;
                ctx.Attacker.Health += (int)healAmount;
                Console.WriteLine("[Effect] Hút máu...");
            });

            // 2. Phản đòn (Thorn Mail): Nếu Boss bị đánh, Hero bị phản lại 5 sát thương cố định.
            pipeline.RegisterOnHitEffect((finalDmg, ctx) => 
            {
                // TODO: Nếu ctx.Defender.Name == "Demon King", trừ 5 HP của ctx.Attacker.
                // In ra: "   [Effect] Boss phản đòn!"
                if(ctx.Defender.Name == "Demon King")
                {
                    ctx.Attacker.Health -= 5;
                    Console.WriteLine("[Effect] Boss phản đòn!");
                }
            });


            // =========================================================
            // SIMULATION
            // =========================================================

            // --- Round 1: Đánh thường ---
            pipeline.ExecuteAttack(hero, boss);
            // Mong đợi: 50 (Atk) - 20 (Def) = 30 dmg. (Có thể Crit nếu hên).
            // Hero hồi 3 HP (10% của 30).
            // Hero bị phản 5 HP.

            Console.WriteLine("--------------------------------");

            // --- Round 2: Hero bật Buff ---
            Console.WriteLine("Hero bật Buff sức mạnh!");
            hero.IsPoweredUp = true;
            pipeline.ExecuteAttack(hero, boss);
            // Mong đợi: (50 - 20) = 30 -> Buff +50% = 45 dmg.
            // Nếu Crit: 45 * 2 = 90 dmg.

            Console.WriteLine("--------------------------------");

            // --- Round 3: Boss đánh Hero ---
            pipeline.ExecuteAttack(boss, hero);
            // Mong đợi: 80 (Atk) - 10 (Def) = 70 dmg.
            // Boss hút 7 máu.
            // Boss KHÔNG bị phản đòn (vì logic phản đòn chỉ check Defender tên Demon King).

            Console.ReadKey();
        }
    }
}