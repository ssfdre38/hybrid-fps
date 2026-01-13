using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using HybridFPS.Utilities;
using HybridFPS.Entities;
using HybridFPS.Systems;

namespace HybridFPS.Core
{
    /// <summary>
    /// Main game class with state management and game loop.
    /// </summary>
    public class Game
    {
        private const int SCREEN_WIDTH = 1280;
        private const int SCREEN_HEIGHT = 720;

        private GameState currentState;
        private Player player;
        private Level level;
        private List<Enemy> enemies;
        private List<Projectile> activeProjectiles;
        private ObjectPool<Projectile> projectilePool;
        private int score;
        private int enemiesKilled;
        private int totalEnemies;

        public void Run()
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Hybrid FPS - Professional Architecture + Playable Game");
            Raylib.SetTargetFPS(60);
            Raylib.DisableCursor();

            Initialize();

            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();
                Update(deltaTime);
                Render();
            }

            Raylib.CloseWindow();
        }

        private void Initialize()
        {
            level = new Level();
            player = new Player(new Vector3(5, 1, 5));
            enemies = new List<Enemy>();
            activeProjectiles = new List<Projectile>();

            projectilePool = new ObjectPool<Projectile>(
                () => new Projectile(Vector3.Zero, Vector3.UnitZ, 0, 0, 0, Color.Yellow),
                (p) => p.Reset(),
                initialSize: 50,
                maxSize: 200
            );

            player.WeaponSystem.OnProjectileSpawned += OnProjectileSpawned;
            player.OnPlayerDeath += OnPlayerDeath;

            SpawnEnemies();

            currentState = GameState.Playing;
        }

        private void SpawnEnemies()
        {
            Vector3[] patrol1 = { new Vector3(10, 1, 10), new Vector3(20, 1, 10), new Vector3(20, 1, 20) };
            Vector3[] patrol2 = { new Vector3(30, 1, 30), new Vector3(40, 1, 30), new Vector3(40, 1, 40) };

            enemies.Add(new Enemy(new Vector3(15, 1, 15), patrol1));
            enemies.Add(new Enemy(new Vector3(35, 1, 35), patrol2));
            enemies.Add(new Enemy(new Vector3(45, 1, 10)));
            enemies.Add(new Enemy(new Vector3(10, 1, 45)));
            enemies.Add(new Enemy(new Vector3(25, 1, 40)));

            foreach (var enemy in enemies)
            {
                enemy.OnEnemyDeath += OnEnemyKilled;
            }

            totalEnemies = enemies.Count;
        }

        private void OnProjectileSpawned(Projectile projectile)
        {
            activeProjectiles.Add(projectile);
        }

        private void OnEnemyKilled(Enemy enemy)
        {
            enemiesKilled++;
            score += 100;

            if (enemiesKilled >= totalEnemies)
            {
                currentState = GameState.Victory;
            }
        }

        private void OnPlayerDeath()
        {
            currentState = GameState.GameOver;
        }

        private void Update(float deltaTime)
        {
            switch (currentState)
            {
                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;
                case GameState.Paused:
                    UpdatePaused();
                    break;
                case GameState.GameOver:
                case GameState.Victory:
                    UpdateEndScreen();
                    break;
            }
        }

        private void UpdatePlaying(float deltaTime)
        {
            player.Update(deltaTime, level);

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                player.WeaponSystem.TryFire(player.Camera.Position, player.GetLookDirection());
            }

            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                player.WeaponSystem.StartReload();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.One))
                player.WeaponSystem.SwitchWeapon(0);
            if (Raylib.IsKeyPressed(KeyboardKey.Two))
                player.WeaponSystem.SwitchWeapon(1);
            if (Raylib.IsKeyPressed(KeyboardKey.Three))
                player.WeaponSystem.SwitchWeapon(2);

            float scroll = Raylib.GetMouseWheelMove();
            if (scroll > 0) player.WeaponSystem.PreviousWeapon();
            if (scroll < 0) player.WeaponSystem.NextWeapon();

            for (int i = activeProjectiles.Count - 1; i >= 0; i--)
            {
                var projectile = activeProjectiles[i];
                projectile.Update(deltaTime);

                if (!projectile.IsActive)
                {
                    activeProjectiles.RemoveAt(i);
                    projectilePool.Return(projectile);
                    continue;
                }

                foreach (var enemy in enemies)
                {
                    if (!enemy.IsAlive) continue;

                    if (Vector3.Distance(projectile.Position, enemy.Position) < 1.0f)
                    {
                        enemy.TakeDamage(projectile.Damage);
                        activeProjectiles.RemoveAt(i);
                        projectilePool.Return(projectile);
                        break;
                    }
                }
            }

            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive)
                {
                    enemy.Update(deltaTime, player, level);
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                currentState = GameState.Paused;
            }
        }

        private void UpdatePaused()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                currentState = GameState.Playing;
            }
        }

        private void UpdateEndScreen()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                ResetGame();
            }
        }

        private void ResetGame()
        {
            enemies.Clear();
            activeProjectiles.Clear();
            enemiesKilled = 0;
            score = 0;
            Initialize();
        }

        private void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.SkyBlue);

            switch (currentState)
            {
                case GameState.Playing:
                    RenderPlaying();
                    break;
                case GameState.Paused:
                    RenderPlaying();
                    RenderPaused();
                    break;
                case GameState.GameOver:
                    RenderGameOver();
                    break;
                case GameState.Victory:
                    RenderVictory();
                    break;
            }

            Raylib.EndDrawing();
        }

        private void RenderPlaying()
        {
            Raylib.BeginMode3D(player.Camera);

            level.Render();

            foreach (var projectile in activeProjectiles)
            {
                projectile.Render();
            }

            foreach (var enemy in enemies)
            {
                enemy.Render();
            }

            Raylib.EndMode3D();

            RenderHUD();
        }

        private void RenderHUD()
        {
            int centerX = SCREEN_WIDTH / 2;
            int centerY = SCREEN_HEIGHT / 2;
            Raylib.DrawLine(centerX - 10, centerY, centerX + 10, centerY, Color.White);
            Raylib.DrawLine(centerX, centerY - 10, centerX, centerY + 10, Color.White);

            int healthBarWidth = 200;
            int healthWidth = (int)(healthBarWidth * (player.Health / 100.0f));
            Raylib.DrawRectangle(10, 10, healthBarWidth, 20, Color.DarkGray);
            Raylib.DrawRectangle(10, 10, healthWidth, 20, Color.Red);
            Raylib.DrawRectangleLines(10, 10, healthBarWidth, 20, Color.White);
            Raylib.DrawText($"HP: {(int)player.Health}", 15, 12, 16, Color.White);

            var weapon = player.WeaponSystem.CurrentWeapon;
            string reloadText = player.WeaponSystem.IsReloading ? " [RELOADING]" : "";
            Raylib.DrawText($"{weapon.Name}: {player.WeaponSystem.CurrentAmmo}/{player.WeaponSystem.ReserveAmmo}{reloadText}",
                          10, 40, 20, Color.White);

            Raylib.DrawText($"Score: {score}", 10, 70, 20, Color.Gold);
            Raylib.DrawText($"Enemies: {totalEnemies - enemiesKilled}/{totalEnemies}",
                          SCREEN_WIDTH - 200, 10, 20, Color.White);
            Raylib.DrawText($"Pool Size: {projectilePool.PoolSize}",
                          SCREEN_WIDTH - 200, 40, 16, Color.LightGray);
        }

        private void RenderPaused()
        {
            Raylib.DrawRectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, new Color(0, 0, 0, 180));
            Raylib.DrawText("PAUSED", SCREEN_WIDTH / 2 - 100, SCREEN_HEIGHT / 2 - 20, 40, Color.White);
            Raylib.DrawText("Press ESC to Resume", SCREEN_WIDTH / 2 - 120, SCREEN_HEIGHT / 2 + 30, 20, Color.LightGray);
        }

        private void RenderGameOver()
        {
            Raylib.DrawText("GAME OVER", SCREEN_WIDTH / 2 - 150, SCREEN_HEIGHT / 2 - 60, 50, Color.Red);
            Raylib.DrawText($"Final Score: {score}", SCREEN_WIDTH / 2 - 120, SCREEN_HEIGHT / 2 + 20, 30, Color.White);
            Raylib.DrawText($"Enemies Killed: {enemiesKilled}/{totalEnemies}",
                          SCREEN_WIDTH / 2 - 140, SCREEN_HEIGHT / 2 + 60, 25, Color.Yellow);
            Raylib.DrawText("Press SPACE to Restart", SCREEN_WIDTH / 2 - 140, SCREEN_HEIGHT / 2 + 120, 20, Color.LightGray);
        }

        private void RenderVictory()
        {
            Raylib.DrawText("VICTORY!", SCREEN_WIDTH / 2 - 120, SCREEN_HEIGHT / 2 - 60, 50, Color.Gold);
            Raylib.DrawText("All enemies eliminated!", SCREEN_WIDTH / 2 - 140, SCREEN_HEIGHT / 2, 25, Color.White);
            Raylib.DrawText($"Final Score: {score}", SCREEN_WIDTH / 2 - 100, SCREEN_HEIGHT / 2 + 40, 30, Color.Yellow);
            Raylib.DrawText("Press SPACE to Play Again", SCREEN_WIDTH / 2 - 150, SCREEN_HEIGHT / 2 + 100, 20, Color.LightGray);
        }
    }
}
