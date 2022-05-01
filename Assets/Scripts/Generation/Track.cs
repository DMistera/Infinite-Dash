using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {

    public Player playerPrefab;

    public Action OnReset;

    private Vector3 endPosition = Vector3.zero;
    private bool shouldAddChunk = false;
    private readonly List<Chunk> chunks = new List<Chunk>();

    public Player Player { get; private set; }

    // Use this for initialization
    void Start() {
        Reset();
    }

    public void Reset() {
        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }
        chunks.Clear();
        endPosition = Vector3.zero;
        Player = Instantiate(playerPrefab, transform);
        OnReset?.Invoke();
        AddChunk(ChunkLibrary.Instance.FirstChunk);
    }

    void Update() {
        if (shouldAddChunk) {
            AddChunk(ChunkLibrary.Instance.GetNext(Player));
            shouldAddChunk = false;
        }

        if (chunks.Count > 3) {
            Destroy(chunks[0].gameObject);
            chunks.RemoveAt(0);
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
        chunks.Add(chunkClone);
    }
}
