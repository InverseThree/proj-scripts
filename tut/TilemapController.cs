using UnityEngine;
using UnityEngine.Tilemaps;
using Fungus;

public class TilemapController : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase baseTile;
    public TileBase correctTile;
    public TileBase wrongTile;

    private TileBase newTile;

    public Flowchart flowchart;

    private bool revertTiles = false;

    public void ReplaceTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase currentTile = tilemap.GetTile(pos);

            if (revertTiles)
                tilemap.SetTile(pos, baseTile);
            else if (tilemap.HasTile(pos))
            {
                newTile = flowchart.GetBooleanVariable("ansCorrect") ? correctTile : wrongTile;
                tilemap.SetTile(pos, newTile);
            }
        }
         
        revertTiles = !revertTiles;
    }
}
