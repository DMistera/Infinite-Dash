using System.Collections;
using UnityEngine;

public class Track : MonoBehaviour {

    public Player playerPrefab;

    private Vector3 endPosition = Vector3.zero;

    // Use this for initialization
    void Start() {

        AddChunk(ChunkLibrary.Instance.FirstChunk);

        /*GameStateHolder.Instance.State = GameState.PLAY;
        Player player = Instantiate(playerPrefab, transform);
        player.transform.position = Vector3.zero;*/
    }

    private void AddChunk(Chunk chunk) {
        Chunk chunkClone = chunk.Clone(transform);
        chunkClone.transform.localPosition = endPosition;
        chunkClone.gameObject.SetActive(true);
        endPosition += chunk.EndPosition + new Vector3(Constants.GRID_SIZE, 0, 0);
        chunkClone.OnPlayerEnter += (player) => {
            AddChunk(ChunkLibrary.Instance.GetNext());
        };
    }


}
