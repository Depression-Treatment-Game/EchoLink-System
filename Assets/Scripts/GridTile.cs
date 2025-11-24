using UnityEngine;

public class GridTile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Grid Position")]
    [SerializeField] private int gridX = 0;
    [SerializeField] private int gridY = 0;

    [Header("Current Tile State")]
    [SerializeField] private string currentType = "Ground"; // "Ground", "Grass", or "Water"
    [SerializeField] private bool isOccupied = false;

    [Header("Grass Colors")]
    private Color groundColor = new Color(0.63f, 0.47f, 0.31f); // Brown
    private Color dryGrassColor = new Color(0.65f, 0.8f, 0.3f); // Yellowish green  
    private Color healthyGrassColor = new Color(0.2f, 0.5f, 0.2f); // Dark green (near water)
    private Color eatenGrassColor = new Color(0.5f, 0.7f, 0.5f); // Medium green (after eaten)

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileType(string type, Color color)
    {
        if (type == "Bunny")
        {
            // CHECK FIRST if tile is occupied
            if (isOccupied)
            {
                Debug.Log($"Tile is already occupied with {currentType}! Cannot place Bunny.");
                return;
            }

            // set the type
            currentType = type;
            spriteRenderer.color = color;
            isOccupied = true;

            Debug.Log($"Bunny placed at ({gridX},{gridY})");

            // Try to eat adjacent grass
            EatAdjacentGrass();

            return;
        }

        // NORMAL CASE: Grass or Water placement
        if (isOccupied)
        {
            Debug.Log($"Tile is already occupied with {currentType}! Cannot place {type}.");
            return;
        }

        currentType = type;
        spriteRenderer.color = color;
        isOccupied = true;

        Debug.Log($"Tile ({gridX},{gridY}) changed to: {type} - Now LOCKED");

        // After placing, check if this affects nearby grass
        CheckAndUpdateNearbyGrass();
    }

    // Check if there's water nearby and update grass accordingly
    public void UpdateGrassGrowth()
    {
        if (currentType != "Grass") // Only grass tiles grow
        {
            return;
        }

        // Don't update if grass was already eaten
        if (spriteRenderer.color == eatenGrassColor)
        {
            Debug.Log($"Grass at ({gridX},{gridY}) was already eaten.");
            return;
        }

        bool hasWaterNearby = CheckForAdjacentWater();

        if (hasWaterNearby)
        {
            spriteRenderer.color = healthyGrassColor; // Dark green
            Debug.Log($"Grass at ({gridX},{gridY}) is healthy! Water nearby!");

            // Check if there's a bunny nearby that should eat this grass
            CheckForAdjacentBunnyAndGetEaten();
        }
        else
        {
            spriteRenderer.color = dryGrassColor; // Light green
            Debug.Log($"Grass at ({gridX},{gridY}) is dry. No water nearby.");
        }
    }

    // Check if grass is healthy (dark green) and can be eaten
    public bool IsHealthyGrass()
    {
        return currentType == "Grass" && spriteRenderer.color == healthyGrassColor;
    }

    public void EatGrass()
    {
        if (IsHealthyGrass())
        {
            spriteRenderer.color = eatenGrassColor; // Change to medium green
            Debug.Log($"Grass at ({gridX},{gridY}) has been eaten! Yum!");
        }
    }

    // Check 4 directions for water (not diagonal)
    private bool CheckForAdjacentWater()
    {
        // Get all tiles in the scene
        GridTile[] allTiles = FindObjectsByType<GridTile>(FindObjectsSortMode.None);

        // Check each direction (up, down, left, right)
        int[] dx = { 0, 0, -1, 1 };  // x offsets: none, none, left, right
        int[] dy = { 1, -1, 0, 0 };  // y offsets: up, down, none, none
        string[] dirNames = { "Up", "Down", "Left", "Right" };

        for (int i = 0; i < 4; i++)
        {
            int checkX = gridX + dx[i];
            int checkY = gridY + dy[i];

            Debug.Log($"Checking {dirNames[i]}: ({checkX},{checkY}) from ({gridX},{gridY})");

            // Find a tile at this position
            foreach (GridTile tile in allTiles)
            {
                if (tile.gridX == checkX && tile.gridY == checkY)
                {
                    Debug.Log($"Found tile at ({checkX},{checkY}): Type = {tile.currentType}");

                    if (tile.currentType == "Water")
                    {
                        Debug.Log($"Water found at {dirNames[i]}!");
                        return true;
                    }
                }
            }
        }

        Debug.Log("No water found nearby.");
        return false;
    }

    // When a tile changes, update all nearby grass tiles
    private void CheckAndUpdateNearbyGrass()
    {
        // Find all grass tiles and tell them to check for water
        GridTile[] allTiles = FindObjectsByType<GridTile>(FindObjectsSortMode.None);

        foreach (GridTile tile in allTiles)
        {
            if (tile.currentType == "Grass")
            {
                tile.UpdateGrassGrowth();
            }
        }
    }

    public void ResetToGround()
    {
        currentType = "Ground";
        spriteRenderer.color = groundColor;
        isOccupied = false;
    }

    private void EatAdjacentGrass()
    {
        // Get all tiles in the scene
        GridTile[] allTiles = FindObjectsByType<GridTile>(FindObjectsSortMode.None);

        // Check each direction (up, down, left, right)
        int[] dx = { 0, 0, -1, 1 };  // x offsets
        int[] dy = { 1, -1, 0, 0 };  // y offsets
        string[] dirNames = { "Up", "Down", "Left", "Right" };

        for (int i = 0; i < 4; i++)
        {
            int checkX = gridX + dx[i];
            int checkY = gridY + dy[i];

            Debug.Log($"Bunny checking {dirNames[i]}: ({checkX},{checkY})");

            // Find a tile at this position
            foreach (GridTile tile in allTiles)
            {
                if (tile.gridX == checkX && tile.gridY == checkY)
                {
                    // If it's healthy grass, eat it!
                    if (tile.IsHealthyGrass())
                    {
                        tile.EatGrass();
                        Debug.Log($"Bunny ate grass at ({checkX},{checkY}) - {dirNames[i]}!");
                    }
                    break; // Found the tile, move to next direction
                }
            }
        }
    }

    private void CheckForAdjacentBunnyAndGetEaten()
    {
        // Get all tiles in the scene
        GridTile[] allTiles = FindObjectsByType<GridTile>(FindObjectsSortMode.None);

        // Check each direction (up, down, left, right)
        int[] dx = { 0, 0, -1, 1 };  // x offsets
        int[] dy = { 1, -1, 0, 0 };  // y offsets

        for (int i = 0; i < 4; i++)
        {
            int checkX = gridX + dx[i];
            int checkY = gridY + dy[i];

            // Find a tile at this position
            foreach (GridTile tile in allTiles)
            {
                if (tile.gridX == checkX && tile.gridY == checkY)
                {
                    // If there's a bunny, trigger it to eat!
                    if (tile.currentType == "Bunny")
                    {
                        Debug.Log($"Found bunny at ({checkX},{checkY}) - triggering eating!");
                        // Trigger the bunny to eat its adjacent grass
                        tile.EatAdjacentGrass();
                        return; // Stop checking
                    }
                    break; // Found the tile, move to next direction
                }
            }
        }
    }
}

