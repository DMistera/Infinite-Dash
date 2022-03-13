public class ChunkDifficulty {
    public float Path;
    public float Obstacles;

    public static ChunkDifficulty Random() {
        return new ChunkDifficulty() {
            Path = UnityEngine.Random.value,
            Obstacles = UnityEngine.Random.value
        };
    }
}
