using System.Numerics;
using HybridFPS.Utilities;
using HybridFPS.Entities;

namespace HybridFPS.AI
{
    public class ChaseState : AIStateBase
    {
        private const float CHASE_SPEED = 3.5f;

        public ChaseState(Enemy enemy) : base(enemy) { }

        public override AIState GetStateType() => AIState.Chase;

        public override void Enter() { }

        public override void Update(float deltaTime, Player player, Level level)
        {
            float distanceToPlayer = Vector3.Distance(enemy.Position, player.Position);

            if (distanceToPlayer > enemy.DetectionRange * 1.5f)
            {
                enemy.ChangeState(AIState.Patrol);
                return;
            }

            if (distanceToPlayer < enemy.AttackRange)
            {
                enemy.ChangeState(AIState.Attack);
                return;
            }

            Vector3 toPlayer = player.Position - enemy.Position;
            toPlayer.Y = 0;

            if (toPlayer.LengthSquared() > 1.0f)
            {
                Vector3 direction = Vector3.Normalize(toPlayer);
                Vector3 newPosition = enemy.Position + direction * CHASE_SPEED * deltaTime;

                if (level.IsWalkable(newPosition))
                {
                    enemy.Position = newPosition;
                }
            }
        }

        public override void Exit() { }
    }
}
