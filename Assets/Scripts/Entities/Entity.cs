using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Entity : MonoBehaviour {

    public Entity Clone(Transform parent) {
        Entity clone = Instantiate(this, parent);
        clone.OriginPolicy = OriginPolicy;
        return clone;
    }
    public ChunkGenerationPolicy OriginPolicy { get; set; }
}

