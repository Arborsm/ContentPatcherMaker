using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.Models;

public class ContentPatcherContent
{
    [Required]
    public string? Format { get; set; } = "1.30.0";

    [Required]
    public string? ChangesPath { get; set; } = "content.json";
}

public class ContentPatcherManifest
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Author { get; set; }

    [Required]
    public string? Version { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public string? UniqueID { get; set; }

    public string? MinimumApiVersion { get; set; }
    public string? UpdateKeys { get; set; }
    public List<string> Dependencies { get; set; } = new();
}

public class ContentPatcherChange
{
    [Required]
    public string? Action { get; set; }

    public string? Target { get; set; }
    public string? FromFile { get; set; }
    public string? FromArea { get; set; }
    public string? ToArea { get; set; }
    public Dictionary<string, object> When { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ContentPatcherPackage
{
    [Required]
    public ContentPatcherManifest Manifest { get; set; } = new();

    public List<ContentPatcherChange> Changes { get; set; } = new();
}

