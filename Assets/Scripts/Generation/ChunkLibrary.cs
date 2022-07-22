using System;
using System.Collections;
using System.Linq;
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

    public int librarySize = 100;
    public int bufferSize = 2;
    public float dynamicDifficultyRange = 0.1f;
    public Chunk chunkPrefab;
    public GenerationMethod generationMethod;

    public Chunk FirstChunk { get; private set; }
    public Action<int, int> OnLoadStep;
    private LibraryEntry[] library;
    private Chunk[] buffer;
    private bool refillBufferLock = false;

    private PlayerSkill activeSkill;

    public void Awake() {
        library = new LibraryEntry[librarySize];
        buffer = new Chunk[bufferSize];
    }

    public void Start() {
        StartCoroutine(Load());
        FirstChunk = ChunkGenerator.Instance.GenerateByTimeline(PlayerActionTimeline.First(), Difficulty.Initial(0f));
        FirstChunk.Ranked = false;
    }

    private IEnumerator Load() {
        if(generationMethod == GenerationMethod.STATIC) {
            yield return LoadLibrary();
        }
        else if(generationMethod == GenerationMethod.DYNAMIC) {
            yield return LoadBuffer(PlayerProfile.Instance.PlayerSkill);
        }
        GameManager.Instance.State = GameState.MENU;
    }

    public Chunk GetNext(PlayerSkill skill) {
        activeSkill = skill;
        return generationMethod switch {
            GenerationMethod.STATIC => TakeFromLibrary(),
            GenerationMethod.DYNAMIC => TakeFromBuffer(),
            _ => throw new NotImplementedException(),
        };
    }

    private Chunk TakeFromLibrary() {
        float minDiff = float.MaxValue;
        int minCount = int.MaxValue;
        LibraryEntry chunkEntry = null;
        foreach (LibraryEntry entry in library) {
            if(entry == null) continue;
            float diff = activeSkill.Difference(entry.Chunk.Difficulty);
            if (entry.UseCount < minCount) {
                minCount = entry.UseCount;
                minDiff = diff;
                chunkEntry = entry;
            }
            if (entry.UseCount <= minCount) {
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

    private void AddChunkToLibrary(Chunk chunk) {
        int index = WorstLibraryEntryIndex();
        if(library[index] != null) {
            Destroy(library[index].Chunk.gameObject);
        }
        library[index] = new LibraryEntry(chunk, LowestUseCount());
    }

    private int WorstLibraryEntryIndex() {
        for (int i = 0; i < librarySize; i++) {
            if (library[i] == null) {
                return i;
            }
        }
        float maxDiff = 0;
        int maxCount = 0;
        int index = 0;
        for (int i = 0; i < librarySize; i++) {
            LibraryEntry entry = library[i];
            if (entry == null) continue;
            float diff = activeSkill.Difference(entry.Chunk.Difficulty);
            if (entry.UseCount > maxCount) {
                maxCount = entry.UseCount;
                maxDiff = diff;
                index = i;
            }
            if (entry.UseCount >= maxCount) {
                if (diff > maxDiff) {
                    maxDiff = diff;
                    index = i;
                }
            }
        }
        return index;
    }

    private int LowestUseCount() {
        int min = int.MaxValue;
        foreach (LibraryEntry entry in library) {
            if(entry != null && entry.UseCount < min) {
                min = entry.UseCount;
            }
        }
        if (min == int.MaxValue) {
            return 0;
        }
        return min;
    }

    private IEnumerator LoadLibrary() {
        for (int i = 0; i < librarySize; i++) {
            Chunk chunk = ChunkGenerator.Instance.GenerateByDifficulty(Difficulty.Random());
            library[i] = new LibraryEntry(chunk);
            OnLoadStep?.Invoke(i, librarySize);
            yield return null;
        }
    }

    private IEnumerator LoadBuffer(PlayerSkill skill) {
        ClearBuffer();
        for (int i = 0; i < bufferSize; i++) {
            OnLoadStep?.Invoke(i, bufferSize);
            buffer[i] = ChunkGenerator.Instance.GenerateByDifficulty(skill.MakeSimilar(dynamicDifficultyRange));
            yield return null;
        }
    }

    private void RefillBuffer() {
        if (IsBufferFull()) {
            return;
        }
        if (!refillBufferLock) {
            refillBufferLock = true;
            Difficulty difficulty = activeSkill.MakeSimilar(dynamicDifficultyRange);
            ChunkGenerator.Instance.GenerateByDifficultyAsync(difficulty, (chunk) => {
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
        else {
            AddChunkToLibrary(result);
        }
        RefillBuffer();
        return result;
    }

    private class LibraryEntry {

        public LibraryEntry(Chunk chunk, int useCount) {
            Chunk = chunk;
            UseCount = useCount;
        }

        public LibraryEntry(Chunk chunk) : this(chunk, 0) {
        }

        public Chunk Chunk { get; set; }
        public int UseCount { get; set; }
    }
}
