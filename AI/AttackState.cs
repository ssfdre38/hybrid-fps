using System.Numerics;
using HybridFPS.Utilities;
using HybridFPS.Entities;

namespace HybridFPS.AI
{
    public class AttackState : AIStateBase
    {
        private float attackTimer;
        private const float ATTACK_COOLDOWN = 1.0f;
        private const float ATTACK_DAMAGE = 12f;

        public AttackState(Enemy enemy) : base(enemy) { }

        public override AIState GetStateType() => AIState.Attack;

        public override void Enter()
        {
            attackTimer = ATTACK_COOLDOWN;
        }

        public override void Update(float deltaTime, Player player, Level level)
        {
            attackTimer += deltaTime;

            float distanceToPlayer = Vector3.Distance(enemy.Position, player.Position);

            if (distanceToPlayer > enemy.AttackRange)
            {
                enemy.ChangeState(AIState.Chase);
                return;
            }

            if (attackTimer >= ATTACK_COOLDOWN)
            {
                player.TakeDamage(ATTACK_DAMAGE);
                attackTimer = 0;
            }
        }

        public override void Exit() { }
    }
}
