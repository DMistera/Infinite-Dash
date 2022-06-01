using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlatformPolicy : ChunkGenerationPolicy {

    public Solid platformPrefab;
    public Solid blockPrefab;
    private int count = 0;
    private int sum = 0;

    public override void ActionEnter(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
            SpawnSolid(state, 1);
            if(state.PreviousAction != null) {
                SpawnExtension(state, true);
            }
        }
    }

    public override void ActionExit(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.SLIDE && state.NextAction != null && state.NextAction.Type == PlayerActionType.JUMP) {
            SpawnExtension(state, false);
        }
    }

    public override void Finally(SimulationState state) {
        float avg = (float)sum / count;
        state.Difficulty.Set(DifficultyType.PLATFORM_LENGTH, (3f - avg) / 3f);
    }

    public override void Step(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
            int lenght = (state.CurrentAction as SlideAction).Length;
            int h = lenght - Mathf.Abs(lenght / 2 - state.Steps);
            h += UnityEngine.Random.Range(-2, 2);
            h = Mathf.Clamp(h, 1, 8);
            SpawnSolid(state, h);
        }
    }

    private void SpawnExtension(SimulationState state, bool reverse) {
        float max = (1f - state.Difficulty.Get(DifficultyType.PLATFORM_LENGTH)) * 3f;
        int length = Mathf.RoundToInt(UnityEngine.Random.Range(0f, max));
        sum += length;
        count++;
        for (int i = 0; i < length; i++) {
            Solid solid = Instantiate(platformPrefab, state.Chunk.transform);
            Vector3 v = state.Player.transform.localPosition;
            int mul = reverse ? -1 : 1;
            v.y -= Constants.GRID_SIZE;
            v.x += (i + 1) * mul;
            state.Chunk.Grid.Set(solid, v);
        }
    }

    private void SpawnSolid(SimulationState state, int height) {
        Vector3 v = state.Player.transform.localPosition;
        for(int i = 0; i < height; i++) {
            Solid solid = Instantiate(blockPrefab, state.Chunk.transform);
            v.y -= Constants.GRID_SIZE;
            state.Chunk.Grid.Set(solid, v);
        }
        v = state.Player.transform.localPosition;
        if (v.y < Mathf.Round(v.y)) {
            v.y = Mathf.Round(v.y);
            state.Player.transform.localPosition = v;
            state.Player.SetVelocityY(0f);
        }
    }
}

