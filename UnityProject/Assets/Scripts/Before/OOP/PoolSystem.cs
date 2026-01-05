using System;
using System.Collections.Generic;

namespace OopExercises.PoolSystem
{
    // YÊU CẦU:
    // - Xây dựng hệ thống Object Pool để tái sử dụng object thay vì tạo/destroy liên tục.
    // - Có PoolManager quản lý nhiều Pool khác nhau (ví dụ: Bullet, Enemy, Effect).
    // - Mỗi Pool có:
    //     + Key (tên pool)
    //     + Factory để tạo object mới khi hết object rảnh
    //     + Danh sách object đang rảnh (inactive)
    //     + Danh sách object đang sử dụng (active)
    //
    // MỤC TIÊU KIẾN THỨC:
    // - Interface, abstract class, kế thừa.
    // - Đóng gói (encapsulation) hệ thống phức tạp.

    // ------------------------------------------------------------
    // 1. Interface IPoolable: mô tả object có thể đưa vào pool.
    // ------------------------------------------------------------
    public interface IPoolable
    {
        // Tên object (dùng để debug / log).
        string Name { get; }

        // Trạng thái đang được sử dụng hay không.
        bool IsActive { get; }

        // Được gọi khi object được lấy ra từ pool (spawn).
        void OnSpawn();

        // Được gọi khi object được trả về pool (despawn).
        void OnDespawn();
    }

    // ------------------------------------------------------------
    // 2. BasePoolable: implement sẵn một phần logic IPoolable.
    // ------------------------------------------------------------
    public abstract class BasePoolable : IPoolable
    {
        private readonly string _name;
        private bool _isActive;

        public string Name { get { return _name; } }
        public bool IsActive { get { return _isActive; } }

        protected BasePoolable(string name)
        {
            // TODO:
            // - Kiểm tra name null/empty.
            // - Gán _name.
            // - Đặt _isActive = false (vì mới tạo xong đang ở trạng thái rảnh).
            if (!string.IsNullOrEmpty(name)) _name = name;
            _isActive = false;
        }

        // Hàm này sẽ được Pool gọi khi spawn.
        public void OnSpawn()
        {
            // TODO:
            // - Đặt _isActive = true.
            // - Gọi hàm ảo OnSpawnInternal() cho class con override.
            _isActive = true;
            OnSpawnInternal();
        }

        // Hàm này sẽ được Pool gọi khi despawn.
        public void OnDespawn()
        {
            // TODO:
            // - Đặt _isActive = false.
            // - Gọi hàm ảo OnDespawnInternal() cho class con override.
            _isActive = false;
            OnDespawnInternal();
        }

        // Các class con override hai hàm này để mô phỏng hành vi cụ thể.
        protected abstract void OnSpawnInternal();
        protected abstract void OnDespawnInternal();
    }

    // ------------------------------------------------------------
    // 3. IObjectFactory: cách tạo ra IPoolable mới
    // ------------------------------------------------------------
    public interface IObjectFactory
    {
        IPoolable Create();
    }

    // ------------------------------------------------------------
    // 4. Ví dụ domain: Bullet, Enemy
    // ------------------------------------------------------------

    public sealed class Bullet : BasePoolable
    {
        private int _damage;

        public int Damage { get { return _damage; } }

        public Bullet(int damage) : base("Bullet")
        {
            // TODO:
            // - Nếu damage < 0 ép về 0.
            // - Gán _damage.
            if (damage < 0) damage = 0;
            _damage = damage;
        }

        protected override void OnSpawnInternal()
        {
            // TODO:
            // - Mô phỏng reset trạng thái bullet, ví dụ:
            //   Console.WriteLine("Bullet spawned with damage " + _damage);
            Console.WriteLine("Bullet spawned with damage " + _damage);
        }

        protected override void OnDespawnInternal()
        {
            // TODO:
            // - Mô phỏng despawn, ví dụ:
            //   Console.WriteLine("Bullet despawned");
            Console.WriteLine("Bullet despawned");
        }
    }

    public sealed class Enemy : BasePoolable
    {
        private string _enemyType;
        private int _health;

        public string EnemyType { get { return _enemyType; } }
        public int Health { get { return _health; } }

        public Enemy(string enemyType, int health) : base("Enemy")
        {
            // TODO:
            // - Nếu enemyType null/empty => gán giá trị mặc định, ví dụ "DefaultEnemy".
            // - Nếu health < 0 => ép về 0.
            // - Gán _enemyType, _health.
            if(!string.IsNullOrEmpty(enemyType))
            {
                _enemyType = enemyType;
            }
            else
            {
                _enemyType = "DefaultEnemy";
            }
            if (health < 0) health = 0;
            _health = health;
        }

        protected override void OnSpawnInternal()
        {
            // TODO:
            // - Mô phỏng spawn enemy: reset máu, in log.
            //   Console.WriteLine("Enemy spawned: type=" + _enemyType + ", health=" + _health);
            Console.WriteLine("Enemy spawned: type=" + _enemyType + ", health=" + _health);
        }

        protected override void OnDespawnInternal()
        {
            // TODO:
            // - Mô phỏng despawn enemy, in log.
            Console.WriteLine("Enemy despawned");
        }
    }

    // ------------------------------------------------------------
    // 5. Factory cụ thể cho Bullet, Enemy
    // ------------------------------------------------------------

    public sealed class BulletFactory : IObjectFactory
    {
        private readonly int _defaultDamage;

        public BulletFactory(int defaultDamage)
        {
            // TODO:
            // - Nếu defaultDamage < 0 => ép về 0.
            // - Gán _defaultDamage.
            if (defaultDamage < 0) defaultDamage = 0;
            _defaultDamage = defaultDamage;
        }

        public IPoolable Create()
        {
            // TODO:
            // - Tạo Bullet mới với _defaultDamage và trả về.
            Bullet newBullet = new Bullet(_defaultDamage);
            return newBullet;
        }
    }

    public sealed class EnemyFactory : IObjectFactory
    {
        private readonly string _enemyType;
        private readonly int _defaultHealth;

        public EnemyFactory(string enemyType, int defaultHealth)
        {
            // TODO:
            // - Kiểm tra enemyType null/empty => gán giá trị mặc định.
            // - Nếu defaultHealth < 0 => ép về 0.
            // - Gán field.
            if (!string.IsNullOrEmpty(enemyType)) _enemyType = enemyType;
            else _enemyType = "DefaultEnemy";
            if (defaultHealth < 0) defaultHealth = 0;
            _defaultHealth = defaultHealth;
        }

        public IPoolable Create()
        {
            // TODO:
            // - Tạo Enemy mới với _enemyType, _defaultHealth.
            Enemy newEnemy = new Enemy(_enemyType, _defaultHealth);
            return newEnemy;
        }
    }

    // ------------------------------------------------------------
    // 6. Class Pool: quản lý một loại object (một key).
    // ------------------------------------------------------------
    public sealed class Pool
    {
        private readonly string _key;
        private readonly IObjectFactory _factory;

        // Object rảnh (inactive) có thể spawn.
        private readonly List<IPoolable> _inactive = new ();

        // Object đang được sử dụng (active).
        private readonly List<IPoolable> _active = new ();

        private readonly int _initialSize;
        private readonly bool _canExpand;

        public string Key { get { return _key; } }

        public int InactiveCount { get { return _inactive.Count; } }
        public int ActiveCount { get { return _active.Count; } }

        public Pool(string key, IObjectFactory factory, int initialSize, bool canExpand)
        {
            // TODO:
            // - Kiểm tra key null/empty.
            // - Kiểm tra factory null.
            // - Nếu initialSize < 0 => ép về 0.
            // - Gán các field.
            // - Gọi Prewarm() để tạo sẵn initialSize object.
            if (!string.IsNullOrEmpty(key)) _key = key;
            if (factory != null) _factory = factory;
            if (initialSize < 0) initialSize = 0;
            _initialSize = initialSize;
            _canExpand = canExpand;
            Prewarm();
        }

        // Tạo sẵn object ban đầu.
        private void Prewarm()
        {
            // TODO:
            // - Tạo _initialSize object bằng _factory.Create() và đưa vào _inactive.
            _inactive.Add(_factory.Create());
        }

        // Lấy object ra để dùng.
        public IPoolable Spawn()
        {
            // TODO:
            // - Nếu _inactive còn object:
            //     + Lấy object đầu tiên (index 0), remove khỏi _inactive.
            //   Ngược lại:
            //     + Nếu _canExpand == false => return null (hết object, không tạo thêm).
            //     + Nếu _canExpand == true => tạo object mới bằng _factory.Create().
            // - Thêm object vào _active.
            // - Gọi obj.OnSpawn().
            // - Trả về obj.
            if(_inactive.Count > 0)
            {
                IPoolable obj = _inactive[0];
                _inactive.RemoveAt(0);
                return obj;
            }
            else
            {
                if (!_canExpand) return null;
                else
                {
                    IPoolable obj = _factory.Create();
                    _active.Add(obj);
                    obj.OnSpawn();
                    return obj;
                }
            }
        }

        // Trả object về pool.
        public void Despawn(IPoolable obj)
        {
            // TODO:
            // - Kiểm tra obj null.
            // - Kiểm tra obj có nằm trong _active không:
            //     + Nếu không nằm trong _active => có thể bỏ qua hoặc ném exception (tuỳ bạn).
            // - Nếu có:
            //     + Remove khỏi _active.
            //     + Gọi obj.OnDespawn().
            //     + Add vào _inactive.
            if(obj != null)
            {
                if (!_active.Contains(obj))
                {
                    return;
                }
                else
                {
                    _active.Remove(obj);
                    obj.OnDespawn();
                    _inactive.Remove(obj);
                }
            }
        }

        // In ra trạng thái pool.
        public void PrintStatus()
        {
            // TODO:
            // - In ra Key, ActiveCount, InactiveCount.
            Console.WriteLine($"{Key}, {_active.Count}, {_inactive.Count}");
        }
    }

    // ------------------------------------------------------------
    // 7. PoolManager: quản lý nhiều Pool khác nhau (Bullet, Enemy, Effect, ...).
    // ------------------------------------------------------------
    public sealed class PoolManager
    {
        private readonly Dictionary<string, Pool> _pools = new();

        // Đăng ký một pool mới.
        public void RegisterPool(string key, IObjectFactory factory, int initialSize, bool canExpand)
        {
            // TODO:
            // - Kiểm tra key null/empty.
            // - Kiểm tra factory null.
            // - Nếu _pools đã chứa key này => có thể ném exception hoặc bỏ qua (tuỳ bạn).
            // - Tạo Pool mới.
            // - Add vào _pools với key.
            if(!string.IsNullOrEmpty(key) && factory != null)
            {
                if (_pools.ContainsKey(key)) return;
                else
                {
                    Pool pool = new Pool(key, factory, initialSize, canExpand);
                    _pools.Add(pool.Key, pool);
                }
            }
        }

        // Lấy pool theo key (nếu không có thì trả về null).
        public Pool GetPool(string key)
        {
            // TODO:
            // - Nếu _pools không chứa key => return null.
            // - Lấy object tương ứng và ép kiểu về Pool.
            if (!_pools.ContainsKey(key)) return null;
            else
            {
                Pool pool = _pools[key];
                return pool;
            }
        }

        // Spawn từ pool theo key.
        public IPoolable Spawn(string key)
        {
            // TODO:
            // - Lấy Pool bằng GetPool.
            // - Nếu null => return null.
            // - Gọi pool.Spawn() và trả về.
            Pool myPool = GetPool(key);
            if(myPool != null)
            {
                return myPool.Spawn();
            }
            else
            {
                return null;
            }
        }

        // Despawn một object về pool tương ứng.
        // LƯU Ý: Ở đây ta không biết object thuộc pool nào, nên yêu cầu truyền thêm key.
        public void Despawn(string key, IPoolable obj)
        {
            // TODO:
            // - Lấy Pool.
            // - Nếu pool != null => gọi pool.Despawn(obj).
            Pool pool = GetPool(key);
            if (pool != null) pool.Despawn(obj);

        }

        // In trạng thái tất cả pool.
        public void PrintAllStatus()
        {
            // TODO:
            // - Duyệt qua _pools.Values.
            // - Ép kiểu từng phần tử về Pool và gọi PrintStatus().
            foreach(Pool pool in _pools.Values)
            {
                pool.PrintStatus();
            }
        }
    }

    public class Program
    {
        static void Main()
        {
            PoolManager poolManager = new PoolManager();

            // Đăng ký pool cho Bullet (ví dụ 5 viên, cho phép expand).
            IObjectFactory bulletFactory = new BulletFactory(defaultDamage: 10);
            poolManager.RegisterPool("BulletPool", bulletFactory, initialSize: 5, canExpand: true);

            // Đăng ký pool cho Enemy (ví dụ 3 con, không cho expand).
            IObjectFactory enemyFactory = new EnemyFactory("Goblin", defaultHealth: 100);
            poolManager.RegisterPool("EnemyPool", enemyFactory, initialSize: 3, canExpand: false);

            // Spawn vài Bullet.
            IPoolable b1 = poolManager.Spawn("BulletPool");
            IPoolable b2 = poolManager.Spawn("BulletPool");
            IPoolable b3 = poolManager.Spawn("BulletPool");

            // Spawn vài Enemy.
            IPoolable e1 = poolManager.Spawn("EnemyPool");
            IPoolable e2 = poolManager.Spawn("EnemyPool");
            IPoolable e3 = poolManager.Spawn("EnemyPool");
            IPoolable e4 = poolManager.Spawn("EnemyPool"); // Có thể null nếu không cho expand.

            poolManager.PrintAllStatus();

            // Despawn một số object.
            poolManager.Despawn("BulletPool", b1);
            poolManager.Despawn("EnemyPool", e1);

            poolManager.PrintAllStatus();
        }
    }
}