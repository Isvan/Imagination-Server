#pragma once

#include <iostream>
#include <BitStream.h>

using namespace std;
using namespace RakNet;
using namespace System;

public ref class WBitStream
{
	void MarshalString(String^ value, string& output);
	//void MarshalString(String^ value, wstring& output);
	bool _destroyBitstream;
	public:
	BitStream* Instance;

	WBitStream();
	WBitStream(BitStream* instance);
	WBitStream(unsigned char* bytes, const int length, bool copyData);
	~WBitStream();

	void Write(unsigned char value);
	void Write(unsigned short value);
	void Write(unsigned long value);
	void Write(unsigned long long value);
	void Write(long value);
	void Write(long long value);
	void WriteChars(String^ value);
	void WriteString(String^ value, int maxLength);
	void WriteString(String^ value, int length, int maxLength);
	void WriteWString(String^ value, bool writeSize, bool nullChar);
	void Write(char* value, int length);
	void Write(unsigned char* value, int length);
	void Write(const char* value, int length, int maxLength);
	void Write(char* value);
	void Write(unsigned char* value);
	void Write(cli::array<unsigned char>^ value);
	void Write(wstring value, bool writeSize, bool nullChar);
	void Write(Single value);
	void Write(bool value);
	void Write(WBitStream^ value, unsigned int length);

	unsigned long GetNumberOfBytesUsed();
	cli::array<unsigned char>^ GetBytes();

	//unsigned char ReadByte();
	//unsigned short ReadUShort();
	//unsigned long ReadULong();
	//unsigned long long ReadULongLong();
	//long ReadLong();
	//long long ReadLongLong();
	//String^ ReadWString();
};
