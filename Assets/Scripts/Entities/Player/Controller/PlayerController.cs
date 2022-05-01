using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Player))]
public abstract class PlayerController : MonoBehaviour {
    protected Player player;

    public void Awake() {
        player = GetComponent<Player>();
    }
}

