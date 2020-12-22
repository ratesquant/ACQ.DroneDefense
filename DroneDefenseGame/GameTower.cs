using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public enum enTowerLevel
    {
        Novice,
        Seasoned,
        Veteran
    }

    public abstract class GameTower: IGridPositionable
    {
        private GridPosition m_position;
        private int m_ammo;
        protected List<GameAgent> m_targets;

        public GameTower(GridPosition position, int ammo)
        {
            m_position = position;
            m_ammo = ammo;
            m_targets = new List<GameAgent>();
        }
        int Ammo
        {
            get
            {
                return m_ammo;
            }
            set
            {
                m_ammo = value;
            }
        }
        public abstract void Update(GameBoard board);

        public List<GameAgent> CurrentTarget
        {
            get 
            {
                return m_targets;
            }
        }

        public GridPosition Position
        {
            get
            {
                return m_position;
            }
        }
    }

    public class GunTower : GameTower
    { 
        private readonly double m_range = 2;
        private readonly double m_damage = 1;

        public GunTower(GridPosition position, int ammo = 100) : base(position, ammo)
        {

        }

        public override void Update(GameBoard board)
        {
            double range2 = GameUtils.Sqr(m_range * board.Grid.Size);

            Position tower_position = board.Grid.GetCellCenter(this.Position);

            bool shots_fired = false;

            //check existing targets
            for (int i = 0; i < m_targets.Count; i++)
            {
                if (m_targets[i].isAlive) 
                {
                    double temp = (m_targets[i].Position - tower_position).Length2;

                    if (temp < range2)
                    {
                        m_targets[i].DoDamage(m_damage, enDamageType.Physical);
                        shots_fired = true;
                        break;
                    }

                }                
            }

            //find closest target and shot it
            if (shots_fired == false)
            {
                int closest_agent = -1;
                double closest_agent_dist = Double.PositiveInfinity;

                for (int i = 0; i < board.Agents.Count; i++)
                {
                    if (board.Agents[i].isAlive)
                    {
                        double temp = (board.Agents[i].Position - tower_position).Length2;

                        if (temp < range2 && temp < closest_agent_dist)
                        {
                            closest_agent_dist = temp;
                            closest_agent = i;
                        }
                    }
                }

                if (closest_agent >= 0)
                {
                    GameAgent agent = board.Agents[closest_agent];

                    agent.DoDamage(m_damage, enDamageType.Physical);
                    // board.Agents[closest_agent]
                    m_targets.Add(agent);
                }
            }
        }
    }

    public class CannonTower : GameTower
    {
        public CannonTower(GridPosition position, int ammo = 100) : base(position, ammo)
        { }
        public override void Update(GameBoard board)
        {
        }
    }

    public class LaserTower : GameTower
    {
        public LaserTower(GridPosition position, int ammo = 100) : base(position, ammo)
        { }
        public override void Update(GameBoard board)
        {
        }
    }

    public class MissileTower : GameTower
    {
        public MissileTower(GridPosition position, int ammo = 100) : base(position, ammo)
        { }
        public override void Update(GameBoard board)
        {
        }
    }

    public class MicrowaveTower : GameTower
    {
        public MicrowaveTower(GridPosition position, int ammo = 100) : base(position, ammo)
        { }
        public override void Update(GameBoard board)
        {
        }
    }

    public class ECMTower : GameTower
    {
        public ECMTower(GridPosition position, int ammo = 100) : base(position, ammo)
        { }
        public override void Update(GameBoard board)
        {
        }
    }


}
