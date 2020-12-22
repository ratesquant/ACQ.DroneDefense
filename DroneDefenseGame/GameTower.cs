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
        private List<GameAgent> m_targets;

        public GameTower(GridPosition position, int ammo)
        {
            m_position = position;
            m_ammo = ammo;
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
        public GunTower(GridPosition position, int ammo = 100) : base(position, ammo)
        {}

        public override void Update(GameBoard board)
        { 
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
