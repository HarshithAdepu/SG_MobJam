namespace SmartGridsToolkit
{
    using UnityEngine;

    [System.Serializable]
    public class Grid2D<T>
    {
        [System.Serializable]
        public struct Grid2DRow
        {
            public T[] items;

            public Grid2DRow(int count)
            {
                items = new T[count];
                for (int i = 0; i < count; i++)
                {
                    items[i] = default;
                }
            }

            public T this[int i]
            {
                get { return items[i]; }
                set { items[i] = value; }
            }

            public int Length
            {
                get { return items.Length; }
            }
        }

        public int CellWidth = 30;
        public int CellHeight = 20;
        public int PaddingW = 5;
        public int PaddingH = 5;
        public bool CustomCellSizeSet = false;
        public bool CustomPaddingSet = false;

        public Grid2DRow[] Grid;
        public int WidthCount;   // The number of columns in the grid.
        public int HeightCount;  // The number of rows in the grid.

        /// <summary>
        /// Default constructor that initializes a 3x3 grid.
        /// </summary>
        public Grid2D() : this(3, 3) { }

        /// <summary>
        /// Initializes a grid with specified width and height.
        /// </summary>
        public Grid2D(int width, int height)
        {
            WidthCount = width;
            HeightCount = height;

            Grid = new Grid2DRow[HeightCount];
            for (int i = 0; i < HeightCount; i++)
            {
                Grid[i] = new Grid2DRow(WidthCount);
            }
        }

        public Grid2DRow this[int i]
        {
            get { return Grid[i]; }
        }

        public int Length
        {
            get { return Grid.Length; }
        }

        public void ResizeGrid(int newWidth, int newHeight)
        {
            Grid2D<T> newGrid = new Grid2D<T>(newWidth, newHeight);

            // Copy existing values to the new grid.
            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    if (i < HeightCount && j < WidthCount)
                    {
                        newGrid[i].items[j] = Grid[i][j];
                    }
                }
            }

            // Update the grid with the new dimensions and data.
            Grid = newGrid.Grid;
            WidthCount = newWidth;
            HeightCount = newHeight;
        }

        public T GetCellValue(int x, int y)
        {
            // The formula considers the y-axis inversion in the inspector
            int adjustedY = (HeightCount - 1) - y;

            if (adjustedY >= 0 && adjustedY < HeightCount && x >= 0 && x < WidthCount)
            {
                return Grid[adjustedY][x];
            }
            else
            {
                Debug.LogWarning("Requested cell is out of grid bounds.");
                return default;
            }
        }

        public void SetCellValue(int x, int y, T value)
        {
            // The formula considers the y-axis inversion in the inspector
            int adjustedY = (HeightCount - 1) - y;

            if (adjustedY >= 0 && adjustedY < HeightCount && x >= 0 && x < WidthCount)
            {
                Grid[adjustedY][x] = value;
            }
            else
            {
                Debug.LogWarning("Trying to set a value out of grid bounds.");
            }
        }
    }
}