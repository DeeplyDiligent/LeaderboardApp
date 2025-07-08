using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaderboardApp.Models;

public class Participant
{
    public Guid Participantid { get; set; }

    [Required(ErrorMessage = "First name is required")]
    public string Firstname { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    public string Lastname { get; set; } = null!;

    [Required(ErrorMessage = "Nickname is required")]
    public string Nickname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid? Teamid { get; set; }

    public Guid Externalid { get; set; }

    public string? Githubhandle { get; set; }

    public string? Mslearnhandle { get; set; }

    public string? Passcode { get; set; }

    public DateTime? Passcodeexpiration { get; set; }

    public DateTime? Lastlogin { get; set; }

    public string? Refreshtoken { get; set; }

    public virtual ICollection<Participantscore> Participantscores { get; set; } = new List<Participantscore>();    

    public virtual Team? Team { get; set; }
}
