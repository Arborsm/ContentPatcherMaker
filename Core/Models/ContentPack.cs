using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.Models;

public sealed class ContentPack
{
    [Required]
    public Manifest Manifest { get; set; } = new();

    [Required]
    public List<Patch> Changes { get; set; } = new();
}

