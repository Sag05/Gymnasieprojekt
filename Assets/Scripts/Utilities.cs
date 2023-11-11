using UnityEngine;

namespace Assets.Scripts
{
    public struct DoubleVector3{
        public Vector3 vector1;
        public Vector3 vector2;
    }

    internal class Utilities
    {
        public static Vector3 Secale6(
            Vector3 value,
            float xPositive, float xNegative,
            float yPositive, float yNegative,
            float zPositive, float zNegative)
        {
            Vector3 result = value;

            if(result.x > 0){
                result.x *= xPositive;
            } else {
                result.x *= xNegative;
            }

            if (result.y > 0){
                result.y *= yPositive;
            } else {
                result.y *= yNegative;
            }

            if (result.z > 0) {
                result.z *= zPositive;
            } else {
                result.z *= zNegative;
            }

            return result;
        }
    }
}
