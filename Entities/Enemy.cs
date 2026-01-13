using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using HybridFPS.Utilities;
using HybridFPS.AI;

namespace HybridFPS.Entities
{
    /// <summary>
    /// Enemy entity with state machine-based AI.
    /// </summary>
    public class Enemy
    {
        public Vector3 Position { get; set; }
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsAlive => Health > 0;

        public float DetectionRange { get; set; } = 25f;
        public float AttackRange { get; set; } = 3f;
        public Vector3[] PatrolPoints { get; set; }

        private Dictionary<AIState, AIStateBase> states;
        private AIStateBase currentState;
        private Color color;

        public event Action<Enemy>? OnEnemyDeath;

        public Enemy(Vector3 position, Vector3[]? patrolPoints = null)
        {
            Position = position;
            MaxHealth = 100f;
            Health = MaxHealth;
            PatrolPoints = patrolPoints ?? Array.Empty<Vector3>();

            Random rand = new Random();
            color = new Color(rand.Next(150, 255), 0, 0, 255);

            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            states = new Dictionary<AIState, AIStateBase>
            {
                { AIState.Idle, new IdleState(this) },
                { AIState.Patrol, new PatrolState(this) },
                { AIState.Chase, new ChaseState(this) },
                { AIState.Attack, new AttackState(this) }
            };

            currentState = states[AIState.Idle];
            currentState.Enter();
        }

        public void Update(float deltaTime, Player player, Level level)
        {
            if (!IsAlive) return;

            currentState.Update(deltaTime, player, level);
        }

        public void ChangeState(AIState newState)
        {
            if (!states.ContainsKey(newState)) return;

            currentState.Exit();
            currentState = states[newState];
            currentState.Enter();
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                OnEnemyDeath?.Invoke(this);
            }
        }

        public void Render()
        {
            if (!IsAlive) return;

            Raylib.DrawCube(Position, 1, 2, 1, color);
            Raylib.DrawCubeWires(Position, 1, 2, 1, Color.Black);

            Vector3 barPos = Position + new Vector3(0, 1.5f, 0);
            float healthPercent = Health / MaxHealth;
            Raylib.DrawCube(barPos, healthPercent, 0.1f, 0.1f, Color.Red);
        }

        public AIState GetCurrentState()
        {
            return currentState.GetStateType();
        }
    }
}
