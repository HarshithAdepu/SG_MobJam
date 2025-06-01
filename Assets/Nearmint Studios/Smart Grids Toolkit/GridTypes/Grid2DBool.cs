namespace SmartGridsToolkit
{
    [System.Serializable]
    public class Grid2DBool : Grid2D<bool>
    {
        public static readonly int DefaultCellWidth = 15;
        public static readonly int DefaultCellHeight = 15;
        public static readonly int DefaultPadding_W = 1;
        public static readonly int DefaultPadding_H = 1;

        public Grid2DBool() : base(3, 3) { }

        public Grid2DBool(int width, int height) : base(width, height) { }
    }
}


