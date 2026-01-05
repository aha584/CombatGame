using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedSyncSagaSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=======================================================================");
            Console.WriteLine("BÀI TẬP: SYNCHRONOUS WORKFLOW ENGINE WITH ROLLBACK (SAGA PATTERN)");
            Console.WriteLine("Mô phỏng: Quản lý giao dịch, tự động hoàn tác khi có lỗi.");
            Console.WriteLine("=======================================================================\n");
            Console.ResetColor();

            // --- TEST CASE 1: HAPPY PATH (Thành công toàn bộ) ---
            Console.WriteLine("--- [CASE 1] SUCCESSFUL ORDER ---");
            var orderContext1 = new OrderContext 
            { 
                OrderId = "ORD-001", 
                ProductName = "Laptop Gaming", 
                Price = 2000, 
                CustomerBalance = 5000, 
                InventoryCount = 10 
            };

            RunOrderWorkflow(orderContext1, shouldFailAtPayment: false);
            
            Console.WriteLine($"\n[Result 1] Balance: {orderContext1.CustomerBalance}, Inventory: {orderContext1.InventoryCount}");
            Console.WriteLine("-----------------------------------------------------------------------\n");


            // --- TEST CASE 2: FAILURE & ROLLBACK (Lỗi giữa chừng) ---
            Console.WriteLine("--- [CASE 2] FAILED ORDER (INSUFFICIENT FUNDS) ---");
            // Giả lập khách hàng không đủ tiền, nhưng hàng đã bị trừ kho ở bước trước đó.
            // Hệ thống cần tự cộng lại hàng vào kho (Rollback).
            var orderContext2 = new OrderContext 
            { 
                OrderId = "ORD-002", 
                ProductName = "Macbook Pro", 
                Price = 3000, 
                CustomerBalance = 100, // Không đủ tiền
                InventoryCount = 5 
            };

            RunOrderWorkflow(orderContext2, shouldFailAtPayment: true);

            Console.WriteLine($"\n[Result 2] Balance: {orderContext2.CustomerBalance}, Inventory: {orderContext2.InventoryCount}");
            Console.WriteLine("(Inventory phải quay về 5, Balance giữ nguyên 100)");

            Console.ReadKey();
        }

        // Helper function để setup và chạy workflow
        static void RunOrderWorkflow(OrderContext context, bool shouldFailAtPayment)
        {
            // 1. Khởi tạo Engine
            var engine = new WorkflowEngine<OrderContext>();

            // 2. Cấu hình Workflow
            engine.Builder()
                // Bước 1: Validate (Dùng Lambda)
                .AddStep("Validation", 
                    ctx => { 
                        Console.WriteLine("   [1] Validating Order..."); 
                        if (string.IsNullOrEmpty(ctx.ProductName)) throw new Exception("Product Name is missing");
                    },
                    ctx => Console.WriteLine("   [1] Undo Validation (Nothing to do)")
                )
                
                // Bước 2: Trừ tồn kho (Dùng Class)
                .AddStep(new InventoryStep())

                // Bước 3: Trừ tiền (Dùng Inline Lambda phức tạp)
                .AddStep("Payment Processing",
                    ctx => 
                    {
                        Console.WriteLine($"   [3] Charging Customer ${ctx.Price}...");
                        if (shouldFailAtPayment || ctx.CustomerBalance < ctx.Price)
                        {
                            throw new InvalidOperationException("Insufficient funds! Payment Failed.");
                        }
                        ctx.CustomerBalance -= ctx.Price;
                        Console.WriteLine("   [3] Payment Success.");
                    },
                    ctx => 
                    {
                        // Logic bù trừ: Hoàn tiền lại
                        Console.WriteLine($"   [!] ROLLBACK Payment: Refunding ${ctx.Price}...");
                        ctx.CustomerBalance += ctx.Price;
                    }
                )
                
                // Bước 4: Gửi Email (Dùng Class)
                .AddStep(new EmailNotificationStep());

            // 3. Thực thi
            engine.Execute(context);
        }
    }

    // =============================================================================
    // 1. DOMAIN MODELS (Dữ liệu giả lập)
    // =============================================================================
    
    public class OrderContext
    {
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal CustomerBalance { get; set; }
        public int InventoryCount { get; set; }
        public bool IsSuccess { get; set; } = false;
    }

    // =============================================================================
    // 2. CORE ENGINE FRAMEWORK (PHẦN BẠN CẦN LÀM)
    // =============================================================================

    // Interface chuẩn cho một bước trong quy trình
    public interface IWorkflowStep<T>
    {
        string Name { get; }
        void Execute(T context);    // Chạy xuôi
        void Compensate(T context); // Chạy ngược (Undo)
    }

    // Class quản lý việc xây dựng và thực thi quy trình
    public class WorkflowEngine<T>
    {
        private readonly List<IWorkflowStep<T>> _steps = new List<IWorkflowStep<T>>();

        // Stack lưu trữ các bước ĐÃ CHẠY THÀNH CÔNG để phục vụ Rollback
        // Gợi ý: Khi chạy bước 1 thành công -> Push bước 1 vào stack.
        // Khi chạy bước 2 lỗi -> Pop stack lấy bước 1 ra -> gọi Compensate.
        private readonly Stack<IWorkflowStep<T>> _executedSteps = new Stack<IWorkflowStep<T>>();

        // --- FLUENT BUILDER API ---
        
        public WorkflowEngine<T> Builder() => this;

        // TODO 1: Implement hàm AddStep nhận vào một instance class implement IWorkflowStep
        public WorkflowEngine<T> AddStep(IWorkflowStep<T> step)
        {
            // Code here
            return this;
        }

        // TODO 2: Implement hàm AddStep nhận vào Lambda (Delegates)
        // Giúp người dùng khai báo nhanh logic mà không cần tạo class riêng
        public WorkflowEngine<T> AddStep(string name, Action<T> execute, Action<T> compensate)
        {
            // Gợi ý: Tạo một class nội bộ hoặc Anonymous implementation của IWorkflowStep<T>
            // để wrap 2 cái Action execute/compensate này lại, rồi add vào list _steps.
            
            // Code here
            return this;
        }

        // TODO 3: Implement Core Logic (Execute & Rollback)
        public void Execute(T context)
        {
            Console.WriteLine(">>> STARTING WORKFLOW...");
            
            // Code logic:
            // 1. Duyệt qua từng step trong _steps.
            // 2. Try: Gọi step.Execute(context).
            //    Nếu thành công: Push step vào _executedSteps.
            // 3. Catch Exception:
            //    In lỗi.
            //    Gọi hàm Rollback() (Private method).
            //    Break loop (hoặc throw tiếp tùy policy).
            
            // Placeholder:
            // foreach (var step in _steps) ...
        }

        // TODO 4: Logic Rollback
        private void Rollback(T context)
        {
            Console.WriteLine("!!! DETECTED ERROR -> STARTING ROLLBACK SEQUENCE !!!");
            
            // Code logic:
            // 1. While stack _executedSteps chưa rỗng.
            // 2. Pop step ra.
            // 3. Gọi step.Compensate(context).
            // 4. Handle lỗi trong lúc rollback (nếu cần thiết, thường là log critical error).
        }
    }

    // Wrapper class helper để biến Lambda thành IWorkflowStep (Dùng cho TODO 2)
    public class LambdaStep<T> : IWorkflowStep<T>
    {
        private readonly string _name;
        private readonly Action<T> _execute;
        private readonly Action<T> _compensate;

        public string Name => _name;

        public LambdaStep(string name, Action<T> execute, Action<T> compensate)
        {
            _name = name;
            _execute = execute;
            _compensate = compensate;
        }

        public void Execute(T context) => _execute(context);
        public void Compensate(T context) => _compensate(context);
    }

    // =============================================================================
    // 3. IMPLEMENTATION (Các bước xử lý cụ thể)
    // =============================================================================

    public class InventoryStep : IWorkflowStep<OrderContext>
    {
        public string Name => "Inventory Check";

        public void Execute(OrderContext context)
        {
            Console.WriteLine($"   [2] Checking Inventory (Current: {context.InventoryCount})...");
            if (context.InventoryCount <= 0) 
            {
                throw new Exception("Out of Stock!");
            }
            context.InventoryCount--; // Trừ kho
            Console.WriteLine("   [2] Item reserved. Inventory -1.");
        }

        public void Compensate(OrderContext context)
        {
            Console.WriteLine("   [!] ROLLBACK Inventory: Restoring item...");
            context.InventoryCount++; // Cộng lại kho
        }
    }

    public class EmailNotificationStep : IWorkflowStep<OrderContext>
    {
        public string Name => "Email Sender";

        public void Execute(OrderContext context)
        {
            Console.WriteLine($"   [4] Sending success email for Order {context.OrderId}...");
            // Giả sử gửi email luôn thành công
            context.IsSuccess = true;
        }

        public void Compensate(OrderContext context)
        {
            Console.WriteLine($"   [!] ROLLBACK Email: Sending 'Apology/Failed' email to customer...");
            context.IsSuccess = false;
        }
    }
}