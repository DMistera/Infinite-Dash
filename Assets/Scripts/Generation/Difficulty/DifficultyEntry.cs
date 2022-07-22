using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DifficultyEntry {
    [SerializeField]
    public DifficultyType type;
    [SerializeField]
    public float value;
}

