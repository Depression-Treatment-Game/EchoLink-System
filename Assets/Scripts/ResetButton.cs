using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public void ResetGame()
    {
        Debug.Log("Resetting game...");

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //public void ResetGameWithoutReload()
    //{
    //    Debug.Log("Resetting tiles...");

    //    // Find all grid tiles and reset them
    //    GridTile[] allTiles = FindObjectsByType<GridTile>(FindObjectsSortMode.None);

    //    foreach (GridTile tile in allTiles)
    //    {
    //        tile.ResetToGround();
    //    }

    //    Debug.Log("Game reset complete!");
    //}

}
