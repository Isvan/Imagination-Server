﻿using System;
using System.IO;
using ImaginationServer.Common;
using ImaginationServer.Common.Data;
using ImaginationServer.Common.Handlers;
using ImaginationServer.World.Packets;
using ImaginationServerWorldPackets;
using Newtonsoft.Json;

namespace ImaginationServer.World.Handlers.World
{
    public class CharacterListRequestHandler : PacketHandler
    {
        public override void Handle(BinaryReader reader, LuClient client)
        {
            using (var database = new DbUtils())
            {
                if (!client.Authenticated) return; // Make sure they've authenticated first!

                // Retrieve their account
                var account = database.GetAccount(client.Username);
                // TODO: Remove now that C# is working
                // Call the C++ code that generates and sends the character list packet of the specified account
                //WorldPackets.SendCharacterListResponse(client.Address, account, WorldServer.Server);
                // Call the C# code that generates and sends the character list packet of the specified account
                new CharacterListResponse(account).Send(client.Address);

                Console.WriteLine($"Sent character list packet to {client.Username}. Users: " +
                                  JsonConvert.SerializeObject(account.Characters));
            }
        }
    }
}