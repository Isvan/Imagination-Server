#include <RakNetworkFactory.h>
#include <MessageIdentifiers.h>
#include <RSACrypt.h>
#include <BigTypes.h>

#include "BaseServer.h"

using namespace System::Runtime::InteropServices;
using namespace System;
using namespace big;

BaseServer::BaseServer(int port, int maxConnections, String^ address)
{
	_port = port;
	_maxConnections = maxConnections;
	Address = (char*)(Marshal::StringToHGlobalAnsi(address)).ToPointer();
}

void BaseServer::Send(WBitStream^ bitStream, WPacketPriority priority, WPacketReliability reliability, char orderingChannel, String^ systemAddress, bool broadcast)
{	
	auto split = systemAddress->Split(':');
	SystemAddress unmanaged;
	unmanaged.SetBinaryAddress((char*)(Marshal::StringToHGlobalAnsi(split[0])).ToPointer());
	unmanaged.port = (unsigned short) strtoul((char*)(Marshal::StringToHGlobalAnsi(split[1])).ToPointer(), NULL, 0);
	_peer->Send(bitStream->Instance, static_cast<PacketPriority>(priority), static_cast<PacketReliability>(reliability), orderingChannel, unmanaged, broadcast);
}

RakPeerInterface* BaseServer::GetPeer()
{
	return _peer;
}

ReplicaManager* BaseServer::GetReplicaManager()
{
	return _replicaManager;
}

void BaseServer::Start(bool encrypt)
{
	_peer = RakNetworkFactory::GetRakPeerInterface();
	
	// Initialize Security
	_peer->SetIncomingPassword("3.25 ND1", 8);
	if (encrypt)
	{
		big::u32 e; RSA_BIT_SIZE n;
		BIGHALFSIZE(RSA_BIT_SIZE, p); BIGHALFSIZE(RSA_BIT_SIZE, q);
		big::RSACrypt<RSA_BIT_SIZE> rsaCrypt;
		rsaCrypt.generateKeys();
		rsaCrypt.getPublicKey(e, n);
		rsaCrypt.getPrivateKey(p, q);
		_peer->InitializeSecurity(0, 0, (char*)p, (char*)q);
	}

	SocketDescriptor socketDescriptor = SocketDescriptor(_port, Address);
	_peer->Startup(_maxConnections, 30, &socketDescriptor, 1);
	_peer->SetMaximumIncomingConnections(_maxConnections);
	
	_replicaManager = new ReplicaManager();
	_networkIdManager = new NetworkIDManager();

	_replicaManager->SetAutoParticipateNewConnections(true);
	_replicaManager->SetAutoSerializeInScope(true);

	_networkIdManager->SetIsNetworkIDAuthority(true);

	_peer->AttachPlugin(_replicaManager);
	_peer->SetNetworkIDManager(_networkIdManager);
	OnStart();
}

void BaseServer::Service()
{
	auto packet = GetPeer()
		->Receive();
	if (!packet) return;
	switch (packet->data[0])
	{
	case ID_USER_PACKET_ENUM:
		OnReceived(native_to_managed_array(packet->data, packet->length), packet->length, gcnew String(packet->systemAddress.ToString()));
		break;
	case ID_DISCONNECTION_NOTIFICATION:
	case ID_CONNECTION_LOST:
		OnDisconnect(gcnew String(packet->systemAddress.ToString()));
		break;
	case ID_NEW_INCOMING_CONNECTION:
		OnConnect(gcnew String(packet->systemAddress.ToString()));
		break;
	default:
		Console::WriteLine("Unknown RakNet packet received! {0}", packet->data[0]);
		break;
	}
}

void BaseServer::SendInitPacket(bool auth, String^ address)
{
	auto split = address->Split(':');
	InitPacket initPacket(this, auth);
	auto unmanaged = new SystemAddress();
	unmanaged->SetBinaryAddress((char*)(Marshal::StringToHGlobalAnsi(split[0])).ToPointer());
	unmanaged->port = (unsigned short)strtoul((char*)(Marshal::StringToHGlobalAnsi(split[1])).ToPointer(), NULL, 0);
	BitStream bitStream;
	bitStream.Write((unsigned char)83);
	bitStream.Write((unsigned short)0);
	bitStream.Write((unsigned long)0);
	bitStream.Write((unsigned char)0);
	bitStream.Write((char*)&initPacket, sizeof(initPacket));
	_peer->Send(&bitStream, SYSTEM_PRIORITY, RELIABLE_ORDERED, 0, *unmanaged, false);
	delete unmanaged;
}

void BaseServer::Stop()
{
	OnStop();
	RakNetworkFactory::DestroyRakPeerInterface(_peer);
	RakNetworkFactory::DestroyReplicaManager(_replicaManager);
	delete(_networkIdManager);
}