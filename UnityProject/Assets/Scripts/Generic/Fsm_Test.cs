using System;
using System.Threading;
using FSMCore;
using RobotDomain;

class Program4
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== FSM SIMULATION: CLEANING ROBOT ===");

        // 1. Setup Context
        var robot = new CleaningRobot();
        robot.DirtDetected = 50; // Sàn đang bẩn

        // 2. Setup States
        var idleState = new IdleState();
        var workingState = new WorkingState();
        var chargingState = new ChargingState();

        // 3. Setup Transitions (Wiring the brain)
        // Idle -> Working: Nếu có bụi (Chúng ta dùng lambda cho nhanh, hoặc tạo class Condition riêng)
        idleState.AddTransition(new FuncCondition<CleaningRobot>(ctx => ctx.DirtDetected > 0 && ctx.BatteryLevel > 20), workingState, 1);
        
        // Working -> Charging: Nếu pin yếu
        workingState.AddTransition(new LowBatteryCondition(), chargingState, 3);

        // Working -> Idle: Nếu hết bụi
        workingState.AddTransition(new FuncCondition<CleaningRobot>(ctx => ctx.DirtDetected <= 0), idleState, 2);

        // Charging -> Idle: Nếu pin đầy
        chargingState.AddTransition(new BatteryFullCondition(), idleState, 1);

        // 4. Initialize Brain
        StateMachine<CleaningRobot> brain = null;
        
        try 
        {
            brain = new StateMachine<CleaningRobot>(robot, idleState);
        }
        catch (NotImplementedException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Setup failed: {ex.Message}");
            Console.ResetColor();
            return;
        }

        // 5. Simulation Loop
        float deltaTime = 1.0f; // Giả lập mỗi loop là 1 giây
        
        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine($"--- Time: {i}s ---");
            
            try 
            {
                brain.Update(deltaTime);
                
                // Hack: Tự động thêm bụi vào giây thứ 15 để test robot có quay lại làm việc không
                if (i == 15) {
                    Console.WriteLine("[EVENT] Someone spilled coffee! (Dirt +30)");
                    robot.DirtDetected += 30;
                }
            }
            catch (NotImplementedException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[CRITICAL] Logic missing: {ex.Message}");
                break;
            }

            Thread.Sleep(500); // Delay để dễ nhìn log
        }

        Console.WriteLine("=== SIMULATION ENDED ===");
        Console.ReadKey();
    }
}

// Helper nhỏ để hỗ trợ Lambda condition trong bài test
namespace FSMCore 
{
    public class FuncCondition<T> : ICondition<T>
    {
        private readonly Func<T, bool> _predicate;
        public FuncCondition(Func<T, bool> predicate) { _predicate = predicate; }
        public bool Check(T context) => _predicate(context);
    }
}