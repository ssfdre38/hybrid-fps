using System.Numerics;
using HybridFPS.Utilities;
using HybridFPS.Entities;

namespace HybridFPS.AI
{
    public class IdleState : AIStateBase
    {
        private float idleTimer;
        private const float IDLE_DURATION = 2f;

        public IdleState(Enemy enemy) : base(enemy) { }

        public override AIState GetStateType() => AIState.Idle;

        public override void Enter()
        {
            idleTimer = 0;
        }

        public override void Update(float deltaTime, Player player, Level level)
        {
            idleTimer += deltaTime;

            float distanceToPlayer = Vector3.Distance(enemy.Position, player.Position);

            if (distanceToPlayer < enemy.DetectionRange)
            {
                enemy.ChangeState(AIState.Chase);
            }
            else if (idleTimer >= IDLE_DURATION && enemy.PatrolPoints.Length > 0)
            {
                enemy.ChangeState(AIState.Patrol);
            }
        }

        public override void Exit() { }
    }
}
