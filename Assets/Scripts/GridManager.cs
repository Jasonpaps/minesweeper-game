using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 8;
    public int height = 8;
    public int mineCount = 10;

    private Tile[,] tiles;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        tiles = new Tile[width, height];
        int minesPlaced = 0;

        float tileSize = 50f; // Match your button size in pixels

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject tileGO = Instantiate(tilePrefab, transform);
                tileGO.name = $"Tile {x},{y}";

                RectTransform rt = tileGO.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x * tileSize, -y * tileSize); // Y is negative to go down

                Tile tile = tileGO.GetComponent<Tile>();
                tile.Init(x, y, this);
                tiles[x, y] = tile;
            }
        }

        // Randomly assign mines
        while (minesPlaced < mineCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            Tile t = tiles[x, y];
            if (!t.isMine)
            {
                t.isMine = true;
                minesPlaced++;
            }
        }

        int nonMineTiles = (width * height) - mineCount;
        GameManager.Instance.SetTotalTiles(nonMineTiles);
        GameManager.Instance.StartGame(mineCount); // Start timer and init mine counter
    }

    public int GetSurroundingMineCount(int x, int y)
    {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (tiles[nx, ny].isMine)
                        count++;
                }
            }
        }

        return count;
    }

    public void FloodFill(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (dx == 0 && dy == 0) continue;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    Tile neighbor = tiles[nx, ny];

                    if (!neighbor.isMine && neighbor.GetComponent<Button>().interactable)
                    {
                        int count = GetSurroundingMineCount(nx, ny);
                        neighbor.Reveal(); // let each neighbor handle its logic
                    }
                }
            }
        }
    }
}
