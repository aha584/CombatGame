using System;
using System.Collections.Generic;
using System.Linq;

// =============================================================
//                      BÀI 1 – GENERIC FILTER
// =============================================================
namespace Exercises.Bai1
{
    public static class GenericFilter
    {
        // TODO: Implement this method
        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            // TODO: Validate arguments

            // TODO: yield return items matching predicate
            foreach(var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
    }

    // Sample model
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }

        // TODO: override ToString()
        public Student(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    public class TestBai1
    {
        public static void Run()
        {
            // TODO: Create list
            // TODO: Use GenericFilter.Filter with lambda
            List<Student> students = new List<Student> { new Student("An", 19), new Student("Binh", 75), new Student("Chi", 35), new Student("Dung", 10) };
            List<Student> afterFilter = GenericFilter.Filter(students, student => student.Age > 30).ToList();
            foreach (Student stu in afterFilter)
            {
                Console.WriteLine($"Name: {stu.Name}; Age: {stu.Age}");
            }
        }
    }
}

// =============================================================
//                 BÀI 2 – GENERIC SORTER + KEY SELECTOR
// =============================================================
namespace Exercises.Bai2
{
    public static class GenericSorter
    {
        // TODO: Sort using keySelector and ascending/descending logic
        public static IEnumerable<T> SortBy<T, TKey>(
            IEnumerable<T> source,
            Func<T, TKey> keySelector,
            bool ascending = true)
        {
            // TODO: Validate inputs
            if (source == null) yield return default(T);
            // TODO: Copy to List<T>
            List<T> temp = source.ToList();
            // TODO: Use Comparison<T> and keySelector
            Comparison<T> comparer = (one, two) =>
            {
                TKey oneKey = keySelector(one);
                TKey twoKey = keySelector(two);

                int result = Comparer<TKey>.Default.Compare(oneKey, twoKey);

                return ascending ? result : -result;
            };
            temp.Sort(comparer);
            foreach(var item in temp)
            {
                yield return item;
            }
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; }

        // TODO: override ToString()
        public Product(string name, float price, int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }
    }

    public class TestBai2
    {
        public static void Run()
        {
            List<Product> myPro = new List<Product>
            {
                new Product("banh", 5.7f,56),
                new Product("my", 3.6f,46),
                new Product("tui", 1.0f,90),
                new Product("xe", 890f,56),
            };

            // TODO: Create product list
            // TODO: Sort by price ascending
            List<Product> ascePro = GenericSorter.SortBy(myPro, x => x.Price).ToList();
            // TODO: Sort by stock descending
            List<Product> descePro = GenericSorter.SortBy(myPro, x => x.Stock,false).ToList();
        }
    }
}

// =============================================================
//              BÀI 3 – GENERIC REPOSITORY + LAMBDA FIND
// =============================================================
namespace Exercises.Bai3
{
    public interface IRepository<T>
    {
        void Add(T item);
        bool Remove(T item);
        IEnumerable<T> Find(Func<T, bool> predicate);
        IEnumerable<T> GetAll();
    }

    public class InMemoryRepository<T> : IRepository<T>
    {
        private readonly List<T> _items;

        public InMemoryRepository()
        {
            // TODO: Initialize list
            _items = new List<T>();
        }

        public void Add(T item)
        {
            // TODO: Implement Add
            _items.Add(item);
        }

        public bool Remove(T item)
        {
            // TODO: Implement Remove
            if (_items.Remove(item)) return true;
            else return false;
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            // TODO: Validate predicate
            if (predicate == null) yield return default(T);
            // TODO: yield return or use LINQ
            else
            {
                foreach(var item in _items)
                {
                    if (predicate(item)) yield return item;
                }
            }
        }

        public IEnumerable<T> GetAll()
        {
            // TODO: Should return copy or read-only
            List<T> copyOfOld = new List<T>();
            foreach(var item in _items)
            {
                copyOfOld.Add(item);
            }
            foreach(var copyItem in copyOfOld)
            {
                yield return copyItem;
            }
            
        }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        // TODO: override ToString()
        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
        }
    }

    public class TestBai3
    {
        public static void Run()
        {
            // TODO: Create repo
            InMemoryRepository<Book> newShell = new();

            // TODO: Add books
            newShell.Add(new Book("The Art", "Bother", 1988));
            newShell.Add(new Book("The Book", "Ether", 1908));
            newShell.Add(new Book("The Child", "Hunger", 1868));
            newShell.Add(new Book("The Angle", "Arte", 1398));
            newShell.Add(new Book("The God", "Hermes", 1768));
            // TODO: Query with lambda (author, year)
            List<Book> copyShell = newShell.Find(book =>  book.Author.Contains("o") && book.Year > 1800).ToList();
        }
    }
}

// =============================================================
//            BÀI 4 – GENERIC PIPELINE PROCESSOR (ADVANCED)
// =============================================================
namespace Exercises.Bai4
{
    public class Pipeline<T>
    {
        private readonly List<Func<T, T>> _steps;

        public Pipeline()
        {
            // TODO: Initialize
            _steps = new();
        }

        public Pipeline<T> AddStep(Func<T, T> step)
        {
            // TODO: Validate step
            if (step == null) return null;
            // TODO: Add to list
            _steps.Add(step);
            // TODO: return this
            return this;
        }

        public T Execute(T input)
        {
            // TODO: Apply all steps sequentially
            foreach (var step in _steps)
            {
                input = step(input);
            }
            // input = step(input);
            return input;
        }
    }

    public class Order
    {
        public string CustomerName { get; set; }
        public float Amount { get; set; }
        public bool IsVip { get; set; }

        // TODO: override ToString()

        public Order(string customerName, float amount, bool isVip)
        {
            CustomerName = customerName;
            Amount = amount;
            IsVip = isVip;
        }
    }

    public class TestBai4
    {
        public static void Run()
        {
            // TODO: Pipeline<int>
            // TODO: Pipeline<Order> with business rules
        }
    }
}

// =============================================================
//     BÀI 5 – STATIC EVENT BUS (GENERIC EVENT DISPATCHER)
// =============================================================
namespace Exercises.Bai5
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _handlers
            = new();

        public static void Subscribe<TEvent>(Action<TEvent> handler)
        {
            // TODO: Validate handler
            if (handler == null) return;
            // TODO: Add to dictionary
            else
            {
                _handlers[typeof(TEvent)].Add(handler);
            }
        }

        public static void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            // TODO: Remove handler
            _handlers[typeof(TEvent)].Remove(handler);
        }

        public static void Publish<TEvent>(TEvent evt)
        {
            // TODO: Get list, cast delegate → Action<TEvent>, invoke
            List<Delegate> listAction = _handlers[typeof(TEvent)];
            foreach(var action in listAction)
            {
                action?.DynamicInvoke(evt);
            }
        }
    }

    // Sample events
    public class PlayerScoredEvent
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }

        public PlayerScoredEvent(string playerName, int score)
        {
            PlayerName = playerName;
            Score = score;
        }
    }

    public class LevelCompletedEvent
    {
        public int LevelIndex { get; set; }
        public float Time { get; set; }

        public LevelCompletedEvent(int levelIndex, float time)
        {
            LevelIndex = levelIndex;
            Time = time;
        }
    }

    public class TestBai5
    {
        public static void Run()
        {
            // TODO: Subscribe with lambda
            // TODO: Publish events
            // TODO: Unsubscribe
        }
    }
}

// =============================================================
//        BÀI 6 – GENERIC VALIDATOR<T> VỚI RULE + MESSAGE
// =============================================================
namespace Exercises.Bai6
{
    // Rule: chứa điều kiện và message lỗi
    public class ValidationRule<T>
    {
        public Func<T, bool> Predicate { get; }
        public string ErrorMessage { get; }

        public ValidationRule(Func<T, bool> predicate, string errorMessage)
        {
            // TODO: Validate inputs and assign fields
            if(!(predicate == null || string.IsNullOrEmpty(errorMessage)))
            {
                Predicate = predicate;
                ErrorMessage = errorMessage;
            }
        }
    }

    public class Validator<T>
    {
        private readonly List<ValidationRule<T>> _rules;

        public Validator()
        {
            // TODO: Initialize list
            _rules = new List<ValidationRule<T>>();
        }

        public Validator<T> AddRule(Func<T, bool> predicate, string errorMessage)
        {
            // TODO: Create ValidationRule<T> and add to list
            // TODO: return this (fluent)
            ValidationRule<T> rule = new(predicate, errorMessage);
            _rules.Add(rule);
            return this;
        }

        public List<string> Validate(T target)
        {
            // TODO: Run all rules on target
            // TODO: Collect error messages where Predicate returns false
            List<string> errorContainer = new();
            foreach(var rule in _rules)
            {
                if (!rule.Predicate(target))
                    errorContainer.Add(rule.ErrorMessage);
            }
            return errorContainer;
        }
    }

    public class UserRegistration
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }

        // TODO: override ToString()

        public UserRegistration(string username, string email, int age)
        {
            Username = username;
            Email = email;
            Age = age;
        }
    }

    public class TestBai6
    {
        public static void Run()
        {
            // TODO:
            // - Tạo Validator<UserRegistration>
            // - Thêm rule: Username không rỗng, Age >= 18, Email chứa '@'
            // - Gọi Validate với nhiều dữ liệu, in ra lỗi
            Validator<UserRegistration> userRegis = new();
            userRegis.AddRule(user => user.Username != null, "UserName Is Null");
            userRegis.AddRule(user => user.Age >= 18, "Age is not over 18");
            userRegis.AddRule(user => user.Email.Contains("@"), "Email is not contain @");
        }
    }
}

// =============================================================
//      BÀI 7 – GENERIC COMMAND PROCESSOR<TContext> (PATTERN)
// =============================================================
namespace Exercises.Bai7
{
    // Command interface dùng generic context
    public interface ICommand<TContext>
    {
        void Execute(TContext context);
    }

    // Implementation dùng lambda thay vì class con
    public class LambdaCommand<TContext> : ICommand<TContext>
    {
        private readonly Action<TContext> _action;

        public LambdaCommand(Action<TContext> action)
        {
            // TODO: Validate action
            if (action == null) return;
            else
            {
                _action = action;
            }
        }

        public void Execute(TContext context)
        {
            // TODO: Invoke action
            _action?.Invoke(context);
        }
    }

    public class CommandProcessor<TContext>
    {
        private readonly List<ICommand<TContext>> _commands;

        public CommandProcessor()
        {
            // TODO: Initialize list
            _commands = new();
        }

        public CommandProcessor<TContext> AddCommand(ICommand<TContext> command)
        {
            // TODO: Add command, return this
            _commands.Add(command);
            return this;
        }

        public CommandProcessor<TContext> AddCommand(Action<TContext> action)
        {
            // TODO: Wrap action vào LambdaCommand<TContext>, reuse AddCommand
            LambdaCommand<TContext> newLambda = new LambdaCommand<TContext>(action);
            this.AddCommand(newLambda);
            return this;
        }

        public void ExecuteAll(TContext context)
        {
            // TODO: Loop and Execute tất cả command
            foreach(var command in _commands)
            {
                command.Execute(context);
            }
        }
    }

    // Sample context
    public class GameContext
    {
        public string CurrentScene { get; set; }
        public int PlayerHealth { get; set; }
        public int Score { get; set; }

        public GameContext(string currentScene, int playerHealth, int score)
        {
            CurrentScene = currentScene;
            PlayerHealth = playerHealth;
            Score = score;
        }
        public void Log()
        {
            Console.WriteLine($"Current Scene: {CurrentScene}, Health: {PlayerHealth}, Score: {Score}");
        }
    }

    public class TestBai7
    {
        public static void Run()
        {
            // TODO:
            // - Tạo CommandProcessor<GameContext>
            // - Thêm vài command bằng lambda, ví dụ:
            //   + giảm máu
            //   + tăng điểm
            //   + chuyển scene
            // - Gọi ExecuteAll và in kết quả context

            CommandProcessor<GameContext> gameCommands = new();
            gameCommands.AddCommand(player => player.PlayerHealth -= 10);
            gameCommands.AddCommand(player => player.Score += 1000);
            gameCommands.AddCommand(player => player.CurrentScene = "Floor 10");

            GameContext player = new GameContext("Floor 9", 20, 0);
            gameCommands.ExecuteAll(player);
            player.Log();
        }
    }
}

// =============================================================
//        BÀI 8 – GENERIC TREE<T> + TRAVERSAL BẰNG LAMBDA
// =============================================================
namespace Exercises.Bai8
{
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public List<TreeNode<T>> Children { get; }

        public TreeNode(T value)  
        {
            // TODO: Gán Value, khởi tạo Children
            Value = value;
            Children = new();
        }

        public TreeNode<T> AddChild(T value)
        {
            // TODO: Tạo node con, thêm vào Children, trả về node mới
            TreeNode<T> newNode = new(value);
            Children.Add(newNode);
            return newNode;
        }
    }

    public static class TreeTraversal
    {
        // Duyệt sâu (DFS) với hành động là lambda
        public static void DepthFirst<T>(
            TreeNode<T> root,
            Action<T> visit)
        {
            // TODO: Validate root + visit
            if(root == null || visit == null) { return; }
            // TODO: DFS: visit(root.Value), sau đó loop Children
            else
            {
                visit(root.Value);
                foreach(var child in root.Children)
                {
                    DepthFirst(child,visit);//Log
                }
            }
        }

        // Tìm node đầu tiên match predicate (DFS)
        public static TreeNode<T> FindFirst<T>(
            TreeNode<T> root,
            Func<T, bool> predicate)
        {
            // TODO: Validate inputs
            // TODO: DFS, trả về node đầu tiên thỏa predicate, hoặc null
            if (root == null || predicate == null) return null;
            else
            {
                TreeNode<T> childNode = new(root.Value);
                if (predicate(root.Value))
                {
                    childNode = root;
                    return childNode;
                }
                else
                {
                    foreach (var child in root.Children)
                    {
                        childNode =  FindFirst(child, predicate);
                    }
                    return childNode;
                }
            }
        }
    }

    public class TestBai8
    {
        public static void Run()
        {
            // TODO:
            // - Tạo cây menu (string)
            // - Duyệt DepthFirst in ra
            // - FindFirst với predicate (ví dụ: node có ký tự 'A')
        }
    }
}

// =============================================================
//   BÀI 9 – GENERIC GROUPING UTILITY (TỰ CÀI GROUPBY CƠ BẢN)
// =============================================================
namespace Exercises.Bai9
{
    public static class GroupingUtility
    {
        // Tự cài đặt GroupBy cơ bản
        public static Dictionary<TKey, List<TSource>> GroupByManual<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            // TODO: Validate arguments
            if(source == null || keySelector == null)
            {
                return null;
            }
            // TODO: Tạo Dictionary<TKey, List<TSource>>
            // TODO: Loop source, tính key, Add vào group tương ứng

            Dictionary<TKey, List<TSource>> myDict = new();
            foreach(var item in source)
            {
                myDict[keySelector(item)].Add(item);
            }

            return myDict;
        }
    }

    public class SaleRecord
    {
        public string SellerName { get; set; }
        public string Region { get; set; }
        public float Amount { get; set; }

        public SaleRecord(string sellerName, string region, float amout)
        {
            SellerName = sellerName;
            Region = region;
            Amount = amout;
        }
    }

    public class TestBai9
    {
        public static void Run()
        {
            // TODO:
            // - Tạo list SaleRecord
            // - GroupByManual theo Region
            // - In từng Region + tổng Amount dùng LINQ hoặc loop
        }
    }
}

// =============================================================
//       BÀI 10 – SERVICE FACTORY & CACHE (GENERIC SINGLETON)
// =============================================================
namespace Exercises.Bai10
{
    // Factory generic tạo instance dùng lambda
    public static class ServiceFactory
    {
        // Cache: mỗi type T chỉ tạo 1 lần (giống singleton đơn giản)
        private static readonly Dictionary<Type, object> _services
            = new();

        public static T GetOrCreate<T>(Func<T> factory)
        {
            // TODO:
            // - Lấy typeof(T)
            // - Nếu đã có trong dictionary, cast và return
            // - Nếu chưa có, gọi factory(), lưu vào dictionary, return
            // - Validate factory
            if (_services.ContainsKey(typeof(T)))
            {
                return (T)_services[typeof(T)];
            }
            else
            {
                if (factory != null)
                {
                    _services[typeof(T)] = factory();
                    return factory();
                }
                else
                {
                    return default(T);
                }
            }
        }
    }

    // Sample services
    public class AudioService
    {
        // TODO: Thêm một vài method giả lập, ví dụ: PlaySound(string name)
        public void PlaySound(string name)
        {
            Console.WriteLine($"Name Of Sound: {name}");
        }
    }

    public class SaveGameService
    {
        // TODO: Add methods: Save(), Load(), etc.
        public void Save()
        {

        }
        public void Load()
        {

        }
    }

    public class TestBai10
    {
        public static void Run()
        {
            // TODO:
            // - Gọi ServiceFactory.GetOrCreate<AudioService>(() => new AudioService())
            AudioService audi1 = ServiceFactory.GetOrCreate<AudioService>(() => new AudioService());
            // - Gọi lại lần 2, kiểm tra reference giống nhau
            AudioService audi2 =  ServiceFactory.GetOrCreate<AudioService>(() => new AudioService());
            bool isSame =  audi1 == audi2;
            audi1.Equals(audi2);

            // - Làm tương tự với SaveGameService
        }
    }
}

// =============================================================
//                      MASTER RUNNER (OPTIONAL)
// =============================================================
namespace Exercises
{
    public class Program
    {
        public static void Main()
        {
            // Bật từng bài để test:

            Exercises.Bai1.TestBai1.Run();
            // Exercises.Bai2.TestBai2.Run();
            // Exercises.Bai3.TestBai3.Run();
            // Exercises.Bai4.TestBai4.Run();
            // Exercises.Bai5.TestBai5.Run();
            // Exercises.Bai6.TestBai6.Run();
            // Exercises.Bai7.TestBai7.Run();
            // Exercises.Bai8.TestBai8.Run();
            // Exercises.Bai9.TestBai9.Run();
            // Exercises.Bai10.TestBai10.Run();
        }
    }
}
