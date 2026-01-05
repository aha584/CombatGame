//
// BÀI TẬP NÂNG CAO: HỆ THỐNG TÚI ĐỒ ĐA HÌNH (POLYMORPHIC INVENTORY)
//
// Mục tiêu:
// 1. Loại bỏ sự phụ thuộc cứng của Hero vào các loại túi cụ thể.
// 2. Sử dụng Generic Constraints (where T : ...)
// 3. Sử dụng Type Checking (is/as) để tự động phân loại vật phẩm vào đúng túi.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedInventory
{
    // ==========================================
    // PHẦN 1: ĐỊNH NGHĨA VẬT PHẨM (ITEMS)
    // ==========================================

    /// <summary>
    /// Interface chung cho tất cả vật phẩm có thể nhặt được.
    /// </summary>
    public interface ILootable
    {
        string Name { get; }
    }

    /// <summary>
    /// Vũ khí.
    /// TODO: Sinh viên tạo class Weapon implement ILootable
    /// </summary>
    public class Weapon : ILootable
    {
        public string Name { get; set; }
        // Có thể thêm chỉ số Damage, Rarity...
        public Weapon(string name) => Name = name;
    }

    /// <summary>
    /// Tiền vàng.
    /// TODO: Sinh viên tạo class Coin implement ILootable
    /// </summary>
    public class Coin : ILootable
    {
        public string Name => "Gold Coin";
        public int Amount { get; set; }
        public Coin(int amount) => Amount = amount;
    }

    /// <summary>
    /// Dược phẩm (Mở rộng thêm để test tính linh hoạt).
    /// </summary>
    public class Potion : ILootable
    {
        public string Name { get; set; }
        public Potion(string name) => Name = name;
    }

    // ==========================================
    // PHẦN 2: HỆ THỐNG TÚI CHỨA (CONTAINERS)
    // ==========================================

    /// <summary>
    /// Interface không Generic để Hero có thể lưu trữ List<IContainer>.
    /// Chứa phương thức quan trọng: TryAdd(ILootable item).
    /// </summary>
    public interface IContainer
    {
        string Name { get; }

        /// <summary>
        /// Thử thêm một vật phẩm vào túi.
        /// </summary>
        /// <param name="item">Vật phẩm cần thêm</param>
        /// <returns>True nếu túi nhận vật phẩm này (đúng loại và chưa đầy), ngược lại False</returns>
        bool TryAdd(ILootable item);

        void ShowContents();
    }

    /// <summary>
    /// Lớp Túi Generic.
    /// Chỉ chấp nhận chứa các vật phẩm kiểu T (với T là ILootable).
    /// Ví dụ: Bag<Weapon>, Bag<Potion>.
    /// </summary>
    public class Bag<T> : IContainer where T : ILootable
    {
        private List<T> _items = new List<T>();
        private int _capacity;
        public string Name { get; private set; }

        public Bag(string name, int capacity)
        {
            Name = name;
            _capacity = capacity;
        }

        /// <summary>
        /// TODO: Sinh viên hoàn thiện logic cốt lõi này.
        /// Gợi ý:
        /// 1. Kiểm tra xem 'item' có phải là kiểu T không (dùng từ khóa 'is').
        /// 2. Nếu đúng kiểu, kiểm tra sức chứa (_capacity).
        /// 3. Nếu thỏa mãn, add vào _items và return true.
        /// 4. Nếu không đúng kiểu T, return false ngay lập tức để Hero thử túi khác.
        /// </summary>
        public bool TryAdd(ILootable item)
        {
            // TODO: Code logic kiểm tra kiểu dữ liệu và sức chứa tại đây
            // if (item is T correctItem) { ... }
            if(item is T)
            {
                if(_items.Count < _capacity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void ShowContents()
        {
            Console.WriteLine($"- {Name} [{_items.Count}/{_capacity}]: " +
                string.Join(", ", _items.Select(x => x.Name)));
        }
    }

    // ==========================================
    // PHẦN 3: NHÂN VẬT (HERO)
    // ==========================================

    public class Hero
    {
        // Bây giờ Hero chỉ cần 1 danh sách chung, không cần biết cụ thể là túi gì
        private List<IContainer> _inventory = new List<IContainer>();

        public void EquipBag(IContainer bag)
        {
            _inventory.Add(bag);
            Console.WriteLine($"Hero equipped: {bag.Name}");
        }

        /// <summary>
        /// Hàm nhặt đồ thông minh.
        /// Tự động tìm túi phù hợp cho vật phẩm.
        /// </summary>
        public void Loot(ILootable item)
        {
            Console.Write($"Looting {item.Name}... ");

            // TODO: Duyệt qua danh sách _inventory
            // TODO: Gọi bag.TryAdd(item). Nếu trả về true (thành công) thì return ngay.
            // TODO: Nếu duyệt hết danh sách mà không túi nào nhận, in ra "No suitable bag found or all full!"

            foreach(var bag in _inventory)
            {
                if (bag.TryAdd(item))
                {
                    return;
                }
            }
            Console.WriteLine("No suitable bag found or all full!");
        }

        public void ShowInventory()
        {
            Console.WriteLine("\n=== HERO INVENTORY ===");
            foreach (var bag in _inventory)
            {
                bag.ShowContents();
            }
            Console.WriteLine("======================\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Hero hero = new Hero();

            // 1. Trang bị các loại túi
            // Lưu ý: Ta dùng Generic để định nghĩa loại túi
            hero.EquipBag(new Bag<Weapon>("Weapon Bag", 2));
            hero.EquipBag(new Bag<Potion>("Potion Belt", 5));
            // CoinPurse có thể là một class riêng nếu logic cộng dồn tiền khác logic list
            // Ở đây dùng tạm Bag<Coin>
            hero.EquipBag(new Bag<Coin>("Coin Purse", 100));

            // 2. Nhặt đồ hỗn loạn
            hero.Loot(new Weapon("Excalibur"));      // Sẽ tự chui vào Weapon Bag
            hero.Loot(new Potion("Health Potion"));  // Sẽ tự chui vào Potion Belt
            hero.Loot(new Coin(100));                // Sẽ tự chui vào Coin Purse
            hero.Loot(new Weapon("Wooden Bow"));

            // 3. Test túi đầy
            hero.Loot(new Weapon("Iron Axe"));       // Weapon Bag sức chứa 2 -> Cái này sẽ fail

            // 4. Test món đồ không có túi chứa
            // Giả sử ta tạo ra một class Armor nhưng Hero chưa đeo túi Armor
            // hero.Loot(new Armor("Chestplate")); -> Nên báo lỗi "No bag found"

            hero.ShowInventory();
        }
    }
}