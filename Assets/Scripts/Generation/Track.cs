using System.Collections;
using UnityEngine;

public class Track : MonoBehaviour {

    public Player playerPrefab;

    private Vector3 endPosition = Vector3.zero;
    private bool shouldAddChunk = false;

    // Use this for initialization
    void Start() {
        Instantiate(playerPrefab, transform);
        AddChunk(ChunkLibrary.Instance.FirstChunk);
        //AddChunk(ChunkLibrary.Instance.GetNext());
    }

    void Update() {
        if (shouldAddChunk) {
            AddChunk(ChunkLibrary.Instance.GetNext());
            shouldAddChunk = false;
        }
    }

    private void AddChunk(Chunk chunk) {
        Chunk chunkClone = chunk.Clone(transform);
        chunkClone.transform.localPosition = endPosition;
        chunkClone.gameObject.SetActive(true);
        endPosition += chunk.EndPosition;
        chunkClone.OnPlayerEnter += (player) => {
            shouldAddChunk = true;
        };
    }
}
