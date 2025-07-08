using System;
using System.Collections.Generic;

namespace LeaderboardApp.Models;

public partial class Challenge
{
    public int ChallengeId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? PostedDate { get; set; }

    public int? ActivityId { get; set; }

    public virtual Activity? Activity { get; set; } 
}
