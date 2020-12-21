using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public class GameTowerFactory
    {
        private static Dictionary<string, Type> m_tower_types = new Dictionary<string, Type>();

        static GameTowerFactory()
        {
            Type base_type = typeof(GameTower);

            Type[] types = GameUtils.GetClassTypes(System.Reflection.Assembly.GetExecutingAssembly(), base_type.Namespace);

            foreach (Type t in types)
            {
                if (!t.IsAbstract && base_type.IsAssignableFrom(t))
                {
                    m_tower_types[t.FullName] = t;
                }
            }
        }

        public static Dictionary<string, Type> TowerTypes
        {
            get 
            {
                return m_tower_types;
            }
        }

        public static Type GetTowerType(string method)
        {
            string name = String.Format("ACQ.DroneDefenceGame.{0}Tower", method);

            Type result;

            m_tower_types.TryGetValue(name, out result); //returs null if not found

            return result;
        }

        public static GameTower GetTower(Type type, double[] x, double[] y)
        {
            GameTower interpolator = Activator.CreateInstance(type, x, y) as GameTower;

            return interpolator;
        }

        public static GameTower GetTower(Type type, params object[] arguments)
        {
            GameTower interpolator = Activator.CreateInstance(type, arguments) as GameTower;

            return interpolator;
        }

        public static GameTower GetInterpolator(string method, params object[] arguments)
        {
            GameTower tower = null;

            Type tower_type = GetTowerType(method);

            if (tower_type != null)
            {
                tower = Activator.CreateInstance(tower_type, arguments) as GameTower;
            }

            return tower;
        }
    }
}
