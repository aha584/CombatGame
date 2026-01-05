using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalUndoSystem
{
    // ==========================================
    // PHẦN 1: CORE FRAMEWORK
    // ==========================================

    /// <summary>
    /// Interface cơ bản cho mọi lệnh.
    /// </summary>
    public interface ICommand
    {
        string Name { get; }
        void Execute();
        void Undo();
    }

    /// <summary>
    /// [QUAN TRỌNG] Functional Command.
    /// Thay vì tạo class riêng cho từng hành động, ta dùng class này để bọc 2 Action (Do & Undo).
    /// Giúp code gọn hơn rất nhiều nhờ Lambda.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public string Name { get; }
        
        // Delegate lưu trữ logic thực thi
        private readonly Action _executeAction;
        
        // Delegate lưu trữ logic hoàn tác
        private readonly Action _undoAction;

        public DelegateCommand(string name, Action executeAction, Action undoAction)
        {
            // TODO 1: 
            // - Gán các tham số vào field.
            // - Kiểm tra null cho executeAction và undoAction.
            Name = name;
            if (executeAction != null)
            {
                _executeAction = executeAction;
            }
            if(undoAction != null)
            {
                _undoAction = undoAction;
            }
        }

        public void Execute()
        {
            // TODO 2: Gọi delegate _executeAction.
            _executeAction?.Invoke();
        }

        public void Undo()
        {
            // TODO 3: Gọi delegate _undoAction.
            _undoAction?.Invoke();
        }
    }

    /// <summary>
    /// Quản lý ngăn xếp Undo/Redo.
    /// </summary>
    public class UndoManager
    {
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        // Event bắn ra khi số lượng item trong stack thay đổi (để cập nhật UI nút bấm)
        // bool canUndo, bool canRedo
        public event Action<bool, bool> OnStackChange;

        public void Execute(ICommand cmd)
        {
            // TODO 4:
            // 1. Thực thi cmd.Execute().
            // 2. Push cmd vào _undoStack.
            // 3. Clear _redoStack (vì lịch sử cũ không còn đúng).
            // 4. Gọi hàm NotifyUI().
            // 5. In log: "Executed: {cmd.Name}"

            cmd.Execute();
            _undoStack.Push(cmd);
            _redoStack.Clear();
            NotifyUI();
            Console.WriteLine($"Executed: {cmd.Name}");
        }

        // Helper Overload: Giúp gọi hàm nhanh bằng Lambda mà không cần new DelegateCommand thủ công bên ngoài
        public void Record(string name, Action doLogic, Action undoLogic)
        {
            // TODO 5:
            // 1. Tạo instance của DelegateCommand(name, doLogic, undoLogic).
            // 2. Gọi hàm Execute(cmd) ở trên.

            DelegateCommand cmd =new(name, doLogic, undoLogic);
            Execute(cmd);

        }

        public void Undo()
        {
            // TODO 6:
            // 1. Kiểm tra _undoStack.Count > 0.
            // 2. Pop lệnh ra khỏi _undoStack.
            // 3. Gọi cmd.Undo().
            // 4. Push cmd vào _redoStack.
            // 5. NotifyUI() và Log.
            
            if(_undoStack.Count > 0)
            {
                ICommand cmd =_undoStack.Pop();
                cmd.Undo();
                _redoStack.Push(cmd);
                NotifyUI();
                Console.WriteLine($"Undo: {cmd.Name}");
            }
        }

        public void Redo()
        {
            // TODO 7:
            // 1. Kiểm tra _redoStack.Count > 0.
            // 2. Pop lệnh ra khỏi _redoStack.
            // 3. Gọi cmd.Execute().
            // 4. Push cmd vào _undoStack.
            // 5. NotifyUI() và Log.
            
             if(_redoStack.Count > 0)
            {
                ICommand cmd = _redoStack.Pop();
                cmd.Execute();
                _undoStack.Push(cmd);
                NotifyUI();
                Console.WriteLine($"Redo: {cmd.Name}");
            }
        }

        private void NotifyUI()
        {
            // Invoke event, báo cho bên ngoài biết có Undo/Redo được không
            OnStackChange?.Invoke(_undoStack.Count > 0, _redoStack.Count > 0);
        }
    }

    // ==========================================
    // PHẦN 2: DOMAIN MODELS (Đối tượng cần quản lý)
    // ==========================================

    public class SmartTextEditor
    {
        // Nội dung văn bản
        public string Content { get; private set; } = "";
        
        // Tham chiếu đến UndoManager để tự ghi lại lịch sử
        private readonly UndoManager _manager;

        public SmartTextEditor(UndoManager manager)
        {
            _manager = manager;
        }

        // Hàm này sử dụng Lambda để định nghĩa Undo ngay tại chỗ!
        public void Append(string text)
        {
            // Kỹ thuật Capture Variable (Closure):
            // Ta cần lưu lại độ dài chuỗi TRƯỚC khi thêm để sau này Undo có thể cắt bớt đi.
            int lengthBefore = Content.Length; 
            string textToAdd = text;

            // Định nghĩa logic thực thi
            Action doAction = () => 
            {
                Content += textToAdd;
                Console.WriteLine($"   (Editor) Content is now: '{Content}'");
            };

            // Định nghĩa logic hoàn tác (cắt bỏ phần vừa thêm)
            Action undoAction = () => 
            {
                // Logic: Lấy Substring từ 0 đến lengthBefore
                if (Content.Length >= lengthBefore)
                {
                    Content = Content.Substring(0, lengthBefore);
                    Console.WriteLine($"   (Editor) Reverted to: '{Content}'");
                }
            };

            // TODO 8: Gọi _manager.Record với doAction và undoAction vừa tạo.
            // Tên lệnh: $"Append '{text}'"

            _manager.Record($"Append '{text}'", doAction, undoAction);
        }

        public void Clear()
        {
            // Lưu lại nội dung cũ để Undo
            string oldContent = Content;

            // TODO 9: Viết logic Record tương tự hàm Append.
            // - Do: Gán Content = ""
            // - Undo: Gán Content = oldContent

            Action doAction = () =>
            {
                Content = "";
            };
            Action undoAction = () =>
            {
                Content = oldContent;
            };

            _manager.Record("Clear", doAction, undoAction);
        }
    }

    public class BankAccount
    {
        public decimal Balance { get; private set; }
        private readonly UndoManager _manager;

        public BankAccount(decimal initialBalance, UndoManager manager)
        {
            Balance = initialBalance;
            _manager = manager;
        }

        public void Deposit(decimal amount)
        {
            // TODO 10: Sử dụng _manager.Record để nạp tiền.
            // - Do: Balance += amount
            // - Undo: Balance -= amount

            Action doAction = () =>
            {
                Balance += amount;
            };
            Action undoAction = () =>
            {
                Balance -= amount;
            };

            _manager.Record("Deposit", doAction, undoAction);
        }
    }

    // ==========================================
    // PHẦN 3: MAIN TEST
    // ==========================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== FUNCTIONAL COMMAND PATTERN (LAMBDA UNDO) ===\n");

            // 1. Setup hệ thống
            UndoManager history = new UndoManager();
            
            // Đăng ký Event để cập nhật UI giả lập
            history.OnStackChange += (canUndo, canRedo) => 
            {
                string u = canUndo ? "[UNDO]" : "[    ]";
                string r = canRedo ? "[REDO]" : "[    ]";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"UI Update: {u}  {r}");
                Console.ResetColor();
            };

            // 2. Test Text Editor
            Console.WriteLine("\n--- TEST 1: TEXT EDITOR ---");
            SmartTextEditor editor = new SmartTextEditor(history);

            editor.Append("Hello");         // Content: Hello
            editor.Append(" World");        // Content: Hello World
            
            Console.WriteLine("\n> Thử Undo 1 lần:");
            history.Undo();                 // Content: Hello

            Console.WriteLine("\n> Thử Undo lần 2:");
            history.Undo();                 // Content: ""

            Console.WriteLine("\n> Thử Redo lại:");
            history.Redo();                 // Content: Hello

            Console.WriteLine("\n> Test Clear và Undo Clear:");
            editor.Clear();                 // Content: ""
            history.Undo();                 // Content: Hello

            // 3. Test Bank Account (Chung 1 Manager vẫn chạy tốt)
            Console.WriteLine("\n--- TEST 2: BANK ACCOUNT ---");
            BankAccount bank = new BankAccount(100, history);
            
            Console.WriteLine($"Số dư ban đầu: {bank.Balance}");
            bank.Deposit(50);               // Balance: 150
            bank.Deposit(20);               // Balance: 170

            Console.WriteLine($"Số dư sau nạp: {bank.Balance}");

            Console.WriteLine("> Undo nạp 20:");
            history.Undo();                 // Balance: 150
            Console.WriteLine($"Số dư: {bank.Balance}");

            // 4. Test Mixed Undo (Vừa Text vừa Bank)
            Console.WriteLine("\n--- TEST 3: MIXED HISTORY ---");
            // Lúc này trong Stack đang có lệnh của Bank và lệnh của Editor lẫn lộn.
            // Hệ thống vẫn xử lý đúng vì DelegateCommand đã gói gọn logic của từng thằng.
            
            while(true) // Giả lập bấm Undo liên tục cho đến hết
            {
                // Đây là cách kiểm tra sự kiện UI (thực tế dùng property CanUndo)
                // Ở đây ta "hack" bằng cách try-catch hoặc check log, 
                // nhưng trong bài này ta cứ Undo hú họa để xem log.
                
                Console.WriteLine("Bấm Enter để Undo tiếp (Gõ 'exit' để thoát)...");
                string input = Console.ReadLine();
                if (input == "exit") break;
                
                try 
                {
                    history.Undo();
                }
                catch
                {
                    Console.WriteLine("Hết lịch sử để Undo!");
                    break;
                }
            }
        }
    }
}