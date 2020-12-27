using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ACQ.DroneDefenceGame
{    public class GameUtils
    {  
        public static Type[] GetClassTypes(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => t.IsClass && String.Equals(t.Namespace, nameSpace)).ToArray();
        }
        public static double Sqr(double x)
        {
            return x * x;
        }

        public static float Bound(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
