using System;
using System.Collections;
using UnityEngine;


public class ChunkLibrary : MonoBehaviour {

    public static ChunkLibrary Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<ChunkLibrary>();
            }
            return instance;
        }
    }
    private static ChunkLibrary instance;

    public static readonly int SIZE = 10;
    public Chunk chunkPrefab;
    public Chunk FirstChunk { get; private set; }
    public Action<int> OnLoadStep;

    public void Start() {
        StartCoroutine(Load());
        FirstChunk = GenerateFirstChunk();
    }

    private IEnumerator Load() {
        for (int i = 0; i < SIZE; i++) {
            Chunk chunk = Instantiate(chunkPrefab, transform);
            chunk.GenerateByDifficulty(ChunkDifficulty.Random());
            chunk.gameObject.SetActive(false);
            OnLoadStep?.Invoke(i);
            yield return null;
        }
        GameStateHolder.Instance.State = GameState.MENU;
    }

    public Chunk GetNext() {
        Chunk[] chunks = GetChunks();
        return chunks[UnityEngine.Random.Range(0, chunks.Length - 1)];
    }

    private Chunk[] GetChunks() {
        return GetComponentsInChildren<Chunk>(true);
    }

    private Chunk GenerateFirstChunk() {
        Chunk chunk = Instantiate(chunkPrefab, transform);
        chunk.GenerateByTimeline(PlayerActionTimeline.First(), 0f);
        chunk.gameObject.SetActive(false);
        return chunk;
    }
}
