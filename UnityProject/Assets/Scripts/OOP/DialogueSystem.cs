//
// Hoàn thiện tất cả các TODO và thay thế toàn bộ throw exception.
// Yêu cầu sinh viên áp dụng OOP: class, composition, collection, xử lý lựa chọn, kiểm tra lỗi.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem
{
    /// <summary>
    /// Đại diện cho một lựa chọn hội thoại (option).
    /// Ví dụ: 
    /// - Text: "Tôi đồng ý"
    /// - NextNodeId: "accept_quest"
    /// </summary>
    public class DialogueOption
    {
        // TODO: Thêm property chỉ đọc cho Text.
        private readonly string _text;
        // TODO: Thêm property chỉ đọc cho NextNodeId.
        private readonly string _nextNodeId;

        public string Text => _text;
        public string NextNodeId => _nextNodeId;

        public DialogueOption(string text, string nextNodeId)
        {
            // TODO:
            // - Gán text, nextNodeId.
            // - Kiểm tra text không rỗng.
            // - nextNodeId có thể null hoặc rỗng nếu là kết thúc (tùy thiết kế).
            if(text != null)
            {
                _text = text;
            }
            _nextNodeId = nextNodeId;
        }
    }

    /// <summary>
    /// Một node hội thoại (một câu nói + các lựa chọn).
    /// </summary>
    public class DialogueNode
    {
        // TODO: Thêm property chỉ đọc Id.
        private readonly string _id;
        // TODO: Thêm property chỉ đọc Text.
        private readonly string _text;
        // TODO: Thêm danh sách các lựa chọn (List<DialogueOption>).
        private List<DialogueOption> _dialogueOptions = new();

        public string Id => _id;
        public string Text => _text;

        // Gợi ý: dùng List<DialogueOption> cho nội bộ, và IReadOnlyList để lộ ra ngoài.
        public IReadOnlyList<DialogueOption> Options => _dialogueOptions;

        public DialogueNode(string id, string text)
        {
            // TODO:
            // - Gán id, text.
            // - Kiểm tra id/text không rỗng.
            if (_id != null)
            {
                _id = id;
            }
            if (_text != null)
            {
                _text = text;
            }
        }

        /// <summary>
        /// Thêm một lựa chọn vào node.
        /// </summary>
        public void AddOption(DialogueOption option)
        {
            // TODO:
            // - Kiểm tra option != null.
            // - Thêm vào danh sách.
            if(option != null)
            {
                _dialogueOptions.Add(option);
            }
        }

        /// <summary>
        /// Node kết thúc nếu không có lựa chọn nào.
        /// </summary>
        public bool IsTerminal()
        {
            // TODO: Trả về true nếu không có option nào.
            if(_dialogueOptions.Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// In ra câu thoại và danh sách lựa chọn.
        /// </summary>
        public void Log()
        {
            // TODO:
            // - In Text.
            // - Nếu có lựa chọn: in từng option với index.
            Console.WriteLine(Text);
            if(_dialogueOptions.Count > 0)
            {
                for(int i = 0; i < _dialogueOptions.Count; i++)
                {
                    DialogueOption option = _dialogueOptions[i];
                    Console.WriteLine($"{option.Text}\n");
                }
            }
        }
    }

    /// <summary>
    /// Quản lý toàn bộ cây hội thoại.
    /// </summary>
    public class DialogueManager
    {
        // TODO: Lưu tất cả node theo Id, gợi ý: Dictionary<string, DialogueNode>
        // TODO: Lưu node hiện tại (currentNode).

        private readonly Dictionary<string, DialogueNode> _nodes = new();
        private DialogueNode _currentNode;

        /// <summary>
        /// Thêm node vào hệ thống.
        /// Không cho phép trùng Id.
        /// </summary>
        public void AddNode(DialogueNode node)
        {
            // TODO:
            // - Kiểm tra node != null.
            // - Kiểm tra node.Id chưa tồn tại trong _nodes.
            // - Nếu trùng: có thể ném Exception.
            // - Nếu không: thêm vào _nodes.
            if(node != null)
            {
                if (!_nodes.ContainsKey(node.Id))
                {
                    _nodes.Add(node.Id, node);
                }
                else
                {
                    _nodes[node.Id] = node;
                }
            }
        }

        /// <summary>
        /// Bắt đầu hội thoại từ node có id tương ứng.
        /// </summary>
        public void Start(string startNodeId)
        {
            // TODO:
            // - Kiểm tra startNodeId không rỗng.
            // - Tìm trong _nodes, nếu không thấy: ném Exception hoặc in lỗi.
            // - Nếu thấy: gán _currentNode, và in ra nội dung node hiện tại (LogCurrentNode).
            if(startNodeId != null)
            {
                if (_nodes.ContainsKey(startNodeId))
                {
                    _currentNode = _nodes[startNodeId];
                    LogCurrentNode();
                }
                else
                {
                    Console.WriteLine($"Can't find dialogue with {startNodeId} ID!");
                }
            }
        }

        /// <summary>
        /// In nội dung node hiện tại và các lựa chọn (nếu có).
        /// </summary>
        public void LogCurrentNode()
        {
            // TODO:
            // - Nếu _currentNode == null: in "No active dialogue."
            // - Ngược lại: gọi _currentNode.Log().
            if(_currentNode != null)
            {
                _currentNode.Log();
            }
            else
            {
                Console.WriteLine("No Active Dialogue");
            }
        }

        /// <summary>
        /// Người chơi chọn một lựa chọn theo index (1-based).
        /// </summary>
        public void ChooseOption(int optionIndex)
        {
            // TODO:
            // - Nếu _currentNode == null: in "No active dialogue." và return.
            // - Lấy danh sách Options của _currentNode.
            // - Kiểm tra optionIndex trong khoảng hợp lệ (1..Options.Count).
            //   Nếu không hợp lệ: in "Invalid option." và return.
            // - Lấy DialogueOption tương ứng.
            // - Nếu NextNodeId null hoặc rỗng → kết thúc hội thoại (set _currentNode = null).
            // - Nếu NextNodeId có giá trị:
            //     + Tìm node tiếp theo trong _nodes.
            //     + Nếu không tìm thấy: in "Missing node: {NextNodeId}", set _currentNode = null.
            //     + Nếu tìm thấy: gán _currentNode = node tiếp theo.
            // - Sau khi cập nhật _currentNode, gọi LogCurrentNode().
            if (_currentNode != null)
            {
                if(optionIndex - 1 >= 0 && optionIndex - 1 <= _currentNode.Options.Count - 1)
                {
                    DialogueOption option = _currentNode.Options[optionIndex - 1];
                    if (option.NextNodeId !=null || option.NextNodeId == "")
                    {
                        _currentNode = null;
                        return;
                    }
                    else
                    {
                        if (_nodes.ContainsKey(option.NextNodeId))
                        {
                            _currentNode = _nodes[option.NextNodeId];
                            LogCurrentNode();
                        }
                        else
                        {
                            Console.WriteLine($"Missing Node: {option.NextNodeId}");
                            _currentNode = null;
                            return;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Option!");
                    return;
                }
            }
            else
            {
                Console.WriteLine("No Active Dialogue");
            }
        }

        /// <summary>
        /// Kiểm tra xem hội thoại đã kết thúc chưa.
        /// </summary>
        public bool IsDialogueFinished()
        {
            // TODO:
            // - Kết thúc nếu _currentNode == null
            //   hoặc _currentNode.IsTerminal() == true.
            if (_currentNode == null || _currentNode.IsTerminal()) return true;
            return false;
        }

        /// <summary>
        /// Reset toàn bộ trạng thái hội thoại (không xoá dữ liệu node).
        /// </summary>
        public void Reset()
        {
            // TODO:
            // - Đưa _currentNode về null.
            _currentNode = null;
        }
    }


    /*
    Node start
      NPC: “Xin chào, bạn có muốn giúp tôi không?”
        1. “Vâng, kể tôi nghe nhiệm vụ.” → sang quest_explain
        2. “Không, tôi bận.” → kết thúc

    Node quest_explain
      NPC: “Ngôi làng đang bị lũ quái quấy phá. Bạn có nhận nhiệm vụ không?”
        1. “Được, tôi sẽ giúp.” → sang accept
        2. “Xin lỗi, tôi không thể.” → sang decline

    Node accept
      NPC: “Tuyệt vời! Đây là phần thưởng sau khi hoàn thành.” (kết thúc)

    Node decline
      NPC: “Tôi hiểu, hy vọng lần sau.” (kết thúc)
    */


    public class DialogueTest
    {
        public static void Main()
        {
            // Tạo DialogueManager
            DialogueManager manager = new DialogueManager();

            // ------------------------
            // Tạo các node hội thoại
            // ------------------------

            // Node bắt đầu: start
            DialogueNode startNode = new DialogueNode(
                id: "start",
                text: "NPC: Xin chào, bạn có muốn giúp tôi không?"
            );

            // Thêm các lựa chọn cho node start
            startNode.AddOption(new DialogueOption(
                text: "Vâng, kể tôi nghe nhiệm vụ.",
                nextNodeId: "quest_explain"
            ));

            startNode.AddOption(new DialogueOption(
                text: "Không, tôi bận.",
                nextNodeId: null   // hoặc "" → coi như kết thúc hội thoại
            ));

            // Node: quest_explain
            DialogueNode questExplainNode = new DialogueNode(
                id: "quest_explain",
                text: "NPC: Ngôi làng đang bị lũ quái quấy phá. Bạn có nhận nhiệm vụ không?"
            );

            questExplainNode.AddOption(new DialogueOption(
                text: "Được, tôi sẽ giúp.",
                nextNodeId: "accept"
            ));

            questExplainNode.AddOption(new DialogueOption(
                text: "Xin lỗi, tôi không thể.",
                nextNodeId: "decline"
            ));

            // Node: accept (kết thúc)
            DialogueNode acceptNode = new DialogueNode(
                id: "accept",
                text: "NPC: Tuyệt vời! Đây là phần thưởng sau khi bạn hoàn thành nhiệm vụ."
            );
            // Không thêm option → node kết thúc

            // Node: decline (kết thúc)
            DialogueNode declineNode = new DialogueNode(
                id: "decline",
                text: "NPC: Tôi hiểu, hy vọng lần sau bạn sẽ có thời gian."
            );
            // Không thêm option → node kết thúc

            // ------------------------
            // Đăng ký node vào manager
            // ------------------------

            manager.AddNode(startNode);
            manager.AddNode(questExplainNode);
            manager.AddNode(acceptNode);
            manager.AddNode(declineNode);

            // ------------------------
            // Bắt đầu hội thoại từ node "start"
            // ------------------------

            manager.Start("start");

            // Vòng lặp đơn giản để test trong Console
            while (!manager.IsDialogueFinished())
            {
                Console.WriteLine();
                Console.Write("Chọn lựa chọn (nhập số, hoặc Enter để thoát): ");

                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Thoát hội thoại.");
                    break;
                }

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Giá trị không hợp lệ, vui lòng nhập số.");
                    continue;
                }

                manager.ChooseOption(choice);
            }

            Console.WriteLine();
            Console.WriteLine("Hội thoại đã kết thúc. Nhấn phím bất kỳ để thoát.");
            Console.ReadKey();
        }
    }
}
