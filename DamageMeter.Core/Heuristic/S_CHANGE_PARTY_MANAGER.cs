﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Tera.Game.Messages;

namespace DamageMeter.Heuristic
{
    class S_CHANGE_PARTY_MANAGER : AbstractPacketHeuristic
    {
        public static S_CHANGE_PARTY_MANAGER Instance => _instance ?? (_instance = new S_CHANGE_PARTY_MANAGER());
        private static S_CHANGE_PARTY_MANAGER _instance;

        public S_CHANGE_PARTY_MANAGER() : base(OpcodeEnum.S_CHANGE_PARTY_MANAGER) { }

        public new void Process(ParsedMessage message)
        {
            base.Process(message);
            if (IsKnown || OpcodeFinder.Instance.IsKnown(message.OpCode)) return;

            if (message.Payload.Count < 2 + 4 + 4 + 4) return;
            if (!OpcodeFinder.Instance.IsKnown(OpcodeEnum.S_PARTY_MEMBER_LIST)) return;
            if (!OpcodeFinder.Instance.KnowledgeDatabase.ContainsKey(OpcodeFinder.KnowledgeDatabaseItem.PartyMemberList)) return;
            var nameOffset = Reader.ReadUInt16();
            if (nameOffset != 14) return;

            var serverId = Reader.ReadUInt32();
            if (BasicTeraData.Instance.Servers.GetServer(serverId) == null) return;

            var playerId = Reader.ReadUInt32();
            var name = "";
            try
            {
                if (Reader.BaseStream.Position != nameOffset - 4) return;
                name = Reader.ReadTeraString();

            }
            catch (Exception e) { return; }
            if (OpcodeFinder.Instance.KnowledgeDatabase.TryGetValue(OpcodeFinder.KnowledgeDatabaseItem.PartyMemberList, out var res))
            {
                var list = (List<PartyMember>)res.Item2;
                if (!list.Any(x => x.PlayerId == playerId && x.ServerId == serverId && x.Name == name)) return;
            }
            else return;
            if (OpcodeFinder.Instance.GetMessage(OpcodeFinder.Instance.PacketCount - 1).OpCode == C_CHANGE_PARTY_MANAGER.Instance.PossibleOpcode)
            {
                if (C_CHANGE_PARTY_MANAGER.Instance.LastPlayerId == playerId && C_CHANGE_PARTY_MANAGER.Instance.LastServerId == serverId)
                {
                    C_CHANGE_PARTY_MANAGER.Instance.Confirm();
                    OpcodeFinder.Instance.SetOpcode(message.OpCode, OPCODE);
                }
            }

        }
    }
}