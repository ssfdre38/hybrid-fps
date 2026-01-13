using System;
using System.Numerics;
using System.Collections.Generic;
using HybridFPS.Utilities;
using HybridFPS.Entities;

namespace HybridFPS.Weapons
{
    /// <summary>
    /// Manages weapon switching, firing, and state.
    /// Uses data-driven WeaponConfig for weapon properties.
    /// </summary>
    public class WeaponSystem
    {
        private List<WeaponConfig> availableWeapons;
        private int currentWeaponIndex;
        private WeaponConfig currentWeapon;

        private int currentAmmo;
        private int reserveAmmo;
        private float fireTimer;
        private float reloadTimer;
        private bool isReloading;

        public event Action<Projectile>? OnProjectileSpawned;
        public event Action<WeaponConfig, int, int>? OnAmmoChanged;
        public event Action<WeaponConfig>? OnWeaponSwitched;

        public WeaponConfig CurrentWeapon => currentWeapon;
        public int CurrentAmmo => currentAmmo;
        public int ReserveAmmo => reserveAmmo;
        public bool IsReloading => isReloading;

        public WeaponSystem()
        {
            availableWeapons = new List<WeaponConfig>
            {
                WeaponConfig.CreatePistol(),
                WeaponConfig.CreateAssaultRifle(),
                WeaponConfig.CreateShotgun()
            };

            currentWeaponIndex = 0;
            EquipWeapon(0);
        }

        public void Update(float deltaTime)
        {
            fireTimer += deltaTime;

            if (isReloading)
            {
                reloadTimer -= deltaTime;
                if (reloadTimer <= 0)
                {
                    CompleteReload();
                }
            }
        }

        public void TryFire(Vector3 position, Vector3 direction)
        {
            if (isReloading || currentAmmo <= 0 || fireTimer < currentWeapon.FireRate)
                return;

            fireTimer = 0;
            currentAmmo--;

            // Spawn projectiles based on weapon config
            for (int i = 0; i < currentWeapon.PelletsPerShot; i++)
            {
                Vector3 spreadDirection = ApplySpread(direction, currentWeapon.Spread);

                var projectile = new Projectile(
                    position + direction * 0.5f,
                    spreadDirection,
                    currentWeapon.ProjectileSpeed,
                    currentWeapon.Damage,
                    currentWeapon.Range,
                    currentWeapon.ProjectileColor
                );

                OnProjectileSpawned?.Invoke(projectile);
            }

            OnAmmoChanged?.Invoke(currentWeapon, currentAmmo, reserveAmmo);

            if (currentAmmo == 0 && reserveAmmo > 0)
            {
                StartReload();
            }
        }

        public void StartReload()
        {
            if (isReloading || currentAmmo == currentWeapon.MagazineSize || reserveAmmo == 0)
                return;

            isReloading = true;
            reloadTimer = currentWeapon.ReloadTime;
        }

        private void CompleteReload()
        {
            int ammoNeeded = currentWeapon.MagazineSize - currentAmmo;
            int ammoToReload = Math.Min(ammoNeeded, reserveAmmo);

            currentAmmo += ammoToReload;
            reserveAmmo -= ammoToReload;
            isReloading = false;

            OnAmmoChanged?.Invoke(currentWeapon, currentAmmo, reserveAmmo);
        }

        public void SwitchWeapon(int index)
        {
            if (index < 0 || index >= availableWeapons.Count || index == currentWeaponIndex || isReloading)
                return;

            EquipWeapon(index);
        }

        public void NextWeapon()
        {
            int next = (currentWeaponIndex + 1) % availableWeapons.Count;
            SwitchWeapon(next);
        }

        public void PreviousWeapon()
        {
            int prev = currentWeaponIndex - 1;
            if (prev < 0) prev = availableWeapons.Count - 1;
            SwitchWeapon(prev);
        }

        private void EquipWeapon(int index)
        {
            currentWeaponIndex = index;
            currentWeapon = availableWeapons[index];
            currentAmmo = currentWeapon.MagazineSize;
            reserveAmmo = currentWeapon.MaxAmmo;
            isReloading = false;
            fireTimer = currentWeapon.FireRate;

            OnWeaponSwitched?.Invoke(currentWeapon);
            OnAmmoChanged?.Invoke(currentWeapon, currentAmmo, reserveAmmo);
        }

        private Vector3 ApplySpread(Vector3 direction, float spread)
        {
            Random rand = new Random();
            float spreadX = ((float)rand.NextDouble() - 0.5f) * spread * 2;
            float spreadY = ((float)rand.NextDouble() - 0.5f) * spread * 2;

            direction.X += spreadX;
            direction.Y += spreadY;
            return Vector3.Normalize(direction);
        }
    }
}
