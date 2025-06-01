namespace SmartGridsToolkit
{
    [System.Serializable]
    public class Grid2DInt : Grid2D<int>
    {
        public static readonly int DefaultCellWidth = 20;
        public static readonly int DefaultCellHeight = 20;
        public static readonly int DefaultPadding_W = 3;
        public static readonly int DefaultPadding_H = 3;

        public Grid2DInt() : base(3, 3) { }

        public Grid2DInt(int width, int height) : base(width, height) { }
    }
}