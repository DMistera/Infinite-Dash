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

    public int size = 100;
    public int bufferSize = 2;
    public float dynamicDifficultyRange = 0.1f;
    public Chunk chunkPrefab;
    public GenerationMethod generationMethod;

    public Chunk FirstChunk { get; private set; }
    public Action<int> OnLoadStep;
    private ChunkEntry[] chunks;
    private Chunk[] buffer;
    private bool refillBufferLock = false;

    public void Awake() {
        chunks = new ChunkEntry[size];
        buffer = new Chunk[bufferSize];
    }

    public void Start() {
        StartCoroutine(Load());
        FirstChunk = ChunkGenerator.Instance.GenerateByTimeline(PlayerActionTimeline.First(), Difficulty.Initial(0f));
    }

    private IEnumerator Load() {
        for (int i = 0; i < size; i++) {
            chunks[i] = new ChunkEntry() {
                Chunk = ChunkGenerator.Instance.GenerateByDifficulty(Difficulty.Random()),
                UseCount = 0
            };
            OnLoadStep?.Invoke(i);
            yield return null;
        }
        LoadBuffer();
        GameManager.Instance.State = GameState.MENU;
    }

    public Chunk GetNext() {
        switch (generationMethod) {
            case GenerationMethod.STATIC:
                return TakeFromLibrary();
            case GenerationMethod.DYNAMIC:
                return TakeFromBuffer();
            default:
                throw new NotImplementedException();
        }
    }

    private Chunk TakeFromLibrary() {
        float minDiff = float.MaxValue;
        int minCount = int.MaxValue;
        ChunkEntry chunkEntry = chunks[0];
        foreach (ChunkEntry entry in chunks) {
            if (entry.UseCount < minCount) {
                minCount = entry.UseCount;
            }
            if (entry.UseCount <= minCount) {
                float diff = PlayerProfile.Instance.PlayerSkill.Difference(entry.Chunk.Difficulty);
                if (diff < minDiff) {
                    minDiff = diff;
                    chunkEntry = entry;
                }
            }
        }
        chunkEntry.UseCount++;
        return chunkEntry.Chunk;
    }

    private void ClearBuffer() {
        for (int i = 0; i < bufferSize; i++) {
            if (buffer[i] != null) {
                Destroy(buffer[i].gameObject);
                buffer[i] = null;
            }
        }
    }

    public void LoadBuffer() {
        ClearBuffer();
        PlayerSkill skill = PlayerProfile.Instance.PlayerSkill;
        for (int i = 0; i < bufferSize; i++) {
            buffer[i] = ChunkGenerator.Instance.GenerateByDifficulty(skill.MakeSimilar(dynamicDifficultyRange));
        }
    }

    private void RefillBuffer() {
        if (IsBufferFull()) {
            return;
        }
        if (!refillBufferLock) {
            refillBufferLock = true;
            PlayerSkill skill = PlayerProfile.Instance.PlayerSkill;
            ChunkGenerator.Instance.GenerateByDifficultyAsync(skill.MakeSimilar(dynamicDifficultyRange), (chunk) => {
                for (int i = 0; i < bufferSize; i++) {
                    if (buffer[i] == null) {
                        buffer[i] = chunk;
                        break;
                    }
                }
                refillBufferLock = false;
                RefillBuffer();
            });
        }
    }

    private bool IsBufferFull() {
        foreach (Chunk chunk in buffer) {
            if(chunk == null) {
                return false;
            }
        }
        return true;
    }

    private Chunk TakeFromBuffer() {
        Chunk result = buffer[0];
        for (int i = 0; i < bufferSize - 1; i++) {
            buffer[i] = buffer[i + 1];
        }
        buffer[bufferSize - 1] = null;
        if (result == null) {
            result = TakeFromLibrary();
        }
        RefillBuffer();
        return result;
    }

    private class ChunkEntry {
        public Chunk Chunk { get; set; }
        public int UseCount { get; set; }
    }
}
