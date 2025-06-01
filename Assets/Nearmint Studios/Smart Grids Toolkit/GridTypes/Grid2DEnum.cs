namespace SmartGridsToolkit
{
    [System.Serializable]
    public class Grid2DEnum<T> : Grid2D<T> where T : System.Enum
    {
        public static readonly int DefaultCellWidth = 80;
        public static readonly int DefaultCellHeight = 20;
        public static readonly int DefaultPadding_W = 0;
        public static readonly int DefaultPadding_H = 0;

        public Grid2DEnum() : base(3, 3) { }

        public Grid2DEnum(int width, int height) : base(width, height) { }
    }
}
