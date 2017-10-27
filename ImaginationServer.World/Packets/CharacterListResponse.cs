﻿using System;
using System.Collections.Generic;
using System.IO;
using ImaginationServer.Common;
using ImaginationServer.Common.Data;

namespace ImaginationServer.World.Packets
{
    // Struct that puts names to each pants ID (for easy use)
    internal enum CharCreatePantsColor : ulong
    {
        PantsBrightRed = 2508,
        PantsBrightOrange = 2509,
        PantsBrickYellow = 2511,
        PantsMediumBlue = 2513,
        PantsSandGreen = 2514,
        PantsDarkGreen = 2515,
        PantsEarthGreen = 2516,
        PantsEarthBlue = 2517,
        PantsBrightBlue = 2519,
        PantsSandBlue = 2520,
        PantsDarkStoneGray = 2521,
        PantsMediumStoneGray = 2522,
        PantsWhite = 2523,
        PantsBlack = 2524,
        PantsReddishBrown = 2526,
        PantsDarkRed = 2527
    }

    // Struct that puts names to each base shirt ID (for easy use)
    internal enum CharCreateShirtColor : ulong
    {
        ShirtBrightRed = 4049,
        ShirtBrightBlue = 4083,
        ShirtBrightYellow = 4117,
        ShirtDarkGreen = 4151,
        ShirtBrightOrange = 4185,
        ShirtBlack = 4219,
        ShirtDarkStoneGray = 4253,
        ShirtMediumStoneGray = 4287,
        ShirtReddishBrown = 4321,
        ShirtWhite = 4355,
        ShirtMediumBlue = 4389,
        ShirtDarkRed = 4423,
        ShirtEarthBlue = 4457,
        ShirtEarthGreen = 4491,
        ShirtBrickYellow = 4525,
        ShirtSandBlue = 4559,
        ShirtSandGreen = 4593
    }

    public class CharacterListResponse
    {
        public Account CharListAccount { get; }

        public CharacterListResponse(Account account)
        {
            CharListAccount = account;
        }

        public void Send(string clientAddress)
        {
            DbUtils db = new DbUtils();
            using (var bitStream = new WBitStream())
            {
                bitStream.WriteHeader(PacketEnums.RemoteConnection.Client, (uint)PacketEnums.WorldServerPacketId.MsgClientCharacterListResponse);

                int charCount = CharListAccount.Characters.Count;
                int frontChar = 0;
                List<Character> playerChars = new List<Character>(charCount);
                for (int i = 0; i < charCount; i++)
                {
                    string charName = CharListAccount.Characters[i];
                    playerChars.Add(db.GetCharacter(charName));
                    if (charName == CharListAccount.SelectedCharacter)
                    {
                        frontChar = i;
                    }
                }

                bitStream.Write((byte) charCount);
                bitStream.Write((byte) frontChar);

                // Write each character
                foreach (Character pChar in playerChars)
                {
                    bitStream.Write(pChar.Id);

                    bitStream.Write((uint) 0); // ???
                    // Write Name
                    string name = pChar.Name;
                    int nameTrace = 0;
                    for (int i = 0; i < 66; i++)
                    {
                        if (i % 2 == 0 && nameTrace < name.Length)
                        {
                            bitStream.WriteString(name[nameTrace].ToString(), 1);
                            nameTrace++;
                        }
                        else
                        {
                            bitStream.Write((byte) 0);
                        }
                    }
                    // Write FtpName
                    string ftpName = pChar.FtpName;
                    nameTrace = 0;
                    for (int i = 0; i < 66; i++)
                    {
                        if (i % 2 == 0 && nameTrace < ftpName.Length)
                        {
                            bitStream.WriteString(ftpName[nameTrace].ToString(), 1);
                            nameTrace++;
                        }
                        else
                        {
                            bitStream.Write((byte) 0);
                        }
                    }
                    bitStream.Write(Convert.ToByte(pChar.NameRejected));
                    bitStream.Write(Convert.ToByte(pChar.FreeToPlay));
                    byte zeroChar = 0;
                    byte oneChar = 1;
                    for (int i = 0; i < 10; i++)
                    {
                        if (i == 1 && playerChars.IndexOf(pChar) == 0)
                        {
                            bitStream.Write(oneChar);
                        }
                        else
                        {
                            bitStream.Write(zeroChar);
                        }
                    }
                    bitStream.Write(pChar.ShirtColor);
                    bitStream.Write(pChar.ShirtStyle);
                    bitStream.Write(pChar.PantsColor);
                    bitStream.Write(pChar.HairStyle);
                    bitStream.Write(pChar.HairColor);
                    bitStream.Write(pChar.Lh); // “lh”, see “<mf />” row in the xml data from chardata packet (no idea what it is)
                    bitStream.Write(pChar.Rh); // “rh”, see “<mf />” row in the xml data from chardata packet (no idea what it is)
                    bitStream.Write(pChar.Eyebrows);
                    bitStream.Write(pChar.Eyes); 
                    bitStream.Write(pChar.Mouth); 
                    bitStream.Write((uint)0); // ???
                    bitStream.Write(pChar.LastZoneId);
                    bitStream.Write(pChar.MapInstance);
                    bitStream.Write(pChar.MapClone); // map clone (name from [53-05-00-02] structure)
                    bitStream.Write((ulong)0); // last login or logout timestamp of character in seconds? (xml is “llog” so both could be possible)
                    bitStream.Write((ushort)pChar.Items.Count); // number of items to follow

                    // equipped item LOTs (order of items doesn’t matter? I think it reads them in order so if we accidentally put 2 shirts the second one will be the one shown.)
                    foreach(var item in pChar.Items) {
                        bitStream.Write(item.Lot); // pretty sure you write this
                    }
                }

                // Debugging
                File.WriteAllBytes("Temp/charlistresponse.bin", bitStream.GetBytes());

                // Send packet
                WorldServer.Server.Send(bitStream, WPacketPriority.SystemPriority,
                    WPacketReliability.ReliableOrdered, 0, clientAddress, false);
            }
        }

    }
}
