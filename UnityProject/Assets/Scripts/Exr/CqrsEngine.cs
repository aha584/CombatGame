// ======================================================================
//  BÀI TẬP: MINI EVENT SOURCING + CQRS ENGINE
//
//  1) Mô phỏng một kiến trúc rất hay gặp trong các hệ thống backend phức tạp:
//     - Event Sourcing
//     - CQRS (Command Query Responsibility Segregation)
//
//  2) EVENT SOURCING (Lưu trữ bằng SỰ KIỆN, không phải trạng thái cuối):
//     - Thay vì chỉ lưu "số dư hiện tại = 1200", hệ thống sẽ lưu:
//         + AccountOpened (mở tài khoản, số dư ban đầu)
//         + MoneyDeposited (nạp tiền)
//         + MoneyWithdrawn (rút tiền)
//         + AccountClosed (đóng tài khoản)
//     - Trạng thái hiện tại của tài khoản được tính bằng cách "replay" toàn bộ
//       chuỗi event này (giống như xem lại lịch sử giao dịch).
//
//  3) CQRS (Tách lệnh và truy vấn):
//     - Command side: xử lý nghiệp vụ, tạo event, lưu xuống Event Store.
//         + Ví dụ: OpenAccountCommand, DepositCommand, WithdrawCommand, CloseAccountCommand.
//     - Query side: chỉ đọc dữ liệu, không đụng vào aggregate hay event.
//         + Dùng "Read Model" (Projection) được build sẵn từ chuỗi event.
//         + Ví dụ: AccountReadModel chứa Id, OwnerName, Balance, IsClosed.
//
//  4) MỤC TIÊU BÀI TẬP:
//     - Xây dựng một hệ thống tối giản nhưng "đúng concept" Event Sourcing + CQRS:
//         + AggregateRootBase + BankAccountAggregate (domain + event sourcing).
//         + Event Store + Event Dispatcher (lưu và publish event).
//         + EventSourcingRepository (rebuild aggregate từ event).
//         + Read Model Store + Projection (CQRS – Query side).
//         + Command Service (Command side – xử lý lệnh).
//     - Chương trình test (Main) sẽ:
//         + Gửi một loạt command (mở tài khoản, nạp, rút, đóng).
//         + In ra READ MODEL để xem trạng thái cuối.
//
//  ⭐ Độ khó 9/10.
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Event Sourcing Core

/// <summary>
/// Event cơ bản trong hệ thống Event Sourcing.
/// </summary>
public interface IEvent
{
    Guid AggregateId { get; }
    int Version { get; }
    DateTime Timestamp { get; }
}

/// <summary>
/// Base đơn giản cho event.
/// </summary>
public abstract class EventBase : IEvent
{
    public Guid AggregateId { get; protected set; }
    public int Version { get; protected set; }
    public DateTime Timestamp { get; protected set; }

    protected EventBase(Guid aggregateId, int version)
    {
        AggregateId = aggregateId;
        Version = version;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Aggregate root cho event sourcing.
/// Lưu tracking event mới và cho phép replay từ lịch sử.
/// </summary>
public abstract class AggregateRootBase
{
    private readonly List<IEvent> _uncommittedEvents = new();

    /// <summary>Id của aggregate.</summary>
    public Guid Id { get; protected set; }

    /// <summary>Version hiện tại (tăng dần theo từng event).</summary>
    public int Version { get; protected set; } = -1;

    /// <summary>
    /// Lấy danh sách event chưa commit (chưa lưu xuống Event Store).
    /// </summary>
    public IReadOnlyCollection<IEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

    /// <summary>
    /// Xóa danh sách event chưa commit sau khi lưu xong.
    /// </summary>
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    /// <summary>
    /// Áp dụng event mới (từ logic hiện tại) + track vào _uncommittedEvents.
    /// </summary>
    protected void ApplyChange(IEvent @event)
    {
        // TODO:
        // 1. Gọi ApplyEventInternal(@event) để cập nhật state.
        // 2. Thêm event vào _uncommittedEvents.
        // 3. Cập nhật Version = event.Version.
        ApplyEventInternal(@event);
        _uncommittedEvents.Add(@event);
        Version = @event.Version;
    }

    /// <summary>
    /// Replay lịch sử event (không track vào _uncommittedEvents).
    /// </summary>
    public void LoadFromHistory(IEnumerable<IEvent> history)
    {
        // TODO:
        // 1. Với mỗi event trong history:
        //    - Gọi ApplyEventInternal(@event) để cập nhật state.
        //    - Cập nhật Version = event.Version.
        foreach(var @event in history)
        {
            ApplyEventInternal(@event);
            Version = @event.Version;
        }
    }

    /// <summary>
    /// Hàm này gọi vào các hàm Apply cụ thể (pattern: Apply(AccountOpenedEvent e))
    /// Sử dụng dynamic hoặc pattern matching tùy bạn.
    /// </summary>
    protected abstract void ApplyEventInternal(IEvent @event);
}

#endregion

#region Event Store + Dispatcher

/// <summary>
/// Lưu trữ event theo từng aggregate (stream).
/// </summary>
public interface IEventStore
{
    void AppendEvents(Guid aggregateId, int expectedVersion, IEnumerable<IEvent> newEvents);
    IReadOnlyList<IEvent> GetEvents(Guid aggregateId);
}

/// <summary>
/// Đơn giản: in-memory event store.
/// </summary>
public sealed class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<Guid, List<IEvent>> _streams = new();
    private readonly IEventDispatcher _dispatcher;

    public InMemoryEventStore(IEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void AppendEvents(Guid aggregateId, int expectedVersion, IEnumerable<IEvent> newEvents)
    {
        // TODO:
        // 1. Lấy stream tương ứng aggregateId (nếu chưa có thì tạo mới).
        // 2. Kiểm tra optimistic concurrency: version cuối cùng phải == expectedVersion.
        // 3. Thêm từng event vào stream, đảm bảo Version tăng dần (Version = index hoặc ++).
        // 4. Sau khi thêm, publish từng event qua _dispatcher.Publish(@event).
        List<IEvent> stream;
        if (_streams.ContainsKey(aggregateId))
        {
            stream = _streams[aggregateId];
        }
        else
        {
            _streams.Add(aggregateId, new());
            stream = _streams[aggregateId];
        }
        foreach(var @event in newEvents)
        {
            if (@event.Version != expectedVersion) continue;
            else
            {
                stream.Add(@event);
            }
        }
        stream = stream.OrderBy(@event => @event.Version).ToList();
        foreach(var @event in stream)
        {
            _dispatcher.Publish(@event);
        }
    }

    public IReadOnlyList<IEvent> GetEvents(Guid aggregateId)
    {
        // TODO:
        // 1. Nếu không có stream => có thể trả list rỗng hoặc throw, tùy bạn.
        // 2. Trả về bản copy read-only của danh sách event.
        if (_streams.ContainsKey(aggregateId))
        {
            return _streams[aggregateId].AsReadOnly();
        }
        else
        {
            return null;
        }
    }
}

/// <summary>
/// Interface publish event đến các handler (projection, v.v.).
/// </summary>
public interface IEventDispatcher
{
    void Publish(IEvent @event);
    void RegisterHandler<TEvent>(Action<TEvent> handler) where TEvent : class, IEvent;
}

/// <summary>
/// Event dispatcher đơn giản dùng dictionary<Type, List<Delegate>>.
/// </summary>
public sealed class InMemoryEventDispatcher : IEventDispatcher
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Publish(IEvent @event)
    {
        // TODO:
        // 1. Lấy type của event.
        // 2. Nếu có handler cho type này, lần lượt Invoke handler(@event).
        //    Gợi ý: dùng Delegate.DynamicInvoke hoặc cast về Action<TEvent>.
        Type eveType = typeof(IEvent);
        foreach(var handler in _handlers[eveType])
        {
            handler?.DynamicInvoke(@event);
        }
    }

    public void RegisterHandler<TEvent>(Action<TEvent> handler) where TEvent : class, IEvent
    {
        // TODO:
        // 1. Lấy type = typeof(TEvent).
        // 2. Thêm handler vào list trong _handlers.
        Type eveType = typeof(IEvent);
        _handlers[eveType].Add(handler);
    }
}

#endregion

#region Event Sourcing Repository

/// <summary>
/// Repository sử dụng Event Store để rebuild aggregate.
/// </summary>
public sealed class EventSourcingRepository<TAggregate>
    where TAggregate : AggregateRootBase, new()
{
    private readonly IEventStore _store;

    public EventSourcingRepository(IEventStore store)
    {
        _store = store;
    }

    /// <summary>
    /// Lấy aggregate theo id bằng cách replay event.
    /// Nếu chưa có event, có thể trả null (hoặc throw) tùy bạn chọn.
    /// </summary>
    public TAggregate? GetById(Guid id)
    {
        // TODO:
        // 1. Lấy events từ _store.GetEvents(id).
        // 2. Nếu không có event => return null.
        // 3. Tạo instance mới TAggregate, gọi LoadFromHistory(events).
        // 4. Trả về aggregate.
        var events = _store.GetEvents(id);
        if(events.Count > 0)
        {
            TAggregate aggregate = new();
            aggregate.LoadFromHistory(events);
            return aggregate;
        }
        else 
        {
            return null;
        }
    }

    /// <summary>
    /// Lưu aggregate: lấy các event chưa commit, gọi AppendEvents, sau đó ClearUncommittedEvents.
    /// </summary>
    public void Save(TAggregate aggregate)
    {
        // TODO:
        // 1. Lấy uncommitted events từ aggregate.
        // 2. expectedVersion = aggregate.Version.
        // 3. Gọi _store.AppendEvents(aggregate.Id, expectedVersion, events).
        // 4. Gọi aggregate.ClearUncommittedEvents().
        var uncommitedEvents = aggregate.GetUncommittedEvents();
        _store.AppendEvents(aggregate.Id, aggregate.Version, uncommitedEvents);
        aggregate.ClearUncommittedEvents();
    }
}

#endregion

#region Domain: Bank Account Events

public sealed class AccountOpenedEvent : EventBase
{
    public string OwnerName { get; }
    public decimal InitialBalance { get; }

    public AccountOpenedEvent(Guid aggregateId, int version, string ownerName, decimal initialBalance)
        : base(aggregateId, version)
    {
        OwnerName = ownerName;
        InitialBalance = initialBalance;
    }
}

public sealed class MoneyDepositedEvent : EventBase
{
    public decimal Amount { get; }

    public MoneyDepositedEvent(Guid aggregateId, int version, decimal amount)
        : base(aggregateId, version)
    {
        Amount = amount;
    }
}

public sealed class MoneyWithdrawnEvent : EventBase
{
    public decimal Amount { get; }

    public MoneyWithdrawnEvent(Guid aggregateId, int version, decimal amount)
        : base(aggregateId, version)
    {
        Amount = amount;
    }
}

public sealed class AccountClosedEvent : EventBase
{
    public AccountClosedEvent(Guid aggregateId, int version)
        : base(aggregateId, version)
    {
    }
}

#endregion

#region Domain: Bank Account Aggregate

/// <summary>
/// Aggregate tài khoản ngân hàng: logic domain + event sourcing.
/// </summary>
public sealed class BankAccountAggregate : AggregateRootBase
{
    public string OwnerName { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public bool IsClosed { get; private set; }

    public BankAccountAggregate() { }

    /// <summary>
    /// Factory mở tài khoản mới.
    /// </summary>
    public static BankAccountAggregate OpenNew(Guid id, string ownerName, decimal initialBalance)
    {
        // TODO:
        // 1. Tạo instance mới BankAccountAggregate.
        // 2. Gán Id cho aggregate.
        // 3. Tạo AccountOpenedEvent với version thích hợp (ví dụ 0).
        // 4. Gọi ApplyChange(event).
        // 5. Trả về aggregate.
        BankAccountAggregate aggregate = new();
        aggregate.Id = id;
        aggregate.OwnerName = ownerName;
        aggregate.Balance = initialBalance;
        AccountOpenedEvent newOpenedEvent = new(id, 0, ownerName, initialBalance);
        aggregate.ApplyChange(newOpenedEvent);
        return aggregate;
    }

    /// <summary>
    /// Nạp tiền vào tài khoản.
    /// </summary>
    public void Deposit(decimal amount)
    {
        // TODO:
        // 1. Kiểm tra tài khoản đã đóng chưa (IsClosed) => nếu đóng thì throw hoặc bỏ qua.
        // 2. Kiểm tra amount > 0.
        // 3. Tạo MoneyDepositedEvent với version = Version + 1.
        // 4. Gọi ApplyChange(event).
        if (IsClosed)
        {
            return;
        }
        else
        {
            if(amount > 0)
            {
                MoneyDepositedEvent newDepositEvent = new(Id, Version + 1, amount);
                ApplyChange(newDepositEvent);
            }
        }
    }

    /// <summary>
    /// Rút tiền khỏi tài khoản.
    /// </summary>
    public void Withdraw(decimal amount)
    {
        // TODO:
        // 1. Kiểm tra IsClosed.
        // 2. Kiểm tra amount > 0.
        // 3. Kiểm tra Balance đủ tiền.
        // 4. Tạo MoneyWithdrawnEvent (Version + 1), ApplyChange.
        if (IsClosed) return;
        else
        {
            if (amount > 0 && Balance >= amount)
            {
                MoneyWithdrawnEvent newWithdrawEvent = new(Id, Version + 1, amount);
                ApplyChange(newWithdrawEvent);
            }
        }
    }

    /// <summary>
    /// Đóng tài khoản.
    /// </summary>
    public void Close()
    {
        // TODO:
        // 1. Nếu đã đóng thì bỏ qua hoặc throw.
        // 2. Có thể yêu cầu Balance == 0 mới cho đóng (tùy bạn).
        // 3. Tạo AccountClosedEvent (Version + 1), ApplyChange.
        if (IsClosed) return;
        else
        {
            AccountClosedEvent newClosedEvent = new(Id, Version + 1);
            ApplyChange(newClosedEvent);
        }
    }

    /// <summary>
    /// Áp dụng event vào state nội bộ (dùng khi replay lịch sử hoặc ApplyChange).
    /// </summary>
    protected override void ApplyEventInternal(IEvent @event)
    {
        // TODO:
        // 1. Dùng pattern matching hoặc switch theo type của event:
        //    - AccountOpenedEvent => set OwnerName, Balance, IsClosed = false, Id = AggregateId
        //    - MoneyDepositedEvent => tăng Balance
        //    - MoneyWithdrawnEvent => giảm Balance
        //    - AccountClosedEvent => IsClosed = true
        if(@event is AccountOpenedEvent accountOpened)
        {
            OwnerName = accountOpened.OwnerName;
            Balance = accountOpened.InitialBalance;
            IsClosed = false;
            Id = accountOpened.AggregateId;
        }
        else if(@event is MoneyDepositedEvent depositedEvent)
        {
            Balance += depositedEvent.Amount;
        }
        else if(@event is MoneyWithdrawnEvent moneyWithdrawnEvent)
        {
            Balance -= moneyWithdrawnEvent.Amount;
        }
        else if (@event is AccountClosedEvent accountClosedEvent)
        {
            IsClosed = true;
        }
    }
}

#endregion

#region CQRS: Read Model + Projection

/// <summary>
/// Read model đơn giản cho tài khoản (dùng để query).
/// </summary>
public sealed class AccountReadModel
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsClosed { get; set; }

    public override string ToString()
        => $"Id={Id}, Owner={OwnerName}, Balance={Balance}, Closed={IsClosed}";
}

/// <summary>
/// Lưu trữ read model trong bộ nhớ.
/// </summary>
public sealed class AccountReadModelStore
{
    private readonly Dictionary<Guid, AccountReadModel> _accounts = new();

    public AccountReadModel GetOrCreate(Guid id)
    {
        if (!_accounts.TryGetValue(id, out var model))
        {
            model = new AccountReadModel { Id = id };
            _accounts[id] = model;
        }
        return model;
    }

    public IEnumerable<AccountReadModel> GetAll() => _accounts.Values;
}

/// <summary>
/// Projection: nhận event và cập nhật read model store.
/// </summary>
public sealed class AccountProjection
{
    private readonly AccountReadModelStore _store;

    public AccountProjection(AccountReadModelStore store)
    {
        _store = store;
    }

    // Các handler sau sẽ được đăng ký với IEventDispatcher.

    public void On(AccountOpenedEvent @event)
    {
        // TODO:
        // 1. GetOrCreate read model theo AggregateId.
        // 2. Set OwnerName, Balance, IsClosed = false.
        AccountReadModel readModel = _store.GetOrCreate(@event.AggregateId);
        readModel.OwnerName = @event.OwnerName;
        readModel.Balance = @event.InitialBalance;
        readModel.IsClosed = false;
    }

    public void On(MoneyDepositedEvent @event)
    {
        // TODO:
        // 1. GetOrCreate.
        // 2. Tăng Balance.
        AccountReadModel readModel = _store.GetOrCreate(@event.AggregateId);
        readModel.Balance += @event.Amount;
    }

    public void On(MoneyWithdrawnEvent @event)
    {
        // TODO:
        // 1. GetOrCreate.
        // 2. Giảm Balance.
        AccountReadModel readModel = _store.GetOrCreate(@event.AggregateId);
        readModel.Balance -= @event.Amount;
    }

    public void On(AccountClosedEvent @event)
    {
        // TODO:
        // 1. GetOrCreate.
        // 2. Set IsClosed = true.
        AccountReadModel readModel = _store.GetOrCreate(@event.AggregateId);
        readModel.IsClosed = true;
    }
}

#endregion

#region Command Side (CQRS)

/// <summary>
/// Command mở tài khoản.
/// </summary>
public sealed class OpenAccountCommand
{
    public Guid AccountId { get; }
    public string OwnerName { get; }
    public decimal InitialBalance { get; }

    public OpenAccountCommand(Guid accountId, string ownerName, decimal initialBalance)
    {
        AccountId = accountId;
        OwnerName = ownerName;
        InitialBalance = initialBalance;
    }
}

/// <summary>
/// Command nạp tiền.
/// </summary>
public sealed class DepositCommand
{
    public Guid AccountId { get; }
    public decimal Amount { get; }

    public DepositCommand(Guid accountId, decimal amount)
    {
        AccountId = accountId;
        Amount = amount;
    }
}

/// <summary>
/// Command rút tiền.
/// </summary>
public sealed class WithdrawCommand
{
    public Guid AccountId { get; }
    public decimal Amount { get; }

    public WithdrawCommand(Guid accountId, decimal amount)
    {
        AccountId = accountId;
        Amount = amount;
    }
}

/// <summary>
/// Command đóng tài khoản.
/// </summary>
public sealed class CloseAccountCommand
{
    public Guid AccountId { get; }

    public CloseAccountCommand(Guid accountId)
    {
        AccountId = accountId;
    }
}

/// <summary>
/// Service xử lý command, sử dụng EventSourcingRepository.
/// </summary>
public sealed class BankAccountCommandService
{
    private readonly EventSourcingRepository<BankAccountAggregate> _repository;

    public BankAccountCommandService(EventSourcingRepository<BankAccountAggregate> repository)
    {
        _repository = repository;
    }

    public void Handle(OpenAccountCommand command)
    {
        // TODO:
        // 1. Gọi BankAccountAggregate.OpenNew(command.AccountId, ...).
        // 2. Gọi _repository.Save(aggregate).
        BankAccountAggregate aggregate = BankAccountAggregate.OpenNew(command.AccountId, command.OwnerName, command.InitialBalance);
        _repository.Save(aggregate);
    }

    public void Handle(DepositCommand command)
    {
        // TODO:
        // 1. Lấy aggregate từ repository (GetById).
        // 2. Gọi aggregate.Deposit.
        // 3. Save.
        BankAccountAggregate aggregate = _repository.GetById(command.AccountId);
        aggregate.Deposit(command.Amount);
        _repository.Save(aggregate);
    }

    public void Handle(WithdrawCommand command)
    {
        // TODO:
        // 1. Tương tự Deposit nhưng gọi Withdraw.
        BankAccountAggregate aggregate = _repository.GetById(command.AccountId);
        aggregate.Withdraw(command.Amount);
        _repository.Save(aggregate);
    }

    public void Handle(CloseAccountCommand command)
    {
        // TODO:
        // 1. Lấy aggregate.
        // 2. Gọi Close.
        // 3. Save.
        BankAccountAggregate aggregate = _repository.GetById(command.AccountId);
        aggregate.Close();
        _repository.Save(aggregate);
    }
}

#endregion

// ======================================================================
//  PROGRAM: TEST MINI EVENT SOURCING + CQRS ENGINE
// ======================================================================
public static class Program1
{
    public static void Main()
    {
        // -------------------------------------------------------------
        // 1. Khởi tạo Dispatcher, Event Store, Read Model, Projection
        // -------------------------------------------------------------
        var dispatcher = new InMemoryEventDispatcher();
        var store = new InMemoryEventStore(dispatcher);
        var repository = new EventSourcingRepository<BankAccountAggregate>(store);
        var readStore = new AccountReadModelStore();
        var projection = new AccountProjection(readStore);

        // Đăng ký projection với dispatcher
        dispatcher.RegisterHandler<AccountOpenedEvent>(projection.On);
        dispatcher.RegisterHandler<MoneyDepositedEvent>(projection.On);
        dispatcher.RegisterHandler<MoneyWithdrawnEvent>(projection.On);
        dispatcher.RegisterHandler<AccountClosedEvent>(projection.On);

        // -------------------------------------------------------------
        // 2. Tạo Command Service
        // -------------------------------------------------------------
        var commandService = new BankAccountCommandService(repository);

        // -------------------------------------------------------------
        // 3. Tạo một vài account id cố định để test
        // -------------------------------------------------------------
        var acc1 = Guid.NewGuid();
        var acc2 = Guid.NewGuid();

        // -------------------------------------------------------------
        // 4. Gửi một số command để mô phỏng nghiệp vụ
        // -------------------------------------------------------------
        Console.WriteLine("=== Gửi command mở tài khoản ===");
        commandService.Handle(new OpenAccountCommand(acc1, "Alice", 1000m));
        commandService.Handle(new OpenAccountCommand(acc2, "Bob", 0m));

        Console.WriteLine("=== Gửi command nạp tiền ===");
        commandService.Handle(new DepositCommand(acc1, 500m));
        commandService.Handle(new DepositCommand(acc2, 200m));

        Console.WriteLine("=== Gửi command rút tiền ===");
        commandService.Handle(new WithdrawCommand(acc1, 300m));

        Console.WriteLine("=== Gửi command đóng tài khoản ===");
        commandService.Handle(new CloseAccountCommand(acc2));

        // -------------------------------------------------------------
        // 5. In ra READ MODEL (CQRS Query side)
        // -------------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== READ MODELS HIỆN TẠI ===");
        foreach (var model in readStore.GetAll())
        {
            Console.WriteLine(model);
        }

        Console.WriteLine();
        Console.WriteLine(">>> Hoàn thành skeleton. Khi bạn implement xong TODO, kết quả sẽ phản ánh đúng event.");
        Console.WriteLine(">>> Nhấn phím bất kỳ để thoát...");
        Console.ReadKey();
    }
}
