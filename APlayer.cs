using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipsBroadcast
{
    [Serializable]
    public class APlayer
    {
        public delegate void OnAliveChangeEvent(bool status);
        public event OnAliveChangeEvent AliveChangeEvent;

        // идентификатор игрока
        public int Id { get; }
        // имя игрока
        public string Name { get; set; }
        public AList<AShip> Ships;
        public AList<APoint> Misses;

        public APlayer(string name, int id)
        {
            Id = id;
            Name = name;
            Ships = new AList<AShip>();
            Misses = new AList<APoint>();
        }

        public APlayer Clone()
        {
            APlayer player = new APlayer(Name, Id);
            foreach (AShip ship in Ships)
            {
                player.Ships.Add(ship.Clone());
            }
            foreach (APoint point in Misses)
            {
                player.Misses.Add(point.Clone());
            }
            return player;
        }

        public bool ProcessMove(APoint point, out bool isKill, out bool IsGameOver)
        {
            isKill = false;
            IsGameOver = false;
            foreach (AShip ship in Ships)
            {
                APoint result;
                if (ship.CheckByPoint(point, out result) == true)
                {
                    result.IsEnabled = false;
                    if (ship.CheckByAlive() == false)
                    {
                        isKill = true;
                        IsGameOver = !CheckAllShips();
                    }
                    return true;
                }
            }
            Misses.Add(point);
            Misses.Last().CellType = AMapCellType.Miss;
            Misses.Last().IsEnabled = false;
            return false;
        }

        public bool CheckAllShips()
        {
            bool flag = false;
            foreach (AShip ship in Ships)
            {
                if (ship.CheckByAlive() == true)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public AList<APoint> Map(bool isHiden)
        {
            AList<APoint> Temp = new AList<APoint>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Temp.Add(new APoint(j, i, AMapCellType.Undefined));
                }
            }
            if (isHiden == true)
            {
                foreach (APoint point in Temp)
                {
                    foreach (AShip ship in Ships)
                    {
                        foreach (APoint e in ship.Points)
                        {
                            if (e.IsEqual(point) == true)
                            {
                                if (e.IsEnabled == false)
                                {
                                    point.CellType = AMapCellType.Destroyed;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (APoint point in Temp)
                {
                    foreach (AShip ship in Ships)
                    {
                        foreach (APoint e in ship.Points)
                        {
                            if (e.IsEqual(point) == true)
                            {
                                if (e.IsEnabled == true)
                                {
                                    point.CellType = AMapCellType.Ship;
                                }
                                else
                                {
                                    point.CellType = AMapCellType.Destroyed;
                                }
                            }
                        }
                    }
                }
            }
            foreach (APoint point in Temp)
            {
                foreach (APoint e in Misses)
                {
                    if (e.IsEqual(point) == true)
                    {
                        point.CellType = AMapCellType.Miss;
                    }
                }
            }
            return Temp;
        }
    }

}
