using System.Collections.Concurrent;
using System.Net;
using CounterStrikeSharp.API.Core;
using ReclaimCS.Shared.CounterStrike;

namespace ReclaimCS.Shared.Menus;

public sealed class CenterHtmlService
{
    private static readonly TimeSpan MaxTransientDuration = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MinTransientDuration = TimeSpan.FromSeconds(0.1);
    private static readonly TimeSpan PersistentRefreshInterval = TimeSpan.FromSeconds(6);
    private static readonly TimeSpan ChangedContentPrintInterval = TimeSpan.FromSeconds(0.25);
    private const int LiveRedrawDurationSeconds = 1;

    private readonly ConcurrentDictionary<ulong, PlayerCenterHtmlState> _states = new();
    private readonly string _logPrefix;

    public CenterHtmlService(string logPrefix = "[ReclaimCS]")
    {
        _logPrefix = string.IsNullOrWhiteSpace(logPrefix) ? "[ReclaimCS]" : logPrefix.Trim();
    }

    public static string Encode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }

    public void ShowTransient(
        CCSPlayerController player,
        string html,
        float durationSeconds,
        CenterHtmlPriority priority = CenterHtmlPriority.Alert,
        string? key = null)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || string.IsNullOrWhiteSpace(html))
            return;

        var duration = TimeSpan.FromSeconds(durationSeconds);
        duration = Clamp(duration, MinTransientDuration, MaxTransientDuration);

        var state = _states.GetOrAdd(playerKey, _ => new PlayerCenterHtmlState());
        var entry = CenterHtmlEntry.Transient(
            string.IsNullOrWhiteSpace(key) ? Guid.NewGuid().ToString("N") : key,
            priority,
            html,
            DateTime.UtcNow.Add(duration));

        lock (state.Sync)
        {
            state.Transient.RemoveAll(candidate => string.Equals(candidate.Key, entry.Key, StringComparison.OrdinalIgnoreCase));
            state.Transient.Add(entry);
        }
    }

    public void SetPersistent(
        CCSPlayerController player,
        string key,
        Func<CCSPlayerController, string?> render,
        CenterHtmlPriority priority = CenterHtmlPriority.Status,
        bool redrawEveryTick = true)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || string.IsNullOrWhiteSpace(key))
            return;

        var state = _states.GetOrAdd(playerKey, _ => new PlayerCenterHtmlState());
        lock (state.Sync)
        {
            state.Persistent[key] = CenterHtmlEntry.Persistent(key, priority, render, redrawEveryTick);
        }
    }

    public void ClearPersistent(CCSPlayerController player, string key)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || !_states.TryGetValue(playerKey, out var state))
            return;

        lock (state.Sync)
        {
            state.Persistent.Remove(key);
        }
    }

    public void ClearTransient(CCSPlayerController player, string key)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || !_states.TryGetValue(playerKey, out var state))
            return;

        lock (state.Sync)
        {
            state.Transient.RemoveAll(entry => string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void ClearOutput(CCSPlayerController player)
    {
        if (player.IsValid)
            RestoreHints(player);
    }

    public void ClearPlayer(CCSPlayerController player, bool clearOutput = true)
    {
        if (!TryGetPlayerKey(player, out var playerKey))
            return;

        if (_states.TryRemove(playerKey, out var state) && clearOutput && state.HadOutput && player.IsValid)
            ClearOutput(player);
    }

    public void ClearAll()
    {
        _states.Clear();
    }

    public void UpdatePlayer(CCSPlayerController player)
    {
        if (!TryGetPlayerKey(player, out var playerKey) || !_states.TryGetValue(playerKey, out var state))
            return;

        List<CenterHtmlEntry> candidates;
        var now = DateTime.UtcNow;
        var hadOutput = false;

        lock (state.Sync)
        {
            state.Transient.RemoveAll(entry => entry.ExpiresAtUtc <= now);
            candidates = state.Transient
                .Concat(state.Persistent.Values)
                .OrderByDescending(entry => entry.Priority)
                .ThenByDescending(entry => entry.CreatedAtUtc)
                .ToList();
            hadOutput = state.HadOutput;
        }

        var rendered = RenderFirstAvailable(player, candidates);
        if (rendered != null)
        {
            var shouldPrint = false;
            lock (state.Sync)
            {
                var htmlChanged = !string.Equals(state.LastHtml, rendered.Html, StringComparison.Ordinal);
                var entryChanged = !string.Equals(state.LastEntryKey, rendered.Key, StringComparison.OrdinalIgnoreCase)
                    || state.LastEntryExpiresAtUtc != rendered.ExpiresAtUtc;
                var canPrintChangedContent = now - state.LastPrintedAtUtc >= ChangedContentPrintInterval;
                var heartbeatDue = now >= state.NextRefreshAtUtc;

                shouldPrint = rendered.RedrawEveryTick
                    || !state.HadOutput
                    || ((htmlChanged || entryChanged) && canPrintChangedContent)
                    || (!htmlChanged && !entryChanged && heartbeatDue);

                if (shouldPrint)
                {
                    state.HadOutput = true;
                    state.LastHtml = rendered.Html;
                    state.LastEntryKey = rendered.Key;
                    state.LastEntryExpiresAtUtc = rendered.ExpiresAtUtc;
                    state.LastPrintedAtUtc = now;
                    state.NextRefreshAtUtc = GetNextRefreshAt(rendered, now);
                }
            }

            if (shouldPrint)
            {
                if (rendered.RedrawEveryTick)
                    player.PrintToCenterHtml(rendered.Html, LiveRedrawDurationSeconds);
                else
                    player.PrintToCenterHtml(rendered.Html, GetDisplaySeconds(rendered, now));
            }

            return;
        }

        if (hadOutput)
            ClearOutput(player);

        lock (state.Sync)
        {
            state.HadOutput = false;
            state.LastHtml = null;
            state.LastEntryKey = null;
            state.LastEntryExpiresAtUtc = DateTime.MinValue;
            state.LastPrintedAtUtc = DateTime.MinValue;
            state.NextRefreshAtUtc = DateTime.MinValue;
            if (state.Transient.Count == 0 && state.Persistent.Count == 0)
                _states.TryRemove(playerKey, out _);
        }
    }

    private CenterHtmlRender? RenderFirstAvailable(CCSPlayerController player, IReadOnlyList<CenterHtmlEntry> candidates)
    {
        foreach (var candidate in candidates)
        {
            try
            {
                var html = candidate.Render(player);
                if (!string.IsNullOrWhiteSpace(html))
                    return new CenterHtmlRender(candidate.Key, candidate.ExpiresAtUtc, candidate.RedrawEveryTick, html);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{_logPrefix} Center HTML renderer '{candidate.Key}' failed for {player.PlayerName}: {ex.Message}");
            }
        }

        return null;
    }

    private static int GetDisplaySeconds(CenterHtmlRender rendered, DateTime now)
    {
        if (rendered.ExpiresAtUtc == DateTime.MaxValue)
            return 1;

        var remaining = rendered.ExpiresAtUtc - now;
        return (int)Math.Ceiling(Clamp(remaining, MinTransientDuration, MaxTransientDuration).TotalSeconds);
    }

    private static DateTime GetNextRefreshAt(CenterHtmlRender rendered, DateTime now)
    {
        if (rendered.ExpiresAtUtc != DateTime.MaxValue)
            return rendered.ExpiresAtUtc;

        return now.Add(PersistentRefreshInterval);
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

    private void RestoreHints(CCSPlayerController player)
    {
        if (player.IsValid)
            TrySetShowHints(player, true);
    }

    private void TrySetShowHints(CCSPlayerController player, bool enabled)
    {
        try
        {
            player.ShowHints = enabled;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{_logPrefix} Failed to {(enabled ? "restore" : "clear")} center HTML hint state for {player.PlayerName}: {ex.Message}");
        }
    }

    private static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
    {
        if (value < min)
            return min;

        return value > max ? max : value;
    }

    private sealed class PlayerCenterHtmlState
    {
        public object Sync { get; } = new();
        public Dictionary<string, CenterHtmlEntry> Persistent { get; } = new(StringComparer.OrdinalIgnoreCase);
        public List<CenterHtmlEntry> Transient { get; } = [];
        public bool HadOutput { get; set; }
        public string? LastHtml { get; set; }
        public string? LastEntryKey { get; set; }
        public DateTime LastEntryExpiresAtUtc { get; set; } = DateTime.MinValue;
        public DateTime LastPrintedAtUtc { get; set; } = DateTime.MinValue;
        public DateTime NextRefreshAtUtc { get; set; } = DateTime.MinValue;
    }

    private sealed record CenterHtmlRender(string Key, DateTime ExpiresAtUtc, bool RedrawEveryTick, string Html);

    private sealed class CenterHtmlEntry
    {
        private CenterHtmlEntry(
            string key,
            CenterHtmlPriority priority,
            DateTime createdAtUtc,
            DateTime expiresAtUtc,
            bool redrawEveryTick,
            Func<CCSPlayerController, string?> render)
        {
            Key = key;
            Priority = priority;
            CreatedAtUtc = createdAtUtc;
            ExpiresAtUtc = expiresAtUtc;
            RedrawEveryTick = redrawEveryTick;
            Render = render;
        }

        public string Key { get; }
        public CenterHtmlPriority Priority { get; }
        public DateTime CreatedAtUtc { get; }
        public DateTime ExpiresAtUtc { get; }
        public bool RedrawEveryTick { get; }
        public Func<CCSPlayerController, string?> Render { get; }

        public static CenterHtmlEntry Transient(string key, CenterHtmlPriority priority, string html, DateTime expiresAtUtc)
        {
            return new CenterHtmlEntry(key, priority, DateTime.UtcNow, expiresAtUtc, false, _ => html);
        }

        public static CenterHtmlEntry Persistent(string key, CenterHtmlPriority priority, Func<CCSPlayerController, string?> render, bool redrawEveryTick)
        {
            return new CenterHtmlEntry(key, priority, DateTime.UtcNow, DateTime.MaxValue, redrawEveryTick, render);
        }
    }
}
