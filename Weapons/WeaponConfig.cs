using HybridFPS.Utilities;
using Raylib_cs;

namespace HybridFPS.Weapons
{
    /// <summary>
    /// Data-driven weapon configuration.
    /// Inspired by Unity's ScriptableObject pattern but using plain C# classes.
    /// </summary>
    public class WeaponConfig
    {
        public WeaponType Type { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
        public float FireRate { get; set; }
        public int MagazineSize { get; set; }
        public int MaxAmmo { get; set; }
        public float ReloadTime { get; set; }
        public float Range { get; set; }
        public float Spread { get; set; }
        public float ProjectileSpeed { get; set; }
        public int PelletsPerShot { get; set; }
        public Color ProjectileColor { get; set; }

        public static WeaponConfig CreatePistol()
        {
            return new WeaponConfig
            {
                Type = WeaponType.Pistol,
                Name = "Pistol",
                Damage = 20f,
                FireRate = 0.5f,
                MagazineSize = 12,
                MaxAmmo = 84,
                ReloadTime = 1.5f,
                Range = 50f,
                Spread = 0.05f,
                ProjectileSpeed = 50f,
                PelletsPerShot = 1,
                ProjectileColor = Color.Yellow
            };
        }

        public static WeaponConfig CreateAssaultRifle()
        {
            return new WeaponConfig
            {
                Type = WeaponType.AssaultRifle,
                Name = "Assault Rifle",
                Damage = 25f,
                FireRate = 0.1f,
                MagazineSize = 30,
                MaxAmmo = 180,
                ReloadTime = 2.5f,
                Range = 100f,
                Spread = 0.08f,
                ProjectileSpeed = 60f,
                PelletsPerShot = 1,
                ProjectileColor = Color.Orange
            };
        }

        public static WeaponConfig CreateShotgun()
        {
            return new WeaponConfig
            {
                Type = WeaponType.Shotgun,
                Name = "Shotgun",
                Damage = 15f,
                FireRate = 1.0f,
                MagazineSize = 8,
                MaxAmmo = 40,
                ReloadTime = 3.0f,
                Range = 30f,
                Spread = 0.15f,
                ProjectileSpeed = 40f,
                PelletsPerShot = 8,
                ProjectileColor = Color.Red
            };
        }
    }
}
