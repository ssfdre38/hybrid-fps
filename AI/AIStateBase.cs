using HybridFPS.Utilities;
using HybridFPS.Entities;

namespace HybridFPS.AI
{
    /// <summary>
    /// Abstract base class for AI states.
    /// Implements the State pattern for enemy behavior.
    /// </summary>
    public abstract class AIStateBase
    {
        protected Enemy enemy;

        public AIStateBase(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public abstract AIState GetStateType();
        public abstract void Enter();
        public abstract void Update(float deltaTime, Player player, Level level);
        public abstract void Exit();
    }
}
