using System;
using System.Collections.Generic;

namespace OopExercises.SaveLoadSystem
{
    // YÊU CẦU:
    // - Xây dựng hệ thống Save/Load với các "slot" (Slot1, Slot2, ...).
    // - Mỗi slot lưu dữ liệu dạng chuỗi key=value (KHÔNG dùng JSON).
    // - Save/Load chỉ là mô phỏng: in ra console, lưu dữ liệu trong bộ nhớ.
    // - Có nhiều đối tượng khác nhau có thể "save được" (PlayerProfile, GameSettings, ...).
  
    //
    // MỤC TIÊU KIẾN THỨC:
    // - Interface ISaveable mô tả đối tượng có thể lưu.
    // - Tách riêng lớp SaveWriter / SaveReader để đóng gói logic ghi/đọc key=value.
    // - SaveManager quản lý nhiều slot, gọi WriteSaveData / ReadSaveData.

    // ------------------------------------------------------------
    // 1. ISaveable: đối tượng có thể lưu / tải dữ liệu.
    // ------------------------------------------------------------
    public interface ISaveable
    {
        // Id dùng để nhận diện loại dữ liệu (ví dụ "PlayerProfile", "GameSettings").
        string SaveId { get; }

        // Ghi dữ liệu vào writer.
        void WriteSaveData(SaveWriter writer);

        // Đọc dữ liệu từ reader.
        void ReadSaveData(SaveReader reader);
    }

    // ------------------------------------------------------------
    // 2. SaveWriter: ghi dữ liệu key=value vào bộ nhớ (List<string>).
    // ------------------------------------------------------------
    public sealed class SaveWriter
    {
        // Lưu từng dòng dạng "key=value".
        private readonly List<string> _lines = new List<string>();

        // Ghi string.
        public void WriteString(string key, string value)
        {
            // TODO:
            // - Kiểm tra key null/empty => ném ArgumentException.
            // - Nếu value == null => coi như chuỗi rỗng.
            // - Tạo chuỗi "key=value" và Add vào _lines.
            try
            {
                string.IsNullOrEmpty(key);
            }
            catch
            {
                throw new ArgumentException("Key Is Null!!");
            }
            if (string.IsNullOrEmpty(value)) value = "";
            _lines.Add(key + "=" + value);
        }

        // Ghi int.
        public void WriteInt(string key, int value)
        {
            // TODO:
            // - Chuyển value thành string (value.ToString()).
            // - Gọi lại WriteString(key, text).
            WriteString(key, value.ToString());
        }

        // Trả về mảng các dòng key=value để SaveManager lưu lại.
        public string[] GetLines()
        {
            // TODO:
            // - Dùng _lines.ToArray() và trả về.
            return _lines.ToArray();
        }

        // In ra console toàn bộ nội dung đã ghi (dùng để debug).
        public void PrintToConsole()
        {
            // TODO:
            // - Duyệt _lines và in từng dòng ra console.
            foreach(string line in _lines)
            {
                Console.WriteLine(line);
            }
        }
    }

    // ------------------------------------------------------------
    // 3. SaveReader: đọc dữ liệu từ các dòng key=value.
    // ------------------------------------------------------------
    public sealed class SaveReader
    {
        // Lưu key -> value (dạng string).
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        public SaveReader(string[] lines)
        {
            // TODO:
            // - Nếu lines == null => coi như mảng rỗng.
            // - Duyệt từng dòng:
            //     + Tìm vị trí dấu '='.
            //     + Nếu không có '=', continue.
            //     + Cắt key = phần trước '=', value = phần sau '='.
            //     + Trim() hai bên key và value.
            //     + Nếu key không rỗng:
            //           _data[key] = value;
            if(!(lines == null))
            {
                foreach(string line in lines)
                {
                    string subLine = line.Remove('=');
                    if(subLine == line)
                    {
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();
                        if (!string.IsNullOrEmpty(key)) _data[key] = value;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        // Đọc string với key, nếu không có thì trả về defaultValue.
        public string ReadString(string key, string defaultValue)
        {
            // TODO:
            // - Nếu key null/empty => ném ArgumentException.
            // - Nếu _data.TryGetValue(key, out var value) == false => trả defaultValue.
            // - Ngược lại trả value.
            if(string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("There is no key");
            }
            else
            {
                if (!_data.TryGetValue(key, out var value))
                {
                    return defaultValue;
                }
                else
                {
                    return value;
                }
            }
        }

        // Đọc int với key, nếu không parse được thì trả về defaultValue.
        public int ReadInt(string key, int defaultValue)
        {
            // TODO:
            // - Gọi ReadString(key, null).
            // - Nếu kết quả == null => trả defaultValue.
            // - Dùng int.TryParse:
            //     + Nếu parse thành công => trả về giá trị parse được.
            //     + Nếu thất bại => trả về defaultValue.
            string readString = ReadString(key, null);
            if (readString == null)
            {
                return defaultValue;
            }
            if (int.TryParse(readString,out int value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }

    // ------------------------------------------------------------
    // 4. SaveSlot: mô tả một slot save.
    // ------------------------------------------------------------
    public sealed class SaveSlot
    {
        public string SlotName { get; private set; }

        // Slot trống hay không.
        public bool IsEmpty { get; private set; }

        // Lưu SaveId của đối tượng đã save (ví dụ "PlayerProfile", "GameSettings"...).
        public string SaveId { get; private set; }

        // Dữ liệu key=value.
        private string[] _lines;

        public SaveSlot(string slotName)
        {
            // TODO:
            // - Kiểm tra slotName null/empty => ArgumentException.
            // - Gán SlotName.
            // - Đặt IsEmpty = true.
            // - Đặt SaveId = null, _lines = null.
            if (string.IsNullOrEmpty(slotName))
            {
                throw new ArgumentException($"there is no {slotName}");
            }
            else
            {
                SlotName = slotName;
                IsEmpty = true;
                SaveId = null;
                _lines = null;
            }
        }

        // Ghi dữ liệu mới vào slot.
        public void Write(string saveId, string[] lines)
        {
            // TODO:
            // - Kiểm tra saveId null/empty.
            // - Nếu lines == null => gán lines = Array.Empty<string>().
            // - Gán SaveId, _lines, IsEmpty = false.
            if (!string.IsNullOrEmpty(saveId))
            {
                if (lines == null)
                {
                    lines = Array.Empty<string>();
                    SaveId = saveId;
                    _lines = lines;
                    IsEmpty = false;
                }
            }
        }

        // Đọc dữ liệu từ slot, trả về SaveReader.
        // Nếu slot rỗng => trả về null.
        public SaveReader CreateReader()
        {
            // TODO:
            // - Nếu IsEmpty == true => return null.
            // - Tạo SaveReader mới với _lines và trả về.
            if (IsEmpty)
            {
                return null;
            }
            else
            {
                SaveReader newSaveReader = new(_lines);
                return newSaveReader;
            }
        }

        // In thông tin slot ra console.
        public void PrintStatus()
        {
            // TODO:
            // - Nếu IsEmpty:
            //       Console.WriteLine($"{SlotName} - Empty");
            // - Ngược lại:
            //       Console.WriteLine($"{SlotName} - SaveId = {SaveId}, Lines = {_lines.Length}");
            if (IsEmpty)
            {
                Console.WriteLine($"{SlotName} - Empty");
            }
            else
            {
                Console.WriteLine($"{SlotName} - SaveId = {SaveId}, Lines = {_lines.Length}");
            }
        }
    }

    // ------------------------------------------------------------
    // 5. SaveManager: quản lý nhiều slot, gọi Save/Load.
    // ------------------------------------------------------------
    public sealed class SaveManager
    {
        // Lưu SlotName -> SaveSlot.
        private readonly Dictionary<string, SaveSlot> _slots = new Dictionary<string, SaveSlot>();

        // Tạo / đăng ký slot mới.
        public void CreateSlot(string slotName)
        {
            // TODO:
            // - Kiểm tra slotName null/empty.
            // - Nếu _slots đã chứa slotName => có thể bỏ qua hoặc ném exception (tuỳ bạn).
            // - Tạo SaveSlot mới và lưu vào _slots[slotName].
            if (!string.IsNullOrEmpty(slotName))
            {
                if (_slots.ContainsKey(slotName))
                {
                    return;
                }
                else
                {
                    SaveSlot newSlot = new(slotName);
                    _slots.Add(slotName, newSlot);
                }
            }
        }

        // Lấy slot theo tên. Nếu không có trả về null.
        private SaveSlot GetSlot(string slotName)
        {
            // TODO:
            // - Nếu _slots.TryGetValue(slotName, out var slot) == false => return null.
            // - Ngược lại trả slot.
            if (!_slots.TryGetValue(slotName, out var slot))
            {
                return null;
            }
            else
            {
                return slot;
            }
        }

        // Lưu đối tượng vào slot.
        public void SaveToSlot(string slotName, ISaveable saveable)
        {
            // TODO:
            // - Kiểm tra saveable null.
            // - Lấy slot bằng GetSlot.
            // - Nếu slot == null => ném Exception (chưa tạo slot).
            // - Tạo SaveWriter.
            // - Gọi saveable.WriteSaveData(writer).
            // - Lấy lines từ writer.GetLines().
            // - Gọi slot.Write(saveable.SaveId, lines).
            // - In ra console:
            //      Console.WriteLine($"[SaveManager] Saved to slot {slotName}, SaveId = {saveable.SaveId}";);
            // - Gọi writer.PrintToConsole() để xem nội dung.
            if(saveable != null)
            {
                SaveSlot slot = GetSlot(slotName);
                if (slot == null) 
                {
                    throw new Exception("chua tao Slot");
                }
                else
                {
                    SaveWriter saveWriter = new();
                    saveable.WriteSaveData(saveWriter);
                    string[] lines = saveWriter.GetLines();
                    slot.Write(saveable.SaveId, lines);
                    Console.WriteLine($"[SaveManager] Saved to slot {slotName}, SaveId = {saveable.SaveId}");
                    saveWriter.PrintToConsole();
                }
            }
        }

        // Tải đối tượng từ slot.
        public void LoadFromSlot(string slotName, ISaveable saveable)
        {
            // TODO:
            // - Kiểm tra saveable null.
            // - Lấy slot bằng GetSlot.
            // - Nếu slot == null:
            //       Console.WriteLine($"[SaveManager] Slot {slotName} does not exist.");
            //       return;
            // - Gọi slot.CreateReader():
            //       + Nếu reader == null:
            //             Console.WriteLine($"[SaveManager] Slot {slotName} is empty.");
            //             return;
            // - Có thể kiểm tra slot.SaveId có khớp saveable.SaveId hay không:
            //       + Nếu không khớp => Console.WriteLine cảnh báo.
            // - Gọi saveable.ReadSaveData(reader).
            // - In ra console: $"[SaveManager] Loaded from slot {slotName}, SaveId = {slot.SaveId}"
            if(saveable != null)
            {
                SaveSlot slot = GetSlot(slotName);
                if(slot == null)
                {
                    Console.WriteLine($"[SaveManager] Slot {slotName} does not exist.");
                    return;
                }
                else
                {
                    SaveReader reader = slot.CreateReader();
                    if(reader != null)
                    {
                        Console.WriteLine($"[SaveManager] Slot {slotName} is empty.");
                        return;
                    }
                    else
                    {
                        if(slot.SaveId == saveable.SaveId)
                        {
                            saveable.ReadSaveData(reader);
                            Console.WriteLine($"[SaveManager] Loaded from slot {slotName}, SaveId = {slot.SaveId}");
                        }
                        else
                        {
                            Console.WriteLine("saveble.SaveId khong khop voi slot.SaveId");
                        }
                    }

                }
            }
        }

        // In tình trạng tất cả slot.
        public void PrintAllSlots()
        {
            // TODO:
            // - Duyệt foreach (var pair in _slots):
            //       var slot = pair.Value;
            //       slot.PrintStatus();
            foreach(var pair in _slots)
            {
                var slot = pair.Value;
                slot.PrintStatus();
            }
        }
    }

    // ------------------------------------------------------------
    // 6. Domain 1: PlayerProfile (ví dụ dữ liệu player).
    // ------------------------------------------------------------
    public sealed class PlayerProfile : ISaveable
    {
        public string SaveId { get { return "PlayerProfile"; } }

        // Dữ liệu mô phỏng.
        public string PlayerName { get; private set; }
        public int Level { get; private set; }
        public int Gold { get; private set; }

        public PlayerProfile(string playerName, int level, int gold)
        {
            // TODO:
            // - Nếu playerName null/empty => gán tên mặc định, ví dụ "Player".
            // - Nếu level < 1 => ép về 1.
            // - Nếu gold < 0 => ép về 0.
            // - Gán các property.
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = "Player";
            }
            PlayerName = playerName;
            if(level <1)
            {
                level = 1;
            }
            Level = level;
            if (gold < 0)
            {
                level = 0;
            }
            Gold = gold;
        }

        // Cho phép chỉnh sửa dữ liệu (mô phỏng gameplay).
        public void SetData(string playerName, int level, int gold)
        {
            // TODO: tương tự constructor, gán lại dữ liệu.
            PlayerName = playerName;
            Level = level;
            Gold = gold;
        }

        public void WriteSaveData(SaveWriter writer)
        {
            // TODO:
            // - writer.WriteString("PlayerName", PlayerName);
            // - writer.WriteInt("Level", Level);
            // - writer.WriteInt("Gold", Gold);
            writer.WriteString("PlayerName", PlayerName);
            writer.WriteString("Level", Level.ToString());
            writer.WriteString("Gold", Gold.ToString());
        }

        public void ReadSaveData(SaveReader reader)
        {
            // TODO:
            // - PlayerName = reader.ReadString("PlayerName", "Player");
            // - Level = reader.ReadInt("Level", 1);
            // - Gold = reader.ReadInt("Gold", 0);
            PlayerName = reader.ReadString("PlayerName", "Player");
            Level = reader.ReadInt("Level", 1);
            Gold = reader.ReadInt("Gold", 0);
        }

        public void PrintInfo()
        {
            // TODO:
            // - In: Console.WriteLine($"Player: {PlayerName}, Level: {Level}, Gold: {Gold}");
            Console.WriteLine($"Player: {PlayerName}, Level: {Level}, Gold: {Gold}");
        }
    }

    // ------------------------------------------------------------
    // 7. Domain 2: GameSettings (ví dụ setting game).
    // ------------------------------------------------------------
    public sealed class GameSettings : ISaveable
    {
        public string SaveId { get { return "GameSettings"; } }

        // Dữ liệu mô phỏng.
        public int MusicVolume { get; private set; }    // 0 - 100
        public int SfxVolume { get; private set; }      // 0 - 100
        public string Language { get; private set; }    // ví dụ: "vi", "en"

        public GameSettings(int musicVolume, int sfxVolume, string language)
        {
            // TODO:
            // - Dùng hàm SetData(...) bên dưới để gán dữ liệu (DRY).
            SetData(musicVolume, sfxVolume, language);
        }

        public void SetData(int musicVolume, int sfxVolume, string language)
        {
            // TODO:
            // - Giới hạn musicVolume, sfxVolume trong [0, 100].
            // - Nếu language null/empty => gán "vi".
            // - Gán property.
            if(musicVolume < 0)
            {
                musicVolume = 0;
            }
            else if(musicVolume > 100)
            {
                musicVolume = 100;
            }
            MusicVolume = musicVolume;

            if (sfxVolume < 0)
            {
                sfxVolume = 0;
            }
            else if (sfxVolume > 100)
            {
                sfxVolume = 100;
            }
            SfxVolume = sfxVolume;

            if (!string.IsNullOrEmpty(language)) language = "vi";
            Language = language;

        }

        public void WriteSaveData(SaveWriter writer)
        {
            // TODO:
            // - writer.WriteInt("MusicVolume", MusicVolume);
            // - writer.WriteInt("SfxVolume", SfxVolume);
            // - writer.WriteString("Language", Language);

            writer.WriteInt("MusicVolume", MusicVolume);
            writer.WriteInt("SfxVolume", SfxVolume);
            writer.WriteString("Language", Language);
        }

        public void ReadSaveData(SaveReader reader)
        {
            // TODO:
            // - int music = reader.ReadInt("MusicVolume", 50);
            // - int sfx   = reader.ReadInt("SfxVolume", 50);
            // - string lang = reader.ReadString("Language", "vi");
            // - Gọi SetData(music, sfx, lang);

            int music = reader.ReadInt("MusicVolume", 50);
            int sfx   = reader.ReadInt("SfxVolume", 50);
            string lang = reader.ReadString("Language", "vi");
            SetData(music, sfx, lang);
        }

        public void PrintInfo()
        {
            // TODO:
            // - In: Console.WriteLine($"Settings: Music={MusicVolume}, Sfx={SfxVolume}, Lang={Language}");
            Console.WriteLine($"Settings: Music={MusicVolume}, Sfx={SfxVolume}, Lang={Language}");
        }
    }

    public class Program
    {
        static void Main()
        {
            SaveManager manager = new SaveManager();

            // Tạo 2 slot.
            manager.CreateSlot("Slot1");
            manager.CreateSlot("Slot2");

            // Tạo dữ liệu giả lập.
            PlayerProfile profile = new PlayerProfile("Alice", 10, 1234);
            GameSettings settings = new GameSettings(80, 60, "en");

            // Save profile vào Slot1, settings vào Slot2.
            manager.SaveToSlot("Slot1", profile);
            manager.SaveToSlot("Slot2", settings);

            manager.PrintAllSlots();

            // Thay đổi dữ liệu trong RAM (mô phỏng chơi game).
            profile.SetData("Alice_New", 1, 0);
            settings.SetData(0, 0, "vi");

            Console.WriteLine("=== After modifying in memory ===");
            profile.PrintInfo();
            settings.PrintInfo();

            // Load lại từ slot.
            manager.LoadFromSlot("Slot1", profile);
            manager.LoadFromSlot("Slot2", settings);

            Console.WriteLine("=== After Load ===");
            profile.PrintInfo();
            settings.PrintInfo();
        }
    }
}