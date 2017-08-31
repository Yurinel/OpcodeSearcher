﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game.Messages;

namespace DamageMeter.Heuristic
{
    class C_CHANGE_PARTY_MANAGER : AbstractPacketHeuristic
    {
        public static C_CHANGE_PARTY_MANAGER Instance => _instance ?? (_instance = new C_CHANGE_PARTY_MANAGER());
        private static C_CHANGE_PARTY_MANAGER _instance;

        public C_CHANGE_PARTY_MANAGER() : base(OpcodeEnum.C_CHANGE_PARTY_MANAGER) { }
        public ushort PossibleOpcode;
        public uint LastPlayerId;
        public uint LastServerId;
        public new void Process(ParsedMessage message)
        {
            base.Process(message);
            if (IsKnown || OpcodeFinder.Instance.IsKnown(message.OpCode)) return;
            if(OpcodeFinder.Instance.IsKnown(OpcodeEnum.S_CHANGE_PARTY_MANAGER)) return;
            if (message.Payload.Count != 4 + 4) return;
            var serverId = Reader.ReadUInt32();
            var playerId = Reader.ReadUInt32();

            if(!DbUtils.IsPartyMember(playerId, serverId)) return;
            PossibleOpcode = message.OpCode;
            LastServerId = serverId;
            LastPlayerId = playerId;
        }

        public void Confirm()
        {
            OpcodeFinder.Instance.SetOpcode(PossibleOpcode, OPCODE);
        }
    }
}
