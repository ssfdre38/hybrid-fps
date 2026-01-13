using System;
using System.Numerics;
using Raylib_cs;
using HybridFPS.Weapons;
using HybridFPS.Systems;

namespace HybridFPS.Entities
{
    /// <summary>
    /// Player entity with health, movement, and weapon management.
    /// </summary>
    public class Player
    {
        public Camera3D Camera;
        public Vector3 Position { get; set; }
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public WeaponSystem WeaponSystem { get; private set; }

        private float yaw;
        private float pitch;
        private const float MOVE_SPEED = 6.0f;
        private const float MOUSE_SENSITIVITY = 0.003f;

        public event Action? OnPlayerDeath;
        public event Action<float, float>? OnHealthChanged;

        public Player(Vector3 startPosition)
        {
            Position = startPosition;
            MaxHealth = 100f;
            Health = MaxHealth;

            Camera = new Camera3D
            {
                Position = Position,
                Target = Position + Vector3.UnitZ,
                Up = Vector3.UnitY,
                FovY = 75.0f,
                Projection = CameraProjection.Perspective
            };

            WeaponSystem = new WeaponSystem();
        }

        public void Update(float deltaTime, Level level)
        {
            HandleMouseLook();
            HandleMovement(deltaTime, level);
            WeaponSystem.Update(deltaTime);
        }

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = Raylib.GetMouseDelta();
            yaw += mouseDelta.X * MOUSE_SENSITIVITY;
            pitch -= mouseDelta.Y * MOUSE_SENSITIVITY;
            pitch = Math.Clamp(pitch, -1.5f, 1.5f);

            Vector3 lookDir = new Vector3(
                MathF.Sin(yaw) * MathF.Cos(pitch),
                MathF.Sin(pitch),
                MathF.Cos(yaw) * MathF.Cos(pitch)
            );

            Camera.Position = Position;
            Camera.Target = Position + lookDir;
        }

        private void HandleMovement(float deltaTime, Level level)
        {
            Vector3 forward = new Vector3(
                MathF.Sin(yaw),
                0,
                MathF.Cos(yaw)
            );
            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

            Vector3 moveDir = Vector3.Zero;

            if (Raylib.IsKeyDown(KeyboardKey.W)) moveDir += forward;
            if (Raylib.IsKeyDown(KeyboardKey.S)) moveDir -= forward;
            if (Raylib.IsKeyDown(KeyboardKey.D)) moveDir += right;
            if (Raylib.IsKeyDown(KeyboardKey.A)) moveDir -= right;

            if (moveDir.LengthSquared() > 0)
            {
                moveDir = Vector3.Normalize(moveDir);
                Vector3 newPosition = Position + moveDir * MOVE_SPEED * deltaTime;

                if (level.IsWalkable(newPosition))
                {
                    Position = newPosition;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;

            OnHealthChanged?.Invoke(Health, MaxHealth);

            if (Health <= 0)
            {
                OnPlayerDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            Health = Math.Min(Health + amount, MaxHealth);
            OnHealthChanged?.Invoke(Health, MaxHealth);
        }

        public Vector3 GetLookDirection()
        {
            return Vector3.Normalize(Camera.Target - Camera.Position);
        }
    }
}
