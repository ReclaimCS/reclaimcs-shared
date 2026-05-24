namespace ReclaimCS.Shared.WebIntegration;

public sealed class DashboardPlayerSyncState
{
    public DateTime NextPollUtc { get; set; } = DateTime.MinValue;
    public bool PollInFlight { get; set; }
    public int LastAppliedVersion { get; set; }
    public bool PushInFlight { get; set; }
    public string LastPushFingerprint { get; set; } = "";
}
