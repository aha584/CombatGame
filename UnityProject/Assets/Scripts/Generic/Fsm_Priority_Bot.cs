using FSMCore;
using System;

public class Fsm_Priority_Bot
{
    public class CleaningPriorityBot
    {
        public float BatteryLevel { get; set; } = 100f;
        public float DirtLevel { get; set; } = 0f;
        public bool IsCharging { get; set; } = false;
        public void Log(string message)
        {
            Console.WriteLine($"[Robot - Battery: {BatteryLevel:F1}%]: {message}");
        }
    }

    public class IdleStatePB : State<CleaningPriorityBot>
    {
        public IdleStatePB() : base("Idle") { }

        public override void Enter(CleaningPriorityBot context)
        {
            context.Log("Enter Idle mode");
        }
        public override void Update(CleaningPriorityBot context, float deltaTime)
        {
            context.BatteryLevel -= 1*deltaTime;
        }
    }

    public class WorkingStatePB : State<CleaningPriorityBot>
    {
        public WorkingStatePB() : base("Working") { }

        public override void Enter(CleaningPriorityBot context)
        {
            context.Log("Enter Working mode");
        }
        public override void Update(CleaningPriorityBot context, float deltaTime)
        {
            context.BatteryLevel -= 2 * deltaTime;
            context.DirtLevel -= 1 * deltaTime;
            context.Log($"Pin: {context.BatteryLevel}, Do ban: {context.DirtLevel}");
        }
        public override void Exit(CleaningPriorityBot context)
        {
            context.Log("STOP CLEANING.");
        }
    } 

    public class ChargingStatePB : State<CleaningPriorityBot>
    {
        public ChargingStatePB() : base("Charging") { }
        public override void Enter(CleaningPriorityBot context)
        {
            context.IsCharging = true;
            context.Log("Enter Charging mode");
        }
        public override void Update(CleaningPriorityBot context, float deltaTime)
        {
            context.BatteryLevel += 10 * deltaTime;
            if (context.BatteryLevel > 100f) context.BatteryLevel = 100f;
            context.Log($"Pin: {context.BatteryLevel}");
        }
        public override void Exit(CleaningPriorityBot context)
        {
            context.IsCharging = false;
            context.Log("Unplugged charger");
        }
    }

    public class CheckCondition<T> : ICondition<T>
    {
        public Func<T, bool> _predicate;
        public CheckCondition(Func<T, bool> predicate) { _predicate = predicate; }
        public bool Check(T context) => _predicate(context);
    }


}
