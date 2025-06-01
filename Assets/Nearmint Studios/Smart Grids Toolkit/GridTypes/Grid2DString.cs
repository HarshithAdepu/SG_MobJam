namespace SmartGridsToolkit
{
    [System.Serializable]
    public class Grid2DString : Grid2D<string>
    {
        public static readonly int DefaultCellWidth = 30;
        public static readonly int DefaultCellHeight = 20;
        public static readonly int DefaultPaddingW = 5;
        public static readonly int DefaultPaddingH = 5;

        public Grid2DString() : base(3, 3) { }

        public Grid2DString(int width, int height) : base(width, height) { }
    }
}


