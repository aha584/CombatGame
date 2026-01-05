using System;
using System.Collections.Generic;

namespace OopExercises.JobScheduler
{
    // YÊU CẦU:
    // - Xây dựng hệ thống quản lý công việc (job) với các trạng thái, độ ưu tiên.
    // - Có hàng đợi job, chọn job ưu tiên cao để chạy trước.
    // - Có xử lý retry đơn giản cho job thất bại.
    //
    // MỤC TIÊU KIẾN THỨC:
    // - Interface + abstract class.
    // - Kế thừa, đa hình (polymorphism) qua IJob / BaseJob.
    // - Đóng gói logic vào JobScheduler.

    // Trạng thái của một job.
    public enum JobStatus
    {
        Pending,   // Chờ thực thi
        Running,   // Đang chạy
        Succeeded, // Thành công
        Failed,    // Thất bại
        Cancelled  // Bị huỷ
    }

    // Interface mô tả một job.
    public interface IJob
    {
        // Tên job (dùng để debug / log).
        string Name { get; }

        // Độ ưu tiên. Số càng lớn, ưu tiên càng cao.
        int Priority { get; }

        // Trạng thái hiện tại.
        JobStatus Status { get; }

        // Số lần đã thử chạy job.
        int AttemptCount { get; }

        // Số lần retry tối đa cho job này.
        int MaxRetry { get; }

        // Thực thi công việc.
        void Execute();

        // Huỷ job (không chạy nữa).
        void Cancel();
    }

    // Lớp base cho các job, cài đặt sẵn một phần logic.
    public abstract class BaseJob : IJob
    {
        private readonly string _name;
        private readonly int _priority;
        private readonly int _maxRetry;

        private JobStatus _status;
        private int _attemptCount;

        public string Name { get { return _name; } }
        public int Priority { get { return _priority; } }
        public JobStatus Status { get { return _status; } }
        public int AttemptCount { get { return _attemptCount; } }
        public int MaxRetry { get { return _maxRetry; } }

        protected BaseJob(string name, int priority, int maxRetry)
        {
            // TODO:
            // - Kiểm tra name null/empty.
            // - Nếu priority < 0 thì có thể ép về 0.
            // - Nếu maxRetry < 0 thì ép về 0.
            // - Gán các field tương ứng.
            // - Đặt _status = JobStatus.Pending.
            // - Đặt _attemptCount = 0.
            if (!string.IsNullOrEmpty(name)) 
            {
                _name = name;
                if (priority < 0) priority = 0;
                _priority = priority;
                if (maxRetry < 0) maxRetry = 0;
                _maxRetry = maxRetry;
                _status = JobStatus.Pending;
                _attemptCount = 0;
            }
        }

        // Hàm thực thi chính, được gọi từ JobScheduler.
        public void Execute()
        {
            // TODO:
            // - Nếu _status là Cancelled thì không chạy, return luôn.
            // - Tăng _attemptCount lên 1.
            // - Đặt _status = JobStatus.Running.
            // - Thử gọi ExecuteInternal():
            //     + Nếu không có exception => _status = JobStatus.Succeeded.
            //     + Nếu có exception => _status = JobStatus.Failed.
            // - Có thể in log mô phỏng.
            if (_status == JobStatus.Cancelled) return;
            _attemptCount++;
            _status = JobStatus.Running;
            try
            {
                ExecuteInternal();
            }
            catch
            {
                _status = JobStatus.Failed;
                return;
            }
            _status = JobStatus.Succeeded;
        }

        public void Cancel()
        {
            // TODO:
            // - Nếu job chưa chạy xong (Status là Pending hoặc Running) thì đặt _status = JobStatus.Cancelled.
            if (_status == JobStatus.Pending || _status == JobStatus.Running) _status = JobStatus.Cancelled;
        }

        // Học viên override hàm này trong class con để định nghĩa logic job cụ thể.
        protected abstract void ExecuteInternal();
    }

    // ------------------------------------------------------------
    // Ví dụ: Job gửi email.
    // ------------------------------------------------------------
    public sealed class EmailJob : BaseJob
    {
        private readonly string _recipient;
        private readonly string _subject;
        private readonly string _body;

        public EmailJob(string recipient, string subject, string body, int priority, int maxRetry)
            : base("EmailJob", priority, maxRetry)
        {
            // TODO:
            // - Gán _recipient, _subject, _body (kiểm tra null/empty nếu muốn).
            if (!string.IsNullOrEmpty(recipient)) _recipient = recipient;
            if (!string.IsNullOrEmpty(subject)) _subject = subject;
            if (!string.IsNullOrEmpty(body)) _body = body;
        }

        protected override void ExecuteInternal()
        {
            // TODO:
            // - Mô phỏng gửi email (Console.WriteLine).
            // - Có thể mô phỏng lỗi ngẫu nhiên:
            //      Random r = new Random();
            //      if (r.Next(0, 3) == 0) throw new Exception("Fake send email error");
            // - Nếu không lỗi => coi như gửi thành công.
            Random r = new Random();
            if (r.Next(0, 3) == 0) throw new Exception("Fake send email error!!");
        }
    }

    // ------------------------------------------------------------
    // Ví dụ: Job xử lý dữ liệu (data processing).
    // ------------------------------------------------------------
    public sealed class DataProcessingJob : BaseJob
    {
        private readonly string _dataName;
        private readonly int _workUnits;

        public DataProcessingJob(string dataName, int workUnits, int priority, int maxRetry)
            : base("DataProcessingJob", priority, maxRetry)
        {
            // TODO:
            // - Gán _dataName.
            // - Nếu workUnits < 0 thì ép về 0.
            _dataName = dataName;
            if (workUnits < 0)
            {
                workUnits = 0;
                _workUnits = workUnits;
            }
        }

        protected override void ExecuteInternal()
        {
            // TODO:
            // - Mô phỏng xử lý dữ liệu.
            // - Ví dụ: in ra "Processing dataX, step i/ workUnits".
            // - Có thể mô phỏng lỗi nếu workUnits quá lớn.
            if (_workUnits > 10000000) throw new Exception("Work Units is too larger!!!");
            else
            {
                for (int i = 0; i <= _workUnits; i++)
                {
                    Console.WriteLine($"Processing dataX, step {i}/{_workUnits}");
                }
            }
        }
    }

    // ------------------------------------------------------------
    // JobScheduler: quản lý hàng đợi job, chạy job theo ưu tiên.
    // KHÔNG dùng generic.
    // ------------------------------------------------------------
    public sealed class JobScheduler
    {
        // Hàng đợi job (tất cả job Pending sẽ được lưu ở đây).
        private readonly List<IJob> _jobs = new ();

        // Lịch sử job đã chạy (tuỳ chọn, để debug).
        private readonly List<IJob> _history = new ();

        // Thêm job vào hàng đợi.
        public void Enqueue(IJob job)
        {
            // TODO:
            // - Kiểm tra job null.
            // - Chỉ cho phép thêm job có Status = Pending.
            // - Thêm job vào _jobs.
            if (!(job == null) && job.Status == JobStatus.Pending) _jobs.Add(job);
        }

        // Lấy ra job tiếp theo để chạy, dựa theo ưu tiên.
        // Job có Priority lớn hơn sẽ được chọn trước.
        // Nếu bằng nhau, ưu tiên job được enqueue trước (giữ thứ tự).
        public IJob DequeueNextJob()
        {
            // TODO:
            // - Nếu không có job nào trong _jobs => return null.
            // - Tìm job có Priority lớn nhất trong _jobs.
            // - Xoá job đó khỏi _jobs và return nó.
            // - Gợi ý: duyệt ArrayList, ép kiểu từng phần tử về IJob.
            if (_jobs.Count == 0) return null;
            IJob maxPriJob = _jobs[0];
            foreach(IJob job in _jobs)
            {
                if (job.Priority > maxPriJob.Priority) maxPriJob = job; 
            }
            if (maxPriJob != null) { _jobs.Remove(maxPriJob); }
            return maxPriJob;
        }

        // Chạy 1 job tiếp theo trong hàng đợi.
        // - Nếu job thất bại và còn quyền retry, đưa lại vào hàng đợi.
        // - Nếu hết retry, coi như bỏ job (vào history).
        public void RunNextJob()
        {
            // TODO:
            // - Gọi DequeueNextJob().
            // - Nếu null thì return (không có job).
            // - Gọi job.Execute().
            // - Nếu job.Status == JobStatus.Failed và job.AttemptCount <= job.MaxRetry:
            //       + Thêm lại job vào hàng đợi (Enqueue).
            //   Ngược lại:
            //       + Đưa job vào _history.
            IJob myJob = DequeueNextJob();
            if(myJob == null)
            {
                return;
            }
            else
            {
                myJob.Execute();
                if(myJob.Status == JobStatus.Failed && myJob.AttemptCount <= myJob.MaxRetry)
                {
                    _jobs.Add(myJob);
                }
                else
                {
                    _history.Add(myJob);
                }
            }
        }

        // Chạy liên tục cho đến khi hết job trong hàng đợi.
        public void RunAll()
        {
            // TODO:
            // - Trong khi _jobs.Count > 0:
            //       + Gọi RunNextJob().
            while (_jobs.Count > 0)
            {
                RunNextJob();
            }
        }

        // Số job đang chờ trong hàng đợi.
        public int PendingCount
        {
            get
            {
                // TODO: trả về _jobs.Count.
                return _jobs.Count;
            }
        }

        // Số job đã kết thúc (lưu trong history).
        public int FinishedCount
        {
            get
            {
                // TODO: trả về _history.Count.
                return _history.Count;
            }
        }

        // In ra lịch sử job đã chạy (tên, trạng thái cuối cùng, số lần thử).
        public void PrintHistory()
        {
            // TODO:
            // - Duyệt _history, ép từng phần tử về IJob.
            // - In ra: Name, Status, AttemptCount, MaxRetry.
            for(int i = 0; i < _history.Count; i++)
            {
                IJob endedJob = _history[i];
                Console.WriteLine($"{endedJob.Name}, {endedJob.Status}, {endedJob.AttemptCount}, {endedJob.MaxRetry}");
            }
        }
    }

    public class Program
    {
        static void Main()
        {
            JobScheduler scheduler = new JobScheduler();

            IJob email1 = new EmailJob("a@example.com", "Hello", "Body A", priority: 5, maxRetry: 2);
            IJob email2 = new EmailJob("b@example.com", "Hi", "Body B", priority: 1, maxRetry: 1);
            IJob data1 = new DataProcessingJob("UserData", 3, priority: 3, maxRetry: 0);

            scheduler.Enqueue(email1);
            scheduler.Enqueue(email2);
            scheduler.Enqueue(data1);

            while(scheduler.PendingCount > 0)
            {
                scheduler.RunNextJob();
            }

            scheduler.RunAll();

            Console.WriteLine("Pending: " + scheduler.PendingCount);
            Console.WriteLine("Finished: " + scheduler.FinishedCount);
            scheduler.PrintHistory();
        }
    }
}