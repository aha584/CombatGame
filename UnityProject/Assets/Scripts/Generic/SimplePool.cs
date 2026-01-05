//
// BÀI TẬP: GENERIC OBJECT POOL (HỆ THỐNG TÁI SỬ DỤNG ĐỐI TƯỢNG)
//
// Mục tiêu:
// 1. Hiểu lý do cần dùng Generic: Tránh viết lặp lại code cho BulletPool, EnemyPool, ParticlePool...
// 2. Sử dụng Generic Constraints: where T : new() và where T : Interface.
// 3. Quản lý bộ nhớ: Tái sử dụng object thay vì new liên tục (Tránh Garbage Collection).
//

using System;
using System.Collections.Generic;
using System.Threading;

namespace PoolingSystem
{
    // ==========================================
    // PHẦN 1: ĐỊNH NGHĨA HÀNH VI CHUNG
    // ==========================================

    /// <summary>
    /// Interface bắt buộc cho mọi object muốn tham gia vào Pool.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Hàm này được gọi khi lấy object ra khỏi Pool (Tương tự Start/Awake).
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Hàm này được gọi khi trả object về Pool (Reset lại máu, vị trí...).
        /// </summary>
        void OnDespawn();

        // Trạng thái để biết object này đang rảnh (trong pool) hay đang bận (đang dùng)
        bool IsActive { get; set; }
    }

    // ==========================================
    // PHẦN 2: LỚP QUẢN LÝ POOL (CORE GENERIC)
    // ==========================================

    /// <summary>
    /// Lớp quản lý việc tái sử dụng đối tượng kiểu T.
    /// YÊU CẦU QUAN TRỌNG:
    /// - T phải là một class (class)
    /// - T phải implement IPoolable (để gọi được OnSpawn, OnDespawn)
    /// - T phải có constructor rỗng (new()) để Pool có thể tự tạo mới lúc đầu.
    /// </summary>
    public class ObjectPool<T> where T : class, IPoolable, new()
    {
        // Hàng đợi chứa các object đang "ngủ" (chờ được dùng)
        private readonly Queue<T> _poolQueue = new Queue<T>();

        /// <summary>
        /// Lấy một object từ Pool ra dùng.
        /// </summary>
        public T Get()
        {
            T item;

            // TODO: Kiểm tra xem _poolQueue có phần tử nào không (Count > 0)?
            // 1. Nếu CÓ: Dequeue lấy ra một cái.
            // 2. Nếu KHÔNG: Phải tạo mới bằng lệnh 'new T()' (Do có ràng buộc new() nên làm được việc này).

            // TODO: Gọi item.OnSpawn() để thiết lập thông số ban đầu.
            // TODO: Gán item.IsActive = true.

            // TODO: Trả về item.
            if(_poolQueue.Count > 0)
            {
                item = _poolQueue.Dequeue();
                item.OnSpawn();
                item.IsActive = true;
                return item;
            }
            else
            {
                item = new T();
                item.OnSpawn();
                item.IsActive = true;
                return item;
            }
        }

        /// <summary>
        /// Trả object về Pool khi dùng xong.
        /// </summary>
        public void Return(T item)
        {
            // TODO: Gọi item.OnDespawn() để reset (lau sạch vết máu, reset đạn...).
            // TODO: Gán item.IsActive = false.
            // TODO: Cho item vào lại hàng đợi (_poolQueue.Enqueue).
            item.OnDespawn();
            item.IsActive = false;
            _poolQueue.Enqueue(item);
        }
    }

    // ==========================================
    // PHẦN 3: CÁC ĐỐI TƯỢNG CỤ THỂ
    // ==========================================

    public class Bullet : IPoolable
    {
        public bool IsActive { get; set; }
        public int ID { get; private set; }
        private static int _counter = 0;

        public Bullet()
        {
            ID = ++_counter; // Gán ID để dễ theo dõi
        }

        public void OnSpawn()
        {
            Console.WriteLine($"[Bullet {ID}] Bay ra khỏi nòng súng! (Reset vị trí)");
        }

        public void OnDespawn()
        {
            Console.WriteLine($"[Bullet {ID}] Thu hồi về băng đạn. (Reset sát thương)");
        }
    }

    public class Zombie : IPoolable
    {
        public bool IsActive { get; set; }
        public int HP { get; set; }

        public void OnSpawn()
        {
            HP = 100; // Reset máu khi sống lại
            Console.WriteLine($"[Zombie] Sống lại với 100 HP! GRRAAA!");
        }

        public void OnDespawn()
        {
            HP = 0;
            Console.WriteLine($"[Zombie] Chui xuống đất (Về pool).");
        }
    }

    // ==========================================
    // PHẦN 4: CHƯƠNG TRÌNH TEST
    // ==========================================

    class Program
    {
        static void Main(string[] args)
        {
            // Tạo 2 pool riêng biệt nhờ Generic
            ObjectPool<Bullet> bulletPool = new ObjectPool<Bullet>();
            ObjectPool<Zombie> zombiePool = new ObjectPool<Zombie>();

            Console.WriteLine("=== TEST 1: LẤY ĐẠN (KHI POOL RỖNG) ===");
            // Lúc này pool rỗng, nó sẽ tự new T() -> Bullet ID 1
            Bullet b1 = bulletPool.Get();
            Bullet b2 = bulletPool.Get(); // -> Bullet ID 2

            Console.WriteLine("\n=== TEST 2: TRẢ ĐẠN VỀ POOL ===");
            // Giả sử bắn xong
            bulletPool.Return(b1);
            // Lúc này b1 nằm trong Queue

            Console.WriteLine("\n=== TEST 3: TÁI SỬ DỤNG ===");
            // Lấy ra lại -> Nó phải trả về đúng cái b1 cũ, KHÔNG ĐƯỢC tạo mới Bullet ID 3
            Bullet b3 = bulletPool.Get();

            Console.WriteLine($"Check ID: Viên đạn vừa lấy ra có ID là {b3.ID} (Mong đợi: 1)");

            Console.WriteLine("\n=== TEST 4: ZOMBIE POOL ===");
            Zombie z1 = zombiePool.Get();
            z1.HP -= 50;
            Console.WriteLine($"Zombie HP: {z1.HP}");

            zombiePool.Return(z1); // Reset máu về 0 (theo logic OnDespawn)

            Zombie z2 = zombiePool.Get(); // Lấy lại con zombie đó -> OnSpawn set lại HP 100
            Console.WriteLine($"Zombie HP after respawn: {z2.HP} (Mong đợi: 100)");

            Console.ReadLine();
        }
    }
}