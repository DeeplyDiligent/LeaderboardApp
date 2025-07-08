using System;
using System.Collections.Generic;

namespace LeaderboardApp.Models;

public partial class Leaderboardentry
{
    public Guid Leaderboardentryid { get; set; }

    public Guid? Teamid { get; set; }

    public string Teamname { get; set; } = null!;

    public int Score { get; set; }

    public DateTime Lastupdated { get; set; }

    public virtual Team? Team { get; set; }
}
