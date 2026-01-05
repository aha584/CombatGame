using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCSharpPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- CHẠY BÀI TẬP 1 ---
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================");
            Console.WriteLine("BÀI 1: RULE ENGINE SYSTEM (HỆ THỐNG XỬ LÝ QUY TẮC)");
            Console.WriteLine("================================================================\n");
            Console.ResetColor();
            
            TransactionRuleSystem.TransactionDemo.Run();

            Console.WriteLine("\n\n");

            // --- CHẠY BÀI TẬP 2 ---
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("================================================================");
            Console.WriteLine("BÀI 2: MIDDLEWARE PIPELINE (HỆ THỐNG ĐƯỜNG ỐNG XỬ LÝ)");
            Console.WriteLine("================================================================\n");
            Console.ResetColor();

            MiddlewareSystem.PipelineDemo.Run();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}

// ==========================================================================================
// BÀI TẬP 1: TRANSACTION RULE ENGINE
// MỤC ĐÍCH HỆ THỐNG:
// - Xây dựng engine cho phép định nghĩa động các quy tắc nghiệp vụ (Business Rules) mà không cần sửa code cứng (hard-code if/else).
// - Ví dụ: Tự động đánh dấu giao dịch gian lận, tự động gửi email khi đơn hàng giá trị cao.
// - Áp dụng: Generics (xử lý nhiều loại model), Func/Action (lưu logic dưới dạng biến), OOP (đa hình).
// ==========================================================================================
namespace TransactionRuleSystem
{
    // --- 1. DOMAIN MODELS ---
    public interface ITransactionContext
    {
        string TransactionId { get; }
        decimal Amount { get; }
        DateTime Timestamp { get; }
    }

    public class BankingTransaction : ITransactionContext
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }
        public bool IsFlaggedFraud { get; set; } = false; // Cờ đánh dấu gian lận
    }

    public class EComOrder : ITransactionContext
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerLevel { get; set; } // VIP, Normal
        public decimal DiscountApplied { get; set; } = 0;
    }

    // --- 2. CORE ENGINE SKELETON (PHẦN BẠN CẦN LÀM) ---

    // Lớp đại diện cho một quy tắc đơn lẻ
    public class Rule<T> where T : ITransactionContext
    {
        public string RuleName { get; set; }
        
        // TODO 1.1: Khai báo một Property kiểu Delegate/Func nhận vào T và trả về bool
        // Dùng để kiểm tra xem transaction có thỏa mãn điều kiện quy tắc không.
        public Func<T, bool> Condition { get; set; }

        // TODO 1.2: Khai báo một Property kiểu Delegate/Action nhận vào T
        // Dùng để thực thi hành động nếu Condition == true.
        public Action<T> Action { get; set; }
    }

    // Lớp quản lý và chạy các quy tắc
    public class RuleEngine<T> where T : ITransactionContext
    {
        private readonly List<Rule<T>> _rules = new List<Rule<T>>();

        // TODO 1.3: Viết hàm AddRule theo phong cách Fluent API (trả về chính RuleEngine)
        // Hàm nhận vào tên rule, biểu thức điều kiện (lambda), và hành động (lambda)
        public RuleEngine<T> AddRule(string name, Func<T, bool> condition, Action<T> action)
        {
            // Code implement tại đây: Tạo object Rule<T> và add vào _rules
            Rule<T> rule = new();
            rule.RuleName = name;
            rule.Condition = condition;
            rule.Action = action;
            _rules.Add(rule);
            return this; 
        }

        // TODO 1.4: Viết hàm ExecuteAll để chạy danh sách dữ liệu qua các rule
        public void ExecuteAll(IEnumerable<T> items)
        {
            Console.WriteLine($"--- Executing Rules for {typeof(T).Name} ---");
            // Code implement tại đây:
            // - Duyệt qua từng item
            // - Với mỗi item, duyệt qua từng rule
            // - Nếu rule.Condition(item) trả về true thì gọi rule.Action(item)
            // - In ra Console tên rule nào đã được kích hoạt
            foreach(var item in items)
            {
                foreach(var rule in _rules)
                {
                    if (rule.Condition(item))
                    {
                        rule.Action(item);
                        Console.WriteLine($"Rule: {rule.RuleName} is Active");
                    }
                }
            }
        }
    }

    // --- 3. TEST DATA & EXECUTION ---
    public static class TransactionDemo
    {
        public static void Run()
        {
            // Data giả lập
            var bankTrans = new List<BankingTransaction>
            {
                new BankingTransaction { TransactionId = "TX001", Amount = 500, SourceAccount = "VN", DestinationAccount = "VN" },
                new BankingTransaction { TransactionId = "TX002", Amount = 90000000, SourceAccount = "VN", DestinationAccount = "US" }, // Đáng ngờ
                new BankingTransaction { TransactionId = "TX003", Amount = 100, SourceAccount = "US", DestinationAccount = "IRAN" } // Cấm
            };

            var orders = new List<EComOrder>
            {
                new EComOrder { TransactionId = "ORD1", Amount = 200, CustomerLevel = "Normal" },
                new EComOrder { TransactionId = "ORD2", Amount = 5000, CustomerLevel = "VIP" } // Đơn to + VIP
            };

            // --- BÀI TẬP: KHỞI TẠO ENGINE VÀ RULE ---

            // 1. Engine cho Ngân hàng
            var bankEngine = new RuleEngine<BankingTransaction>();

            // Rule: Nếu chuyển tiền > 10,000,000 ra nước ngoài -> Đánh dấu gian lận
            bankEngine.AddRule(
                "Anti-Money Laundering",
                t => t.Amount > 10000000 && t.SourceAccount != t.DestinationAccount,
                t => 
                {
                    t.IsFlaggedFraud = true;
                    Console.WriteLine($"[ALERT] Giao dịch {t.TransactionId} bị đánh dấu gian lận!");
                }
            );

            // Rule: Nếu chuyển tới tài khoản bị cấm (Ví dụ chứa "IRAN") -> In cảnh báo
            bankEngine.AddRule(
                "Sanction Check",
                t => t.DestinationAccount == "IRAN",
                t => Console.WriteLine($"[BLOCK] Chặn giao dịch {t.TransactionId} tới quốc gia bị cấm.")
            );

            // 2. Engine cho Thương mại điện tử
            var orderEngine = new RuleEngine<EComOrder>();

            // Rule: Nếu khách VIP và đơn > 1000 -> Giảm giá 10%
            orderEngine.AddRule(
                "VIP Discount",
                o => o.CustomerLevel == "VIP" && o.Amount > 1000,
                o => 
                {
                    o.DiscountApplied = o.Amount * 0.1m;
                    Console.WriteLine($"[INFO] Đơn hàng {o.TransactionId} được giảm giá: {o.DiscountApplied}");
                }
            );

            // THỰC THI
            bankEngine.ExecuteAll(bankTrans);
            Console.WriteLine();
            orderEngine.ExecuteAll(orders);
        }
    }
}

// ==========================================================================================
// BÀI TẬP 2: MIDDLEWARE PIPELINE
// MỤC ĐÍCH HỆ THỐNG:
// - Mô phỏng cơ chế Pipeline của ASP.NET Core hoặc kiến trúc Microservices.
// - Dữ liệu đi qua một đường ống, qua các tầng (Middleware) xử lý tuần tự (Logging -> Auth -> Validate -> Main Process).
// - Các tầng có quyền dừng quy trình (Short-circuit) hoặc sửa đổi dữ liệu.
// - Áp dụng: Delegate Chain (Callback hell pattern), Lambda, Generics.
// ==========================================================================================
namespace MiddlewareSystem
{
    // --- 1. CORE FRAMEWORK SKELETON (PHẦN BẠN CẦN LÀM) ---

    // Dữ liệu ngữ cảnh đi xuyên suốt pipeline
    public class PipeContext<T>
    {
        public T Payload { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public bool IsAborted { get; private set; }

        public void Abort() => IsAborted = true;
    }

    // Delegate đại diện cho "hàm tiếp theo" trong chuỗi
    public delegate void NextDelegate();

    // Interface cho Middleware dạng class
    public interface IMiddleware<T>
    {
        // Invoke nhận vào Context và hàm next
        void Invoke(PipeContext<T> context, NextDelegate next);
    }

    public class PipelineBuilder<T>
    {
        // Danh sách các component. Mỗi component là một Action nhận Context và NextDelegate
        private readonly List<Action<PipeContext<T>, NextDelegate>> _components = new();

        // TODO 2.1: Implement hàm Use để đăng ký Middleware dạng Class
        public PipelineBuilder<T> Use(IMiddleware<T> middleware)
        {
            // Gợi ý: Wrap middleware.Invoke vào Lambda Action rồi add vào _components
            // _components.Add((ctx, next) => ... );
            _components.Add((ctx, next) => middleware?.Invoke(ctx, next));
            return this;
        }

        // TODO 2.2: Implement hàm Use để đăng ký Middleware dạng Lambda (Inline)
        public PipelineBuilder<T> Use(Action<PipeContext<T>, NextDelegate> middlewareAction)
        {
            // Code tại đây
            if (middlewareAction == null) return null;
            _components.Add(middlewareAction);
            return this;
        }

        // TODO 2.3: Hàm Build - Phần khó nhất (Design Pattern: Chain of Responsibility)
        // Mục tiêu: Kết nối tất cả các Action trong list _components thành một Action<PipeContext<T>> duy nhất.
        public Action<PipeContext<T>> Build()
        {
            Action<PipeContext<T>> app = context => 
            { 
                // Đây là điểm cuối cùng của pipeline (khi không còn middleware nào nữa)
                Console.WriteLine("--- End of Pipeline ---"); 
            };

            // Gợi ý: Dùng vòng lặp ngược (Reverse) từ component cuối lên đầu.
            // Component hiện tại sẽ "bao bọc" component kế tiếp.
            // next = () => component(context, nextCu)

            for (int i = _components.Count - 1; i > -1; i--)
            {
                var past = app;
                app = ctx => _components[i](ctx, () => past(ctx));
            }

            /* Code gợi ý logic (bạn hãy tự viết lại cho hiểu):
             * var componentsReversed = _components.AsEnumerable().Reverse().ToList();
             * foreach (var component in componentsReversed)
             * {
             *     var currentNext = app;
             *     app = ctx => component(ctx, () => currentNext(ctx));
             * }
             */

            return app;
        }
    }

    // --- 2. IMPLEMENTATION VÍ DỤ ---
    
    // Model dữ liệu web request giả lập
    public class HttpRequestFake
    {
        public string Url { get; set; }
        public string UserRole { get; set; } // Admin, Guest
        public string Body { get; set; }
    }

    // Middleware xử lý Exception toàn cục
    public class ExceptionHandlingMiddleware : IMiddleware<HttpRequestFake>
    {
        public void Invoke(PipeContext<HttpRequestFake> context, NextDelegate next)
        {
            try
            {
                Console.WriteLine("[TryCatch] Bắt đầu theo dõi lỗi...");
                next(); // Gọi thằng tiếp theo
                Console.WriteLine("[TryCatch] Không có lỗi xảy ra.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TryCatch] ĐÃ BẮT ĐƯỢC LỖI: {ex.Message}");
                // Không throw tiếp để app không crash
            }
        }
    }

    // --- 3. TEST EXECUTION ---
    public static class PipelineDemo
    {
        public static void Run()
        {
            var builder = new PipelineBuilder<HttpRequestFake>();

            // THIẾT KẾ PIPELINE
            builder
                // 1. Error Handling (Dạng Class)
                .Use(new ExceptionHandlingMiddleware())
                
                // 2. Logger (Dạng Lambda)
                .Use((ctx, next) => 
                {
                    Console.WriteLine($"[Log] Request tới: {ctx.Payload.Url}");
                    next();
                    Console.WriteLine($"[Log] Đã xử lý xong {ctx.Payload.Url}");
                })

                // 3. Authentication (Dạng Lambda - Có chặn luồng)
                .Use((ctx, next) =>
                {
                    if (ctx.Payload.UserRole == "Guest" && ctx.Payload.Url.Contains("/admin"))
                    {
                        Console.WriteLine("[Auth] 403 Forbidden! Bạn không phải Admin.");
                        ctx.Abort(); 
                        return; // KHÔNG gọi next(), pipeline dừng tại đây
                    }
                    Console.WriteLine("[Auth] Quyền hợp lệ.");
                    next();
                })

                // 4. Core Business Logic
                .Use((ctx, next) =>
                {
                    Console.WriteLine(">>> ĐANG XỬ LÝ LOGIC NGHIỆP VỤ (DB Save...)");
                    if (ctx.Payload.Body == "DIE") throw new Exception("Database Connection Failed!");
                    next();
                });

            // Build pipeline
            var app = builder.Build();

            // --- TEST CASES ---

            Console.WriteLine("--- CASE 1: Request hợp lệ ---");
            app(new PipeContext<HttpRequestFake> { 
                Payload = new HttpRequestFake { Url = "/admin/users", UserRole = "Admin", Body = "Hello" } 
            });

            Console.WriteLine("\n--- CASE 2: Request bị chặn quyền ---");
            app(new PipeContext<HttpRequestFake> { 
                Payload = new HttpRequestFake { Url = "/admin/settings", UserRole = "Guest", Body = "Hack" } 
            });

            Console.WriteLine("\n--- CASE 3: Request gây lỗi hệ thống ---");
            app(new PipeContext<HttpRequestFake> { 
                Payload = new HttpRequestFake { Url = "/home", UserRole = "User", Body = "DIE" } 
            });
        }
    }
}