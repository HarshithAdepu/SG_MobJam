
namespace SmartGridsToolkit
{
    using UnityEngine;

    public enum ExampleEnum
    {
       Red, Green, Yellow
    }

    public class GridGenerator : MonoBehaviour
    {
        [Header("Test Object")]
        public GameObject whiteCube;
        public GameObject numberedQuad;
        public GameObject redObject;
        public GameObject greenObject;
        public GameObject yellowObject;

        [Header("Select Your Test")]
        public bool mazeGridLayout;
        public bool quadGridWithValues;
        public bool enumGrid;

        [Header("2D Grids")]
        public Grid2DBool mazeGrid;
        public Grid2DFloat quadNumberedGrid;
        public Grid2DEnum<ExampleEnum> colorEnum;

        private void Start()
        {
            if (mazeGridLayout)
                GenerateMaze();
            else if (quadGridWithValues)
                QuadGrid();
            else if (enumGrid)
                EnumGridTest();
        }

        void GenerateMaze()
        {
            for (int i = 0; i < mazeGrid.WidthCount; i++)
            {
                for (int j = 0; j < mazeGrid.HeightCount; j++)
                {
                    var cellValue = mazeGrid.GetCellValue(i, j);

                    if (cellValue)
                    {
                        GameObject cube = Instantiate(whiteCube);
                        cube.transform.position = new Vector3(i, 0, j);
                    }
                }
            }
        }

        void QuadGrid()
        {
            for (int i = 0; i < quadNumberedGrid.WidthCount; i++)
            {
                for (int j = 0; j < quadNumberedGrid.HeightCount; j++)
                {
                    var cellValue = quadNumberedGrid.GetCellValue(i, j);

                    GameObject quad = Instantiate(numberedQuad);
                    quad.transform.position = new Vector3(i, 0.1f, j);

                    quad.GetComponentInChildren<TextMesh>().text = cellValue.ToString(); //2
                }
            }
        }

        void EnumGridTest()
        {
            for (int i = 0; i < colorEnum.WidthCount; i++)
            {
                for (int j = 0; j < colorEnum.HeightCount; j++)
                {
                    var cellValue = colorEnum.GetCellValue(i, j);

                    switch (cellValue)
                    {
                        case ExampleEnum.Red:
                            GameObject obj1 = Instantiate(redObject);
                            obj1.transform.position = new Vector3(i, 0f, j);
                            break;
                        case ExampleEnum.Green:
                            GameObject obj2 = Instantiate(greenObject);
                            obj2.transform.position = new Vector3(i, 0f, j);
                            break;
                        case ExampleEnum.Yellow:
                            GameObject obj3 = Instantiate(yellowObject);
                            obj3.transform.position = new Vector3(i, 0f, j);
                            break;
                    }
                }
            }
        }

    }
}


