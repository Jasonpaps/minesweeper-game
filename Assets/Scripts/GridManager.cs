using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public RectTransform gridParent;
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

        float tileSize = 50f;

        // 🧮 Step 1: Calculate total grid size
        float totalWidth = width * tileSize;
        float totalHeight = height * tileSize;

        // 🎯 Step 2: Compute center offset
        Vector2 offset = new Vector2(-totalWidth / 2 + tileSize / 2, totalHeight / 2 - tileSize / 2);

        // 🔁 Step 3: Create tiles with offset
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject tileGO = Instantiate(tilePrefab, gridParent);
                tileGO.name = $"Tile {x},{y}";

                RectTransform rt = tileGO.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x * tileSize, -y * tileSize) + offset;

                Tile tile = tileGO.GetComponent<Tile>();
                tile.Init(x, y, this);
                tiles[x, y] = tile;
            }
        }

        // Place mines randomly
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
        GameManager.Instance.StartGame(mineCount);
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
