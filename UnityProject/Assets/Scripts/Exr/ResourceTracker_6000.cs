// ============================================================================
// BÀI TẬP HỆ THỐNG (6/10): Lifetime Scope + Resource Tracker
//
// Hệ thống này dùng để làm gì?
// - Trong dự án thật, ta thường tạo rất nhiều "resource" gắn với vòng đời (lifetime):
//   - Subscription (event bus, observable, callback)
//   - Timer / Scheduled job
//   - File handle / stream
//   - Unity: coroutine handle, Addressables handle, etc.
// - Nếu quên Dispose/Unsubscribe, ứng dụng bị memory leak, logic leak (callback chạy mãi),
//   hoặc crash do dùng object đã chết.
// - Lifetime Scope giúp gom các resource theo "scope" (một vòng đời):
//   - Scope cha có thể chứa scope con
//   - Khi scope bị Dispose -> tự Dispose tất cả resource đã đăng ký + dispose cả scope con
// - Resource Tracker dùng để debug:
//   - Resource nào còn sống?
//   - Resource nào tạo ra nhưng chưa dispose?
//   - Ai tạo ra? (tag/note)
//   - Đếm số resource theo loại
//
// Mục tiêu bài tập:
// - Xây dựng LifetimeScope để quản lý IDisposable + action cleanup
// - Hỗ trợ scope cha-con và dispose theo thứ tự đúng
// - Xây dựng tracker để report leak và thống kê
//
// HƯỚNG DẪN:
// - Hoàn thiện các TODO.
// - Chạy Program.Main để xem test case.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

public static class Program23
{
    public static void Main()
    {
        Console.WriteLine("===== LifetimeScope Demo =====");

        ResourceTracker tracker = new();

        using (LifetimeScope root = new("Root", tracker))
        {
            // 1) Tạo resource và đăng ký vào root
            TrackedDisposable d1 = new("RootDisposable_1", tracker);
            root.Add(d1, note: "created in root");

            // 2) Tạo scope con (ChildA)
            LifetimeScope childA = root.CreateChild("ChildA");
            TrackedDisposable d2 = new("ChildA_Disposable_1", tracker);
            childA.Add(d2, note: "created in childA");

            // 3) Đăng ký cleanup action (không phải IDisposable)
            // Ví dụ: unsubscribe callback / release handle / return to pool...
            childA.AddCleanup(() => Console.WriteLine("[Cleanup] ChildA custom cleanup"), note: "custom cleanup A");

            // 4) Tạo scope con (ChildB) và cố tình "quên dispose" một resource để test leak
            LifetimeScope childB = root.CreateChild("ChildB");
            TrackedDisposable leak = new("LEAK_ME", tracker);
            childB.Add(leak, note: "this will be leaked if childB not disposed properly?");

            // 5) Dispose riêng childA trước
            Console.WriteLine();
            Console.WriteLine(">>> Dispose ChildA manually");
            childA.Dispose();

            Console.WriteLine();
            Console.WriteLine(">>> Tracker report after disposing ChildA");
            Console.WriteLine(tracker.BuildReport(includeLiveList: true));

            // 6) Không dispose childB ở đây.
            // Root.Dispose() cuối using sẽ phải dispose luôn childB.
            Console.WriteLine();
            Console.WriteLine(">>> Exiting root using (root will dispose children)");
        }

        Console.WriteLine();
        Console.WriteLine(">>> Tracker report after disposing Root");
        // Sau khi root disposed, không nên còn resource sống.
        // Nếu còn -> leak.
        // TODO: học viên cần đảm bảo tracker report đúng.
        // Lưu ý: trong demo này, root sẽ dispose childB, nên leak không còn.
        // Bạn có thể thử "tắt" dispose child scope để tạo leak có chủ ý.
        // (Tạo thêm test case phía dưới)
        Console.WriteLine("(New tracker instance for leak scenario)");

        // -----------------------
        // Leak scenario (cố ý)
        // -----------------------
        ResourceTracker tracker2 = new();
        LifetimeScope leakRoot = new("LeakRoot", tracker2);

        LifetimeScope leakChild = leakRoot.CreateChild("LeakChild");
        TrackedDisposable d3 = new("LeakDisposable", tracker2);
        leakChild.Add(d3, note: "will leak if leakRoot disposed doesn't dispose children");

        // TODO: Nếu học viên làm sai logic dispose chain, report sẽ hiện leak.
        leakRoot.Dispose();

        Console.WriteLine();
        Console.WriteLine(">>> Leak scenario report");
        Console.WriteLine(tracker2.BuildReport(includeLiveList: true));
    }
}

// ============================================================================
// TRACKER
// ============================================================================

public enum ResourceState
{
    Alive,
    Disposed,
}

public sealed class ResourceTracker
{
    // Một record nhỏ để debug: resource thuộc scope nào, note gì, còn sống không...
    private readonly Dictionary<Guid, ResourceRecord> _records = new();

    public Guid TrackCreated(string typeName, string name, string scopeName, string note)
    {
        // TODO:
        // - Tạo id
        // - Lưu record state Alive
        // - return id
        Guid newID = Guid.NewGuid();
        ResourceRecord record = new(newID, typeName, name, scopeName, note);
        record.State = ResourceState.Alive;
        _records[newID] = record;
        return newID;
    }

    public void TrackDisposed(Guid id)
    {
        // TODO:
        // - Nếu id tồn tại, set state Disposed và set disposedAt
        // - Nếu không tồn tại, có thể ignore hoặc throw (tự quyết định)
        if (_records.ContainsKey(id))
        {
            _records[id].State = ResourceState.Disposed;
            _records[id].DisposedAtUtc = DateTime.UtcNow;
        }
        else
        {
            throw new Exception("No record for this ID!!");
        }
    }

    public string BuildReport(bool includeLiveList)
    {
        // TODO:
        // - Tổng số created
        // - Tổng số disposed
        // - Tổng số alive (leak candidates)
        // - Group theo typeName
        // - Nếu includeLiveList: in danh sách alive (type + name + scope + note)

        int createdCount = _records.Count;
        List<KeyValuePair<Guid, ResourceRecord>> aliveList = _records.Where(x => x.Value.State == ResourceState.Alive).ToList();
        int aliveCount = aliveList.Count;
        int disposedCount = createdCount - aliveCount;
        aliveList.GroupBy(x => x.Value.TypeName);//??
        string result = string.Empty;

        if (includeLiveList)
        {
            result += $"Record Count: {createdCount}, Disposed Count: {disposedCount}, Alive Count: {aliveCount} \r\n";
            foreach (var alive in aliveList)
            {
                result += $"Record No.{alive.Value.Id}: Type: {alive.Value.TypeName}, Name: {alive.Value.Name}, Scope: {alive.Value.ScopeName}, Note: {alive.Value.Note}.\r\n";
            }
            return result;
        }
        else
        {
            result += $"Record Count: {createdCount}, Disposed Count: {disposedCount}, Alive Count: {aliveCount}";
            return result;
        }
    }

    private sealed class ResourceRecord
    {
        public ResourceRecord(Guid id, string typeName, string name, string scopeName, string note)
        {
            Id = id;
            TypeName = typeName;
            Name = name;
            ScopeName = scopeName;
            Note = note;
            State = ResourceState.Alive;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public string TypeName { get; }
        public string Name { get; }
        public string ScopeName { get; }
        public string Note { get; }

        public ResourceState State { get; set; }
        public DateTime CreatedAtUtc { get; }
        public DateTime? DisposedAtUtc { get; set; }
    }
}

// ============================================================================
// LIFETIME SCOPE
// ============================================================================

public sealed class LifetimeScope : IDisposable
{
    private readonly List<ITrackedHandle> _handles = new();
    private readonly List<LifetimeScope> _children = new();

    private readonly ResourceTracker _tracker;

    private bool _isDisposed;

    public LifetimeScope(string name, ResourceTracker tracker)
    {
        Name = name;
        _tracker = tracker;
    }

    public string Name { get; }

    // Tạo scope con.
    public LifetimeScope CreateChild(string name)
    {
        // TODO:
        // - Nếu scope đã disposed -> throw
        // - Tạo child
        // - Add vào _children
        // - return child
        if (_isDisposed) throw new Exception("Is Disposed!!");
        LifetimeScope child = new(name, _tracker);
        _children.Add(child);
        return child;
    }

    // Add một IDisposable vào scope để auto-dispose khi scope dispose.
    public void Add(IDisposable disposable, string note)
    {
        // TODO:
        // - Nếu disposed -> throw
        // - Wrap disposable thành tracked handle để tracker biết
        // - Add vào _handles
        if (_isDisposed) throw new Exception("Is Disposed");
        DisposableHandle dispoHand = new(disposable, _tracker, Name, note);
        _handles.Add(dispoHand);
    }

    // Add một cleanup action (không có IDisposable).
    public void AddCleanup(Action cleanup, string note)
    {
        // TODO:
        // - Nếu disposed -> throw
        // - Wrap cleanup thành tracked handle
        // - Add vào _handles
        if (_isDisposed) throw new Exception("Is Disposed!!");
        CleanupHandle cleanHand = new(cleanup, _tracker, Name, note);
        _handles.Add(cleanHand);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        // TODO (quan trọng):
        // - Dispose children trước hay handles trước? (bạn tự quyết định và giải thích)
        // - Dispose theo thứ tự ngược lại (LIFO) để giống stack unwind
        // - Nếu một handle dispose bị exception: nên làm gì?
        //   gợi ý: catch + tiếp tục dispose các handle khác (aggregate errors optional)
        foreach(var handle in _handles)
        {
            try
            {
                handle.DisposeSafely();
            }
            catch
            { 
                continue;
            }
        }
        foreach (var child in _children)
        {
            child.Dispose();
        }
    }
}

// ============================================================================
// TRACKED HANDLES (internal)
// ============================================================================

internal interface ITrackedHandle
{
    void DisposeSafely();
}

internal sealed class DisposableHandle : ITrackedHandle
{
    private readonly IDisposable _disposable;
    private readonly ResourceTracker _tracker;
    private readonly Guid _id;

    public DisposableHandle(IDisposable disposable, ResourceTracker tracker, string scopeName, string note)
    {
        _disposable = disposable;
        _tracker = tracker;

        string typeName = disposable.GetType().Name;
        string name = disposable is INamedResource nr ? nr.Name : typeName;

        _id = _tracker.TrackCreated(typeName, name, scopeName, note);
    }

    public void DisposeSafely()
    {
        // TODO:
        // - dispose _disposable
        // - tracker.TrackDisposed(_id)
        _disposable.Dispose();
        _tracker.TrackDisposed(_id);
    }
}

internal sealed class CleanupHandle : ITrackedHandle
{
    private readonly Action _cleanup;
    private readonly ResourceTracker _tracker;
    private readonly Guid _id;

    public CleanupHandle(Action cleanup, ResourceTracker tracker, string scopeName, string note)
    {
        _cleanup = cleanup;
        _tracker = tracker;

        _id = _tracker.TrackCreated(typeName: "CleanupAction", name: "Action", scopeName, note);
    }

    public void DisposeSafely()
    {
        // TODO:
        // - invoke _cleanup
        // - tracker.TrackDisposed(_id)
        _cleanup?.Invoke();
        _tracker.TrackDisposed(_id);
    }
}

// ============================================================================
// DEMO RESOURCES
// ============================================================================

public interface INamedResource
{
    string Name { get; }
}

public sealed class TrackedDisposable : IDisposable, INamedResource
{
    private bool _disposed;

    private readonly ResourceTracker _tracker;
    private readonly Guid _id;

    public TrackedDisposable(string name, ResourceTracker tracker)
    {
        Name = name;
        _tracker = tracker;

        // Ghi nhận tạo resource (đây là resource "thật" cũng track)
        // Lưu ý: LifetimeScope cũng track qua DisposableHandle -> sẽ có 2 record nếu bạn track cả 2 tầng.
        // Để đơn giản, bạn có thể:
        // - Chỉ track ở handle (khuyến nghị) hoặc
        // - Track cả resource (để so sánh) -> học viên tự quyết định.
        // TODO: chọn 1 hướng và implement cho consistent.
        _id = Guid.Empty;
    }

    public string Name { get; }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Console.WriteLine($"[Dispose] {Name}");

        // TODO: nếu bạn chọn track ở resource, gọi _tracker.TrackDisposed(_id)
        _tracker.TrackDisposed(_id);
    }
}
