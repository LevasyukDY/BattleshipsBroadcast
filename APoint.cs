using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipsBroadcast
{
    [Serializable]
    public enum AMapCellType
    {
        Undefined,
        Ship,
        Miss,
        Destroyed
    }

    [Serializable]
    public class APoint
    {

        public delegate void OnChangeCellTypeEvent();
        public event OnChangeCellTypeEvent ChangeCellTypeEvent;

        public int X { get; }
        public int Y { get; }
        private AMapCellType cellType;
        public AMapCellType CellType { 
            get => cellType; 
            set
            {
                cellType = value;
                ChangeCellTypeEvent?.Invoke();
            }
        }
        public bool IsEnabled;

        public APoint(int x, int y, AMapCellType cellType = AMapCellType.Undefined)
        {
            X = x;
            Y = y;
            CellType = cellType;
            IsEnabled = true;
        }

        public APoint(int x, int y, AMapCellType cellType = AMapCellType.Undefined, bool enabled = true)
        {
            X = x;
            Y = y;
            CellType = cellType;
            IsEnabled = enabled;
        }

        public APoint Clone()
        {
            return new APoint(X, Y, CellType, IsEnabled);
        }

        public bool IsEqual(APoint point)
        {
            if (point.X == X && point.Y == Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
