using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.Models;

public sealed class Patch
{
    [Required]
    public string Action { get; set; } = string.Empty; // e.g., EditData, EditImage, Load, etc.

    [Required]
    public string Target { get; set; } = string.Empty; // e.g., Data path or asset name

    public Dictionary<string, object?> Fields { get; set; } = new(); // action-specific fields

    public Dictionary<string, string>? When { get; set; } // conditions
}

