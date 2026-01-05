using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedJobScheduler
{
    // ==========================================
    // PHẦN 1: ĐỊNH NGHĨA INTERFACE VÀ CLASS
    // ==========================================

    public enum JobStatus { Pending, Running, Completed, Failed }

    // Interface phi-generic để Scheduler có thể lưu trữ chung một danh sách List<IJob>
    // dù bên dưới là Job<string> hay Job<int>.
    public interface IJob
    {
        Guid Id { get; }
        string Name { get; }
        int Priority { get; }
        JobStatus Status { get; }
        void Execute(); // Hàm chạy job
    }

    // Lớp Generic Job. 
    // T là kiểu dữ liệu mà Job này sẽ xử lý (VD: string cho Email, int cho tính toán).
    public class Job<T> : IJob
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; private set; }
        public int Priority { get; private set; }
        public JobStatus Status { get; private set; }

        // Dữ liệu cần xử lý
        private readonly T _data;

        // DELEGATE: Logic xử lý chính của Job. 
        // Thay vì viết abstract method, ta nhận vào một Action<T>.
        private readonly Action<T> _workLogic;

        // Constructor
        public Job(string name, T data, int priority, Action<T> workLogic)
        {
            Name = name;
            Priority = priority;
            _data = data;
            _workLogic = workLogic;
            Status = JobStatus.Pending;
        }

        public void Execute()
        {
            // TODO 1: Cài đặt logic thực thi Job
            // 1. Chuyển Status sang Running.
            // 2. Gọi delegate _workLogic với tham số _data.
            // 3. Nếu chạy êm đẹp -> Status = Completed.
            // 4. Nếu có lỗi (Exception) -> Status = Failed và in lỗi ra Console.
            // Gợi ý: Dùng try-catch.

            Status = JobStatus.Running;
            try
            {
                _workLogic(_data);
            }
            catch(Exception ex)
            {
                Status = JobStatus.Failed;
                Console.WriteLine(ex.ToString());
            }
            Status = JobStatus.Completed;
        }
    }

    public class JobScheduler
    {
        // Danh sách chứa các Job (đa hình qua interface IJob)
        private readonly List<IJob> _jobQueue = new List<IJob>();

        // DELEGATE/EVENT: Thông báo ra bên ngoài
        // OnJobCompleted: được gọi khi một job chạy xong (Completed).
        // OnJobFailed: được gọi khi một job bị lỗi (Failed).
        public Action<IJob> OnJobCompleted { get; set; }
        public Action<IJob> OnJobFailed { get; set; }

        // Hàm thêm Job vào hàng đợi
        public void AddJob(IJob job)
        {
            // TODO 2: Thêm job vào _jobQueue và in ra thông báo đã nhận job.

            _jobQueue.Add(job);
            Console.WriteLine("da nhan job");
        }

        // Hàm helper để tạo nhanh Job<T> và thêm vào queue (Sử dụng Generic Method)
        public void Enqueue<T>(string name, T data, int priority, Action<T> logic)
        {
            // TODO 3: 
            // 1. Tạo instance của class Job<T> với các tham số truyền vào.
            // 2. Gọi hàm AddJob bên trên.

            Job<T> job = new(name, data, priority, logic);
            AddJob(job);
        }

        public void Run()
        {
            Console.WriteLine("\n[Scheduler] Bắt đầu chạy các Job...\n");

            while (_jobQueue.Count > 0)
            {
                // TODO 4: Sử dụng LINQ và Lambda để tìm Job có Priority CAO NHẤT.
                // Nếu Priority bằng nhau, chọn cái nào cũng được (hoặc cái đầu tiên).
                // IJob jobToRun = _jobQueue....?

                var maxPriJob = _jobQueue.Max(job => job.Priority);

                IJob jobToRun = _jobQueue.Where(job => job.Priority == maxPriJob) as IJob; // Sửa dòng này sau khi làm xong TODO 4
                if (jobToRun == null) break;

                // Xóa khỏi hàng đợi
                _jobQueue.Remove(jobToRun);

                // Thực thi
                Console.WriteLine($"--- Đang chạy: {jobToRun.Name} (Prio: {jobToRun.Priority}) ---");
                jobToRun.Execute();

                // TODO 5: Kích hoạt (Invoke) các Delegate thông báo (Callback).
                // - Nếu jobToRun.Status == JobStatus.Completed -> gọi OnJobCompleted
                // - Nếu jobToRun.Status == JobStatus.Failed -> gọi OnJobFailed
                // Nhớ kiểm tra null trước khi gọi (hoặc dùng ?.)
                if (jobToRun.Status == JobStatus.Completed) OnJobCompleted?.Invoke(jobToRun);
                else if (jobToRun.Status == JobStatus.Failed) OnJobFailed?.Invoke(jobToRun);
                
            }
            Console.WriteLine("\n[Scheduler] Đã xử lý hết hàng đợi.");
        }
    }

    // ==========================================
    // PHẦN 2: CHƯƠNG TRÌNH TEST (MAIN)
    // ==========================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // 1. Khởi tạo Scheduler
            JobScheduler scheduler = new JobScheduler();

            // 2. Đăng ký các sự kiện (Delegate) để giám sát hệ thống
            // Sử dụng Lambda Expression để xử lý sự kiện
            scheduler.OnJobCompleted = (job) => 
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[EVENT] Job '{job.Name}' đã hoàn thành xuất sắc!");
                Console.ResetColor();
            };

            scheduler.OnJobFailed = (job) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[EVENT] Cảnh báo! Job '{job.Name}' đã thất bại.");
                Console.ResetColor();
            };

            // 3. Thêm Job vào Scheduler bằng cách dùng Lambda cho logic xử lý
            
            // CASE A: Job gửi Email (Dữ liệu là string email)
            // Logic: In ra màn hình mô phỏng gửi mail.
            scheduler.Enqueue<string>("Email Welcome", "user@gmail.com", 5, (email) => 
            {
                // Đây là logic của Job, được viết dưới dạng Lambda
                Console.WriteLine($"   -> Đang kết nối SMTP...");
                Console.WriteLine($"   -> Gửi email tới: {email}");
                System.Threading.Thread.Sleep(500); // Giả lập delay
            });

            // CASE B: Job Xử lý ảnh (Dữ liệu là đường dẫn ảnh) - Priority thấp hơn
            scheduler.Enqueue<string>("Resize Image", "avatar.jpg", 1, (path) => 
            {
                Console.WriteLine($"   -> Đang load ảnh từ {path}");
                Console.WriteLine($"   -> Resize về 100x100 px");
            });

            // CASE C: Job Tính toán (Dữ liệu là danh sách số nguyên) - Priority CAO NHẤT
            // Logic: Tính tổng.
            scheduler.Enqueue<List<int>>("Tính lương", new List<int> { 1000, 2000, 500 }, 10, (numbers) => 
            {
                int total = numbers.Sum();
                Console.WriteLine($"   -> Tổng lương cần trả: {total}$");
            });

            // CASE D: Job Lỗi (Để test OnJobFailed)
            scheduler.Enqueue<int>("Job Xui Xẻo", 0, 8, (num) => 
            {
                Console.WriteLine("   -> Đang chạy phép chia...");
                int result = 100 / num; // Sẽ gây lỗi DivideByZeroException
            });

            // 4. Chạy Scheduler
            scheduler.Run();

            Console.ReadLine();
        }
    }
}