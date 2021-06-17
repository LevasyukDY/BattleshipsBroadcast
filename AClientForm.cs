using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace BattleshipsBroadcast
{
    class AClientForm: Form
    {

        public AServer Server;
        public AClient Client;

        public AClient LobbyClient;
        private Thread GameReceiver;

        public AClientForm(string adress, int sendport, int receiveport, int lobbyport) : base()
        {

            Text = "Локальный клиент";
            ClientSize = new Size(800, 600);

            Server = new AServer(adress, sendport); // 8000
            Client = new AClient(adress, receiveport); // 8001
            LobbyClient = new AClient(adress, lobbyport);

            Client.StartReceive("ClientReceiver");
            LobbyClient.StartReceive("LobbyReceiver");

            InitLobby();

        }

        private void InitLobby()
        {
            Controls.Clear();

            Text = "Локальный клиент - Лобби";
            ClientSize = new Size(800, 600);

            Button CreateGame = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 100, 10), Size = new Size(200, 40), Text = "Создать сервер" };

            // список игр в локальной сети
            AList<ARoom> Notes = new AList<ARoom>();
            // Форма для отображения игр действующих в локальной сети
            NotesView Lobbys = new NotesView(Notes) { Parent = this, Location = new Point(10, 60), Height = 530 };

            LobbyClient.Receive += (frame) =>
            {
                if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                {
                    AList<ARoom> rooms = (AList<ARoom>)frame.Data;
                    if (Notes.Count > 0 && rooms.Count > 0)
                    {
                        foreach (ARoom room in rooms)
                        {
                            ARoom temp;
                            if (FindById(room.Id, Notes, out temp) == false)
                            {
                                Notes.Add(room);
                            }
                            else
                            {
                                temp = room;
                            }
                        }
                    }
                    else
                    {
                        foreach (ARoom room in rooms)
                        {

                            Notes.Add(room);
                        }
                    }
                }
                ), frame);
            };

            CreateGame.Click += (object sender, EventArgs e) => {
                InitCreateRoomForm();
            };

            Lobbys.ConnectEvent += (room) => {            
                InitInputYourNameForm(room);
            };

        }

        private void InitInputYourNameForm(ARoom Room)
        {
            Controls.Clear();

            Text = "Локальный клиент - Подключение к " + Room.Name;
            ClientSize = new Size(800, 600);

            Label RoomTitle = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 195), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 24), Text = "Настройки комнаты" };

            Label PlayerNameInputLabel = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 55), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Имя" };
            TextBox PlayerNameInput = new TextBox() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 105), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12) };

            Button Done = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 155), Size = new Size(200, 40), Font = new Font(Font.FontFamily, 12), Text = "Готово" };
            Button Back = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 + 50, ClientSize.Height / 2 + 155), Size = new Size(200, 40), Font = new Font(Font.FontFamily, 12), Text = "Вернуться в лобби" };
            
            
            Done.Click += (object sender, EventArgs e) => {
                InitSetShipsForm(Room, PlayerNameInput.Text, false);
            };

            Back.Click += (object sender, EventArgs e) => {
                InitLobby();
            };

        }

        private void InitCreateRoomForm()
        {
            Controls.Clear();

            Text = "Локальный клиент - Создание новой комнаты";
            ClientSize = new Size(800, 600);

            Label RoomTitle = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 260, ClientSize.Height / 2 - 195), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 24), Text = "Настройки комнаты" };

            Label RoomNameInputLabel = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 255, ClientSize.Height / 2 - 130), Size = new Size(500, 20), Font = new Font(Font.FontFamily, 12), Text = "Название комнаты" };
            TextBox RoomNameInput = new TextBox() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 95), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12) };

            Label PlayerNameInputLabel = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 255, ClientSize.Height / 2 - 30), Size = new Size(500, 20), Font = new Font(Font.FontFamily, 12), Text = "Имя" };
            TextBox PlayerNameInput = new TextBox() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 5), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12) };

            Button Done = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 70), Size = new Size(200, 40), Font = new Font(Font.FontFamily, 12), Text = "Готово" };
            Button Back = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 + 50, ClientSize.Height / 2 + 70), Size = new Size(200, 40), Font = new Font(Font.FontFamily, 12), Text = "Назад" };

            Done.Click += (object sender, EventArgs e) => {
                ARoom room = new ARoom(0, RoomNameInput.Text, new APlayer(PlayerNameInput.Text, 1));
                InitSetShipsForm(room, PlayerNameInput.Text, true);
            };

            Back.Click += (object sender, EventArgs e) => {
                InitLobby();
            };

        }

        private void InitSetShipsForm(ARoom Room, string name, bool isRoomHead) {
            Controls.Clear();
            Text = "Морской бой - Расставьте корабли...";
            ClientSize = new Size(900, 700);

            Random random = new Random(Convert.ToInt32((int)DateTime.Now.Ticks));

            AList<APoint> map = new AList<APoint>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    map.Add(new APoint(j, i, AMapCellType.Undefined));
                }
            }

            List<int> ships = new List<int>() { 0, 0, 0, 0 };

            AList<APoint> TempPoints = new AList<APoint>();
            AList<AList<APoint>> Ships = new AList<AList<APoint>>();

            GroupBox ShipsOptions = new GroupBox() { Parent = this, Location = new Point(630, 10), Size = new Size(260, 610), Text = "Доступные корабли" };
            Label OneShip = new Label() { Parent = ShipsOptions, Location = new Point(10, 30), Size = new Size(240, 40), Text = "Однопалубный (х" + ships[0] + ")", Font = new Font(Font.FontFamily, 10) };
            Label TwoShip = new Label() { Parent = ShipsOptions, Location = new Point(10, 80), Size = new Size(240, 40), Text = "Двухпалубный (х" + ships[1] + ")", Font = new Font(Font.FontFamily, 10) };
            Label ThreeShip = new Label() { Parent = ShipsOptions, Location = new Point(10, 130), Size = new Size(240, 40), Text = "Трехпалубный (х" + ships[2] + ")", Font = new Font(Font.FontFamily, 10) };
            Label FourShip = new Label() { Parent = ShipsOptions, Location = new Point(10, 180), Size = new Size(240, 40), Text = "Четырехпалубный (х" + ships[3] + ")", Font = new Font(Font.FontFamily, 10) };

            Button SetShip = new Button() { Parent = ShipsOptions, Location = new Point(10, ShipsOptions.Height - 150), Size = new Size(240, 40), Text = "Поставить корабль" };
            Button ClearField = new Button() { Parent = ShipsOptions, Location = new Point(10, ShipsOptions.Height - 100), Size = new Size(240, 40), Text = "Очистить поле" };
            Button StartGame = new Button() { Parent = ShipsOptions, Location = new Point(10, ShipsOptions.Height - 50), Size = new Size(240, 40), Text = "Начать игру", Enabled = false };
            FieldMapView mapView = new FieldMapView(map, 50, true) { Parent = this, Location = new Point(10, 10), Size = new Size(610, 610) };

            RichTextBox Chrono = new RichTextBox() { Parent = this, Location = new Point(10, 630), Size = new Size(880, 60), Text = "", BackColor = BackColor, BorderStyle = BorderStyle.None };

            mapView.ChooseCellEvent += (point) => {
                if (point.CellType.Equals(AMapCellType.Undefined) == true)
                {
                    if (TempPoints.Count < 4)
                    {
                        Chrono.Text = "";
                        point.CellType = AMapCellType.Ship;
                        TempPoints.Add(point);
                        SetShip.Text = "Поставить корабль (" + TempPoints.Count + ")";
                    }
                    else
                    {
                        Chrono.Text = "Количество клеток, занимаемых кораблем не может быть больше 4!";
                    }
                }
                else
                {
                    if (ContainsInShips(point, Ships) == false)
                    {
                        Chrono.Text = "";
                        point.CellType = AMapCellType.Undefined;
                        if (TempPoints.Contains(point)) TempPoints.Remove(point);
                        SetShip.Text = "Поставить корабль (" + TempPoints.Count + ")";
                    }
                    else
                    {
                        Chrono.Text = "Попытка изменения клетки уже заданного корабля!";
                    }
                }
            };

            ClearField.Click += (object sender, EventArgs e) =>
            {
                for (int i = 0; i < ships.Count; i++)
                {
                    ships[i] = 0;
                }
                Ships.Clear();
                TempPoints.Clear(); foreach (APoint point in map)
                {
                    point.IsEnabled = true;
                    point.CellType = AMapCellType.Undefined;
                }
                StartGame.Enabled = false;
                SetShip.Enabled = true;
                OneShip.Text = "Однопалубный (х" + ships[0] + ")";
                TwoShip.Text = "Двухпалубный (х" + ships[1] + ")";
                ThreeShip.Text = "Трехпалубный (х" + ships[2] + ")";
                FourShip.Text = "Четырехпалубный (х" + ships[3] + ")";
            };

            SetShip.Click += (object sender, EventArgs e) => {
                if (CheckByCorrectCount(ships, TempPoints.Count))
                {
                    ships[TempPoints.Count - 1]++;
                    switch (TempPoints.Count)
                    {
                        case 1:
                            OneShip.Text = "Однопалубный (х" + ships[0] + ")";
                            break;
                        case 2:
                            TwoShip.Text = "Двухпалубный (х" + ships[1] + ")";
                            break;
                        case 3:
                            ThreeShip.Text = "Трехпалубный (х" + ships[2] + ")";
                            break;
                        case 4:
                            FourShip.Text = "Четырехпалубный (х" + ships[3] + ")";
                            break;
                    }
                    AList<APoint> TempShip = new AList<APoint>();
                    foreach (APoint point in TempPoints)
                    {
                        point.IsEnabled = false;
                        TempShip.Add(point);
                    }
                    Ships.Add(TempShip);
                    TempPoints.Clear();
                    SetShip.Text = "Поставить корабль (" + TempPoints.Count + ")";
                    if (ships.Sum() == 10)
                    {
                        SetShip.Enabled = false;
                        StartGame.Enabled = true;
                        foreach (APoint point in map)
                        {
                            point.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    Chrono.Text = "Максимальное количество кораблей данного типа задано или не было выбрано ни одного корабля!";
                    foreach (APoint point in TempPoints)
                    {
                        point.CellType = AMapCellType.Undefined;
                    }
                    TempPoints.Clear();
                    SetShip.Text = "Поставить корабль (" + TempPoints.Count + ")";
                }
            };

            StartGame.Click += (object sender, EventArgs e) => {
                if (isRoomHead == true)
                {
                    ARoom Clone = new ARoom(Room.ToCRoom(), Room.Id);
                    Clone.ActivePlayer = Clone.Players.Last();
                    Clone.GameStatus = Room.GameStatus;
                    Clone.ActivePlayer.Ships.Clear();
                    foreach (AList<APoint> ship in Ships)
                    {
                        AList<APoint> Temp = new AList<APoint>();
                        foreach (APoint point in ship)
                        {
                            APoint pt = point.Clone();
                            pt.IsEnabled = true;
                            Temp.Add(pt);
                        }
                        Clone.ActivePlayer.Ships.Add(new AShip(Temp));
                    }
                    Server.StartSending(new AFrame(Room.Id, Clone, AMessageType.CreateGame), true, "ClientSender");
                    bool IsActive = true;
                    Client.Receive += (frame) =>
                    {
                        if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                        {
                            if (IsActive == true)
                            {
                                ARoom TempRoom;
                                switch (frame.MessageType)
                                {
                                    case AMessageType.Connect:
                                        TempRoom = (ARoom)frame.Data;
                                        InitWaitingRoomForm(TempRoom, name);
                                        break;
                                    case AMessageType.Send:
                                        TempRoom = (ARoom)frame.Data;
                                        IsActive = false;
                                        InitGameRoomForm(TempRoom, name);
                                        break;
                                    case AMessageType.PlayerDisconnect:
                                        break;
                                    case AMessageType.GameOver:
                                        break;
                                }
                            }
                        }
                    ), frame);
                    };
                }
                else
                {
                    ARoom Clone = new ARoom(Room.ToCRoom(), Room.Id);
                    Clone.ActivePlayer = Clone.Players.Last();
                    Clone.GameStatus = Room.GameStatus;
                    Clone.ActivePlayer.Ships.Clear();
                    foreach (AList<APoint> ship in Ships)
                    {
                        AList<APoint> Temp = new AList<APoint>();
                        foreach (APoint point in ship)
                        {
                            APoint pt = point.Clone();
                            pt.IsEnabled = true;
                            Temp.Add(pt);
                        }
                        Clone.ActivePlayer.Ships.Add(new AShip(Temp));
                    }
                    Clone.ActivePlayer.Name = name;
                    Server.StartSending(new AFrame(Room.Id, Clone, AMessageType.Connect), true, "ClientSender");
                    bool IsActive = true;
                    Client.Receive += (frame) =>
                    {
                        if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                        {
                            if (IsActive == true)
                            {
                                ARoom TempRoom;
                                switch (frame.MessageType)
                                {
                                    case AMessageType.Connect:
                                        TempRoom = (ARoom)frame.Data;
                                        InitWaitingRoomForm(TempRoom, name);
                                        break;
                                    case AMessageType.Send:
                                        TempRoom = (ARoom)frame.Data;
                                        IsActive = false;
                                        InitGameRoomForm(TempRoom, name);
                                        break;
                                    case AMessageType.PlayerDisconnect:
                                        break;
                                    case AMessageType.GameOver:
                                        break;
                                }
                            }
                        }
                ), frame);
                    };
                }
            };

        }

        private void InitGameOverForm(ARoom room, string name) 
        {
            Text = "Локальный клиент - " + room.Name + " - GAME OVER";
            ClientSize = new Size(800, 600);
            Controls.Clear();

            APlayer enemy;
            APlayer player = GetPlayers(name, room.Players, out enemy);

            Label ResultTitle = new Label() { Parent = this, Location = new Point(10, 10), Width = 780, Text = "Игра окончена! Победил " + room.ActivePlayer.Name, Font = new Font(Font.FontFamily, 14) };

            Label PlayerLabel = new Label() { Parent = this, Location = new Point(10, 50), Width = 370, Text = player.Name, Font = new Font(Font.FontFamily, 12), ForeColor = Color.White, BackColor = GetPlayerColor(player, room.ActivePlayer) };
            Label EnemyLabel = new Label() { Parent = this, Location = new Point(ClientRectangle.Width - 380, 50), Width = 370, Text = enemy.Name, Font = new Font(Font.FontFamily, 12), ForeColor = Color.White, BackColor = GetPlayerColor(enemy, room.ActivePlayer) };

            FieldMapView PlayerMapView = new FieldMapView(player.Map(false), 26, false) { Parent = this, Location = new Point(10, 100), Size = new Size(370, 370) };
            FieldMapView EnemyMapView = new FieldMapView(enemy.Map(false), 26, true) { Parent = this, Location = new Point(ClientRectangle.Width - 380, 100), Size = new Size(370, 370) };

            Button BackToLobby = new Button() { Parent = this, Location = new Point(ClientRectangle.Width / 2 - 150, ClientRectangle.Height - 50), Size = new Size(300, 40), Text = "Вернуться в лобби", Font = new Font(Font.FontFamily, 14) };

            BackToLobby.Click += (object sender, EventArgs e) => {
                Server.StartSending(new AFrame(room.Id, new CRoom(room.Id, room.Name, name), AMessageType.PlayerDisconnect), true, "ClientSender");
                InitLobby();
            };

        }

        private void InitGameRoomForm(ARoom room, string name)
        {
            Controls.Clear();

            Text = "Локальный клиент - " + room.Name;
            ClientSize = new Size(800, 600);

            APlayer enemy;
            APlayer player = GetPlayers(name, room.Players, out enemy);

            Label PlayerLabel = new Label() { Parent = this, Location = new Point(10, 10), Width = 370, Text = player.Name, Font = new Font(Font.FontFamily, 12), ForeColor = Color.White, BackColor = GetPlayerColor(player, room.ActivePlayer) };
            Label EnemyLabel = new Label() { Parent = this, Location = new Point(ClientRectangle.Width - 380, 10), Width = 370, Text = enemy.Name, Font = new Font(Font.FontFamily, 12), ForeColor = Color.White, BackColor = GetPlayerColor(enemy, room.ActivePlayer) };

            FieldMapView PlayerMapView = new FieldMapView(player.Map(false), 26, false) { Parent = this, Location = new Point(10, 50), Size = new Size(370, 370), BackColor = Color.LightGray };
            FieldMapView EnemyMapView = new FieldMapView(enemy.Map(true), 26, true) { Parent = this, Location = new Point(ClientRectangle.Width - 380, 50), Size = new Size(370, 370), BackColor = Color.LightGray };

            string ChronoText = "";

            RichTextBox Chrono = new RichTextBox() { Parent = this, Location = new Point(10, 430), Size = new Size(780, 160), BackColor = BackColor, ScrollBars = RichTextBoxScrollBars.Vertical };

            EnemyMapView.ChooseCellEvent += (point) => {
                if (room.ActivePlayer == player)
                {
                    bool IsShipDestroyed;
                    bool IsGameOver;
                    if (enemy.ProcessMove(point, out IsShipDestroyed, out IsGameOver) == false)
                    {
                        ChronoText += "Промах\n";
                        room.NextPlayer();
                        ChronoText += "Ход переходит к " + room.ActivePlayer.Name + "\n";
                    }
                    else
                    {
                        if (IsGameOver == false)
                        {
                            if (IsShipDestroyed == true)
                            {
                                ChronoText += "Корабль уничтожен\n";
                            }
                            else
                            {
                                ChronoText += "Корабль подбит\n";
                            }
                        }
                        else
                        {
                            ChronoText += "Все корабли противника уничтожены, вы победили!\n";
                            Server.StartSending(new AFrame(room.Id, room.Clone(), AMessageType.GameOver), true, "ClientSender");
                            InitGameOverForm(room.Clone(), name);
                        }
                    }
                    ARoom Clone = room.Clone();
                    Server.StartSending(new AFrame(room.Id, Clone, AMessageType.Send), true, "ClientSender");
                }
            };

            GameReceiver = new Thread(new ParameterizedThreadStart((object obj) => {
                Client.Receive += (frame) => {
                    if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                    {
                        switch (frame.MessageType)
                        {
                            case AMessageType.Send:
                                if (room.ActivePlayer.Name != player.Name && ((ARoom)frame.Data).ActivePlayer.Name == player.Name)
                                {
                                    ChronoText += "Ход переходит к вам\n";
                                }
                                room = (ARoom)frame.Data;
                                player = GetPlayers(name, room.Players, out enemy);
                                PlayerMapView.Update(player.Map(false), 26, false);
                                EnemyMapView.Update(enemy.Map(true), 26, true);
                                PlayerLabel.BackColor = GetPlayerColor(player, room.ActivePlayer);
                                EnemyLabel.BackColor = GetPlayerColor(enemy, room.ActivePlayer);
                                Chrono.Text = ChronoText;
                                break;
                            case AMessageType.PlayerDisconnect:
                                room = (ARoom)frame.Data;
                                player = GetPlayers(name, room.Players, out enemy);
                                PlayerMapView.Update(player.Map(false), 26, false);
                                EnemyMapView.Update(enemy.Map(true), 26, true);
                                PlayerLabel.BackColor = GetPlayerColor(player, room.ActivePlayer);
                                EnemyLabel.BackColor = GetPlayerColor(enemy, room.ActivePlayer);
                                break;
                            case AMessageType.GameOver:
                                InitGameOverForm((ARoom)frame.Data, name);
                                break;
                        }
                    }
                ), frame);
                };
            }))
            { Name = "GameReceiver", IsBackground = true };
            GameReceiver.Start();           
        }

        private void InitWaitingRoomForm(ARoom room, string player)
        {
            Controls.Clear();

            Text = "Локальный клиент - Комната ожидания";
            ClientSize = new Size(800, 600); 

            Label RoomTitle = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 195), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 24), Text = room.Name };

            Label PlyersCount = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 125), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Игроки: " + room.Players.Count + "/2"};
            Label PlayerName = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 85), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Ваше имя: " + player };
            Label GameStatus = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 45), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Ожидаем подключения игроков" };

            Button Back = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 155), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Вернуться в лобби" };

            Back.Click += (object sender, EventArgs e) => {
                Server.StartSending(new AFrame(room.Id, room, AMessageType.PlayerDisconnect), true, "ClientSender");
                InitLobby();
            };

        }

        private bool FindById(int id, AList<ARoom> RoomsList, out ARoom Room)
        {
            foreach (ARoom room in RoomsList)
            {
                if (room.Id == id)
                {
                    Room = room;
                    return true;
                }
            }
            Room = null;
            return false;
        }

        private APlayer GetPlayers(string name, AList<APlayer> PlayerList, out APlayer Enemy)
        {
            APlayer Player = null;
            Enemy = null;
            foreach (APlayer player in PlayerList)
            {
                if (player.Name == name)
                {
                    Player = player;
                }
                else
                {
                    Enemy = player;
                }
            }
            return Player;
        }

        private bool CheckByCorrectCount(List<int> ships, int count)
        {
            switch (count)
            {
                case 1:
                    if (ships[count - 1] + 1 > 5 - count)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case 2:
                    if (ships[count - 1] + 1 > 5 - count)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case 3:
                    if (ships[count - 1] + 1 > 5 - count)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case 4:
                    if (ships[count - 1] + 1 > 5 - count)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default: return false;
            }
        }

        private bool ContainsInShips(APoint point, AList<AList<APoint>> ships)
        {
            foreach (AList<APoint> ship in ships)
            {
                if (ship.Contains(point) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private Color GetPlayerColor(APlayer player, APlayer active)
        {
            if (player.Equals(active) == true)
            {
                return Color.Green;
            }
            else
            {
                return Color.Gray;
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AClientForm));
            this.SuspendLayout();
            // 
            // AClientForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AClientForm";
            this.ResumeLayout(false);

        }
    }
}
