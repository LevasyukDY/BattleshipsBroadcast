using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Coursework
{
    class PlayerNoteView: Panel
    {

        delegate Bitmap Func(int score);

        APlayer player;
        private Label PlayerName;
        private Label PlayerScore;
        private Label PlayerMoveScore;
        private PictureBox DicePicture;

        public PlayerNoteView() : base()
        {

        }

        public PlayerNoteView(APlayer Player, bool active) : base()
        {

            player = Player;
            Size = new Size(760, 50);
            if (active)
                BackColor = Color.LightGreen;
            else BackColor = Color.Transparent;

            Func F = (score) => {
                switch (score)
                {
                    case 1:
                        return Properties.Resources._1;
                    case 2:
                        return Properties.Resources._2;
                    case 3:
                        return Properties.Resources._3;
                    case 4:
                        return Properties.Resources._4;
                    case 5:
                        return Properties.Resources._5;
                    case 6:
                        return Properties.Resources._6;
                    default:
                        return new Bitmap(50, 50);
                }
            };

            PlayerName = new Label() { Parent = this, Location = new Point(0, 10), Size = new Size(300, 30), Font = new Font(Font.FontFamily, 14), Text = player.Name };
            PlayerScore = new Label() { Parent = this, Location = new Point(310, 10), Size = new Size(200, 50), Font = new Font(Font.FontFamily, 14), Text = "Score: " + player.Score };
            PlayerMoveScore = new Label() { Parent = this, Location = new Point(510, 10), Size = new Size(200, 30), Font = new Font(Font.FontFamily, 14), Text = "Last Round: " + player.LastRound.Sum() };
            if (player.LastRound.Count > 0)
            {
                DicePicture = new PictureBox() { Parent = this, Location = new Point(710, 0), Size = new Size(50, 50), Image = F(player.LastRound.Last()), SizeMode = PictureBoxSizeMode.StretchImage };
            }
            
        }

        
    }
}
