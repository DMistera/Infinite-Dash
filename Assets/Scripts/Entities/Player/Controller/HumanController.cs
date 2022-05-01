using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HumanController : PlayerController {
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            player.OnSpace();
        }
    }
}

