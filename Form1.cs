using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipsBroadcast
{
    public partial class Form1 : Form
    {

        private const string GROUP_ADRESS = "224.0.0.0";
        private const int SERVER_RECEIVE_PORT = 8000;
        private const int CLIENT_RECEIVE_PORT = 8001;
        private const int LOBBY_PORT = 8002;

        AServerForm ServerForm;
        AClientForm ClientForm;

        public Form1()
        {
            InitializeComponent();
            InitMain();
        }

        private void InitMain()
        {
            Text = "Сетевая игра \"Морской бой\"";
            ClientSize = new Size(464, 223);

            Button StartServer = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2 - 45), Size = new Size(300, 33), Font = new Font(Font.FontFamily, 12), Text = "Запустить сервер" };
            Button StartClient = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2 + 5), Size = new Size(300, 33), Font = new Font(Font.FontFamily, 12), Text = "Запустить клиент" };

            StartServer.Click += (object sender, EventArgs e) => {
                if ((ServerForm is null) == false)
                {
                    ServerForm.Server.StopSending();
                    ServerForm.Client.StopReceive();
                    ServerForm.LobbyServer.StopSending();
                    ServerForm.Close();
                }
                ServerForm = new AServerForm(GROUP_ADRESS, CLIENT_RECEIVE_PORT, SERVER_RECEIVE_PORT, LOBBY_PORT);
                ServerForm.Show();
            };

            StartClient.Click += (object sender, EventArgs e) => {
                if ((ClientForm is null) == false)
                {
                    ClientForm.Server.StopSending();
                    ClientForm.Client.StopReceive();
                    ClientForm.LobbyClient.StopReceive();
                    ClientForm.Close();
                }
                ClientForm = new AClientForm(GROUP_ADRESS, SERVER_RECEIVE_PORT, CLIENT_RECEIVE_PORT, LOBBY_PORT);
                ClientForm.Show();
            };

        }

    }
}
