using System;
using Avalonia.Media.Imaging;

namespace ContentPatcherMaker.Services.Preview;

public interface IPreviewService
{
    event EventHandler? PreviewUpdated;
    Bitmap? CurrentBitmap { get; }
    void LoadBaseImage(string path);
    void ApplyChangeOverlay(string overlayPath);
    void ClearOverlay();
}

