using System;

namespace Avalonia.Labs.Controls.Base;

/// <summary>
/// Contains the tap event data 
/// </summary>
public class TapEventArgs : EventArgs
{
    public TapEventArgs(Point position)
    {
        Position = position;
    }

    public Point Position
    {
        get;
        set;
    }
}
