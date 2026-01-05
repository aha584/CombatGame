// ============================================================================
// BÀI TẬP HỆ THỐNG (8/10): Hot-Reload Safe System (Reload an toàn, không leak)
// ----------------------------------------------------------------------------
// Hệ thống này dùng để làm gì?
// - Cho phép "reload" module (logic/config) ngay trong runtime mà KHÔNG restart app.
// - Mục tiêu thực chiến: tránh 3 lỗi cực phổ biến khi reload:
//   1) Double subscribe (mỗi lần reload subscribe thêm -> event bắn N lần)
//   2) Logic leak (timer/job cũ vẫn chạy)
//   3) Stale cache (cache cũ không bị reset -> kết quả sai)
//
// Bạn sẽ xây một HotReloadHost quản lý vòng đời module theo key:
// - Load / Reload / Unload module
// - Capture state trước khi tắt, Restore state sau khi bật (có version)
// - Mỗi module có 1 LifetimeScope riêng để gom mọi resource (subscription, timer, cleanup)
// - Có Diagnostics để phát hiện leak/double side-effect sau reload
//
// HƯỚNG DẪN LÀM:
// - Hoàn thiện các TODO.
// - Chạy Program.Main: kết quả phải đúng như EXPECT ở cuối.
// ============================================================================

using System;
using System.Collections.Generic;

// ============================================================================
// PROGRAM (TEST HARNESS)
// ============================================================================

public static class Program2
{
    public static void Main()
    {
        Console.WriteLine("===== Hot Reload Safe System Demo =====");

        // Global services (giả lập "app container")
        Diagnostics diagnostics = new();
        EventBus eventBus = new(diagnostics);
        ConfigStore config = new();

        // Seed config
        config.Set("Economy.PriceMultiplier", "1.0");
        config.Set("Economy.DailyBonus", "100");

        // Hot reload host
        HotReloadHost host = new(
            registry: new ServiceRegistry(),
            shared: new HostSharedServices(eventBus, config, diagnostics));

        // Factories (mô phỏng thay đổi module implementation)
        ModuleFactory economyV1 = ctx => new EconomyTunerModule_V1(ctx);
        ModuleFactory promoV1 = ctx => new EventDrivenPromoModule_V1(ctx);

        Console.WriteLine();
        Console.WriteLine(">>> Load modules");
        host.Load(ModuleKey.Economy, economyV1);
        host.Load(ModuleKey.Promo, promoV1);

        Console.WriteLine();
        Console.WriteLine(">>> Fire PurchaseEvent (expected log exactly ONCE)");
        eventBus.Publish(new PurchaseEvent("u_001", 4.99m));

        Console.WriteLine();
        Console.WriteLine(">>> Reload Promo module 3 times (must NOT double subscribe)");
        host.Reload(ModuleKey.Promo, promoV1);
        host.Reload(ModuleKey.Promo, promoV1);
        host.Reload(ModuleKey.Promo, promoV1);

        Console.WriteLine();
        Console.WriteLine(">>> Fire PurchaseEvent again (expected log exactly ONCE)");
        eventBus.Publish(new PurchaseEvent("u_002", 9.99m));

        Console.WriteLine();
        Console.WriteLine(">>> Change economy config and reload Economy");
        config.Set("Economy.PriceMultiplier", "1.5");
        config.Set("Economy.DailyBonus", "250");
        host.Reload(ModuleKey.Economy, economyV1);

        Console.WriteLine();
        Console.WriteLine(">>> Query economy service (must reflect NEW config)");
        IEconomyTuner economy = host.Registry.GetRequired<IEconomyTuner>();
        Console.WriteLine($"PriceMultiplier = {economy.PriceMultiplier} (expected 1.5)");
        Console.WriteLine($"DailyBonus      = {economy.DailyBonus} (expected 250)");

        Console.WriteLine();
        Console.WriteLine(">>> Diagnostics report (must not increase subscriptions across reload)");
        Console.WriteLine(diagnostics.BuildReport());

        Console.WriteLine();
        Console.WriteLine(">>> Unload all");
        host.Unload(ModuleKey.Promo);
        host.Unload(ModuleKey.Economy);

        Console.WriteLine();
        Console.WriteLine(">>> Diagnostics after unload (active should be 0)");
        Console.WriteLine(diagnostics.BuildReport());

        // --------------------------------------------------------------------
        // EXPECT (mang tính minh họa):
        // - PurchaseEvent mỗi lần chỉ log 1 lần, dù reload Promo 3 lần.
        // - Economy reload xong phản ánh config mới (1.5, 250).
        // - Diagnostics:
        //   - ActiveSubscriptions không tăng dần theo reload (không leak) => 2
        //   - ActiveScopes phản ánh đúng load/unload => 6
        // --------------------------------------------------------------------
    }
}

// ============================================================================
// DOMAIN KEYS
// ============================================================================

public enum ModuleKey
{
    Economy,
    Promo,
}

// ============================================================================
// HOST: shared services + module factory
// ============================================================================

public delegate IReloadableModule ModuleFactory(ModuleContext context);

public sealed class HostSharedServices
{
    public HostSharedServices(EventBus eventBus, ConfigStore config, Diagnostics diagnostics)
    {
        EventBus = eventBus;
        Config = config;
        Diagnostics = diagnostics;
    }

    public EventBus EventBus { get; }
    public ConfigStore Config { get; }
    public Diagnostics Diagnostics { get; }
}

// ============================================================================
// RELOADABLE MODULE API
// ============================================================================

public interface IReloadableModule
{
    ModuleKey Key { get; }

    // Version của schema state (để migrate khi module thay đổi structure)
    int StateVersion { get; }

    void Initialize(ModuleContext ctx);
    void Shutdown(ModuleContext ctx);

    // Capture/Restore state qua reload
    object CaptureState();
    void RestoreState(int version, object? state);
}

public sealed class ModuleContext
{
    public ModuleContext(ModuleKey key, HostSharedServices shared, ServiceRegistry registry, LifetimeScope2 scope)
    {
        Key = key;
        Shared = shared;
        Registry = registry;
        Scope = scope;
    }

    public ModuleKey Key { get; }
    public HostSharedServices Shared { get; }
    public ServiceRegistry Registry { get; }
    public LifetimeScope2 Scope { get; }
}

// ============================================================================
// HOT RELOAD HOST
// ============================================================================

public sealed class HotReloadHost
{
    private readonly Dictionary<ModuleKey, ModuleSlot> _slots = new();

    private readonly HostSharedServices _shared;

    public HotReloadHost(ServiceRegistry registry, HostSharedServices shared)
    {
        Registry = registry;
        _shared = shared;
    }

    public ServiceRegistry Registry { get; }

    public void Load(ModuleKey key, ModuleFactory factory)
    {
        // TODO:
        // - Nếu đã load -> throw hoặc ignore (tự quyết định, nhưng phải nhất quán)
        // - Tạo LifetimeScope cho module
        // - Tạo ModuleContext
        // - Create module từ factory
        // - RestoreState (first load: state null)
        // - Initialize
        // - Lưu slot
        if (_slots.ContainsKey(key))
        {
            throw new Exception("Is Loaded!!");
        }
        else
        {
            ModuleContext ctx = new(key, _shared, Registry, new LifetimeScope2($"{key}", _shared.Diagnostics));
            IReloadableModule module = factory(ctx);
            module.RestoreState(module.StateVersion, null);
            module.Initialize(ctx);
            ModuleSlot slot = new(module, ctx);
            _slots[key] = slot;
        }
    }

    public void Reload(ModuleKey key, ModuleFactory newFactory)
    {
        // TODO (CỰC QUAN TRỌNG):
        // Thứ tự chuẩn để tránh leak/double side-effects:
        // 1) Lấy slot hiện tại
        // 2) CaptureState (payload + version)
        // 3) Shutdown module cũ
        // 4) Dispose LifetimeScope cũ (phải dispose mọi subscription/timer)
        // 5) Tạo scope mới + context mới
        // 6) Tạo module mới từ factory
        // 7) RestoreState(version, payload) (có migrate nếu version khác)
        // 8) Initialize module mới
        // 9) Replace slot
        //
        // BONUS: nếu tạo module mới fail -> rollback slot cũ (không bắt buộc trong bài 8/10)
        ModuleSlot slot = _slots[key];
        slot.Module.CaptureState();
        slot.Module.Shutdown(slot.Context);
        slot.Context.Scope.Dispose();
        ModuleContext newCtx = new(key, _shared, Registry, new($"{key}", _shared.Diagnostics));
        IReloadableModule newModule = newFactory(newCtx);
        newModule.RestoreState(slot.Module.StateVersion, slot.Module.CaptureState());
        newModule.Initialize(newCtx);
        slot = new(newModule, newCtx);
        _slots[key] = slot;
    }

    public void Unload(ModuleKey key)
    {
        // TODO:
        // - Nếu không tồn tại -> return
        // - Shutdown
        // - Dispose scope
        // - Remove slot
        if (!_slots.ContainsKey(key))
        {
            return;
        }
        else
        {
            _slots[key].Module.Shutdown(_slots[key].Context);
            _slots[key].Context.Scope.Dispose();
            _slots.Remove(key);
        }
    }

    private sealed class ModuleSlot
    {
        public ModuleSlot(IReloadableModule module, ModuleContext context)
        {
            Module = module;
            Context = context;
        }

        public IReloadableModule Module { get; }
        public ModuleContext Context { get; }
    }
}

// ============================================================================
// SERVICES: DI/REGISTRY MINI
// ============================================================================

public sealed class ServiceRegistry
{
    private readonly Dictionary<Type, object> _services = new();

    public void Register<TService>(TService instance) where TService : class
    {
        // TODO:
        // - Nếu đã có, replace (hoặc throw). Bài này nên replace để support reload.
        if (_services.ContainsKey(typeof(TService))) _services[typeof(TService)] = instance;
        else
        {
            _services[typeof(TService)] = instance;
        }
    }

    public bool TryGet<TService>(out TService service) where TService : class
    {
        // TODO
        //return _services.TryGetValue(typeof(TService), out (service as TService));

        if (_services.ContainsKey(typeof(TService)))
        {
            service = _services[typeof(TService)] as TService;
            return true;
        }
        else
        {
            service = null;
            return false;
        }
    }

    public TService GetRequired<TService>() where TService : class
    {
        // TODO: nếu không có -> throw message rõ ràng
        if (_services.ContainsKey(typeof(TService)))
        {
            return _services[typeof(TService)] as TService;
        }
        else
        {
            throw new Exception($"There is no Value for {typeof(TService)}!!!");
        }
    }

    public void Remove<TService>() where TService : class
    {
        _services.Remove(typeof(TService));
    }
}

// ============================================================================
// LIFETIME SCOPE (tái dùng ý tưởng bài trước, phiên bản rút gọn)
// ============================================================================

public sealed class LifetimeScope2 : IDisposable
{
    private readonly List<IDisposable> _disposables = new();
    private readonly Diagnostics _diagnostics;

    private bool _disposed;

    public LifetimeScope2(string name, Diagnostics diagnostics)
    {
        Name = name;
        _diagnostics = diagnostics;

        _diagnostics.OnScopeCreated(name);
    }

    public string Name { get; }

    public T Track<T>(T disposable) where T : IDisposable
    {
        // TODO:
        // - Nếu disposed -> throw
        // - Add vào list
        // - return disposable để gọi chain
        if (_disposed)
        {
            throw new Exception("This scope is Disposed!!");
        }
        else
        {
            _disposables.Add(disposable);
            return disposable;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        // TODO:
        // - Dispose LIFO (ngược thứ tự add)
        // - Catch exception để đảm bảo dispose hết
        try
        {
            _disposables[_disposables.Count - 1].Dispose();
        }
        catch
        {
            return;
        }
    }
}

// ============================================================================
// EVENT BUS + SUBSCRIPTION
// ============================================================================

public interface IEvent2 { }

public sealed class PurchaseEvent : IEvent2
{
    public PurchaseEvent(string userId, decimal amount)
    {
        UserId = userId;
        Amount = amount;
    }

    public string UserId { get; }
    public decimal Amount { get; }
}

public sealed class EventBus //Luu y class nay
{
    private readonly Diagnostics _diagnostics;

    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public EventBus(Diagnostics diagnostics)
    {
        _diagnostics = diagnostics;
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent2
    {
        // TODO:
        // - Add handler vào _handlers[typeof(TEvent)]
        // - Tăng diagnostics subscription count
        // - Return subscription IDisposable để remove handler khi dispose
        _handlers[typeof(TEvent)].Add(handler);
        _diagnostics.OnSubscriptionCreated();
        Subscription newSub = new(this, typeof(TEvent), handler, _diagnostics);
        return newSub;
    }

    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent2
    {
        if (!_handlers.TryGetValue(typeof(TEvent), out List<Delegate> list))
            return;

        // Copy list để tránh modify trong lúc iterate (sub/unsub trong handler)
        Delegate[] copy = list.ToArray();

        foreach (Delegate d in copy)
        {
            if (d is Action<TEvent> action)
                action(evt);//Invoke
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly EventBus _bus;
        private readonly Type _eventType;
        private readonly Delegate _handler;
        private readonly Diagnostics _diagnostics;

        private bool _disposed;

        public Subscription(EventBus bus, Type eventType, Delegate handler, Diagnostics diagnostics)
        {
            _bus = bus;
            _eventType = eventType;
            _handler = handler;
            _diagnostics = diagnostics;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            // TODO:
            // - Remove handler khỏi bus
            // - Giảm diagnostics subscription count
            _bus._handlers[_eventType].Remove(_handler);
            _diagnostics.OnSubscriptionDisposed();
        }
    }
}

// ============================================================================
// CONFIG STORE (giả lập remote config)
// ============================================================================

public sealed class ConfigStore
{
    private readonly Dictionary<string, string> _map = new();

    public void Set(string key, string value)
    {
        _map[key] = value;
    }

    public string GetRequired(string key)
    {
        if (!_map.TryGetValue(key, out string value))
            throw new InvalidOperationException($"Missing config key: {key}");

        return value;
    }
}

// ============================================================================
// DIAGNOSTICS (phát hiện leak / double side-effects)
// ============================================================================

public sealed class Diagnostics
{
    // Đếm subscription active để phát hiện double subscribe/leak
    private int _activeSubscriptions;

    // Track scopes
    private int _activeScopes;
    private int _createdScopes;
    private int _disposedScopes;

    public void OnSubscriptionCreated() => _activeSubscriptions++;
    public void OnSubscriptionDisposed() => _activeSubscriptions--;

    public void OnScopeCreated(string name)
    {
        _createdScopes++;
        _activeScopes++;
    }

    public void OnScopeDisposed(string name)
    {
        _disposedScopes++;
        _activeScopes--;
    }

    public string BuildReport()
    {
        // TODO: format report rõ ràng, ví dụ:
        // ActiveSubscriptions=..., ActiveScopes=..., CreatedScopes=..., DisposedScopes=...
        return $"ActiveSubscriptions = {_activeSubscriptions}, ActiveScopes = {_activeScopes}, CreatedScopes = {_createdScopes}, DisposedScopes = {_disposedScopes}";
    }
}

// ============================================================================
// MODULE A: ECONOMY TUNER (V1)
// ============================================================================

public interface IEconomyTuner
{
    float PriceMultiplier { get; }
    int DailyBonus { get; }
}

public sealed class EconomyTunerModule_V1 : IReloadableModule
{
    private ModuleContext _ctx;

    // State muốn persist qua reload
    private int _appliedCount;
    private string _lastConfigHash = "";

    // Cache runtime (phải tránh stale khi reload)
    private Dictionary<string, float> _computedPriceCache = new();

    // Exposed service values
    private float _priceMultiplier;
    private int _dailyBonus;

    public EconomyTunerModule_V1(ModuleContext ctx)
    {
        _ctx = ctx;
    }

    public ModuleKey Key => ModuleKey.Economy;

    public int StateVersion => 1;

    public void Initialize(ModuleContext ctx)
    {
        // TODO:
        // - Đọc config
        // - Apply vào _priceMultiplier/_dailyBonus
        // - Reset cache nếu cần
        // - Register IEconomyTuner service vào ctx.Registry
        // - Tăng _appliedCount, update _lastConfigHash
        //
        // LƯU Ý: Nếu làm sai, sau reload vẫn dùng cache cũ -> giá trị sai.
        string priceMulti = ctx.Shared.Config.GetRequired($"{ctx.Key}.PriceMultiplier");
        string dailyBonus = ctx.Shared.Config.GetRequired($"{ctx.Key}.DailyBonus");

        _priceMultiplier = float.Parse(priceMulti);
        _dailyBonus = int.Parse(dailyBonus);
        _computedPriceCache.Clear();
        EconomyService service = new(this);
        ctx.Registry.Register(service);
        _appliedCount++;
        _lastConfigHash = priceMulti + dailyBonus;
    }

    public void Shutdown(ModuleContext ctx)
    {
        // TODO:
        // - Nếu module register service, có thể remove hoặc replace ở init mới
        // - Không được để lại side-effect ngoài scope
        // Gợi ý: trong bài này, registry replace là đủ, remove optional.
        if(ctx.Registry.TryGet(out EconomyService service))
        {
            ctx.Registry.Register(service);
        }
    }

    public object CaptureState()
    {
        // TODO:
        // - Return payload chứa _appliedCount + _lastConfigHash
        // - Dạng record/class nhỏ

        //Tra ve clone cua chinh no
        return this.MemberwiseClone();
        //return new EconomyStateV1(_appliedCount, _lastConfigHash);
    }

    public void RestoreState(int version, object? state)
    {
        // TODO:
        // - Nếu state null: first load
        // - Nếu version == 1: cast payload và restore
        // - Nếu version khác: migrate (bonus)
        if(state == null)
        {
            return;
        }
        else if(version == 1)
        {
            //StateVersion = version;
            EconomyStateV1 newState = state as EconomyStateV1;
        }
        else
        {
            return;
        }
    }

    // Implementation of service (inner)
    private sealed class EconomyService : IEconomyTuner
    {
        private readonly EconomyTunerModule_V1 _owner;

        public EconomyService(EconomyTunerModule_V1 owner)
        {
            _owner = owner;
        }

        public float PriceMultiplier => _owner._priceMultiplier;
        public int DailyBonus => _owner._dailyBonus;
    }

    private sealed class EconomyStateV1
    {
        public EconomyStateV1(int appliedCount, string lastConfigHash)
        {
            AppliedCount = appliedCount;
            LastConfigHash = lastConfigHash;
        }

        public int AppliedCount { get; }
        public string LastConfigHash { get; }
    }
}

// ============================================================================
// MODULE B: PROMO (event-driven) (V1)
// ============================================================================

public sealed class EventDrivenPromoModule_V1 : IReloadableModule
{
    private int _promoShownCount;

    public EventDrivenPromoModule_V1(ModuleContext ctx) { }

    public ModuleKey Key => ModuleKey.Promo;

    public int StateVersion => 1;

    public void Initialize(ModuleContext ctx)
    {
        // TODO (CỰC QUAN TRỌNG):
        // - Subscribe PurchaseEvent qua ctx.Shared.EventBus
        // - Subscription phải được Track vào ctx.Scope (để auto-unsubscribe khi reload)
        //
        // Nếu học viên subscribe mà KHÔNG track vào scope => reload sẽ double subscribe.
        Action<PurchaseEvent> newEve = new Action<PurchaseEvent>(OnPurchase);

        IDisposable newDispo = ctx.Shared.EventBus.Subscribe(newEve);
        ctx.Scope.Track(newDispo);
    }

    public void Shutdown(ModuleContext ctx)
    {
        // TODO:
        // - Không tự unsubscribe thủ công nếu đã quản lý bằng scope
        // - Nếu có cache/worker riêng, phải cleanup
        ctx.Scope.Dispose();
    }

    public object? CaptureState()
    {
        // TODO: return state payload (promoShownCount)
        return new PromoStateV1(_promoShownCount);
    }

    public void RestoreState(int version, object? state)
    {
        // TODO: restore _promoShownCount
        //StateVersion = version;
        PromoStateV1 promoState = state as PromoStateV1;
    }

    private void OnPurchase(PurchaseEvent evt)
    {
        _promoShownCount++;
        Console.WriteLine($"[Promo] Applied for user={evt.UserId}, amount={evt.Amount}, shownCount={_promoShownCount}");
    }

    private sealed class PromoStateV1
    {
        public PromoStateV1(int shownCount)
        {
            ShownCount = shownCount;
        }

        public int ShownCount { get; }
    }
}
