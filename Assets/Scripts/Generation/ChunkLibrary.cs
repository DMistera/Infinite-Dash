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

    public void Awake() {
        library = new LibraryEntry[librarySize];
        buffer = new Chunk[bufferSize];
    }

    public void Start() {
        StartCoroutine(Load());
        FirstChunk = ChunkGenerator.Instance.GenerateByTimeline(PlayerActionTimeline.First(), Difficulty.Initial(0f));
    }

    private IEnumerator Load() {
        if(generationMethod == GenerationMethod.STATIC) {
            yield return LoadLibrary();
        }
        else if(generationMethod == GenerationMethod.DYNAMIC) {
            yield return LoadBuffer();
        }
        GameManager.Instance.State = GameState.MENU;
    }

    public Chunk GetNext(Player player) {
        Debug.Log("Want: " + player.Skill.Rank());
        return generationMethod switch {
            GenerationMethod.STATIC => TakeFromLibrary(player),
            GenerationMethod.DYNAMIC => TakeFromBuffer(player),
            _ => throw new NotImplementedException(),
        };
    }

    private Chunk TakeFromLibrary(Player player) {
        float minDiff = float.MaxValue;
        int minCount = int.MaxValue;
        LibraryEntry chunkEntry = null;
        foreach (LibraryEntry entry in library) {
            if(entry == null) continue;
            float diff = player.Skill.Difference(entry.Chunk.Difficulty);
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

    private void AddChunkToLibrary(Player player, Chunk chunk) {
        int index = WorstLibraryEntryIndex(player);
        if(library[index] != null) {
            Destroy(library[index].Chunk.gameObject);
        }
        library[index] = new LibraryEntry(chunk);
    }

    private int WorstLibraryEntryIndex(Player player) {
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
            float diff = player.Skill.Difference(entry.Chunk.Difficulty);
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

    private IEnumerator LoadLibrary() {
        for (int i = 0; i < librarySize; i++) {
            Chunk chunk = ChunkGenerator.Instance.GenerateByDifficulty(Difficulty.Random());
            library[i] = new LibraryEntry(chunk);
            OnLoadStep?.Invoke(i, librarySize);
            yield return null;
        }
    }

    private IEnumerator LoadBuffer() {
        ClearBuffer();
        PlayerSkill skill = PlayerProfile.Instance.PlayerSkill;
        for (int i = 0; i < bufferSize; i++) {
            OnLoadStep?.Invoke(i, bufferSize);
            buffer[i] = ChunkGenerator.Instance.GenerateByDifficulty(skill.MakeSimilar(dynamicDifficultyRange));
            yield return null;
        }
    }

    private void RefillBuffer(Player player) {
        if (IsBufferFull()) {
            return;
        }
        if (!refillBufferLock) {
            refillBufferLock = true;
            PlayerSkill skill = player.Skill;
            ChunkGenerator.Instance.GenerateByDifficultyAsync(skill.MakeSimilar(dynamicDifficultyRange), (chunk) => {
                for (int i = 0; i < bufferSize; i++) {
                    if (buffer[i] == null) {
                        buffer[i] = chunk;
                        break;
                    }
                }
                refillBufferLock = false;
                RefillBuffer(player);
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

    private Chunk TakeFromBuffer(Player player) {
        Chunk result = buffer[0];
        for (int i = 0; i < bufferSize - 1; i++) {
            buffer[i] = buffer[i + 1];
        }
        buffer[bufferSize - 1] = null;
        if (result == null) {
            result = TakeFromLibrary(player);
        }
        else {
            Debug.Log("Generated chunk!");
            AddChunkToLibrary(player, result);
        }
        RefillBuffer(player);
        Debug.Log("Got: " + result.Difficulty.Rank());
        return result;
    }

    private class LibraryEntry {
        public LibraryEntry(Chunk chunk) {
            Chunk = chunk;
            UseCount = 0;
        }

        public Chunk Chunk { get; set; }
        public int UseCount { get; set; }
    }
}
