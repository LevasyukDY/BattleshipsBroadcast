using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace BattleshipsBroadcast
{
    // класс записи-вида для формы лобби
    [Serializable]
    class GameNoteView: Panel
    {
        // событие вызываемое при нажатии на кнопку подключения к серверу
        public delegate void OnConnectEvent(GameNoteView note);
        public event OnConnectEvent ConnectEvent;

        // класс-котроллер (по-крайней мере им задумывался)
        ARoom Source;

        private Label Id;
        private Panel Status;
        private Label StatusLabel;
        private Label HostLabel;
        private Label PlayersCount;
        private Label RoomName;
        private Button Connect;

        public GameNoteView(): base()
        {

        }

        public GameNoteView(ARoom note) : base()
        {
            Source = note;
            SetAll();
        }

        // обновляем данные записи (при получении фрейма с новым статусом сервера)
        public void SetSource(ARoom note)
        {
            Source = note;
            SetAll();
        }

        // отображаем данные записи
        private void SetAll()
        {
            Size = new Size(760, 50);

            Id = new Label() { Parent = this, Location = new Point(0, 10), Size = new Size(100, 30), Font = new Font(Font.FontFamily, 18), Text = Source.Id.ToString() };
            Status = new Panel() { Parent = this, Location = new Point(110, 0), Size = new Size(140, 50), BackColor = Color.IndianRed };
            StatusLabel = new Label() { Parent = Status, Location = new Point(0, 10), Size = new Size(140, 30), Font = new Font(Font.FontFamily, 18), ForeColor = Color.White };
            PlayersCount = new Label() { Parent = this, Location = new Point(270, 10), Size = new Size(100, 30), Font = new Font(Font.FontFamily, 18), Text = Source.Players.Count + "/" + 2 };
            //HostLabel = new Label() { Parent = this, Location = new Point(300, 10), Size = new Size(250, 30), Font = new Font(Font.FontFamily, 18), Text = Source.HostIp + ":" + Source.HostPort };
            RoomName = new Label() { Parent = this, Location = new Point(370, 10), Size = new Size(270, 30), Font = new Font(Font.FontFamily, 18), Text = Source.Name };
            Connect = new Button() { Parent = this, Location = new Point(660, 0), Size = new Size(100, 50), Font = new Font(Font.FontFamily, 9), Text = "Подключиться" };

            SetStatusColor(Source.GameStatus);

            // вызываем собтие, если пользователь нажал на кнопку "Подключиться"
            Connect.Click += (object sender, EventArgs e) => {
                ConnectEvent?.Invoke(this);
            };

            // если статус сервера изменился - обновляем данные
            Source.ChangeRoomStatusEvent += (status) => {
                SetStatusColor(status);
            };
        }

        private void SetStatusColor(AGameStatus status)
        {
            switch (status)
            {
                case AGameStatus.Game:
                    Status.BackColor = Color.IndianRed;
                    StatusLabel.BackColor = Color.IndianRed;
                    StatusLabel.Text = "Игра идет";
                    Connect.Enabled = false;
                    break;
                case AGameStatus.Wait:
                    Status.BackColor = Color.LightGreen;
                    StatusLabel.BackColor = Color.LightGreen;
                    StatusLabel.Text = "Ожидание";
                    Connect.Enabled = true;
                    break;
            }
        }

    }
}
