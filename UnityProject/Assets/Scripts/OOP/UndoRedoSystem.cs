using System;
using System.Collections.Generic;

// Bài 22: Hệ thống Undo/Redo tổng quát (UndoRedoManager)
// YÊU CẦU:
// - Hoàn thiện tất cả TODO trong file này.
// - Thay thế toàn bộ throw new NotImplementedException() bằng code của bạn.
// - Không sửa tên class / interface / public method (có thể thêm method private nếu cần).
//
// MỤC TIÊU KIẾN THỨC:
// - Interface + đóng gói (encapsulation).
// - Command Pattern: mỗi thao tác được gói thành "lệnh" có thể Undo/Redo.
// - Quản lý lịch sử Undo/Redo bằng Stack.
// - Thiết kế hệ thống Undo/Redo dùng chung cho nhiều loại lệnh, không chỉ text.
//
// GỢI Ý SỬ DỤNG:
// - Mọi lệnh muốn Undo/Redo được đều implement IUndoableCommand.
// - UndoRedoManager chỉ phụ trách quản lý lịch sử, không biết chi tiết logic của từng lệnh.

namespace OopExercises.UndoRedoSystem
{
    // Interface mô tả "một lệnh có thể Undo/Redo".
    public interface IUndoableCommand
    {
        // Thực hiện lệnh.
        void Execute();

        // Hoàn tác lệnh đã thực hiện.
        void Undo();

        // Tên/miêu tả ngắn gọn cho lệnh (dùng để debug / log).
        string Name { get; }
    }

    // Hệ thống Undo/Redo tổng quát.
    public sealed class UndoRedoManager
    {
        // Stack lưu các lệnh đã thực thi và có thể Undo.
        private readonly Stack<IUndoableCommand> _undoStack = new();

        // Stack lưu các lệnh đã Undo và có thể Redo.
        private readonly Stack<IUndoableCommand> _redoStack = new();

        // Giới hạn số lượng lệnh tối đa lưu trong lịch sử.
        // Nếu MaxHistory <= 0 => không giới hạn.
        public int MaxHistory { get; }

        public UndoRedoManager(int maxHistory = 0)
        {
            // TODO:
            // - Gán MaxHistory = maxHistory.
            // - Nếu maxHistory < 0 có thể ép về 0 (coi như không giới hạn).
            if(maxHistory < 0)
            {
                maxHistory = 0;
            }
            MaxHistory = maxHistory;
        }

        // Số lệnh đang có trong stack Undo.
        public int UndoCount
        {
            get
            {
                // TODO: trả về số phần tử trong _undoStack.
                return _undoStack.Count;
            }
        }

        // Số lệnh đang có trong stack Redo.
        public int RedoCount
        {
            get
            {
                // TODO: trả về số phần tử trong _redoStack.
                return _redoStack.Count;
            }
        }

        // Cho biết hiện tại có thể Undo không.
        public bool CanUndo
        {
            get
            {
                // TODO: trả về true nếu _undoStack có ít nhất 1 phần tử.
                if(_undoStack.Count >= 1)
                {
                    return true;
                }
                return false;
            }
        }

        // Cho biết hiện tại có thể Redo không.
        public bool CanRedo
        {
            get
            {
                // TODO: trả về true nếu _redoStack có ít nhất 1 phần tử.
                if (_redoStack.Count >= 1)
                {
                    return true;
                }
                return false;
            }
        }

        // Thực thi một lệnh mới:
        // - Gọi Execute() của lệnh.
        // - Đẩy vào Undo stack.
        // - Xoá sạch Redo stack (vì lịch sử Redo cũ không còn hợp lệ).
        // - Nếu vượt quá MaxHistory thì bỏ bớt lệnh cũ nhất.
        public void ExecuteCommand(IUndoableCommand command)
        {
            // TODO:
            // - Kiểm tra command null? => ArgumentNullException.
            // - Gọi command.Execute().
            // - Đẩy command vào _undoStack.
            // - Xoá sạch _redoStack.
            // - Nếu MaxHistory > 0 và UndoCount > MaxHistory thì loại bỏ lệnh cũ nhất.
            //   Gợi ý: để loại bỏ lệnh cũ nhất, có thể:
            //     + Chuyển stack sang mảng, hoặc
            //     + Tạo một stack tạm để đảo ngược thứ tự.
            if(command == null)
            {
                throw new ArgumentNullException("There is no command!");
            }
            else
            {
                command.Execute();
                _undoStack.Push(command);
                _redoStack.Clear();
                if(MaxHistory > 0 && UndoCount > MaxHistory)
                {
                    Stack<IUndoableCommand> _revUndoStack = new();
                    while (_undoStack.Count > 0)
                    {
                        _revUndoStack.Push(_undoStack.Pop());
                    }
                    _revUndoStack.Pop();
                    while(_revUndoStack.Count > 0)
                    {
                        _undoStack.Push(_revUndoStack.Pop());
                    }
                    
                }
            }
        }

        // Hoàn tác lệnh gần nhất.
        public void Undo()
        {
            // TODO:
            // - Nếu !CanUndo thì có thể:
            //     + return; hoặc
            //     + ném InvalidOperationException (tùy bạn chọn).
            // - Pop một lệnh từ _undoStack.
            // - Gọi Undo() trên lệnh đó.
            // - Đẩy lệnh này sang _redoStack.
            if (!CanUndo)
            {
                return;
            }
            else
            {
                IUndoableCommand command = _undoStack.Pop();
                _redoStack.Push(command);

            }
        }

        // Thực hiện lại lệnh vừa bị Undo.
        public void Redo()
        {
            // TODO:
            // - Nếu !CanRedo thì có thể return hoặc ném InvalidOperationException.
            // - Pop một lệnh từ _redoStack.
            // - Gọi Execute() trên lệnh đó.
            // - Đẩy lệnh này sang _undoStack.
            if (!CanRedo)
            {
                return;
            }
            else
            {
                IUndoableCommand command = _redoStack.Pop();
                _undoStack.Push(command);

            }
        }

        // Xoá toàn bộ lịch sử Undo/Redo.
        public void ClearHistory()
        {
            // TODO: Clear cả _undoStack và _redoStack.
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }

    // ======================================================================
    // DOMAIN 1: TEXT EDITOR (MINH HOẠ)
    // ======================================================================

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
            // TODO: Hoàn thiện logic chèn text vào cuối _content.
            // Gợi ý: nếu text null => coi như chuỗi rỗng, sau đó nối chuỗi.
            if (string.IsNullOrEmpty(text)) text = "";
            _content += text;
        }

        // Xoá count ký tự từ cuối chuỗi.
        // Nếu count lớn hơn độ dài chuỗi hiện tại, xoá toàn bộ.
        public void RemoveLastCharacters(int count)
        {
            // TODO:
            // - Nếu count <= 0 thì không làm gì.
            // - Nếu count >= độ dài chuỗi hiện tại => _content = string.Empty.
            // - Ngược lại: cắt chuỗi bằng Substring.
            if(count <= 0)
            {
                return;
            }
            else if(count >= _content.Length)
            {
                _content = string.Empty;
            }
            else
            {
                string subContent = _content.Substring(_content.Length - 1 - count, count);
                _content.Replace(subContent, string.Empty);
            }
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
            // TODO:
            // - Kiểm tra document null? => ArgumentNullException.
            // - Gán _document và _text (nếu text null có thể chuyển thành string.Empty).
            if(document == null)
            {
                throw new ArgumentNullException("There is no document!!");
            }
            else
            {
                _document = document;
                if (text == null) text = string.Empty;
                _text = text;
            }
        }

        public void Execute()
        {
            // TODO:
            // - Lưu lại độ dài Content hiện tại vào _previousLength.
            // - Gọi InsertText trên _document để chèn _text.
            _previousLength = _document.Content.Length;
            _document.InsertText(_text);
        }

        public void Undo()
        {
            // TODO:
            // - Tính số ký tự đã chèn = Content.Length - _previousLength.
            // - Nếu số này <= 0 thì không làm gì (phòng lỗi).
            // - Gọi RemoveLastCharacters() với số ký tự đã chèn.
            int charToInsert = _document.Content.Length - _previousLength;
            if(charToInsert <= 0)
            {
                return;
            }
            else
            {
                _document.RemoveLastCharacters(charToInsert);
            }
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
            // TODO:
            // - Kiểm tra document null? => ArgumentNullException.
            // - Kiểm tra countToRemove >= 0, nếu < 0 => ArgumentOutOfRangeException.
            // - Gán vào field tương ứng.
            if(_document == null)
            {
                throw new ArgumentNullException("there is no document!!");
            }
            else
            {
                _document = document;
                if(countToRemove >= 0)
                {
                    _countToRemove = countToRemove;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Qua gioi han");
                }
            }
        }

        public void Execute()
        {
            // TODO:
            // - Nếu _countToRemove <= 0 => không làm gì.
            // - Lấy độ dài hiện tại của Content.
            // - Tính số ký tự thực sự sẽ bị xoá = min(_countToRemove, length).
            // - Lưu đoạn text sẽ bị xoá (phần cuối chuỗi) vào _removedText.
            // - Gọi RemoveLastCharacters() trên _document với số ký tự thực sự bị xoá.
            if (_countToRemove <= 0) return;
            else
            {
                int contentLength = _document.Content.Length;
                int removeCharNumber = Math.Min(_countToRemove, contentLength);
                _removedText = _document.Content.Substring(contentLength - 1 - removeCharNumber, removeCharNumber);
                _document.RemoveLastCharacters(removeCharNumber);
            }
        }

        public void Undo()
        {
            // TODO:
            // - Chèn lại _removedText vào cuối tài liệu (InsertText).
            // - Có thể kiểm tra _removedText null/empty để tránh lỗi.
            _document.InsertText(_removedText);
        }
    }

    // Lớp "surface" để test hệ thống với TextDocument.
    public sealed class TextEditor
    {
        private readonly TextDocument _document;
        private readonly UndoRedoManager _undoRedo;

        public TextEditor(int maxHistory = 0)
        {
            // TODO:
            // - Khởi tạo _document.
            // - Khởi tạo _undoRedo với maxHistory.
            _document = new();
            _undoRedo = new(maxHistory);
        }

        // Thuộc tính cho phép xem nội dung hiện tại.
        public string Content
        {
            get
            {
                // TODO: trả về Content của _document.
                return _document.Content;
            }
        }

        public bool CanUndo => _undoRedo.CanUndo;
        public bool CanRedo => _undoRedo.CanRedo;

        public int UndoCount => _undoRedo.UndoCount;
        public int RedoCount => _undoRedo.RedoCount;

        // Chèn text bằng command.
        public void InsertText(string text)
        {
            // TODO:
            // - Tạo InsertTextCommand với _document và text.
            // - Gọi _undoRedo.ExecuteCommand(command).
            InsertTextCommand insertCommand = new(_document, text);
            _undoRedo.ExecuteCommand(insertCommand);
        }

        // Xoá count ký tự cuối bằng command.
        public void RemoveLastCharacters(int count)
        {
            // TODO:
            // - Tạo RemoveLastCharactersCommand.
            // - Gọi _undoRedo.ExecuteCommand(command).
            RemoveLastCharactersCommand removeCommand = new(_document, count);
            _undoRedo.ExecuteCommand(removeCommand);
        }

        // Giao tiếp với UndoRedoManager.
        public void Undo()
        {
            // TODO: Gọi Undo trên _undoRedo.
            _undoRedo.Undo();
        }

        public void Redo()
        {
            // TODO: Gọi Redo trên _undoRedo.
            _undoRedo.Redo();
        }
    }

    // ======================================================================
    // DOMAIN 2: NUMBER / CALCULATOR (MINH HOẠ THỨ HAI)
    // ======================================================================

    // Lớp mô tả một giá trị số nguyên đơn giản.
    public sealed class NumberValue
    {
        // Giá trị hiện tại.
        private int _value;

        // Cho phép đọc giá trị hiện tại.
        public int Value => _value;

        public NumberValue(int initialValue = 0)
        {
            // TODO: Gán _value = initialValue.
            _value = initialValue;
        }

        // Cộng thêm delta vào giá trị hiện tại.
        public void Add(int delta)
        {
            // TODO: Cập nhật _value = _value + delta.
            _value += delta;
        }

        // Nhân giá trị hiện tại với factor.
        public void Multiply(int factor)
        {
            // TODO: Cập nhật _value = _value * factor.
            _value += factor;
        }

        // Đặt trực tiếp giá trị (dùng cho Undo nâng cao).
        public void SetValue(int newValue)
        {
            // TODO: Gán _value = newValue.
            _value = newValue;
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
            // TODO:
            // - Kiểm tra number null? => ArgumentNullException.
            // - Gán _number và _delta.
            if (number == null)
            {
                throw new ArgumentNullException("Number is null");
            }
            else
            {
                _number = number;
                _delta = delta;
            }
        }

        public void Execute()
        {
            // TODO: Gọi Add(delta) trên _number.
            _number.Add(_delta);
        }

        public void Undo()
        {
            // TODO: Để Undo, chỉ cần Add(-delta).
            _number.Add(-_delta);
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
            // TODO:
            // - Kiểm tra number null? => ArgumentNullException.
            // - Gán _number và _factor.
            if (number == null)
            {
                throw new ArgumentNullException("Number is null");
            }
            else
            {
                _number = number;
                _previousValue = number.Value;
                _factor = factor;
            }
        }

        public void Execute()
        {
            // TODO:
            // - Lưu _previousValue = _number.Value.
            // - Gọi Multiply(_factor) trên _number.
            _number.Multiply(_factor);
        }

        public void Undo()
        {
            // TODO:
            // - Gọi SetValue(_previousValue) để khôi phục lại giá trị cũ.
            _number.SetValue(_previousValue);
        }
    }

    // Lớp "mặt tiền" để test hệ thống với NumberValue.
    public sealed class NumberCalculator
    {
        private readonly NumberValue _number;
        private readonly UndoRedoManager _undoRedo;

        public NumberCalculator(int initialValue = 0, int maxHistory = 0)
        {
            // TODO:
            // - Khởi tạo _number với initialValue.
            // - Khởi tạo _undoRedo với maxHistory.
            _number = new(initialValue);
            _undoRedo = new(maxHistory);
        }

        public int Value
        {
            get
            {
                // TODO: Trả về Value của _number.
                return _number.Value;
            }
        }

        public bool CanUndo => _undoRedo.CanUndo;
        public bool CanRedo => _undoRedo.CanRedo;

        public int UndoCount => _undoRedo.UndoCount;
        public int RedoCount => _undoRedo.RedoCount;

        public void Add(int delta)
        {
            // TODO:
            // - Tạo AddNumberCommand với _number và delta.
            // - Gọi _undoRedo.ExecuteCommand(command)
            AddNumberCommand addCommand = new(_number, delta);
            _undoRedo.ExecuteCommand(addCommand);
        }

        public void Multiply(int factor)
        {
            // TODO:
            // - Tạo MultiplyNumberCommand.
            // - Gọi _undoRedo.ExecuteCommand(command).
            MultiplyNumberCommand multiCommand = new(_number, factor);
        }

        public void Undo()
        {
            // TODO: Gọi Undo trên _undoRedo.
            _undoRedo.Undo();
        }

        public void Redo()
        {
            // TODO: Gọi Redo trên _undoRedo.
            _undoRedo.Redo();
        }
    }

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
