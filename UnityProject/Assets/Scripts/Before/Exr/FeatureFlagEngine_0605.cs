// ============================================================================
// BÀI TẬP HỆ THỐNG (6.5/10): Feature Flag Engine + Contextual Override
//
// Hệ thống này dùng để làm gì?
// - Feature Flag là cơ chế bật/tắt tính năng mà không cần sửa code ở nhiều nơi.
// - Trong thực tế, game/app thường có nhiều nguồn cấu hình:
//   1) Default (giá trị mặc định trong code)
//   2) Remote (từ server / Firebase Remote Config / dashboard)
//   3) Runtime override (ép tại runtime để debug, QA, hoặc theo một flow đặc biệt)
// - Ngoài ra, flag có thể phụ thuộc "ngữ cảnh" (context) như:
//   - Country / Region / User segment / App version / Platform / AB test bucket ...
//
// Mục tiêu bài tập:
// - Xây dựng engine resolve flag value dựa trên Context + nhiều layer override
// - Có cache hợp lý và cơ chế invalidate cache
// - Có debug trace để biết flag được resolve từ layer nào và vì sao
//
// Yêu cầu kỹ thuật gợi ý (không bắt buộc):
// - OOP + Generic
// - Delegate / Func / Predicate
// - LINQ (nếu muốn)
// - Clean architecture nhỏ: tách resolver, source, cache, trace
//
// HƯỚNG DẪN LÀM:
// - Tìm các TODO và hoàn thiện
// - Chạy Program.Main để xem output kỳ vọng
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

public static class Program3
{
    public static void Main()
    {
        // -------------------------
        // 1) Tạo context (ngữ cảnh)
        // -------------------------
        UserContext ctxVN = new(
            userId: "u_001",
            country: "VN",
            platform: Platform.Android,
            appVersion: new SemVersion(1, 2, 0),
            segment: "new",
            abBucket: 12);

        UserContext ctxBR = new(
            userId: "u_002",
            country: "BR",
            platform: Platform.iOS,
            appVersion: new SemVersion(1, 1, 5),
            segment: "payer",
            abBucket: 77);

        // -------------------------
        // 2) Khai báo default flags
        // -------------------------
        // Default = giá trị fallback khi không có remote/override nào match.
        Dictionary<FlagKey, object> defaults = new()
        {
            { FlagKey.NewStoreUI, false },
            { FlagKey.ShowConsentFlow, false },
            { FlagKey.AdsRefreshSeconds, 45 },
            { FlagKey.PriceMultiplier, 1.0f },
            { FlagKey.WelcomePackVariant, "A" },
        };

        // -------------------------
        // 3) Remote rules (giả lập)
        // -------------------------
        // Remote thường là list rule: nếu match context -> set value.
        // Ví dụ: BR bắt show consent, payer có multiplier, VN Android bật store UI...
        List<RemoteRule> remoteRules = new()
        {
            // BR: show consent
            new RemoteRule(
                key: FlagKey.ShowConsentFlow,
                value: true,
                predicate: c => c.Country == "BR",
                priority: 100,
                note: "BR always show consent"),

            // VN + Android: bật store UI mới
            new RemoteRule(
                key: FlagKey.NewStoreUI,
                value: true,
                predicate: c => c.Country == "VN" && c.Platform == Platform.Android,
                priority: 50,
                note: "VN Android rollout"),

            // Payer: giảm refresh ads xuống 30s
            new RemoteRule(
                key: FlagKey.AdsRefreshSeconds,
                value: 30,
                predicate: c => c.Segment == "payer",
                priority: 60,
                note: "Payer faster refresh"),

            // AppVersion >= 1.2.0: welcome pack variant B
            new RemoteRule(
                key: FlagKey.WelcomePackVariant,
                value: "B",
                predicate: c => c.AppVersion >= new SemVersion(1, 2, 0),
                priority: 40,
                note: "Newer version uses variant B"),
        };

        // -------------------------
        // 4) Runtime override (giả lập)
        // -------------------------
        // Runtime override ưu tiên cao nhất, dùng để debug/QA.
        // Có thể đặt theo:
        // - Global override (mọi user)
        // - Override theo userId
        // - Override theo predicate giống remote
        RuntimeOverrideStore runtimeOverrides = new();
        runtimeOverrides.SetGlobal(FlagKey.PriceMultiplier, 1.2f, "QA: boost price globally");
        runtimeOverrides.SetForUser("u_001", FlagKey.AdsRefreshSeconds, 10, "Debug: force refresh 10s for u_001");

        // -------------------------
        // 5) Tạo resolver + cache
        // -------------------------
        // TODO: Học viên sẽ hoàn thiện resolver/cache/trace bên dưới.
        IFlagSource defaultSource = new DefaultFlagSource(defaults);
        IFlagSource remoteSource = new RemoteFlagSource(remoteRules);
        IFlagSource runtimeSource = new RuntimeFlagSource(runtimeOverrides);

        FlagResolver resolver = new(
            sourcesInPriorityOrder: new IFlagSource[]
            {
                runtimeSource, // Highest priority
                remoteSource,
                defaultSource  // Lowest priority
            },
            cache: new FlagCache());

        // -------------------------
        // 6) TEST: resolve flags
        // -------------------------
        Console.WriteLine("===== Resolve for VN user =====");
        PrintAllFlags(resolver, ctxVN);

        Console.WriteLine();
        Console.WriteLine("===== Resolve for BR user =====");
        PrintAllFlags(resolver, ctxBR);

        Console.WriteLine();
        Console.WriteLine("===== Cache demo (should hit cache) =====");
        _ = resolver.Get<bool>(FlagKey.NewStoreUI, ctxVN, out _);
        _ = resolver.Get<bool>(FlagKey.NewStoreUI, ctxVN, out _);

        Console.WriteLine();
        Console.WriteLine("===== Invalidate cache demo =====");
        // Giả lập remote thay đổi: thêm rule bật NewStoreUI cho BR iOS
        remoteRules.Add(new RemoteRule(
            key: FlagKey.NewStoreUI,
            value: true,
            predicate: c => c.Country == "BR" && c.Platform == Platform.iOS,
            priority: 55,
            note: "BR iOS rollout"));

        // TODO: invalidate cache để lần sau resolve lấy rule mới
        resolver.InvalidateAll();

        Console.WriteLine("Re-resolve BR after remote change:");
        bool newStoreBr = resolver.Get<bool>(FlagKey.NewStoreUI, ctxBR, out FlagTrace traceBr);
        Console.WriteLine($"NewStoreUI = {newStoreBr} | trace: {traceBr}");

        // -------------------------
        // 7) Output kỳ vọng (mang tính minh họa)
        // -------------------------
        // VN user:
        // - NewStoreUI: true (remote VN Android)
        // - ShowConsentFlow: false (default)
        // - AdsRefreshSeconds: 10 (runtime override for user u_001)
        // - PriceMultiplier: 1.2 (runtime global)
        // - WelcomePackVariant: B (remote by version >= 1.2.0)
        //
        // BR user:
        // - NewStoreUI: false (default) -> sau invalidate + rule mới thì true
        // - ShowConsentFlow: true (remote BR)
        // - AdsRefreshSeconds: 30 (remote payer)
        // - PriceMultiplier: 1.2 (runtime global)
        // - WelcomePackVariant: A (default vì version 1.1.5 < 1.2.0)
    }

    private static void PrintAllFlags(FlagResolver resolver, UserContext ctx)
    {
        Print<object>(resolver, FlagKey.NewStoreUI, ctx);
        Print<object>(resolver, FlagKey.ShowConsentFlow, ctx);
        Print<object>(resolver, FlagKey.AdsRefreshSeconds, ctx);
        Print<object>(resolver, FlagKey.PriceMultiplier, ctx);
        Print<object>(resolver, FlagKey.WelcomePackVariant, ctx);
    }

    private static void Print<T>(FlagResolver resolver, FlagKey key, UserContext ctx)
    {
        T value = resolver.Get<T>(key, ctx, out FlagTrace trace);
        Console.WriteLine($"{key} = {value} | trace: {trace}");
    }
}

// ============================================================================
// DOMAIN
// ============================================================================

public enum Platform
{
    Android,
    iOS,
    Windows,
    Mac,
}

public enum FlagKey
{
    NewStoreUI,
    ShowConsentFlow,
    AdsRefreshSeconds,
    PriceMultiplier,
    WelcomePackVariant,
}

public sealed class UserContext
{
    public UserContext(
        string userId,
        string country,
        Platform platform,
        SemVersion appVersion,
        string segment,
        int abBucket)
    {
        UserId = userId;
        Country = country;
        Platform = platform;
        AppVersion = appVersion;
        Segment = segment;
        AbBucket = abBucket;
    }

    public string UserId { get; }
    public string Country { get; }
    public Platform Platform { get; }
    public SemVersion AppVersion { get; }
    public string Segment { get; }
    public int AbBucket { get; }
}

// Simple semantic version (major.minor.patch)
public readonly struct SemVersion : IComparable<SemVersion>
{
    public SemVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }

    public int CompareTo(SemVersion other)
    {
        // TODO: implement comparison by Major -> Minor -> Patch
        // Return -1 if this < other, 0 if equal, 1 if this > other
        //Can upgrade
        if(this.Major == other.Major)
        {
            if(this.Minor == other.Minor)
            {
                if (this.Patch == other.Patch)
                {
                    return 0;
                }
                else if (this.Patch > other.Patch)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (this.Minor > other.Minor)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if(this.Major > other.Major)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static bool operator >=(SemVersion a, SemVersion b) => a.CompareTo(b) >= 0;
    public static bool operator <=(SemVersion a, SemVersion b) => a.CompareTo(b) <= 0;
    public static bool operator >(SemVersion a, SemVersion b) => a.CompareTo(b) > 0;
    public static bool operator <(SemVersion a, SemVersion b) => a.CompareTo(b) < 0;

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}

// ============================================================================
// TRACE
// ============================================================================

public enum FlagLayer
{
    RuntimeOverride,
    Remote,
    Default,
    Cache,
    Missing,
}

public readonly struct FlagTrace
{
    public FlagTrace(FlagLayer layer, string note)
    {
        Layer = layer;
        Note = note;
    }

    public FlagLayer Layer { get; }
    public string Note { get; }

    public override string ToString()
    {
        // TODO: format "Layer=Remote, Note=VN Android rollout"
        return $"Layer={Layer}, Note={Note}";
    }
}

// ============================================================================
// SOURCES
// ============================================================================

public interface IFlagSource
{
    // Trả về true nếu source có giá trị cho key+context, kèm trace note.
    bool TryGetValue(FlagKey key, UserContext context, out object value, out string note);
}

public sealed class DefaultFlagSource : IFlagSource
{
    private readonly Dictionary<FlagKey, object> _defaults;

    public DefaultFlagSource(Dictionary<FlagKey, object> defaults)
    {
        _defaults = defaults;
    }

    public bool TryGetValue(FlagKey key, UserContext context, out object value, out string note)
    {
        // TODO: lấy value từ _defaults. Nếu có -> note="default"
        // Nếu không có -> value=null, note="not found", return false
        if (_defaults.ContainsKey(key))
        {
            value = _defaults[key];
            note = "default";
            return true;
        }
        else
        {
            value = null;
            note = "not found!!";
            return false;
        }
    }
}

public sealed class RemoteRule
{
    public RemoteRule(FlagKey key, object value, Func<UserContext, bool> predicate, int priority, string note)
    {
        Key = key;
        Value = value;
        Predicate = predicate;
        Priority = priority;
        Note = note;
    }

    public FlagKey Key { get; }
    public object Value { get; }
    public Func<UserContext, bool> Predicate { get; }
    public int Priority { get; }
    public string Note { get; }
}

public sealed class RemoteFlagSource : IFlagSource
{
    private readonly List<RemoteRule> _rules;

    public RemoteFlagSource(List<RemoteRule> rules)
    {
        _rules = rules;
    }

    public bool TryGetValue(FlagKey key, UserContext context, out object value, out string note)
    {
        // TODO:
        // - Lọc rule theo key
        // - Filter rule match predicate(context)
        // - Chọn rule có Priority cao nhất (nếu bằng nhau, bạn tự quyết định tie-break)
        // - Trả về value = rule.Value, note = rule.Note
        var listOK = _rules.Where(x => x.Key == key && x.Predicate(context)).OrderBy(x => x.Priority).ToList();
        if(listOK.Count > 0)
        {
            value = listOK[0].Value;
            note = listOK[0].Note;
            return true;
        }
        value = null;
        note = "Not Found!!";
        return false;
    }
}

// ============================================================================
// RUNTIME OVERRIDES
// ============================================================================

public sealed class RuntimeOverrideStore
{
    // Global override: key -> value
    private readonly Dictionary<FlagKey, OverrideEntry> _global = new();

    // Per-user override: userId -> (key -> value)
    private readonly Dictionary<string, Dictionary<FlagKey, OverrideEntry>> _perUser = new();

    public void SetGlobal(FlagKey key, object value, string note)
    {
        _global[key] = new OverrideEntry(value, note);
    }

    public void SetForUser(string userId, FlagKey key, object value, string note)
    {
        if (!_perUser.TryGetValue(userId, out Dictionary<FlagKey, OverrideEntry> map))
        {
            map = new Dictionary<FlagKey, OverrideEntry>();
            _perUser[userId] = map;
        }

        map[key] = new OverrideEntry(value, note);
    }

    public bool TryGet(FlagKey key, UserContext ctx, out object value, out string note)
    {
        // TODO:
        // - Ưu tiên per-user override trước global
        // - Nếu không có -> return false
        if (_perUser.ContainsKey(ctx.UserId))
        {
            value = _perUser[ctx.UserId][key].Value;
            note = _perUser[ctx.UserId][key].Note;
            return true;
        }
        else
        {
            value = null;
            note = "Not Found!!";
            return false;
        }
    }

    private readonly struct OverrideEntry
    {
        public OverrideEntry(object value, string note)
        {
            Value = value;
            Note = note;
        }

        public object Value { get; }
        public string Note { get; }
    }
}

public sealed class RuntimeFlagSource : IFlagSource
{
    private readonly RuntimeOverrideStore _store;

    public RuntimeFlagSource(RuntimeOverrideStore store)
    {
        _store = store;
    }

    public bool TryGetValue(FlagKey key, UserContext context, out object value, out string note)
    {
        return _store.TryGet(key, context, out value, out note);
    }
}

// ============================================================================
// CACHE
// ============================================================================

public interface IFlagCache
{
    bool TryGet(FlagKey key, UserContext ctx, out object value, out FlagTrace trace);
    void Set(FlagKey key, UserContext ctx, object value, FlagTrace trace);
    void InvalidateAll();
}

public sealed class FlagCache : IFlagCache
{
    // TODO: thiết kế cache key hợp lý.
    // Gợi ý: cache key = (FlagKey, ContextKey)
    // ContextKey có thể là string: "country|platform|version|segment|bucket|userId"
    // hoặc struct để tránh string allocations.

    private readonly Dictionary<string, CacheEntry> _cache = new();

    public bool TryGet(FlagKey key, UserContext ctx, out object value, out FlagTrace trace)
    {
        // TODO:
        // - Tạo cacheKey
        // - Nếu hit -> trace.Layer = Cache nhưng vẫn giữ note gốc (hoặc note "cached: ...")
        string cacheKey = $"({key}, {ctx.Country}|{ctx.Platform}|{ctx.AppVersion}|{ctx.Segment}|{ctx.AbBucket}|{ctx.UserId})";
        if (_cache.ContainsKey(cacheKey))
        {
            value = _cache[cacheKey].Value;
            trace = _cache[cacheKey].Trace;
            return true;
        }
        value = null;
        trace = new FlagTrace();
        return false;
    }

    public void Set(FlagKey key, UserContext ctx, object value, FlagTrace trace)
    {
        // TODO: set cache
        string cacheKey = $"({key}, {ctx.Country}|{ctx.Platform}|{ctx.AppVersion}|{ctx.Segment}|{ctx.AbBucket}|{ctx.UserId})";
        _cache.Add(cacheKey, new CacheEntry(value, trace));
    }

    public void InvalidateAll()
    {
        _cache.Clear();
    }

    private readonly struct CacheEntry
    {
        public CacheEntry(object value, FlagTrace trace)
        {
            Value = value;
            Trace = trace;
        }

        public object Value { get; }
        public FlagTrace Trace { get; }
    }
}

// ============================================================================
// RESOLVER
// ============================================================================

public sealed class FlagResolver
{
    private readonly IFlagSource[] _sources;
    private readonly IFlagCache _cache;

    public FlagResolver(IFlagSource[] sourcesInPriorityOrder, IFlagCache cache)
    {
        _sources = sourcesInPriorityOrder;
        _cache = cache;
    }

    public T Get<T>(FlagKey key, UserContext ctx, out FlagTrace trace)
    {
        // 1) Cache
        if (_cache.TryGet(key, ctx, out object cachedValue, out FlagTrace cachedTrace))
        {
            trace = cachedTrace;
            return CastOrThrow<T>(cachedValue, key, trace);
        }

        // 2) Resolve by sources (runtime -> remote -> default)
        foreach (IFlagSource source in _sources)
        {
            if (!source.TryGetValue(key, ctx, out object value, out string note))
                continue;

            // TODO: quyết định layer dựa trên source nào.
            // Gợi ý: có thể wrap source với metadata layer, hoặc so sánh type.
            FlagTrace resolvedTrace = CreateTraceFromSource(source, note);

            _cache.Set(key, ctx, value, resolvedTrace);

            trace = resolvedTrace;
            return CastOrThrow<T>(value, key, trace);
        }

        // 3) Missing
        trace = new FlagTrace(FlagLayer.Missing, "No source returned a value");
        throw new InvalidOperationException($"Missing flag: {key}");
    }

    public void InvalidateAll()
    {
        _cache.InvalidateAll();
    }

    private static FlagTrace CreateTraceFromSource(IFlagSource source, string note)
    {
        // TODO:
        // - Nếu source là RuntimeFlagSource -> RuntimeOverride
        // - Nếu source là RemoteFlagSource  -> Remote
        // - Nếu source là DefaultFlagSource -> Default
        // - Nếu unknown -> Missing/Default (tự chọn)
        if(source is RuntimeFlagSource)
        {
            return new FlagTrace(FlagLayer.RuntimeOverride, note);
        }
        else if(source is RemoteFlagSource)
        {
            return new FlagTrace(FlagLayer.Remote, note);
        }
        else if (source is DefaultFlagSource)
        {
            return new FlagTrace(FlagLayer.Default, note);
        }
        else
        {
            return new FlagTrace(FlagLayer.Missing, note);
        }
    }

    private static T CastOrThrow<T>(object value, FlagKey key, FlagTrace trace)
    {
        // TODO:
        // - Nếu value không cast được sang T -> throw với message rõ ràng
        //   ví dụ: "Flag AdsRefreshSeconds expected Int32 but got Single"
        if(value is T _Tvalue)
        {
            return _Tvalue;
        }
        else
        {
            throw new Exception($"Flag {key} expected {typeof(T)} but got {typeof(object)}");
        }
    }
}
