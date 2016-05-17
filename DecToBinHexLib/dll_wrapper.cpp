#include <string>
#include "Library.h"

extern "C"
{
	__declspec(dllexport) void __cdecl GetResult(int _number, char *_buffer, int _buffer_size)
	{
		std::string res = DecToBinHexLib::ComputeForInt(_number);
		if (res.size() < _buffer_size)
		{
			std::strcpy(_buffer, res.c_str());
		}
	}

	__declspec(dllexport) int __cdecl GetBufferSize()
	{
		return DecToBinHexLib::GetMaxResultLength() + 1;
	}
}