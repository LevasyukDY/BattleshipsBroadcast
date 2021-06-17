using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace BattleshipsBroadcast
{
    class MapCellView: Button
    {
        public delegate void OnClickEvent(APoint point);
        public event OnClickEvent ClickEvent;

        private APoint Source;

        public MapCellView(APoint point): base()
        {

            Source = point;

            Source.ChangeCellTypeEvent += () => {
                BackColor = SetCellColor(Source);
            };

            BackColor = SetCellColor(Source);

            Click += (object sender, EventArgs e) => {
                if (Source.IsEnabled == true)
                {
                    ClickEvent?.Invoke(Source);
                    BackColor = SetCellColor(Source);
                }
            };

        }

        public void Update(APoint point)
        {
            Source = point;
            BackColor = SetCellColor(Source);
        }

        public Color SetCellColor(APoint point)
        {
            switch (point.CellType)
            {
                case AMapCellType.Undefined:
                    return Color.Gray;
                case AMapCellType.Ship:
                    return Color.Green;
                case AMapCellType.Miss:
                    return Color.Yellow;
                case AMapCellType.Destroyed:
                    return Color.Red;
                default:
                    return Color.Transparent;
            }
        }

    }
}
