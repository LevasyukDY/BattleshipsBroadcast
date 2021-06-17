using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace BattleshipsBroadcast
{

    enum AProcessType
    {
        Miss,
        Destroy,
        Beat,
        GameOver,
        Undefined
    }

    class FieldMapView: Panel
    {

        public delegate void OnChooseCellEvent(APoint point);
        public event OnChooseCellEvent ChooseCellEvent;

        AList<MapCellView> CellsViewList;
        AList<APoint> Source;
        AList<AList<APoint>> Ships;

        public FieldMapView(AList<APoint> source, int size, bool isEnebled) : base()
        {
            CellsViewList = new AList<MapCellView>();
            Source = source;
            foreach (APoint point in Source)
            {

                point.IsEnabled = isEnebled;

                MapCellView cellView = new MapCellView(point) { Parent = this, Location = new Point(10 + point.X * (size + 10), 10 + point.Y * (size + 10)), Size = new Size(size, size), FlatStyle = FlatStyle.Flat };

                cellView.FlatAppearance.BorderSize = 0;

                cellView.ClickEvent += (cell) =>
                {
                    ChooseCellEvent?.Invoke(cell);
                };

                CellsViewList.Add(cellView);
            }            

        }

        public void Update(AList<APoint> source, int size, bool isEnebled)
        {
            for (int i = 0; i < source.Count; i++)
            {
                CellsViewList[i].Update(source[i]);
            }
           
        }

        public AProcessType ProcessMove(APoint point)
        {
            bool IsGameOver = true;
            AProcessType flag = AProcessType.Undefined;
            foreach (APoint cell in Source)
            {
                if (cell.IsEqual(point) == true)
                {
                    switch (point.CellType)
                    {
                        case AMapCellType.Ship:
                            cell.CellType = AMapCellType.Destroyed;
                            cell.IsEnabled = false;
                            foreach (AList<APoint> ship in Ships)
                            {
                                if (ship.Contains(point) == true)
                                {
                                    ship.Remove(point);
                                    if (ship.Count > 0)
                                    {
                                        flag = AProcessType.Beat;
                                    }
                                    else
                                    {
                                        flag = AProcessType.Destroy;
                                    }
                                }
                                if (ship.Count > 0)
                                {
                                    IsGameOver = false;
                                }
                            }
                            if (IsGameOver == true)
                            {
                                flag = AProcessType.GameOver;
                            }
                            break;
                        case AMapCellType.Undefined:
                            cell.CellType = AMapCellType.Miss;
                            flag = AProcessType.Miss;
                            break;
                        default: 
                            flag = AProcessType.Undefined;
                            break;
                    }
                }
            }
            return flag;
        }

        public AList<APoint> HidenMap()
        {
            AList<APoint> Temp = new AList<APoint>();
            foreach (APoint point in Source)
            {
                switch (point.CellType)
                {
                    case AMapCellType.Destroyed:
                        Temp.Add(point);
                        break;
                    case AMapCellType.Miss:
                        Temp.Add(point);
                        break;
                    default:
                        Temp.Add(new APoint(point.X, point.Y, AMapCellType.Undefined));
                        break;
                }
            }
            return Temp;
        }

    }
}
