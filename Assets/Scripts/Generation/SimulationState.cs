using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SimulationState {
    public Chunk Chunk { get; set; }
    public Difficulty Difficulty { get; set; }
    public Player Player { get; set; }
    public PlayerAction CurrentAction { get; set; }
    public PlayerAction PreviousAction { get; set; }
    public int Steps { get; set; } = 0;
    public float ActionDeltaY { get; set; } = 0f;
}

