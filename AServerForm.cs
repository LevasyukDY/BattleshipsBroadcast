using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace BattleshipsBroadcast
{
    class AServerForm : Form
    {

        public delegate void OnRoomListChangeEvent();
        public event OnRoomListChangeEvent RoomListChangeEvent;

        public AServer Server;
        public AClient Client;

        public AServer LobbyServer;

        RichTextBox Chrono;
        AList<ARoom> Rooms;

        public AServerForm(string adress, int sendport, int receiveport, int lobbyport) : base()
        {

            Text = "Локальный сервер";
            ClientSize = new Size(800, 600);

            Server = new AServer(adress, sendport); // 8001
            Client = new AClient(adress, receiveport); // 8000

            Thread LobbyThread;

            Rooms = new AList<ARoom>();

            Chrono = new RichTextBox() { Parent = this, Location = new Point(10, 10), Size = new Size(500, 580) };

            RoomListChangeEvent += () => {
                LobbyThread = new Thread(new ParameterizedThreadStart((object obj) => {
                    AServer LobbyServer = new AServer(adress, lobbyport);
                    while (true)
                    {
                        LobbyServer.StartSending(new AFrame(0, Rooms.Clone(), AMessageType.Undefined), true, "LobbyServer"); Thread.Sleep(500);
                    }
                }))
                { Name = "LobbyThread", IsBackground = true };
                LobbyThread.Start();
            };

            Client.StartReceive("ServerReceiver");

            Client.Receive += (frame) => {
                if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                {
                    ARoom room;
                    APlayer player;
                    ARoom aroom;
                    CRoom croom;
                    switch (frame.MessageType)
                    {
                        case AMessageType.CreateGame:
                            aroom = (ARoom)frame.Data;
                            if (FindRoomByName(aroom.Name, out room) == false)
                            {
                                room = new ARoom(aroom.ToCRoom(), aroom.Id);
                                room.ActivePlayer = room.Players.Last();
                                room.GameStatus = aroom.GameStatus;
                                room.ActivePlayer.Ships.Clear();
                                foreach (AShip ship in aroom.ActivePlayer.Ships)
                                {
                                    AList<APoint> Temp = new AList<APoint>();
                                    foreach (APoint point in ship.Points)
                                    {
                                        Temp.Add(point.Clone());
                                    }
                                    room.ActivePlayer.Ships.Add(new AShip(Temp));
                                }
                                room.Id = Rooms.Count + 1;
                                Rooms.Add(room);
                                Server.StartSending(new AFrame(room.Id, room, AMessageType.Connect), true, "ServerSender");
                                Chrono.AppendText("[System] : Создана комната - " + room.Name + "\n");
                                RoomListChangeEvent?.Invoke();
                            }
                            break;
                        case AMessageType.Connect:
                            aroom = (ARoom)frame.Data;
                            if (FindRoomByName(aroom.Name, out room) == true)
                            {
                                APlayer newPlayer = new APlayer(aroom.ActivePlayer.Name, room.Players.Count + 1);
                                foreach (AShip ship in aroom.ActivePlayer.Ships)
                                {
                                    AList<APoint> Temp = new AList<APoint>();
                                    foreach (APoint point in ship.Points)
                                    {
                                        Temp.Add(point.Clone());
                                    }
                                    newPlayer.Ships.Add(new AShip(Temp));
                                }
                                if (room.AddPlayer(newPlayer) == true)
                                {
                                    Server.StartSending(new AFrame(room.Id, room, AMessageType.Connect), true, "ServerSender");
                                    Chrono.AppendText("[" + room.Name + "] : Игрок " + aroom.Name + " подключился\n");
                                    if (room.GameStatus.Equals(AGameStatus.Game) == true)
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.Send), true, "ServerSender");
                                        Chrono.AppendText("[" + room.Name + "] : Игра началась\n");
                                    }
                                }
                            }
                            break;
                        case AMessageType.PlayerDisconnect:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                if (FindPlayerByName(croom.PlayerName, room, out player) == true)
                                {
                                    room.Players.Remove(player);
                                    Chrono.AppendText("[" + room.Name + "] : Игрок " + croom.PlayerName + " отключился\n");
                                }
                                if (room.GameStatus.Equals(AGameStatus.Game) == true)
                                {
                                    if (room.Players.Count > 1)
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.PlayerDisconnect), true, "ServerSender");
                                    }
                                    else
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.GameOver), true, "ServerSender");
                                        Chrono.AppendText("[System] : комната " + room.Name + " расформирована\n");
                                        Rooms.Remove(room);
                                    }
                                }
                                else
                                {
                                    if (room.Players.Count >= 1)
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.PlayerDisconnect), true, "ServerSender");
                                    }
                                    else
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.GameOver), true, "ServerSender");
                                        Chrono.AppendText("[System] : комната " + room.Name + " расформирована\n");
                                        Rooms.Remove(room);
                                    }
                                }
                                RoomListChangeEvent?.Invoke();
                            }
                            break;
                        case AMessageType.Send:
                            aroom = (ARoom)frame.Data;
                            if (FindRoomByName(aroom.Name, out room) == true)
                            {
                                Chrono.AppendText("[" + room.Name + "] : Игрок " + room.ActivePlayer.Name + " сделал ход\n");
                                room = aroom.Clone();
                                Server.StartSending(new AFrame(room.Id, aroom, AMessageType.Send), true, "ServerSender");
                            }
                            break;
                        case AMessageType.GameOver:
                            aroom = (ARoom)frame.Data;
                            if (FindRoomByName(aroom.Name, out room) == true)
                            {
                                Server.StartSending(new AFrame(room.Id, aroom, AMessageType.GameOver), true, "ServerSender");
                                Chrono.AppendText("[" + room.Name + "] : Игрок " + aroom.ActivePlayer.Name + " победил в игре\n");
                                Chrono.AppendText("[System] : комната " + room.Name + " расформирована\n");
                                Rooms.Remove(room);
                                Server.StopSending();
                            }
                            break;
                        case AMessageType.Wait:
                            break;
                    }
                }
                ), frame);

            };

        }

        private bool FindRoomByName(string name, out ARoom Room)
        {
            foreach (ARoom room in Rooms)
            {
                if (room.Name == name)
                {
                    Room = room;
                    return true;
                }
            }
            Room = null;
            return false;
        }

        private bool FindPlayerByName(string name, ARoom Room, out APlayer Player)
        {
            foreach (APlayer player in Room.Players)
            {
                if (player.Name == name)
                {
                    Player = player;
                    return true;
                }
            }
            Player = null;
            return false;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AServerForm));
            this.SuspendLayout();
            // 
            // AServerForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AServerForm";
            this.ResumeLayout(false);

        }
    }
}
