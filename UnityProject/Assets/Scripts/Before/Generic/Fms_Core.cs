using System;
using System.Collections.Generic;
using System.Linq;

namespace FSMCore
{
    // 1. Interface định nghĩa hành vi của một Điều kiện chuyển đổi
    public interface ICondition<T>
    {
        bool Check(T context);
    }

    // 2. Class đại diện cho việc chuyển đổi trạng thái
    public class Transition<T>
    {
        public int Priority { get; }
        public ICondition<T> Condition { get; private set; }
        public State<T> NextState { get; private set; }

        public Transition(ICondition<T> condition, State<T> nextState, int priority)
        {
            Condition = condition;
            NextState = nextState;
            Priority = priority;
        }
    }

    // 3. Class cơ sở cho mọi Trạng thái (Abstract Class)
    public abstract class State<T>
    {
        public string Name { get; protected set; }
        protected List<Transition<T>> _transitions = new List<Transition<T>>();

        public State(string name)
        {
            Name = name;
        }

        public void AddTransition(ICondition<T> condition, State<T> nextState, int priority)
        {
            _transitions.Add(new Transition<T>(condition, nextState, priority));
        }

        public virtual void Enter(T context) { }
        
        public virtual void Exit(T context) { }

        // TODO 1: Học viên cần định nghĩa logic Update trừu tượng
        public abstract void Update(T context, float deltaTime);

        // TODO 2: Viết hàm kiểm tra xem có Transition nào thỏa mãn không.
        // Nếu có điều kiện thỏa mãn, trả về State tiếp theo. Nếu không, trả về null.
        public State<T> CheckTransitions(T context)
        {
            // Gợi ý: Duyệt qua list _transitions, gọi Condition.Check(context)
            int maxPri = int.MinValue;
            Transition<T> currentTransition = null;
            foreach (var transition in _transitions)
            {
                if (transition.Priority > maxPri)
                {
                    maxPri = transition.Priority;
                    currentTransition = transition;
                }
            }
            if (currentTransition.Condition.Check(context))
            {
                return currentTransition.NextState;
            }
            return null;
        }
    }

    // 4. Class bộ não quản lý FSM (StateMachine)
    public class StateMachine<T>
    {
        private State<T> _currentState;
        private T _context; // Đối tượng chịu tác động (VD: Robot, Character)

        public State<T> CurrentState => _currentState;
        public bool IsRunning { get; private set; }

        public StateMachine(T context, State<T> initialState)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (initialState == null) throw new ArgumentNullException(nameof(initialState));

            _context = context;
            _currentState = initialState;
            IsRunning = true;
            
            // Kích hoạt trạng thái đầu tiên
            _currentState.Enter(_context);
        }

        // TODO 3: Viết logic Update cho StateMachine
        // Nhiệm vụ:
        // 1. Gọi Update của _currentState
        // 2. Kiểm tra Transition từ _currentState.
        // 3. Nếu có State mới -> Gọi Exit state cũ -> Gán state mới -> Gọi Enter state mới.
        public void Update(float deltaTime)
        {
            // Code flow:
            // 1. _currentState.Update(...)
            // 2. var nextState = _currentState.CheckTransitions(...)
            // 3. Perform transition logic if nextState is not null

            _currentState.Update(_context, deltaTime);
            var nextState = _currentState.CheckTransitions(_context);
            if (nextState != null)
            {
                _currentState.Exit(_context);
                _currentState = nextState;
                _currentState.Enter(_context);
            }
        }

        public void Stop()
        {
            if (_currentState != null)
            {
                _currentState.Exit(_context);
            }
            IsRunning = false;
        }
    }
}






public interface IConditions<T>
{
    public bool Check();
}

public interface IState<T>
{
    int Prioprity { get; }
    //void GetPriority() to caculate this state priority by math function
    void Update();
}

public class Transition<T>
{
    public IConditions<T> condi;
    public IState<T> state;
}

public class Brain<T>
{
    public List<Transition<T>> trans;
    IState<T> current;
    public void Update()
    {
        List<Transition<T>> temp = new();
        foreach(var transition in trans)
        {
            if (transition.condi.Check())
            {
                temp.Add(transition);
            }
        }

        if(temp.Count > 0)
        {
            var tran = temp[0];
            foreach(var transition in trans)
            {
                if(tran.state.Prioprity < transition.state.Prioprity)
                {
                    tran = transition;
                }
            }

            current = tran.state;
        }
        current.Update();
    }
}