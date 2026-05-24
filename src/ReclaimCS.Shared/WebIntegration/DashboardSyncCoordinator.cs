using System.Text.Json;

namespace ReclaimCS.Shared.WebIntegration;

public static class DashboardSyncCoordinator
{
    public static bool TryBeginPoll(
        DashboardPlayerSyncState state,
        double intervalSeconds,
        DateTime? nowUtc = null,
        double minIntervalSeconds = 1.0,
        double maxIntervalSeconds = 30.0)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if (state.PollInFlight || state.NextPollUtc > now)
            return false;

        state.PollInFlight = true;
        state.NextPollUtc = now.AddSeconds(Math.Clamp(intervalSeconds, minIntervalSeconds, maxIntervalSeconds));
        return true;
    }

    public static void CompletePoll(DashboardPlayerSyncState state)
    {
        state.PollInFlight = false;
    }

    public static bool TryBeginPush(DashboardPlayerSyncState state, string fingerprint)
    {
        if (state.PushInFlight)
            return false;

        if (string.Equals(state.LastPushFingerprint, fingerprint, StringComparison.Ordinal))
            return false;

        state.PushInFlight = true;
        return true;
    }

    public static void CompletePush(DashboardPlayerSyncState state, string fingerprint, bool success)
    {
        if (success)
            state.LastPushFingerprint = fingerprint;

        state.PushInFlight = false;
    }

    public static string CreateFingerprint(object payload)
    {
        return JsonSerializer.Serialize(payload);
    }
}
