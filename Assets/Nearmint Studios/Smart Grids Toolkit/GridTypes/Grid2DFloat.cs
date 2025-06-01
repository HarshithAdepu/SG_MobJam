namespace SmartGridsToolkit
{
    [System.Serializable]
    public class Grid2DFloat : Grid2D<float>
    {
        public static readonly int DefaultCellWidth = 25;
        public static readonly int DefaultCellHeight = 25;
        public static readonly int DefaultPadding_W = 5;
        public static readonly int DefaultPadding_H = 5;

        public Grid2DFloat() : base(3, 3) { }

        public Grid2DFloat(int width, int height) : base(width, height) { }
    }
}


