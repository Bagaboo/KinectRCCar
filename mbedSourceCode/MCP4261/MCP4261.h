#ifndef MCP4261_H
#define MCP4261_H

#include "mbed.h"

#define CONSTANT 65280

class MCP4261 {
public:
    friend class MCP4261Test;

    enum LEDDirection
    {
        Down,
        Up
    };
    
    enum Address
    {
        POT_0,POT_1,NVPOT_0,NVPOT_1, TCON, STATUS, 
        EPROM_0, EPROM_1, EPROM_2, EPROM_3, EPROM_4, 
        EPROM_5, EPROM_6, EPROM_7, EPROM_8, EPROM_9
    };
    
    enum data
    {
        POT_MIN = 0,
        POT_MAX = 256
    };
    
    enum error
    {
        CMDERR_16 = 9,
        CMDERR_8 = 1
    };

    MCP4261();
    ~MCP4261();
    int Read(Address address);
    bool Write(Address, int);
    bool Increment(Address);
    bool Decrement(Address);
    bool GetChipSelect();
    bool GetIsContinuous();
    void SetIsContinuous(bool);
    void SetChipSelect(bool);
    bool Reset();
    
private:    
    static bool CheckAddress(int);
    static bool CheckValidResult(int, const int);
    
    int _isContinuous;
    
    SPI *_spi;
    DigitalOut *_cs;
};

#endif