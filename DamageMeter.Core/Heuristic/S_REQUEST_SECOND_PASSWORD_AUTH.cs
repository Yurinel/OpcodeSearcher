﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game.Messages;

namespace DamageMeter.Heuristic
{
    public class S_REQUEST_SECOND_PASSWORD_AUTH : AbstractPacketHeuristic
    {
        public static S_REQUEST_SECOND_PASSWORD_AUTH Instance => _instance ?? (_instance = new S_REQUEST_SECOND_PASSWORD_AUTH());
        private static S_REQUEST_SECOND_PASSWORD_AUTH _instance;

        private S_REQUEST_SECOND_PASSWORD_AUTH() : base(OpcodeEnum.S_REQUEST_SECOND_PASSWORD_AUTH)
        {
        }

        public new void Process(ParsedMessage message)
        {
            base.Process(message);

            if (IsKnown || OpcodeFinder.Instance.IsKnown(message.OpCode)) { return; }
            if(OpcodeFinder.Instance.PacketCount > 7) { return; }
            if (message.Payload.Count != 0) { return; }
            var previousPacket = OpcodeFinder.Instance.GetMessage(OpcodeFinder.Instance.PacketCount - 1);
            var opcodeLoadingScreen = OpcodeFinder.Instance.GetOpcode(OpcodeEnum.S_LOADING_SCREEN_CONTROL_INFO);
            if (opcodeLoadingScreen!= null && opcodeLoadingScreen.HasValue && opcodeLoadingScreen == previousPacket.OpCode)
            {
                OpcodeFinder.Instance.SetOpcode(message.OpCode, OPCODE);
            }
        }
    }
}