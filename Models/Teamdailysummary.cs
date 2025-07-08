using System;
using System.Collections.Generic;

namespace LeaderboardApp.Models;

public partial class Teamdailysummary
{
    public int Summaryid { get; set; }

    public Guid? Teamid { get; set; }

    public DateOnly Day { get; set; }

    public int Totalsuggestionscount { get; set; }

    public int Totalacceptancescount { get; set; }

    public int Totallinessuggested { get; set; }

    public int Totallinesaccepted { get; set; }

    public int Totalactiveusers { get; set; }

    public int Totalchatacceptances { get; set; }

    public int Totalchatturns { get; set; }

    public int Totalactivechatusers { get; set; }

    public virtual Team? Team { get; set; }
}
