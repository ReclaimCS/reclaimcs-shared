namespace ReclaimCS.Shared.PlayerModels;

[Flags]
public enum PlayerModelRole
{
    None = 0,
    Human = 1,
    Zombie = 2,
    Any = Human | Zombie
}
