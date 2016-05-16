#include <string>
#include <sstream>
#include <iomanip>
#include <bitset>
#include "Library.h"

namespace DecToBinHexLib
{
	const std::string SEPARATOR = ", ";

	std::string ComputeForInt(int _number)
	{
		if (_number == 0)
		{
			return "0" + SEPARATOR + "0";
		}

		std::string sign = "";
		if (_number < 0)
		{
			sign = "-";
			_number = -_number;
		}

		std::ostringstream ostr;
		std::string bin = std::bitset<sizeof(int) * CHAR_BIT>(_number).to_string();

		ostr << sign << bin.substr(bin.find('1'));
		ostr << SEPARATOR;
		ostr << sign << std::hex << std::noshowbase << std::uppercase << _number;

		return ostr.str();
	}

	int GetMaxResultLength()
	{
		auto binsize = sizeof(int) * CHAR_BIT;
		auto hexsize = binsize / 4;

		return binsize + hexsize + SEPARATOR.length();
	}
}