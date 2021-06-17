using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipsBroadcast
{
    [Serializable]
    public enum AMessageType
    {
        Undefined,
        CreateGame,
        Connect,
        PlayerDisconnect,
        Send,
        Wait,
        GameOver,
        ServerClose
    }

    [Serializable]
    public class AFrame
    {
        // тип посылаемого фрейма
        public AMessageType MessageType;
        // идентификатор прослушиваемой группы
        public int Id { get; }
        // данные посылаемые во фрейме
        public object Data { get; }

        public AFrame(int id, object data, AMessageType messageType)
        {
            Id = id;
            Data = data;
            MessageType = messageType;
        }

    }
}
