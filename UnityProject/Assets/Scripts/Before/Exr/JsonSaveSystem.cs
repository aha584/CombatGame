using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OopExercises.JsonSaveSystem
{
    // =========================================================
    // BÀI TẬP: HỆ THỐNG SAVE/LOAD DÙNG JSON (PHIÊN BẢN NÂNG CAO)
    // ---------------------------------------------------------
    // YÊU CẦU HỆ THỐNG:
    //  - Có SaveManager<TSlotKey> quản lý nhiều slot bằng Dictionary.
    //  - Mỗi slot lưu:
    //      + SlotKey (generic)
    //      + SaveId (chuỗi nhận diện kiểu dữ liệu)
    //      + Kiểu dữ liệu (TypeName hoặc FullName)
    //      + JsonContent (chuỗi JSON)
    //  - Save/Load generic:
    //      + Save<T>(slotKey, data)
    //      + Load<T>(slotKey) => T
    //      + Kiểm tra kiểu dữ liệu có khớp không (phòng lỗi).
    //
    // KỸ THUẬT C#: 
    //  - Generic + constraint: where TSlotKey : notnull
    //  - Dictionary<TKey, TValue>
    //  - Delegate + lambda:
    //      + ISaveSerializer dùng Func nội bộ (tùy chọn).
    //      + SaveManager.PrintAllSlots(Func<SaveSlot<TSlotKey>, bool> filter)
    //  - System.Text.Json (JsonSerializer, JsonSerializerOptions)
    //  - Một chút reflection: typeof(T).FullName dùng làm TypeName.
    //
    // MỤC TIÊU HỌC VIÊN:
    //  - Tự cài đặt Save/Load JSON generic.
    //  - Hiểu cách map slot -> dữ liệu kiểu bất kỳ.
    // =========================================================

    // ---------------------------------------------------------
    // 1. Interface ISaveSerializer: trừu tượng hóa việc serialize/deserialize.
    // ---------------------------------------------------------
    public interface ISaveSerializer
    {
        // Chuyển object sang JSON.
        string Serialize<T>(T data);

        // Chuyển JSON về object.
        T Deserialize<T>(string json);
    }

    // ---------------------------------------------------------
    // 2. JsonSaveSerializer: triển khai ISaveSerializer bằng System.Text.Json.
    // ---------------------------------------------------------
    public sealed class JsonSaveSerializer : ISaveSerializer
    {
        // Tuỳ chọn JSON (bật indent, bỏ qua null, v.v...)
        public JsonSaveSerializer()
        {

        }

        public string Serialize<T>(T data)
        {
            // TODO (JSON-2):
            // - Nếu data == null => có thể ném ArgumentNullException hoặc serialize "null".
            if (data == null) throw new ArgumentNullException();
            // - Dùng JsonSerializer.Serialize(data, _options).
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(string json)
        {
            // TODO (JSON-3):
            // - Kiểm tra json null/empty:
            //      + Có thể ném ArgumentException hoặc trả default.
            // - Dùng JsonSerializer.Deserialize<T>(json, _options).
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("json is null or empty", nameof(json));

            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    // ---------------------------------------------------------
    // 3. SaveSlot<TSlotKey>: mô tả một slot save.
    // ---------------------------------------------------------
    public sealed class SaveSlot<TSlotKey>
    {
        // Khóa slot (string, int, enum,... tuỳ TSlotKey).
        public TSlotKey SlotKey { get; }

        // Slot trống hay không.
        public bool IsEmpty { get; private set; }

        // Id logic của dữ liệu (ví dụ "PlayerProfile", "GameSettings"...).
        public string SaveId { get; private set; }

        // Tên kiểu dữ liệu .NET, dùng để kiểm tra khi load (typeof(T).FullName).
        public string TypeName { get; private set; }

        // Nội dung JSON.
        public string JsonContent { get; private set; }

        public SaveSlot(TSlotKey slotKey)
        {
            // TODO (SLOT-1):
            // - Nếu slotKey là kiểu reference và null => ném ArgumentNullException.
            // - Gán SlotKey.
            // - Ban đầu slot trống:
            //      + IsEmpty = true
            //      + SaveId = null
            //      + TypeName = null
            //      + JsonContent = null
            // Gợi ý: có thể dùng pattern matching: if (slotKey is null) ...
            if (slotKey is null)
                throw new ArgumentNullException(nameof(slotKey));

            SlotKey = slotKey;
            IsEmpty = true;
            SaveId = null;
            TypeName = null;
            JsonContent = null;
        }

        // Ghi dữ liệu JSON vào slot.
        public void Write(string saveId, string typeName, string json)
        {
            // TODO (SLOT-2):
            // - Kiểm tra saveId và typeName null/empty => ném ArgumentException.
            // - json có thể null/empty nhưng thường không.
            // - Gán SaveId, TypeName, JsonContent.
            // - Đặt IsEmpty = false.
            if (string.IsNullOrWhiteSpace(saveId))
                throw new ArgumentException("saveId is null or empty", nameof(saveId));
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("typeName is null or empty", nameof(typeName));

            SaveId = saveId;
            TypeName = typeName;
            JsonContent = json ?? "null";
            IsEmpty = false;
        }

        // Xoá dữ liệu trong slot (trở về trạng thái trống).
        public void Clear()
        {
            // TODO (SLOT-3):
            // - Đặt IsEmpty = true
            // - SaveId, TypeName, JsonContent = null
            IsEmpty = true;
            SaveId = null;
            TypeName = null;
            JsonContent = null;
        }

        // In thông tin slot ra console (không in JSON, chỉ metadata).
        public void PrintStatus()
        {
            if (IsEmpty)
            {
                Console.WriteLine($"Slot [{SlotKey}] - Empty");
            }
            else
            {
                Console.WriteLine($"Slot [{SlotKey}] - SaveId={SaveId}, Type={TypeName}, JsonLength={JsonContent?.Length ?? 0}");
            }
        }
    }

    // ---------------------------------------------------------
    // 4. SaveManager<TSlotKey>: quản lý nhiều slot và gọi serializer.
    // ---------------------------------------------------------
    public sealed class SaveManager<TSlotKey> where TSlotKey : notnull
    {
        private readonly ISaveSerializer _serializer;
        private readonly Dictionary<TSlotKey, SaveSlot<TSlotKey>> _slots = new();

        public SaveManager(ISaveSerializer serializer)
        {
            // TODO (MAN-1):
            // - Kiểm tra serializer null => ném ArgumentNullException.
            // - Gán vào _serializer.
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        // Tạo slot nếu chưa tồn tại.
        public SaveSlot<TSlotKey> CreateSlot(TSlotKey slotKey)
        {
            // TODO (MAN-2):
            // - Nếu đã tồn tại slot với slotKey => trả về slot cũ.
            // - Nếu chưa có:
            //      + Tạo SaveSlot<TSlotKey> mới.
            //      + Lưu vào _slots.
            //      + Trả về slot vừa tạo.
            if (_slots.TryGetValue(slotKey, out SaveSlot<TSlotKey> existing))
                return existing;

            SaveSlot<TSlotKey> slot = new(slotKey);
            _slots[slotKey] = slot;
            return slot;
        }

        // Lấy slot; nếu không có trả null.
        private SaveSlot<TSlotKey> GetSlot(TSlotKey slotKey)
        {
            // TODO (MAN-3):
            // - TryGetValue, nếu không có thì trả null.
            return _slots.TryGetValue(slotKey, out SaveSlot<TSlotKey> slot) ? slot : null;
        }

        // Tạo SaveId mặc định từ kiểu T (ví dụ: typeof(T).Name).
        // Học viên có thể custom để phù hợp với game.
        private static string GetDefaultSaveId<T>()
        {
            // TODO (MAN-4):
            // - Trả về chuỗi nhận diện kiểu:
            //      + Đơn giản: typeof(T).Name
            //      + Hoặc đầy đủ: typeof(T).FullName
            return typeof(T).FullName ?? typeof(T).Name;
        }

        // Ghi dữ liệu kiểu T vào slot.
        public void Save<T>(TSlotKey slotKey, T data)
            where T : class
        {
            // TODO (MAN-5):
            // - Kiểm tra data null => ném ArgumentNullException.
            // - Tạo hoặc lấy slot tương ứng:
            //      var slot = CreateSlot(slotKey);
            // - Xác định SaveId:
            //      var saveId = GetDefaultSaveId<T>();
            //   (Nâng cao: có thể cho phép truyền saveId từ ngoài bằng overload khác).
            // - Lấy typeName = typeof(T).FullName.
            // - Serialize data thành json: _serializer.Serialize(data);
            // - Gọi slot.Write(saveId, typeName, json).
            // - In log ra console:
            //      Console.WriteLine($"[SaveManager] Saved {saveId} to slot {slotKey}");
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            SaveSlot<TSlotKey> slot = CreateSlot(slotKey);
            string saveId = GetDefaultSaveId<T>();
            string typeName = typeof(T).FullName ?? typeof(T).Name;
            string json = _serializer.Serialize(data);
            slot.Write(saveId, typeName, json);
            Console.WriteLine($"[SaveManager] Saved {saveId} to slot {slotKey}");
        }

        // Đọc dữ liệu kiểu T từ slot.
        public T Load<T>(TSlotKey slotKey)
            where T : class
        {
            // TODO (MAN-6):
            // - Lấy slot bằng GetSlot(slotKey).
            // - Nếu slot == null:
            //      + In log: slot không tồn tại.
            //      + Trả null.
            // - Nếu slot.IsEmpty:
            //      + In log: slot trống.
            //      + Trả null.
            // - Kiểm tra kiểu:
            //      + expectedTypeName = typeof(T).FullName.
            //      + So sánh với slot.TypeName:
            //           * Nếu KHÔNG khớp => in cảnh báo, trả null.
            // - Dùng _serializer.Deserialize<T>(slot.JsonContent) để lấy object.
            // - In log: load thành công.
            // - Trả object kết quả.
            SaveSlot<TSlotKey> slot = GetSlot(slotKey);
            if (slot == null)
            {
                Console.WriteLine($"[SaveManager] Slot {slotKey} does not exist.");
                return null;
            }

            if (slot.IsEmpty)
            {
                Console.WriteLine($"[SaveManager] Slot {slotKey} is empty.");
                return null;
            }

            string expectedTypeName = typeof(T).FullName;
            if (!string.Equals(slot.TypeName, expectedTypeName, StringComparison.Ordinal))
            {
                Console.WriteLine($"[SaveManager] Type mismatch on slot {slotKey}. Slot has {slot.TypeName}, but requested {expectedTypeName}.");
                return null;
            }

            T result = _serializer.Deserialize<T>(slot.JsonContent);
            Console.WriteLine($"[SaveManager] Loaded {slot.SaveId} from slot {slotKey}");
            return result;
        }

        // Xoá nội dung một slot.
        public void ClearSlot(TSlotKey slotKey)
        {
            // TODO (MAN-7):
            // - Lấy slot.
            // - Nếu slot != null => slot.Clear() và in log.
            SaveSlot<TSlotKey> slot = GetSlot(slotKey);
            if (slot == null)
            {
                Console.WriteLine($"[SaveManager] Slot {slotKey} does not exist.");
                return;
            }

            slot.Clear();
            Console.WriteLine($"[SaveManager] Cleared slot {slotKey}");
        }

        // In trạng thái tất cả slot.
        // Có thể truyền filter bằng lambda để lọc slot cần in.
        public void PrintAllSlots(Func<SaveSlot<TSlotKey>, bool> filter = null)
        {
            // TODO (MAN-8):
            // - Duyệt _slots.Values.
            // - Nếu filter == null => in tất cả.
            // - Nếu filter != null => chỉ in slot mà filter(slot) == true.
            // - Dùng slot.PrintStatus() để in.
            foreach (SaveSlot<TSlotKey> slot in _slots.Values)
            {
                if (filter == null || filter(slot))
                    slot.PrintStatus();
            }
        }
    }

    // ---------------------------------------------------------
    // 5. Domain: PlayerProfile & GameSettings (POCO, auto-props).
    //    - Ở đây ta không cần interface, vì JSON có thể serialize
    //      trực tiếp object bất kỳ (POCO).
    //    - Để thực tế hơn, ta đóng băng setter (private set) và
    //      cung cấp hàm SetData để cập nhật.
    // ---------------------------------------------------------
    public sealed class PlayerProfile
    {
        // Các property auto-implemented với private set.
        public string PlayerName { get; private set; }
        public int Level { get; private set; }
        public int Exp { get; private set; }

        // Bắt buộc cần constructor không tham số để JSON có thể tạo object.
        public PlayerProfile()
        {
        }

        public PlayerProfile(string playerName, int level, int exp)
        {
            SetData(playerName, level, exp);
        }

        public void SetData(string playerName, int level, int exp)
        {
            // TODO (DOMAIN-1):
            // - Nếu playerName null/empty => gán "Noname".
            // - Level >= 1.
            // - Exp >= 0.
            // - Gán vào property.
            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Noname";
            if (level < 1)
                level = 1;
            if (exp < 0)
                exp = 0;

            PlayerName = playerName;
            Level = level;
            Exp = exp;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"[PlayerProfile] Name={PlayerName}, Level={Level}, Exp={Exp}");
        }
    }

    public sealed class GameSettings
    {
        public int MusicVolume { get; private set; } // 0 - 100
        public int SfxVolume { get; private set; }   // 0 - 100
        public string Language { get; private set; }

        public GameSettings()
        {
        }

        public GameSettings(int musicVolume, int sfxVolume, string language)
        {
            SetData(musicVolume, sfxVolume, language);
        }

        public void SetData(int musicVolume, int sfxVolume, string language)
        {
            // TODO (DOMAIN-2):
            // - Clamp musicVolume, sfxVolume vào [0, 100].
            // - Nếu language null/empty => "vi".
            // - Gán property.
            musicVolume = Math.Clamp(musicVolume, 0, 100);
            sfxVolume = Math.Clamp(sfxVolume, 0, 100);
            if (string.IsNullOrWhiteSpace(language))
                language = "vi";

            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
            Language = language;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"[GameSettings] Music={MusicVolume}, Sfx={SfxVolume}, Lang={Language}");
        }
    }

    // ---------------------------------------------------------
    // 6. Program: tạo dữ liệu mẫu, Save/Load để kiểm thử.
    //    - Dùng SaveManager<string> cho đơn giản (slotKey là string).
    //    - Học viên chỉ cần hoàn thiện TODO, không cần sửa Main.
    // ---------------------------------------------------------
    public static class Program
    {
        public static void Main()
        {
            // Tạo serializer dùng System.Text.Json.
            ISaveSerializer serializer = new JsonSaveSerializer();

            // Tạo SaveManager với khóa slot là string.
            SaveManager<string> manager = new SaveManager<string>(serializer);

            // Tạo slot.
            manager.CreateSlot("Slot1");
            manager.CreateSlot("Slot2");

            // Tạo dữ liệu domain ban đầu.
            PlayerProfile profile = new PlayerProfile("Alice", 10, 1234);
            GameSettings settings = new GameSettings(80, 60, "en");

            Console.WriteLine("=== Initial Data ===");
            profile.PrintInfo();
            settings.PrintInfo();

            // Lưu vào slot.
            manager.Save("Slot1", profile);
            manager.Save("Slot2", settings);

            Console.WriteLine();
            Console.WriteLine("=== Slots Status (all) ===");
            manager.PrintAllSlots();

            Console.WriteLine();
            Console.WriteLine("=== Slots Status (only non-empty) ===");
            // Dùng lambda filter: chỉ in slot không rỗng.
            manager.PrintAllSlots(slot => !slot.IsEmpty);

            // Thay đổi dữ liệu trong bộ nhớ (mô phỏng runtime thay đổi).
            profile.SetData("TEMP", 1, 0);
            settings.SetData(0, 0, "vi");

            Console.WriteLine();
            Console.WriteLine("=== After modifying in memory ===");
            profile.PrintInfo();
            settings.PrintInfo();

            // Load lại từ slot (tạo object mới).
            PlayerProfile loadedProfile = manager.Load<PlayerProfile>("Slot1");
            GameSettings loadedSettings = manager.Load<GameSettings>("Slot2");

            Console.WriteLine();
            Console.WriteLine("=== After Load (new instances) ===");
            loadedProfile?.PrintInfo();
            loadedSettings?.PrintInfo();

            Console.WriteLine();
            Console.WriteLine("=== Clear Slot1 and print status ===");
            manager.ClearSlot("Slot1");
            manager.PrintAllSlots();

            Console.WriteLine();
            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}
