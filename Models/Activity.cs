using System;
using System.Collections.Generic;

namespace LeaderboardApp.Models;

public partial class Activity
{
    public int Activityid { get; set; }

    public string Name { get; set; } = null!;

    public string Weighttype { get; set; } = null!;

    public decimal Weight { get; set; }

    public string Scope { get; set; } = null!;

    public string Frequency { get; set; } = null!;
}
