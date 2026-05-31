namespace ReclaimCS.Shared.KillFeed;

public sealed class KillFeedIconOptions
{
    public bool Enabled { get; set; }
    public int Headshot { get; set; }
    public int ThroughSmoke { get; set; }
    public int NoScope { get; set; }
    public int AssistedFlash { get; set; }
    public int AttackerBlind { get; set; }
    public int AttackerInAir { get; set; }
    public int Penetrated { get; set; }
    public int Dominated { get; set; }
    public int Revenge { get; set; }
    public int SquadWipe { get; set; }
    public Dictionary<string, KillFeedIconRule> Icons { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public void Normalize()
    {
        Icons ??= new Dictionary<string, KillFeedIconRule>(StringComparer.OrdinalIgnoreCase);

        foreach (var rule in Icons.Values)
            rule.Normalize();
    }
}

