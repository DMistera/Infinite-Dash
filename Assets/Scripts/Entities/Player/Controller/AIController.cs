using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AIController : PlayerController {

    private float lastChunkX = 0f;

    public void Start() {
        player.OnActiveChunkChange += (chunk) => {
            lastChunkX = 0f;
        };
    }
    public void Update() {
        if (player.ActiveChunk != null) {
            foreach (float x in player.ActiveChunk.Solution.JumpPositions) {
                if (x > lastChunkX && x <= GetXInActiveChunk()) {
                    player.OnSpace();
                }
            }
        }
        lastChunkX = GetXInActiveChunk();
    }

    private float GetXInActiveChunk() {
        if (player.ActiveChunk == null) {
            return 0f;
        }
        return (transform.position - player.ActiveChunk.transform.position).x;
    }
}

