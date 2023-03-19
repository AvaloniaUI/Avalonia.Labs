namespace Avalonia.Labs.Controls.Base;

public static class MatrixExtensions
{
    public static Point GetPoint(this Matrix matrix, Point? point = null) => matrix.Transform(point ?? new Point(1, 1));
}
