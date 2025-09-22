using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.Models;

public sealed class Manifest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = "1.0.0";

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string UniqueID { get; set; } = string.Empty;

    public string MinimumApiVersion { get; set; } = "4.0.0";

    public List<string>? UpdateKeys { get; set; }

    [Required]
    public ManifestContentPackFor ContentPackFor { get; set; } = new();
}

public sealed class ManifestContentPackFor
{
    [Required]
    public string UniqueID { get; set; } = "Pathoschild.ContentPatcher";
    public string? MinimumVersion { get; set; }
}

