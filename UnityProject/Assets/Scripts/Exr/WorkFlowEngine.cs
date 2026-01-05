using System;
using System.Collections.Generic;
using System.Diagnostics;

// ======================================================================
//  BÀI TẬP: WORKFLOW ENGINE GENERIC + MIDDLEWARE PIPELINE
//  TODO: Hoàn thiện tất cả phần được đánh dấu TODO.
// ======================================================================

// Xây dựng một hệ thống cho phép:
// Định nghĩa workflow gồm nhiều bước (step) xử lý theo thứ tự.

// Mỗi step:
// Nhận context kiểu generic TContext.
// Trả về kết quả StepResult (Success/Fail + Message).
// Có thể bị bỏ qua nếu không thỏa điều kiện (condition).
// Có middleware pipeline giống kiểu ASP.NET:
// Middleware chạy trước và sau quá trình thực thi workflow.
// Middleware có thể:

// Ghi log.
// Đo thời gian thực thi.
// Thay đổi context trước khi chạy.
// Chặn không cho workflow chạy nếu không thỏa điều kiện.

// WorkflowEngine<TContext>:
// Cho phép đăng ký bước (AddStep) với condition.
// Cho phép đăng ký middleware (AddMiddleware).

// Hàm Run(context) trả về WorkflowResult<TContext> chi tiết:
// Context cuối cùng sau khi chạy.
// Danh sách step đã chạy kèm trạng thái, thời gian.

// Đánh giá độ khó : 8/10

#region Core Models

/// <summary>
/// Kết quả của một step đơn lẻ trong workflow.
/// </summary>
public sealed class StepResult
{
    /// <summary>Step có chạy thành công hay không.</summary>
    public bool Success { get; }

    /// <summary>Thông điệp mô tả (lý do fail, log,...).</summary>
    public string Message { get; }

    public StepResult(bool success, string message = null)
    {
        Success = success;
        Message = message;
    }

    /// <summary>
    /// Helper tạo StepResult thành công.
    /// </summary>
    public static StepResult Ok(string message = null)
        => new StepResult(true, message);

    /// <summary>
    /// Helper tạo StepResult thất bại.
    /// </summary>
    public static StepResult Fail(string message)
        => new StepResult(false, message);
}

/// <summary>
/// Thông tin thực thi một step trong workflow (dùng cho logging / debug).
/// </summary>
public sealed class StepExecutionInfo
{
    public string StepName { get; }
    public bool Success { get; }
    public string Message { get; }
    public TimeSpan Duration { get; }

    public StepExecutionInfo(string stepName, bool success, string message, TimeSpan duration)
    {
        StepName = stepName;
        Success = success;
        Message = message;
        Duration = duration;
    }

    public override string ToString()
        => $"{StepName} | Success={Success} | Duration={Duration.TotalMilliseconds:0.##} ms | Message={Message}";
}

/// <summary>
/// Kết quả tổng thể của việc chạy 1 workflow.
/// </summary>
public sealed class WorkflowResult<TContext>
{
    /// <summary>Context sau khi chạy xong toàn bộ workflow.</summary>
    public TContext Context { get; }

    /// <summary>Workflow có thành công hay không (tùy định nghĩa: có thể là "tất cả step thành công").</summary>
    public bool IsSuccess { get; }

    /// <summary>Danh sách thông tin thực thi của từng step.</summary>
    public IReadOnlyList<StepExecutionInfo> Steps { get; }

    public WorkflowResult(TContext context, bool isSuccess, IReadOnlyList<StepExecutionInfo> steps)
    {
        Context = context;
        IsSuccess = isSuccess;
        Steps = steps;
    }
}

#endregion

#region Step Abstractions

/// <summary>
/// Interface mô tả một bước trong workflow.
/// </summary>
public interface IWorkflowStep<TContext>
{
    /// <summary>Tên step, dùng cho log / debug.</summary>
    string Name { get; }

    /// <summary>Thực thi step trên context.</summary>
    StepResult Execute(TContext context);
}

/// <summary>
/// Step cơ bản, có thể kế thừa để tránh lặp code.
/// </summary>
public abstract class WorkflowStepBase<TContext> : IWorkflowStep<TContext>
{
    public string Name { get; }

    protected WorkflowStepBase(string name)
    {
        Name = name;
    }

    public abstract StepResult Execute(TContext context);
}

/// <summary>
/// Một step có condition đi kèm: chỉ chạy nếu condition(context) == true.
/// </summary>
internal sealed class ConditionalStep<TContext>
{
    public IWorkflowStep<TContext> Step { get; }
    public Func<TContext, bool> Condition { get; }

    public ConditionalStep(IWorkflowStep<TContext> step, Func<TContext, bool> condition)
    {
        Step = step;
        Condition = condition;
    }
}

#endregion

#region Middleware Abstractions

/// <summary>
/// Context dùng trong quá trình thực thi workflow + middleware.
/// Cho phép middleware truy cập context + danh sách step + log,... nếu cần mở rộng.
/// </summary>
public sealed class WorkflowExecutionContext<TContext>
{
    /// <summary>Context hiện tại của workflow.</summary>
    public TContext Context { get; set; }

    /// <summary>Danh sách step đã thực thi (dùng để middleware đọc/ghi nếu muốn).</summary>
    public IList<StepExecutionInfo> ExecutedSteps { get; }

    public WorkflowExecutionContext(TContext context, IList<StepExecutionInfo> executedSteps)
    {
        Context = context;
        ExecutedSteps = executedSteps;
    }
}

/// <summary>
/// Delegate mô tả middleware.
///  - context: thông tin thực thi hiện tại
///  - next: hàm để gọi middleware tiếp theo / workflow core
/// </summary>
public delegate void WorkflowMiddleware<TContext>(
    WorkflowExecutionContext<TContext> context,
    Action next);

#endregion

#region Workflow Engine

/// <summary>
/// Workflow định nghĩa: chứa danh sách step (kèm condition).
/// </summary>
public sealed class WorkflowDefinition<TContext>
{
    private readonly List<ConditionalStep<TContext>> _steps = new();

    /// <summary>
    /// Thêm step vào workflow.
    /// condition == null nghĩa là luôn chạy step này.
    /// </summary>
    public WorkflowDefinition<TContext> AddStep(
        IWorkflowStep<TContext> step,
        Func<TContext, bool> condition = null)
    {
        // TODO: thêm step vào danh sách _steps và trả về this (để hỗ trợ fluent API)
        _steps.Add(new ConditionalStep<TContext>(step, condition));
        return this;
    }

    /// <summary>
    /// Trả về các step (internal dùng trong WorkflowEngine).
    /// </summary>
    internal IReadOnlyList<ConditionalStep<TContext>> GetSteps()
        => _steps;
}

/// <summary>
/// Engine thực thi workflow với hỗ trợ middleware pipeline.
/// </summary>
public sealed class WorkflowEngine<TContext>
{
    private readonly List<WorkflowMiddleware<TContext>> _middlewares = new();

    /// <summary>
    /// Đăng ký một middleware vào pipeline.
    /// Thứ tự đăng ký chính là thứ tự chạy (giống ASP.NET).
    /// </summary>
    public void AddMiddleware(WorkflowMiddleware<TContext> middleware)
    {
        // TODO: thêm middleware vào _middlewares
        _middlewares.Add(middleware);
    }

    /// <summary>
    /// Thực thi workflow với context ban đầu và definition.
    /// </summary>
    public WorkflowResult<TContext> Run(TContext context, WorkflowDefinition<TContext> definition)
    {
        // TODO:
        // 1. Tạo danh sách StepExecutionInfo để chứa log thực thi
        // 2. Tạo WorkflowExecutionContext
        // 3. Xây dựng "core" delegate: hàm này sẽ chạy toàn bộ step trong definition theo thứ tự:
        //      - kiểm tra condition
        //      - đo thời gian
        //      - gọi Execute của step
        //      - lưu StepExecutionInfo
        //      - có thể dừng workflow nếu step thất bại (tùy bạn định nghĩa)
        // 4. Bọc core bằng các middleware đã đăng ký (_middlewares) theo mô hình pipeline:
        //      middlewareN(..., () => middlewareN-1(..., () => core()))
        //    Gợi ý: có thể build từ cuối lên đầu bằng vòng for ngược.
        // 5. Gọi delegate đầu tiên để thực thi toàn bộ pipeline.
        // 6. Tính toán IsSuccess (ví dụ: tất cả StepExecutionInfo.Success == true).
        // 7. Trả về WorkflowResult<TContext>.

        List<StepExecutionInfo> _stepsExcu = new();
        WorkflowExecutionContext<TContext> workflowExcu = new(context, _stepsExcu);
        Core(context, definition, workflowExcu);

        _middlewares[0]?.Invoke(workflowExcu, () => InvokeBack(1, workflowExcu, context, definition));

        _stepsExcu.Clear();
        _stepsExcu.AddRange(workflowExcu.ExecutedSteps);

        WorkflowResult<TContext> result = new(context, true, _stepsExcu);

        foreach(var stepExcu in workflowExcu.ExecutedSteps)
        {
            if (stepExcu.Success) continue;
            else
            {
                WorkflowResult<TContext> resultFalse = new(context, false, _stepsExcu);
            }
        }
        return result;
    }
    //Co the bo
    public void CallBack(int i, WorkflowExecutionContext<TContext> workflowExcu, TContext context, WorkflowDefinition<TContext> definition)
    {
        if (i == _middlewares.Count - 1)
        {
            _middlewares[i](workflowExcu, () => Core(context, definition, workflowExcu));
            return;
        }
        _middlewares[i](workflowExcu, () => CallBack(i + 1, workflowExcu, context, definition));
    }

    public void InvokeBack(int i, WorkflowExecutionContext<TContext> workflowExcu, TContext context, WorkflowDefinition<TContext> definition)
    {
        if (i == _middlewares.Count - 1)
        {
            _middlewares[i]?.Invoke(workflowExcu, () => Core(context, definition, workflowExcu));
            return;
        }
        _middlewares[i]?.Invoke(workflowExcu, () => InvokeBack(i + 1, workflowExcu, context, definition));
    }

    public void Core(TContext context, WorkflowDefinition<TContext> definition, WorkflowExecutionContext<TContext> workflowExcu)
    {
        List<StepExecutionInfo> _stepsExcu = new();
        WorkflowExecutionContext<TContext> workflowExcuCopy = new(context, _stepsExcu);
        foreach (var step in definition.GetSteps())
        {
            DateTime start = DateTime.Now;
            TimeSpan startTime = start.TimeOfDay;
            if (step.Condition(context))
            {
                StepResult result = step.Step.Execute(context);
                if (result.Success)
                {
                    DateTime end = DateTime.Now;
                    TimeSpan endTime = end.TimeOfDay;
                    TimeSpan newDuration = endTime - startTime;

                    StepExecutionInfo stepExcu = new(step.Step.Name, result.Success, result.Message, newDuration);
                    workflowExcuCopy.ExecutedSteps.Add(stepExcu);
                }
                else
                {
                    break;
                }
            }
        }
        workflowExcu = workflowExcuCopy;
    }

    // Gợi ý: bạn có thể tách riêng method CoreExecute để code gọn hơn nếu muốn.
}

#endregion

// ======================================================================
//  DOMAIN MẪU: ĐƠN HÀNG (ORDER)
// ======================================================================

public sealed class OrderContext
{
    // Đơn hàng giả lập cho mục đích bài tập.
    public string CustomerId { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public bool IsVipCustomer { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsApproved { get; set; }

    // Flag để mô phỏng hệ thống kiểm tra fraud / rủi ro.
    public bool IsFlaggedAsFraud { get; set; }

    public override string ToString()
        => $"Customer={CustomerId}, Total={TotalAmount}, Vip={IsVipCustomer}, Discount={DiscountPercent}, Approved={IsApproved}, Fraud={IsFlaggedAsFraud}";
}

// ----------------------------------------------------------------------
//  CÁC STEP CỤ THỂ TRONG WORKFLOW ĐƠN HÀNG
//  TODO: Học viên hoàn thiện logic Execute cho từng step.
// ----------------------------------------------------------------------

/// <summary>
/// Step: kiểm tra tổng tiền có hợp lệ không (ví dụ: > 0, không vượt quá ngưỡng,...).
/// </summary>
public sealed class ValidateOrderAmountStep : WorkflowStepBase<OrderContext>
{
    public ValidateOrderAmountStep() : base("ValidateOrderAmount") { }

    public override StepResult Execute(OrderContext context)
    {
        // TODO: ví dụ:
        //  - Nếu TotalAmount <= 0 => Fail
        //  - Nếu TotalAmount quá lớn (ví dụ > 100_000) => Fail
        //  - Ngược lại => Ok
        if (context.TotalAmount <= 0)
        {
            return StepResult.Fail("Fail");
        }
        else if (context.TotalAmount > 1000000)
        {
            return StepResult.Fail("Fail");
        }
        else
        {
            return StepResult.Ok("OK");
        }
    }
}

/// <summary>
/// Step: áp dụng discount cho khách VIP.
/// </summary>
public sealed class ApplyVipDiscountStep : WorkflowStepBase<OrderContext>
{
    public ApplyVipDiscountStep() : base("ApplyVipDiscount") { }

    public override StepResult Execute(OrderContext context)
    {
        // TODO: ví dụ:
        //  - Nếu IsVipCustomer == true => tăng DiscountPercent
        //  - Có thể giới hạn mức giảm tối đa
        //  - Trả về Ok với message mô tả
        if (context.IsVipCustomer)
        {
            context.DiscountPercent += (decimal)0.1f;
            if(context.DiscountPercent > (decimal)0.5f)
            {
                context.DiscountPercent = (decimal)0.5f;
            }
            return StepResult.Ok("Co VIP");
        }
        return StepResult.Ok("Khong co VIP");
    }
}

/// <summary>
/// Step: mô phỏng hệ thống fraud check bên ngoài (ví dụ random flag).
/// </summary>
public sealed class FraudDetectionStep : WorkflowStepBase<OrderContext>
{
    private static readonly Random _random = new();

    public FraudDetectionStep() : base("FraudDetection") { }

    public override StepResult Execute(OrderContext context)
    {
        // TODO: ví dụ:
        //  - Random một giá trị để mô phỏng hệ thống check fraud
        //  - Có thể khoảng 5% đơn bị flag IsFlaggedAsFraud = true
        //  - Nếu bị flag thì có thể Fail hoặc vẫn Ok nhưng kèm message warning (do bạn thiết kế)
        if(_random.Next(0,1) < 0.05f)
        {
            context.IsFlaggedAsFraud = true;
            return StepResult.Fail("Is Fraud");
        }
        return StepResult.Ok("Not Fraud");
    }
}

/// <summary>
/// Step: approve đơn hàng nếu qua được tất cả điều kiện.
/// </summary>
public sealed class ApproveOrderStep : WorkflowStepBase<OrderContext>
{
    public ApproveOrderStep() : base("ApproveOrder") { }

    public override StepResult Execute(OrderContext context)
    {
        // TODO: ví dụ:
        //  - Nếu không bị flag fraud và total hợp lệ thì IsApproved = true
        //  - Ngược lại IsApproved = false, trả về Fail với message
        if(!context.IsFlaggedAsFraud && context.TotalAmount > 0 && context.TotalAmount < 1000000)
        {
            context.IsApproved = true;
            return StepResult.Ok("Approved");
        }
        else
        {
            context.IsApproved = false;
            return StepResult.Fail("Not Approved");
        }
    }
}


// ======================================================================
//  CHƯƠNG TRÌNH TEST
// ======================================================================
public static class Program7
{
    public static void Main()
    {
        // -------------------------------------------------------------
        // 1. TẠO WORKFLOW ĐỊNH NGHĨA
        // -------------------------------------------------------------
        var workflowDefinition = new WorkflowDefinition<OrderContext>()
            .AddStep(new ValidateOrderAmountStep())
            .AddStep(new ApplyVipDiscountStep(), ctx => ctx.IsVipCustomer)  // chỉ chạy nếu là VIP
            .AddStep(new FraudDetectionStep())
            .AddStep(new ApproveOrderStep());

        // -------------------------------------------------------------
        // 2. TẠO ENGINE VÀ ĐĂNG KÝ MIDDLEWARE
        // -------------------------------------------------------------
        var engine = new WorkflowEngine<OrderContext>();

        // Middleware 1: log trước/sau toàn bộ workflow.
        engine.AddMiddleware((ctx, next) =>
        {
            Console.WriteLine("=== Middleware 1: Bắt đầu workflow ===");
            var sw = Stopwatch.StartNew();

            next(); // gọi middleware tiếp theo / core

            sw.Stop();
            Console.WriteLine("=== Middleware 1: Kết thúc workflow, duration = {0} ms ===", sw.ElapsedMilliseconds);
        });

        // Middleware 2: log số step đã chạy sau khi core kết thúc.
        engine.AddMiddleware((ctx, next) =>
        {
            next();
            Console.WriteLine("=== Middleware 2: Đã chạy {0} step ===", ctx.ExecutedSteps.Count);
        });

        // -------------------------------------------------------------
        // 3. TẠO DỮ LIỆU TEST
        // -------------------------------------------------------------
        var orders = new[]
        {
            new OrderContext
            {
                CustomerId = "CUST-001",
                TotalAmount = 500,
                IsVipCustomer = false
            },
            new OrderContext
            {
                CustomerId = "CUST-002",
                TotalAmount = 1500,
                IsVipCustomer = true
            },
            new OrderContext
            {
                CustomerId = "CUST-003",
                TotalAmount = -10,
                IsVipCustomer = true
            },
        };

        // -------------------------------------------------------------
        // 4. CHẠY TEST
        // -------------------------------------------------------------
        foreach (var order in orders)
        {
            Console.WriteLine();
            Console.WriteLine("###############################");
            Console.WriteLine("Chạy workflow cho đơn hàng: " + order);

            var result = engine.Run(order, workflowDefinition);

            Console.WriteLine("KẾT QUẢ WORKFLOW:");
            Console.WriteLine("  IsSuccess: " + result.IsSuccess);
            Console.WriteLine("  Context cuối: " + result.Context);
            Console.WriteLine("  Các step đã chạy:");

            foreach (var info in result.Steps)
            {
                Console.WriteLine("    - " + info);
            }
        }

        Console.WriteLine();
        Console.WriteLine(">>> Nhấn phím bất kỳ để kết thúc...");
        Console.ReadKey();
    }
}
