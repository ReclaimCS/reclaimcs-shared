using CounterStrikeSharp.API;

namespace ReclaimCS.Shared.Menus;

public sealed class InteractiveMenuState<TMode>
    where TMode : struct, Enum
{
    public InteractiveMenuState(TMode mode, int selectedIndex, PlayerButtons lastButtons)
    {
        Mode = mode;
        SelectedIndex = selectedIndex;
        LastButtons = lastButtons;
    }

    public TMode Mode { get; set; }
    public int SelectedIndex { get; set; }
    public PlayerButtons LastButtons { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string Status { get; set; } = "";
    public bool Closing { get; set; }
}
