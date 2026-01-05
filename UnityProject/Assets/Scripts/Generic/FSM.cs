using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSM : MonoBehaviour
{
    private Player _context = new Player();
    private Brain<Player> _brain;

    public enum PlayerStates
    {
        Heal,
        Reload,
        Attack,
        Idle
    }

    void Start()
    {
        Debug.Log("=== UTILITY FSM SIMULATION: ATTACK ENEMY AI ===");

        var states = new Dictionary<PlayerStates, State<Player>>()
        {
            { PlayerStates.Heal, new HealState() },
            { PlayerStates.Reload, new ReloadState() },
            { PlayerStates.Attack, new AttackState() },
            { PlayerStates.Idle, new IdleState() }
        };
        
        _brain = new Brain<Player>(_context, states, PlayerStates.Idle);

        StartCoroutine(SimulateFSM());
    }

    IEnumerator SimulateFSM()
    {
        float deltaTime = 1f;

        for (int t = 0; t < 20; t++)
        {
            Debug.Log($"--- Time: {t}s ---");

            _brain.UpdateState(deltaTime);

            if (t == 5)
            {
                Debug.Log("[EVENT] Enemy spotted");
                _context.isEnemyVisible = true;
                _context.distanceToTarget = 4f;
            }

            if (t == 10)
            {
                Debug.Log("[EVENT] Gun jammed, ammo lost");
                _context.currentAmmo = 0;
            }

            if (t == 15)
            {
                Debug.Log("[EVENT] Player took damage (-60 HP)");
                _context.currentHealth -= 60;

                var state = _brain.FindStateByName("Heal");
                state.Apply = () => false;

                //3s troi qua
                state.Apply = () => true;
            }

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("=== SIMULATION ENDED ===");
    }

    public class Player
    {
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        public int currentAmmo = 40;
        public int maxAmmo = 40;

        public bool isEnemyVisible = false;
        public float distanceToTarget = 10f;
        public float attackRange = 5f;

        public void Log(string message)
        {
            Debug.Log($"[HP {currentHealth}/{maxHealth} | Ammo {currentAmmo}/{maxAmmo}] {message}");
        }
    }


    public abstract class State<T>
    {
        public Func<bool> Apply;
        public string Name { get; protected set; }
        public State(string name)
        {
            Name = name;
        }

        public abstract float CalculateUtility(T context);
        public abstract void OnEnter(T context);
        public abstract void OnExit(T context);
        public abstract void OnUpdate(T context, float deltaTime);
    }

    public class Brain<T>
    {
        public Dictionary<PlayerStates, State<T>> _states;
        public State<T> _currentState;
        public T _context;
        public Brain(T context, Dictionary<PlayerStates, State<T>> states, PlayerStates initialState)
        {
            _context = context;
            _states = states;
            _currentState = _states[initialState];
            _currentState.OnEnter(_context);
        }

        public void UpdateState(float deltaTime)
        {
            float bestUtility = -1f;
            State<T> bestState = _currentState;

            foreach(var state in _states.Values)
            {
                float utility = state.CalculateUtility(_context);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestState = state;
                }
            }

            if(bestState != _currentState)
            {
                _currentState.OnExit(_context);
                _currentState = bestState;
                _currentState.OnEnter(_context);
            }

            _currentState.OnUpdate(_context, deltaTime);
        }

        public State<T> FindStateByName(string stateLabel)
        {
            if(_currentState.Name == stateLabel)
            {
                return _currentState;
            }
            return null;
        }
    }

    public class HealState : State<FSM.Player>
    {
        public HealState() : base("Heal") { }

        public override float CalculateUtility(Player context)
        {
            return 1f - (context.currentHealth / context.maxHealth);
        }

        public override void OnEnter(Player context)
        {
            context.Log("Start Healing");
        }

        public override void OnExit(Player context)
        {
            context.Log("Stop Healing");
        }

        public override void OnUpdate(Player context, float deltaTime)
        {
            context.currentHealth += 10 * deltaTime;
            if (context.currentHealth > context.maxHealth)
                context.currentHealth = context.maxHealth;
        }
    }

    public class ReloadState : State<FSM.Player>
    {
        public ReloadState() : base("Reload") { }
        public override float CalculateUtility(Player context)
        {
            return 1f - (float)context.currentAmmo / context.maxAmmo;   
        }

        public override void OnEnter(Player context)
            => context.Log("Reloading...");

        public override void OnExit(Player context)
            => context.Log("Reload finished");

        public override void OnUpdate(Player context, float deltaTime)
        {
            context.currentAmmo += Mathf.CeilToInt(20 * deltaTime);
            if (context.currentAmmo > context.maxAmmo)
            {
                context.currentAmmo = context.maxAmmo;
            }
        }
    }

    public class AttackState : State<FSM.Player>
    {
        public AttackState() : base("Attack") { }
        public override float CalculateUtility(Player context)
        {
            if (!context.isEnemyVisible) return 0f;
            if (context.distanceToTarget > context.attackRange) return 0f;

            return 1f - (context.distanceToTarget / context.attackRange);
        }

        public override void OnEnter(Player context)
            => context.Log("Start attacking");

        public override void OnExit(Player context) { }

        public override void OnUpdate(Player context, float deltaTime)
        {
            context.Log("Attacking...");
            context.currentAmmo = Mathf.Max(0, context.currentAmmo - 1);
        }
    }

    public class IdleState : State<FSM.Player>
    {
        public IdleState() : base("Idle") { }
        public override float CalculateUtility(Player context)
        {
            return 0.1f;
        }

        public override void OnEnter(Player context)
            => context.Log("Idle...");

        public override void OnExit(Player context) { }

        public override void OnUpdate(Player context, float deltaTime) { }
    }
}