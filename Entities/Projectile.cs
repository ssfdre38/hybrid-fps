using System.Numerics;
using Raylib_cs;

namespace HybridFPS.Entities
{
    /// <summary>
    /// Represents a projectile (bullet) in the game.
    /// Pooled for performance.
    /// </summary>
    public class Projectile
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Damage { get; private set; }
        public float MaxRange { get; private set; }
        public Color Color { get; set; }
        public bool IsActive { get; set; }

        private Vector3 startPosition;
        private float distanceTraveled;

        public Projectile(Vector3 position, Vector3 direction, float speed, float damage, float maxRange, Color color)
        {
            Position = position;
            startPosition = position;
            Velocity = direction * speed;
            Damage = damage;
            MaxRange = maxRange;
            Color = color;
            IsActive = true;
            distanceTraveled = 0;
        }

        public void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
            distanceTraveled = Vector3.Distance(startPosition, Position);

            if (distanceTraveled >= MaxRange)
            {
                IsActive = false;
            }
        }

        public void Reset()
        {
            IsActive = false;
            distanceTraveled = 0;
        }

        public void Render()
        {
            if (IsActive)
            {
                Raylib.DrawSphere(Position, 0.15f, Color);
            }
        }
    }
}
