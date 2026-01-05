using System;


namespace RobotDomain
{
    using FSMCore;

    // Entity: Đối tượng ngữ cảnh
    public class CleaningRobot
    {
        public float BatteryLevel { get; set; } = 100f;
        public float DirtDetected { get; set; } = 0f;
        public bool IsCharging { get; set; } = false;

        public void Log(string message)
        {
            Console.WriteLine($"[Robot - Battery: {BatteryLevel:F1}%]: {message}");
        }
    }

    // --- Conditions ---

    // TODO 4: Viết Condition kiểm tra pin yếu (LowBatteryCondition)
    // Điều kiện: BatteryLevel < 20
    public class LowBatteryCondition : ICondition<CleaningRobot>
    {
        public bool Check(CleaningRobot context)
        {
            if(context.BatteryLevel < 20)
            {
                return true;
            }
            return false;
        }
    }

    // Điều kiện: Pin đầy
    public class BatteryFullCondition : ICondition<CleaningRobot>
    {
        public bool Check(CleaningRobot context) => context.BatteryLevel >= 95f;
    }

    // --- States ---

    public class IdleState : State<CleaningRobot>
    {
        public IdleState() : base("Idle") { }

        public override void Enter(CleaningRobot context) => context.Log("Entering IDLE mode.");
        
        public override void Update(CleaningRobot context, float deltaTime)
        {
            // Giả lập tiêu hao pin chậm khi đứng yên
            context.BatteryLevel -= 1 * deltaTime;
        }
    }

    // TODO 5: Hoàn thiện trạng thái WorkingState
    // Yêu cầu:
    // - Update: Giảm DirtDetected (dọn dẹp), Giảm BatteryLevel nhanh hơn Idle.
    // - In ra log khi đang dọn dẹp.
    public class WorkingState : State<CleaningRobot>
    {
        public WorkingState() : base("Working") { }

        public override void Enter(CleaningRobot context) => context.Log("START CLEANING.");

        public override void Update(CleaningRobot context, float deltaTime)
        {
            context.BatteryLevel -= 2 * deltaTime;
            context.DirtDetected -= 1 * deltaTime;
            context.Log($"Pin: {context.BatteryLevel}, Do ban: {context.DirtDetected}");
        }

        public override void Exit(CleaningRobot context) => context.Log("STOP CLEANING.");
    }

    public class ChargingState : State<CleaningRobot>
    {
        public ChargingState() : base("Charging") { }

        public override void Enter(CleaningRobot context)
        {
            context.IsCharging = true;
            context.Log("Going to charging dock...");
        }

        public override void Update(CleaningRobot context, float deltaTime)
        {
            context.BatteryLevel += 10 * deltaTime; // Sạc nhanh
            if (context.BatteryLevel > 100) context.BatteryLevel = 100;
        }

        public override void Exit(CleaningRobot context)
        {
            context.IsCharging = false;
            context.Log("Unplugged from dock.");
        }
    }
}