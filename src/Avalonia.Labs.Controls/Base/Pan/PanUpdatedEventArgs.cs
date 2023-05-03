using System;

namespace Avalonia.Labs.Controls.Base.Pan;

/// <summary>
/// Contains the pan updates event data 
/// </summary>
public class PanUpdatedEventArgs : EventArgs
{
    public PanUpdatedEventArgs(PanGestureStatus statusType, double totalX, double totalY)
    {
        StatusType = statusType;
        TotalX = totalX;
        TotalY = totalY;
    }

    public PanGestureStatus StatusType { get; set; }
    public double TotalX { get; set; }
    public double TotalY { get; set; }
}
