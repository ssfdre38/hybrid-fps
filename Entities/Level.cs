using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace HybridFPS.Entities
{
    /// <summary>
    /// Represents the game level with walls and collision detection.
    /// </summary>
    public class Level
    {
        private List<Wall> walls;

        public Level()
        {
            walls = new List<Wall>();
            BuildLevel();
        }

        private void BuildLevel()
        {
            walls.Add(new Wall(new Vector3(25, 2, 0), new Vector3(50, 4, 1)));
            walls.Add(new Wall(new Vector3(25, 2, 50), new Vector3(50, 4, 1)));
            walls.Add(new Wall(new Vector3(0, 2, 25), new Vector3(1, 4, 50)));
            walls.Add(new Wall(new Vector3(50, 2, 25), new Vector3(1, 4, 50)));

            walls.Add(new Wall(new Vector3(15, 2, 15), new Vector3(4, 4, 4)));
            walls.Add(new Wall(new Vector3(35, 2, 35), new Vector3(4, 4, 4)));
            walls.Add(new Wall(new Vector3(15, 2, 35), new Vector3(4, 4, 4)));
            walls.Add(new Wall(new Vector3(35, 2, 15), new Vector3(4, 4, 4)));
            walls.Add(new Wall(new Vector3(25, 2, 25), new Vector3(6, 4, 2)));
        }

        public bool IsWalkable(Vector3 position)
        {
            if (position.X < 1 || position.X > 49 || position.Z < 1 || position.Z > 49)
                return false;

            foreach (var wall in walls)
            {
                if (wall.CollidesWith(position, 0.5f))
                    return false;
            }

            return true;
        }

        public void Render()
        {
            Raylib.DrawPlane(new Vector3(25, 0, 25), new Vector2(60, 60), Color.DarkGreen);
            Raylib.DrawGrid(60, 2.0f);

            foreach (var wall in walls)
            {
                wall.Render();
            }
        }
    }

    public class Wall
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }

        public Wall(Vector3 position, Vector3 size)
        {
            Position = position;
            Size = size;
        }

        public bool CollidesWith(Vector3 point, float radius)
        {
            Vector3 halfSize = Size / 2;
            Vector3 min = Position - halfSize;
            Vector3 max = Position + halfSize;

            Vector3 closest = new Vector3(
                System.Math.Clamp(point.X, min.X, max.X),
                System.Math.Clamp(point.Y, min.Y, max.Y),
                System.Math.Clamp(point.Z, min.Z, max.Z)
            );

            float distance = Vector3.Distance(point, closest);
            return distance < radius;
        }

        public void Render()
        {
            Raylib.DrawCube(Position, Size.X, Size.Y, Size.Z, Color.Gray);
            Raylib.DrawCubeWires(Position, Size.X, Size.Y, Size.Z, Color.Black);
        }
    }
}
