namespace HybridFPS.Utilities
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public enum WeaponType
    {
        Pistol,
        AssaultRifle,
        Shotgun
    }
}
