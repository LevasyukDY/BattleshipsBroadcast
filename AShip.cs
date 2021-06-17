using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipsBroadcast
{
    [Serializable]
    public class AShip
    {


        public AList<APoint> Points;
        public bool IsAlive;

        public AShip(AList<APoint> points)
        {
            Points = new AList<APoint>();
            foreach (APoint point in points)
            {
                Points.Add(point.Clone());
            }
            IsAlive = true;
        }

        public AShip()
        {
            Points = new AList<APoint>();
            IsAlive = true;
        }

        public bool CheckByAlive()
        {
            bool temp = false;
            foreach (APoint point in Points)
            {
                if (point.IsEnabled == true)
                {
                    temp = true;
                }
            }
            IsAlive = temp;
            return IsAlive;
        }

        public bool CheckByPoint(APoint point, out APoint result)
        {
            foreach (APoint e in Points)
            {
                if (e.IsEqual(point) == true)
                {
                    result = e;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public AShip Clone()
        {
            AShip Temp = new AShip();
            foreach (APoint point in Points)
            {
                Temp.Points.Add(point.Clone());
            }
            return Temp;
        }

    }
}
