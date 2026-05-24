using System.Collections.Concurrent;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using ReclaimCS.Shared.CounterStrike;

namespace ReclaimCS.Shared.Menus;

public sealed class InteractiveCenterMenu<TMode>
    where TMode : struct, Enum
{
    private static readonly PlayerButtons DefaultCloseButton = (PlayerButtons)8589934592L;
    private static readonly TimeSpan DefaultMenuTimeout = TimeSpan.FromSeconds(45);

    private readonly CenterHtmlService _centerHtmlService;
    private readonly string _menuKey;
    private readonly Func<CCSPlayerController, InteractiveMenuState<TMode>, string?> _render;
    private readonly Func<CCSPlayerController, InteractiveMenuState<TMode>, int> _getItemCount;
    private readonly Action<CCSPlayerController, InteractiveMenuState<TMode>> _activateSelection;
    private readonly Action<CCSPlayerController, InteractiveMenuState<TMode>> _backOrClose;
    private readonly TimeSpan _timeout;
    private readonly CenterHtmlPriority _priority;
    private readonly PlayerButtons _closeButton;
    private readonly ConcurrentDictionary<ulong, InteractiveMenuState<TMode>> _menus = new();

    public InteractiveCenterMenu(
        CenterHtmlService centerHtmlService,
        string menuKey,
        Func<CCSPlayerController, InteractiveMenuState<TMode>, string?> render,
        Func<CCSPlayerController, InteractiveMenuState<TMode>, int> getItemCount,
        Action<CCSPlayerController, InteractiveMenuState<TMode>> activateSelection,
        Action<CCSPlayerController, InteractiveMenuState<TMode>> backOrClose,
        TimeSpan? timeout = null,
        CenterHtmlPriority priority = CenterHtmlPriority.Menu,
        PlayerButtons? closeButton = null)
    {
        _centerHtmlService = centerHtmlService;
        _menuKey = string.IsNullOrWhiteSpace(menuKey) ? throw new ArgumentException("Menu key is required.", nameof(menuKey)) : menuKey;
        _render = render;
        _getItemCount = getItemCount;
        _activateSelection = activateSelection;
        _backOrClose = backOrClose;
        _timeout = timeout ?? DefaultMenuTimeout;
        _priority = priority;
        _closeButton = closeButton ?? DefaultCloseButton;
    }

    public bool Open(CCSPlayerController player, InteractiveMenuState<TMode> state)
    {
        if (!TryGetPlayerKey(player, out var playerKey))
            return false;

        RefreshTimeout(state);
        _menus[playerKey] = state;
        _centerHtmlService.SetPersistent(player, _menuKey, RenderMenu, _priority, redrawEveryTick: true);
        return true;
    }

    public void UpdatePlayer(CCSPlayerController player)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || !_menus.TryGetValue(playerKey, out var state))
            return;

        if (state.ExpiresAtUtc <= DateTime.UtcNow)
        {
            Close(player, clearOutput: false);
            return;
        }

        HandleButtons(player, state);
    }

    public void ClearPlayer(CCSPlayerController player)
    {
        Close(player, clearOutput: false);
    }

    public void Close(CCSPlayerController player, bool clearOutput)
    {
        if (TryGetPlayerKey(player, out var playerKey))
            _menus.TryRemove(playerKey, out _);

        _centerHtmlService.ClearPersistent(player, _menuKey);
        if (clearOutput)
            _centerHtmlService.ClearOutput(player);
    }

    public void RefreshTimeout(InteractiveMenuState<TMode> state)
    {
        state.ExpiresAtUtc = DateTime.UtcNow.Add(_timeout);
    }

    public bool TryGetState(CCSPlayerController player, out InteractiveMenuState<TMode>? state)
    {
        state = null;
        return TryGetPlayerKey(player, out var playerKey) && _menus.TryGetValue(playerKey, out state);
    }

    private string? RenderMenu(CCSPlayerController player)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || !_menus.TryGetValue(playerKey, out var state))
            return null;

        return state.ExpiresAtUtc <= DateTime.UtcNow ? null : _render(player, state);
    }

    private void HandleButtons(CCSPlayerController player, InteractiveMenuState<TMode> state)
    {
        var buttons = player.Buttons;

        if (WasPressed(buttons, state.LastButtons, PlayerButtons.Forward))
            MoveSelection(player, state, -1);
        else if (WasPressed(buttons, state.LastButtons, PlayerButtons.Back))
            MoveSelection(player, state, 1);
        else if (WasPressed(buttons, state.LastButtons, PlayerButtons.Use))
            ActivateSelection(player, state);
        else if (WasPressed(buttons, state.LastButtons, _closeButton))
            _backOrClose(player, state);

        state.LastButtons = buttons;
    }

    private void MoveSelection(CCSPlayerController player, InteractiveMenuState<TMode> state, int direction)
    {
        if (state.Closing)
            return;

        var count = _getItemCount(player, state);
        if (count <= 0)
            return;

        state.SelectedIndex = Wrap(state.SelectedIndex + direction, count);
        RefreshTimeout(state);
    }

    private void ActivateSelection(CCSPlayerController player, InteractiveMenuState<TMode> state)
    {
        if (state.Closing)
            return;

        RefreshTimeout(state);
        _activateSelection(player, state);
    }

    public static bool WasPressed(PlayerButtons current, PlayerButtons previous, PlayerButtons button)
    {
        return current.HasFlag(button) && !previous.HasFlag(button);
    }

    public static int Wrap(int value, int count)
    {
        if (count <= 0)
            return 0;

        if (value < 0)
            return count - 1;

        return value >= count ? 0 : value;
    }

    private static bool TryGetPlayerKey(CCSPlayerController? player, out ulong key)
    {
        key = 0;
        if (player is not { IsValid: true })
            return false;

        try
        {
            key = player.GetRuntimeKey();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
