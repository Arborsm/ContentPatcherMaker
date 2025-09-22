using System;
using System.IO;
using Avalonia.Media.Imaging;

namespace ContentPatcherMaker.Services.Preview;

public class ImagePreviewService : IPreviewService
{
    private Bitmap? _baseBitmap;
    private Bitmap? _overlayBitmap;

    public event EventHandler? PreviewUpdated;

    public Bitmap? CurrentBitmap
    {
        get
        {
            if (_baseBitmap == null)
                return null;
            // naive composition for starter: return overlay if present otherwise base
            return _overlayBitmap ?? _baseBitmap;
        }
    }

    public void LoadBaseImage(string path)
    {
        _baseBitmap?.Dispose();
        _overlayBitmap?.Dispose();
        _baseBitmap = File.Exists(path) ? new Bitmap(path) : null;
        _overlayBitmap = null;
        PreviewUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyChangeOverlay(string overlayPath)
    {
        _overlayBitmap?.Dispose();
        _overlayBitmap = File.Exists(overlayPath) ? new Bitmap(overlayPath) : null;
        PreviewUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void ClearOverlay()
    {
        _overlayBitmap?.Dispose();
        _overlayBitmap = null;
        PreviewUpdated?.Invoke(this, EventArgs.Empty);
    }
}

