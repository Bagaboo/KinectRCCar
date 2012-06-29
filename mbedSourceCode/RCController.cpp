#include "mbed.h"
#include "RCController.h"

RCController::RCController() {
    _mcp = new MCP4261;

    _led1 = new DigitalOut(LED1);
    _led2 = new DigitalOut(LED2);
    _led3 = new DigitalOut(LED3);
    _led4 = new DigitalOut(LED4);

    _pot0Value = _mcp->Read(MCP4261::POT_0) - CONSTANT;
    _pot1Value = _mcp->Read(MCP4261::POT_1) - CONSTANT;
}

RCController::~RCController() 
{
    delete _mcp;
    delete _led1;
    delete _led2;
    delete _led3;
    delete _led4;
    
    _mcp = NULL;
    _led1 = NULL;
    _led2 = NULL;
    _led3 = NULL;
    _led4 = NULL;
    
    _led_POT0 = NULL;
    _led_POT1 = NULL;
}

int RCController::Read(MCP4261::Address address) {
    int result = _mcp->Read(address);
    clearLED(address);
    return result;
}

bool RCController::Write(MCP4261::Address address, int data) {
    lightLED(address,data);
    return _mcp->Write(address,data);    
}

bool RCController::Increment(MCP4261::Address address) {
    lightLEDInc(address);
    return _mcp->Increment(address);
}

bool RCController::Decrement(MCP4261::Address address) {
    lightLEDDec(address);
    return _mcp->Decrement(address);
}

void RCController::SetIsContinuous(bool value)
{
    _mcp->SetIsContinuous(value);
}

bool RCController::GetIsContinuous()
{
    bool result = _mcp->GetIsContinuous();
    return result;
}

bool RCController::GetChipSelect()
{
    bool result = _mcp->GetChipSelect();
    return result;
}

bool RCController::Reset()
{
    return _mcp->Reset();
}

void RCController::lightLEDInc(MCP4261::Address address) {
    if (address == MCP4261::POT_0)
        lightLED(address, _pot0Value + 1);
    if (address == MCP4261::POT_1)
        lightLED(address, _pot1Value + 1);
}

void RCController::lightLEDDec(MCP4261::Address address) {
    if (address == MCP4261::POT_0)
        lightLED(address, _pot0Value - 1);
    if (address == MCP4261::POT_1)
        lightLED(address, _pot1Value - 1);
}

void RCController::lightLED(MCP4261::Address address, int data) {

    if (address == MCP4261::POT_0) {

        if (data > _pot0Value) {
            _led_POT0 = _led2;
            _pot0Value = data;
        } else if (data < _pot0Value) {
            _led_POT0 = _led1;
            _pot0Value = data;
        }
        if (_led_POT0 != NULL)
            *_led_POT0 = 1;
    }

    if (address == MCP4261::POT_1) {

        if (data < _pot1Value) {
            _led_POT1 = _led3;
            _pot1Value = data;
        } else if (data > _pot1Value) {
            _led_POT1 = _led4;
            _pot1Value = data;
        }
        if (_led_POT1 != NULL)
            *_led_POT1 = 1;
    }
}

void RCController::clearLED(MCP4261::Address address) {
    if (address == MCP4261::POT_0)
        if (_led_POT0 != NULL) {
            *_led_POT0 = 0;
            _led_POT0 = NULL;
        }

    if (address == MCP4261::POT_1)
        if (_led_POT1 != NULL) {
            *_led_POT1 = 0;
            _led_POT1 = NULL;
        }
}
