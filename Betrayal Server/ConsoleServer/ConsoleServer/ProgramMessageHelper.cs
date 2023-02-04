using Riptide;

namespace Betrayal.ConsoleServer
{
    internal class ProgramMessageHelper
    {
        public static void SendBoolMessage(ushort fromClientId, bool data, ServerToClientId messageType, MessageSendMode sendMode)
        {
            Message message = Message.Create(sendMode, messageType);
            message.AddUShort(fromClientId);
            message.AddBool(data);
            Program.SendMessageFromClient(message, fromClientId);
        }

        public static void SendIntMessage(ushort fromClientId, int data, ServerToClientId messageType, MessageSendMode sendMode)
        {
            Message message = Message.Create(sendMode, messageType);
            message.AddUShort(fromClientId);
            message.AddInt(data);
            Program.SendMessageFromClient(message, fromClientId);
        }

        public static void SendStringMessage(ushort fromClientId, string data, ServerToClientId messageType, MessageSendMode sendMode)
        {
            Message message = Message.Create(sendMode, messageType);
            message.AddUShort(fromClientId);
            message.AddString(data);
            Program.SendMessageFromClient(message, fromClientId);
        }

        public static void SendFloatArrayMessage(ushort fromClientId, float[] data, ServerToClientId messageType, MessageSendMode sendMode)
        {
            Message message = Message.Create(sendMode, messageType);
            message.AddUShort(fromClientId);
            message.AddFloats(data, false);
            Program.SendMessageFromClient(message, fromClientId);
        }

        public static void SendIntArrayMessage(ushort fromClientId, int[] data, ServerToClientId messageType, MessageSendMode sendMode)
        {
            Message message = Message.Create(sendMode, messageType);
            message.AddUShort(fromClientId);
            message.AddInts(data, false);
            Program.SendMessageFromClient(message, fromClientId);
        }

        public static void SendRigidbodyMessage(ushort fromClientId, int objId, float[] data, ServerToClientId messageType, MessageSendMode sendMode)
        {
            Message message = Message.Create(sendMode, messageType);
            message.AddInt(objId);
            message.AddFloats(data, false);
            Program.SendMessageFromClient(message, fromClientId);
        }
    }
}
