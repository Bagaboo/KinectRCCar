#ifndef RCCONTROLLER_H
#define RCCONTROLLER_H

#include "MCP4261.h"

#define CONSTANT 65280

class RCController
{
    public:
        RCController();
        ~RCController();
        
        int  Read(MCP4261::Address);
        bool Write(MCP4261::Address, int);
        bool Increment(MCP4261::Address);
        bool Decrement(MCP4261::Address);
        void SetIsContinuous(bool);
        bool GetIsContinuous();
        bool GetChipSelect();
        bool Reset();
    private:
        void lightLEDInc(MCP4261::Address);
        void lightLEDDec(MCP4261::Address);
        void lightLED(MCP4261::Address, int);
        void clearLED(MCP4261::Address);
        
        MCP4261 *_mcp;
        
        int _pot0Value;
        int _pot1Value;
        
        DigitalOut *_led1;
        DigitalOut *_led2;
        DigitalOut *_led3;
        DigitalOut *_led4;
        
        DigitalOut *_led_POT0;
        DigitalOut *_led_POT1;
};

#endif