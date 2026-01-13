using System;
using System.Numerics;
using HybridFPS.Utilities;
using HybridFPS.Entities;

namespace HybridFPS.AI
{
    public class PatrolState : AIStateBase
    {
        private int currentPatrolIndex;
        private float waitTimer;
        private const float WAIT_TIME = 1.5f;
        private const float PATROL_SPEED = 2.0f;

        public PatrolState(Enemy enemy) : base(enemy) { }

        public override AIState GetStateType() => AIState.Patrol;

        public override void Enter()
        {
            waitTimer = 0;
            if (enemy.PatrolPoints.Length > 0)
            {
                currentPatrolIndex = 0;
            }
        }

        public override void Update(float deltaTime, Player player, Level level)
        {
            float distanceToPlayer = Vector3.Distance(enemy.Position, player.Position);

            if (distanceToPlayer < enemy.DetectionRange)
            {
                enemy.ChangeState(AIState.Chase);
                return;
            }

            if (enemy.PatrolPoints.Length == 0)
            {
                enemy.ChangeState(AIState.Idle);
                return;
            }

            Vector3 targetPoint = enemy.PatrolPoints[currentPatrolIndex];
            float distanceToTarget = Vector3.Distance(enemy.Position, targetPoint);

            if (distanceToTarget < 1.5f)
            {
                waitTimer += deltaTime;
                if (waitTimer >= WAIT_TIME)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % enemy.PatrolPoints.Length;
                    waitTimer = 0;
                }
            }
            else
            {
                Vector3 direction = targetPoint - enemy.Position;
                direction.Y = 0;
                direction = Vector3.Normalize(direction);

                Vector3 newPosition = enemy.Position + direction * PATROL_SPEED * deltaTime;
                if (level.IsWalkable(newPosition))
                {
                    enemy.Position = newPosition;
                }
            }
        }

        public override void Exit() { }
    }
}
