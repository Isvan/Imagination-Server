// ImaginationServer.World.Packets.h

#pragma once

#include "Stdafx.h"
#include <BitStream.h>
#include <MessageIdentifiers.h>

using namespace RakNet;
using namespace System;
using namespace System::Collections::Generic;
using namespace cli;
using namespace ImaginationServer::Common::Data;

namespace ImaginationServerWorldPackets {

	public ref class WorldPackets
	{
	public:
		static void SendCharacterListResponse(String^ address, Account^ account);

		static void WriteStringToBitStream(const char* myString, int stringSize, int maxChars, RakNet::BitStream* output)
		{
			// Write the string to provided BitStream along with the size
			output->Write(myString, stringSize);

			// Check to see if there are any bytes remaining according to user
			// specification
			auto remaining = maxChars - stringSize;

			// If so, fill with 0x00
			for (auto i = 0; i < remaining; i++)
			{
				unsigned char zero = 0;

				output->Write(zero);
			}
		}

		static void CreatePacketHeader(MessageID messageId, unsigned short connectionType, unsigned long internalPacketId, BitStream* output)
		{
			unsigned char unknown1 = 0; // This is an unknown uchar

			// Write data to provided BitStream
			output->Write(messageId);
			output->Write(connectionType);
			output->Write(internalPacketId);
			output->Write(unknown1);
		}
	};
}
