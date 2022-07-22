using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EloMatch {
    public float Difference { get; set; }
    public float Score { get; set; }

    public override string ToString() {
        return $"{Difference},{(int)Score}";
    }
}

