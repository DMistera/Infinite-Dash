using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils {
    public class Math {
        public static float NextGaussian(float mean, float stdDev) {
             //reuse this if you are generating many
            float u1 = 1.0f - UnityEngine.Random.value; //uniform(0,1] random doubles
            float u2 = 1.0f - UnityEngine.Random.value;
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                         Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }

        public static Vector2 RotateVector(Vector2 v, float delta) {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        public static float Round(float value, float denominator = 1f) {
            float remain = value % denominator;
            if(remain < denominator / 2f) {
                return value - remain;
            }
            else {
                return value - remain + denominator;
            }

        }
    }
}
