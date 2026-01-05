using System;
using System.Collections.Generic;

//
// Bài: Hệ thống Undo/Redo tổng quát (phiên bản nâng cao)
// ------------------------------------------------------
// So với bản cũ:
//  - Thêm generic: UndoRedoManager<TCommand> where TCommand : IUndoableCommand
//  - Thêm event dùng delegate + lambda để theo dõi log / UI.
//  - Thêm hàm helper generic TrimStackHistory<T>().
//  - Vẫn áp dụng Command Pattern, dùng chung cho nhiều domain (Text, Number, ...).
//
// YÊU CẦU:
//  - Hoàn thiện tất cả TODO trong file này.
//  - Thay thế toàn bộ throw new NotImplementedException() bằng code của bạn.
//  - Không đổi tên class / interface / public method (có thể thêm method private nếu cần).
//
// MỤC TIÊU KIẾN THỨC:
//  - OOP + Command Pattern.
//  - Generic class (UndoRedoManager<TCommand>).
//  - Sử dụng delegate, event, lambda với hệ thống Undo/Redo.
//  - Tư duy “hệ thống dùng chung” cho nhiều loại lệnh.
//

namespace OopExercises.UndoRedoSystemAdvanced
{
    // ---------------------------------------------------------------------
    // 1. Interface lệnh có thể Undo/Redo
    // ---------------------------------------------------------------------
    public interface IUndoableCommand
    {
        // Thực hiện lệnh.
        void Execute();

        // Hoàn tác lệnh đã thực hiện.
        void Undo();

        // Tên/miêu tả ngắn gọn cho lệnh (dùng để debug / log).
        string Name { get; }
    }

    // ---------------------------------------------------------------------
    // 2. UndoRedoManager<TCommand> (generic)
    // ---------------------------------------------------------------------
    // TCommand: loại lệnh quản lý, thường là IUndoableCommand.
    // Trong các domain cụ thể, ta dùng:
    //    UndoRedoManager<IUndoableCommand> _undoRedo = new UndoRedoManager<IUndoableCommand>(...);
    //
    public sealed class UndoRedoManager<TCommand> where TCommand : IUndoableCommand
    {
        // Stack lưu các lệnh đã thực thi và có thể Undo.
        private readonly Stack<TCommand> _undoStack = new();

        // Stack lưu các lệnh đã Undo và có thể Redo.
        private readonly Stack<TCommand> _redoStack = new();

        // Giới hạn số lượng lệnh tối đa lưu trong lịch sử.
        // Nếu MaxHistory <= 0 => không giới hạn.
        public int MaxHistory { get; }

        // Event (delegate) để hook vào log/UI bằng lambda:
        // - OnExecuted: khi vừa ExecuteCommand().
        // - OnUndone  : khi vừa Undo().
        // - OnRedone  : khi vừa Redo().
        public event Action<TCommand> OnExecuted;
        public event Action<TCommand> OnUndone;
        public event Action<TCommand> OnRedone;

        public UndoRedoManager(int maxHistory = 0)
        {
            // TODO (ADV-1):
            // - Nếu maxHistory < 0 => ép về 0 (coi như không giới hạn).
            // - Gán MaxHistory.
            // - Không cần khởi tạo stack vì đã khởi tạo ngay trên field.
            throw new NotImplementedException();
        }

        // Số lệnh đang có trong stack Undo.
        public int UndoCount
        {
            get
            {
                // TODO (ADV-2): trả về số phần tử trong _undoStack.
                throw new NotImplementedException();
            }
        }

        // Số lệnh đang có trong stack Redo.
        public int RedoCount
        {
            get
            {
                // TODO (ADV-3): trả về số phần tử trong _redoStack.
                throw new NotImplementedException();
            }
        }

        // Cho biết hiện tại có thể Undo không.
        public bool CanUndo
        {
            get
            {
                // TODO (ADV-4): trả về true nếu _undoStack có ít nhất 1 phần tử.
                throw new NotImplementedException();
            }
        }

        // Cho biết hiện tại có thể Redo không.
        public bool CanRedo
        {
            get
            {
                // TODO (ADV-5): trả về true nếu _redoStack có ít nhất 1 phần tử.
                throw new NotImplementedException();
            }
        }

        // Hàm helper generic để giới hạn history.
        // Dùng lại được cho mọi loại Stack<T>.
        private static void TrimStackHistory<T>(Stack<T> stack, int maxCount)
        {
            // TODO (ADV-6):
            // - Nếu maxCount <= 0 => không làm gì (coi như không giới hạn).
            // - Nếu stack.Count <= maxCount => không làm gì.
            // - Ngược lại:
            //      + Ta cần loại bỏ phần tử "cũ nhất" (ở đáy stack).
            //      + Gợi ý:
            //          * Tạo một stack tạm.
            //          * Pop từ stack gốc sang stack tạm, bỏ qua các phần tử dư.
            //          * Sau đó đảo ngược lại nếu cần.
            //   (Tự thiết kế logic, mục tiêu là giữ lại đúng maxCount phần tử mới nhất).
            throw new NotImplementedException();
        }

        // Thực thi một lệnh mới:
        // - Gọi Execute() của lệnh.
        // - Đẩy vào Undo stack.
        // - Xoá sạch Redo stack (vì lịch sử Redo cũ không còn hợp lệ).
        // - Nếu vượt quá MaxHistory thì bỏ bớt lệnh cũ nhất.
        // - Gọi event OnExecuted (nếu có).
        public void ExecuteCommand(TCommand command)
        {
            // TODO (ADV-7):
            // - Kiểm tra command null? => ArgumentNullException.
            // - Gọi command.Execute().
            // - Đẩy command vào _undoStack.
            // - Xoá sạch _redoStack (Clear()).
            // - Gọi TrimStackHistory(_undoStack, MaxHistory).
            // - Nếu OnExecuted != null => gọi OnExecuted(command).
            throw new NotImplementedException();
        }

        // Hoàn tác lệnh gần nhất.
        public void Undo()
        {
            // TODO (ADV-8):
            // - Nếu !CanUndo:
            //      + Có thể return; hoặc ném InvalidOperationException (tuỳ bạn).
            // - Pop một lệnh từ _undoStack.
            // - Gọi lệnh.Undo().
            // - Đẩy lệnh sang _redoStack.
            // - Gọi OnUndone(lệnh) nếu event != null.
            throw new NotImplementedException();
        }

        // Thực hiện lại lệnh vừa bị Undo.
        public void Redo()
        {
            // TODO (ADV-9):
            // - Nếu !CanRedo => return hoặc ném InvalidOperationException.
            // - Pop một lệnh từ _redoStack.
            // - Gọi lệnh.Execute().
            // - Đẩy lệnh sang _undoStack.
            // - Gọi OnRedone(lệnh) nếu event != null.
            throw new NotImplementedException();
        }

        // Xoá toàn bộ lịch sử Undo/Redo.
        public void ClearHistory()
        {
            // TODO (ADV-10):
            // - _undoStack.Clear();
            // - _redoStack.Clear();
            throw new NotImplementedException();
        }
    }

    // =====================================================================
    // DOMAIN 1: TEXT EDITOR
    // =====================================================================

    // Lớp mô tả nội dung văn bản đang được chỉnh sửa.
    public sealed class TextDocument
    {
        // Nội dung hiện tại của văn bản.
        private string _content = string.Empty;

        // Cho phép đọc nội dung nhưng không cho sửa trực tiếp.
        public string Content => _content;

        // Chèn thêm text vào cuối nội dung hiện tại.
        public void InsertText(string text)
        {
            // TODO (TXT-1):
            // - Nếu text == null => coi như "".
            // - Cập nhật _content = _content + text.
            throw new NotImplementedException();
        }

        // Xoá count ký tự từ cuối chuỗi.
        // Nếu count lớn hơn độ dài chuỗi hiện tại, xoá toàn bộ.
        public void RemoveLastCharacters(int count)
        {
            // TODO (TXT-2):
            // - Nếu count <= 0 => không làm gì.
            // - Nếu count >= _content.Length => _content = string.Empty.
            // - Ngược lại:
            //      + Lấy newLength = _content.Length - count.
            //      + _content = _content.Substring(0, newLength).
            throw new NotImplementedException();
        }
    }

    // Lệnh chèn text vào tài liệu.
    public sealed class InsertTextCommand : IUndoableCommand
    {
        // Tài liệu cần thao tác.
        private readonly TextDocument _document;

        // Đoạn text sẽ chèn.
        private readonly string _text;

        // Độ dài nội dung trước khi chèn (dùng để Undo).
        private int _previousLength;

        public string Name => $"Insert \"{_text}\"";

        public InsertTextCommand(TextDocument document, string text)
        {
            // TODO (TXT-3):
            // - Kiểm tra document null? => ArgumentNullException.
            // - Gán _document = document.
            // - Nếu text == null => _text = string.Empty, ngược lại gán text.
            throw new NotImplementedException();
        }

        public void Execute()
        {
            // TODO (TXT-4):
            // - Lưu _previousLength = _document.Content.Length.
            // - Gọi _document.InsertText(_text).
            throw new NotImplementedException();
        }

        public void Undo()
        {
            // TODO (TXT-5):
            // - Tính số ký tự đã chèn = _document.Content.Length - _previousLength.
            // - Nếu số này <= 0 => không làm gì (phòng lỗi).
            // - Gọi _document.RemoveLastCharacters(số ký tự đã chèn).
            throw new NotImplementedException();
        }
    }

    // Lệnh xoá một số ký tự ở cuối văn bản.
    public sealed class RemoveLastCharactersCommand : IUndoableCommand
    {
        // Tài liệu cần thao tác.
        private readonly TextDocument _document;

        // Số ký tự cần xoá khi Execute được gọi.
        private readonly int _countToRemove;

        // Nội dung đã bị xoá (dùng để Undo).
        private string _removedText = string.Empty;

        public string Name => $"Remove last ({_countToRemove}) chars";

        public RemoveLastCharactersCommand(TextDocument document, int countToRemove)
        {
            // TODO (TXT-6):
            // - Kiểm tra document null? => ArgumentNullException.
            // - Kiểm tra countToRemove >= 0, nếu < 0 => ArgumentOutOfRangeException.
            // - Gán vào _document và _countToRemove.
            throw new NotImplementedException();
        }

        public void Execute()
        {
            // TODO (TXT-7):
            // - Nếu _countToRemove <= 0 => không làm gì.
            // - Lấy length = _document.Content.Length.
            // - Tính removeCount = Math.Min(_countToRemove, length).
            // - Lấy đoạn cuối sẽ bị xoá:
            //      _removedText = _document.Content.Substring(length - removeCount, removeCount);
            // - Gọi _document.RemoveLastCharacters(removeCount).
            throw new NotImplementedException();
        }

        public void Undo()
        {
            // TODO (TXT-8):
            // - Nếu string.IsNullOrEmpty(_removedText) => không làm gì.
            // - Gọi _document.InsertText(_removedText).
            throw new NotImplementedException();
        }
    }

    // Lớp "surface" để test hệ thống với TextDocument.
    public sealed class TextEditor
    {
        private readonly TextDocument _document;
        private readonly UndoRedoManager<IUndoableCommand> _undoRedo;

        public TextEditor(int maxHistory = 0)
        {
            // TODO (TXT-9):
            // - Khởi tạo _document.
            // - Khởi tạo _undoRedo với maxHistory.
            // - (Tuỳ chọn) Đăng ký event _undoRedo.OnExecuted bằng lambda để log tên lệnh.
            throw new NotImplementedException();
        }

        // Thuộc tính cho phép xem nội dung hiện tại.
        public string Content
        {
            get
            {
                // TODO (TXT-10): trả về Content của _document.
                throw new NotImplementedException();
            }
        }

        public bool CanUndo => _undoRedo.CanUndo;
        public bool CanRedo => _undoRedo.CanRedo;

        public int UndoCount => _undoRedo.UndoCount;
        public int RedoCount => _undoRedo.RedoCount;

        // Chèn text bằng command.
        public void InsertText(string text)
        {
            // TODO (TXT-11):
            // - Tạo InsertTextCommand với _document và text.
            // - Gọi _undoRedo.ExecuteCommand(command).
            throw new NotImplementedException();
        }

        // Xoá count ký tự cuối bằng command.
        public void RemoveLastCharacters(int count)
        {
            // TODO (TXT-12):
            // - Tạo RemoveLastCharactersCommand.
            // - Gọi _undoRedo.ExecuteCommand(command).
            throw new NotImplementedException();
        }

        // Giao tiếp với UndoRedoManager.
        public void Undo()
        {
            // TODO (TXT-13): Gọi _undoRedo.Undo().
            throw new NotImplementedException();
        }

        public void Redo()
        {
            // TODO (TXT-14): Gọi _undoRedo.Redo().
            throw new NotImplementedException();
        }
    }

    // =====================================================================
    // DOMAIN 2: NUMBER / CALCULATOR
    // =====================================================================

    // Lớp mô tả một giá trị số nguyên đơn giản.
    public sealed class NumberValue
    {
        // Giá trị hiện tại.
        private int _value;

        // Cho phép đọc giá trị hiện tại.
        public int Value => _value;

        public NumberValue(int initialValue = 0)
        {
            // TODO (NUM-1): Gán _value = initialValue.
            throw new NotImplementedException();
        }

        // Cộng thêm delta vào giá trị hiện tại.
        public void Add(int delta)
        {
            // TODO (NUM-2): Cập nhật _value = _value + delta.
            throw new NotImplementedException();
        }

        // Nhân giá trị hiện tại với factor.
        public void Multiply(int factor)
        {
            // TODO (NUM-3): Cập nhật _value = _value * factor.
            throw new NotImplementedException();
        }

        // Đặt trực tiếp giá trị (dùng cho Undo nâng cao).
        public void SetValue(int newValue)
        {
            // TODO (NUM-4): Gán _value = newValue.
            throw new NotImplementedException();
        }
    }

    // Lệnh cộng một số vào NumberValue.
    public sealed class AddNumberCommand : IUndoableCommand
    {
        private readonly NumberValue _number;
        private readonly int _delta;

        public string Name => $"Add ({_delta})";

        public AddNumberCommand(NumberValue number, int delta)
        {
            // TODO (NUM-5):
            // - Kiểm tra number null? => ArgumentNullException.
            // - Gán _number và _delta.
            throw new NotImplementedException();
        }

        public void Execute()
        {
            // TODO (NUM-6): Gọi _number.Add(_delta).
            throw new NotImplementedException();
        }

        public void Undo()
        {
            // TODO (NUM-7): Để Undo, chỉ cần _number.Add(-_delta).
            throw new NotImplementedException();
        }
    }

    // Lệnh nhân NumberValue với một hệ số.
    public sealed class MultiplyNumberCommand : IUndoableCommand
    {
        private readonly NumberValue _number;
        private readonly int _factor;

        // Lưu lại giá trị trước khi nhân để Undo.
        private int _previousValue;

        public string Name => $"Multiply by ({_factor})";

        public MultiplyNumberCommand(NumberValue number, int factor)
        {
            // TODO (NUM-8):
            // - Kiểm tra number null? => ArgumentNullException.
            // - Gán _number và _factor.
            throw new NotImplementedException();
        }

        public void Execute()
        {
            // TODO (NUM-9):
            // - Lưu _previousValue = _number.Value.
            // - Gọi _number.Multiply(_factor).
            throw new NotImplementedException();
        }

        public void Undo()
        {
            // TODO (NUM-10):
            // - Gọi _number.SetValue(_previousValue) để khôi phục lại giá trị cũ.
            throw new NotImplementedException();
        }
    }

    // Lớp "mặt tiền" để test hệ thống với NumberValue.
    public sealed class NumberCalculator
    {
        private readonly NumberValue _number;
        private readonly UndoRedoManager<IUndoableCommand> _undoRedo;

        public NumberCalculator(int initialValue = 0, int maxHistory = 0)
        {
            // TODO (NUM-11):
            // - Khởi tạo _number với initialValue.
            // - Khởi tạo _undoRedo với maxHistory.
            // - (Tuỳ chọn) Đăng ký OnExecuted để log lệnh.
            throw new NotImplementedException();
        }

        public int Value
        {
            get
            {
                // TODO (NUM-12): Trả về Value của _number.
                throw new NotImplementedException();
            }
        }

        public bool CanUndo => _undoRedo.CanUndo;
        public bool CanRedo => _undoRedo.CanRedo;

        public int UndoCount => _undoRedo.UndoCount;
        public int RedoCount => _undoRedo.RedoCount;

        public void Add(int delta)
        {
            // TODO (NUM-13):
            // - Tạo AddNumberCommand với _number và delta.
            // - Gọi _undoRedo.ExecuteCommand(command).
            throw new NotImplementedException();
        }

        public void Multiply(int factor)
        {
            // TODO (NUM-14):
            // - Tạo MultiplyNumberCommand.
            // - Gọi _undoRedo.ExecuteCommand(command).
            throw new NotImplementedException();
        }

        public void Undo()
        {
            // TODO (NUM-15): Gọi _undoRedo.Undo().
            throw new NotImplementedException();
        }

        public void Redo()
        {
            // TODO (NUM-16): Gọi _undoRedo.Redo().
            throw new NotImplementedException();
        }
    }

    // ---------------------------------------------------------------------
    // 8. Program: test nhanh 2 domain
    // ---------------------------------------------------------------------
    public class Program
    {
        static void Main()
        {
            // Test Text
            var textEditor = new TextEditor(maxHistory: 10);
            textEditor.InsertText("Hello");
            textEditor.InsertText(" World");
            Console.WriteLine(textEditor.Content); // Hello World
            textEditor.Undo();
            Console.WriteLine(textEditor.Content); // Hello

            // Test Number
            var calc = new NumberCalculator(initialValue: 10, maxHistory: 10);
            calc.Add(5);          // 15
            calc.Multiply(2);     // 30
            Console.WriteLine(calc.Value);
            calc.Undo();          // back to 15
            Console.WriteLine(calc.Value);
            calc.Undo();          // back to 10
            Console.WriteLine(calc.Value);
        }
    }
}
