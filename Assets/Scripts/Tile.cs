using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isMine;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    private bool isFlagged = false;
    private bool isHolding = false;
    private float holdTime = 0f;
    private float holdThreshold = 0.5f;

    private int x, y;
    private GridManager grid;

    public void Init(int x, int y, GridManager grid)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
    }

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("ERROR: TextMeshProUGUI component not found in TileButton prefab!");
        }
        else
        {
            text.text = string.Empty;
        }

    }

    void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdThreshold)
            {
                isHolding = false;
                ToggleFlag();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHolding = true;
        holdTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        if (holdTime < holdThreshold)
        {
            Reveal();
        }

        isHolding = false;
        holdTime = 0f;
    }

    void ToggleFlag()
    {
        if (!button.interactable) return;

        isFlagged = !isFlagged;
        text.text = isFlagged ? "🚩" : "";

        // 🔄 Notify GameManager of flag change
        GameManager.Instance.UpdateFlags(isFlagged ? +1 : -1);
    }


    public void Reveal()
    {
        if (!button.interactable || isFlagged) return;

        button.interactable = false;

        if (isMine)
        {
            text.text = "💣";
            GetComponent<Image>().color = Color.red;
            GameManager.Instance.GameOver();
            return;
        }

        int mineCount = grid.GetSurroundingMineCount(x, y);
        text.text = mineCount > 0 ? mineCount.ToString() : "";
        GetComponent<Image>().color = Color.white;

        // ✅ Register reveal with GameManager
        GameManager.Instance.RegisterReveal();

        if (mineCount == 0)
        {
            grid.FloodFill(x, y);
        }
    }

}
