using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIController : PlayerController {

    public float reaction = 140f;
    private float lastChunkX = 0f;
    private readonly List<float> jumpPositions = new List<float>();
    private int nextIndex = 0;

    public void Start() {
        player.OnActiveChunkChange += (chunk) => {
            lastChunkX = 0f;
            jumpPositions.Clear();
            foreach(float x in chunk.Solution.JumpPositions) {
                float mean = player.forwardVelocity * reaction / 1000f;
                float stdDev = mean / Mathf.Sqrt(2f / Mathf.PI);
                float next = Utils.Math.NextGaussian(x, stdDev);
                jumpPositions.Add(next);
            }
            nextIndex = 0;
        };
    }

    public void Update() {
        if (nextIndex < jumpPositions.Count) {
            float x = jumpPositions[nextIndex];
            if (x > lastChunkX && x <= GetXInActiveChunk()) {
                player.OnSpace();
                nextIndex++;
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

