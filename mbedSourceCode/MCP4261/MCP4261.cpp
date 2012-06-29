#include "mbed.h"
#include "MCP4261.h"
#include <bitset>

MCP4261::MCP4261() {
    _spi = new SPI(p5,p6,p7);
    _cs = new DigitalOut(p8);
    
    Reset();
    SetChipSelect(false);
    SetIsContinuous(false);
}

MCP4261::~MCP4261()
{
    //reset to midpoint;
    Reset();
    
    SetChipSelect(false);
    SetIsContinuous(false);
    
    delete _spi;
    delete _cs;
    
    _spi = NULL;
    _cs = NULL;
    
}

int MCP4261::Read(Address address) {

    int response = 0;

    SetChipSelect(true);

    if (CheckAddress(address)) {

        int command;

        command = address << 2;
        command = (command | 3) << 10;

        _spi->format(16);

        response = _spi->write(command);

    }

    if (!GetIsContinuous())
        SetChipSelect(false);
    
    bool isValid = CheckValidResult(response, CMDERR_16);
       
    if(isValid)
        return response - 65024;
    else
        return -1;
}

bool MCP4261::Write(Address address, int data) {

    int response = 0;

    SetChipSelect(true);


    if (CheckAddress(address) && data >= 0 && data <= 256) {

        int command;

        command = address << 2;
        command = (command | 0) << 10;
        command = command | data;

        _spi->format(16);
        response = _spi->write(command);

        if (!GetIsContinuous())
            SetChipSelect(false);
    }
    
    bool isValid = CheckValidResult(response, CMDERR_16);
    
    return isValid;
}

bool MCP4261::Increment(Address address) {

    int response = 0;

    SetChipSelect(true);

    if (CheckAddress(address)) {
        int command;

        command = address << 2;
        command = (command | 1) << 2;

        _spi->format(8);
        response = _spi->write(command);

        if (!GetIsContinuous())
            SetChipSelect(false);
    }
    
    bool isValid = CheckValidResult(response, CMDERR_8);
    
    return isValid;
}

bool MCP4261::Decrement(Address address) {

    int response = 0;

    SetChipSelect(true);
    if (CheckAddress(address)) {
        int command;

        command = address << 2;
        command = (command | 2) << 2;

        _spi->format(8);
        response = _spi->write(command);

        if (!GetIsContinuous())
            SetChipSelect(false);

    }

    bool isValid = CheckValidResult(response, CMDERR_8);
    
    return isValid;
}

bool MCP4261::CheckAddress(int address) {
    return address >= POT_0 && address <= EPROM_9;
}

bool MCP4261::CheckValidResult(int response, const int errorPos)
{
    //why "!". bit is set to 1 if the command was good. 0 if there was an error.
    bool isError = (response & (1 << errorPos));
    
    return isError;
}

bool MCP4261::GetIsContinuous() {
    return _isContinuous;
}
void MCP4261::SetIsContinuous(bool isContinuous) {
    _isContinuous = isContinuous;
    
    SetChipSelect(_isContinuous);
}

bool MCP4261::GetChipSelect() {
    return !*_cs;
}
void MCP4261::SetChipSelect(bool cs) {
    if(*_cs != cs);
        *_cs = !cs;
}

bool MCP4261::Reset()
{
    bool result0 = Write(POT_0, 128);
    bool result1 = Write(POT_1, 128);
    
    return result0 == true && result1 == true;
}