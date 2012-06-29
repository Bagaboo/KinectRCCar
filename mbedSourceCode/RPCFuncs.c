#include "mbed.h"
#include "RPCFuncs.h"
#include "RCController.h"
#include "SerialRPCInterface.h"


RCController _rcController;

SerialRPCInterface _rpc(USBTX,USBRX, 115200);

RPCFunction Read(&read, "Read");
RPCFunction Write(&write, "Write");
RPCFunction Increment(&increment, "Increment");
RPCFunction Decrement(&decrement, "Decrement");
RPCFunction SetIsContinuous(&setIsContinuous, "SetIsContinuous");
RPCFunction GetIsContinuous(&getIsContinuous, "GetIsContinuous");
RPCFunction GetChipSelect(&getChipSelect, "GetChipSelect");
RPCFunction Reset(&reset, "Reset");

void read(char * input, char * output) {
    MCP4261::Address address = (MCP4261::Address) atoi(input);

    int response = _rcController.Read(address);

    sprintf(output,"%d",response);
}

void write(char * input, char * output) {

    MCP4261::Address address = (MCP4261::Address) atoi(strtok(input, " "));

    int data = atoi(strtok(NULL, " "));

    int response = _rcController.Write(address, data);

    sprintf(output,"%d",response);
}

void increment(char * input, char * output) {
    MCP4261::Address address = (MCP4261::Address) atoi(input);

    int response = _rcController.Increment(address);

    sprintf(output,"%d",response);
}

void decrement(char * input, char * output) {
    MCP4261::Address address = (MCP4261::Address) atoi(input);

    int response = _rcController.Decrement(address);

    sprintf(output,"%X",response);
}

void setIsContinuous(char * input, char * output) {
    int isContinuous = atoi(input);

    _rcController.SetIsContinuous(isContinuous);

    int response = _rcController.GetIsContinuous();

    sprintf(output, "%d", response);
}

void getIsContinuous(char * input, char * output) {
    int response = _rcController.GetIsContinuous();

    sprintf(output, "%d", response);
}

void getChipSelect(char * input, char * output){
    int response = _rcController.GetChipSelect();
    
    sprintf(output, "%d", response);
}

void reset(char* input, char* output) {
    bool response = _rcController.Reset();
    
    sprintf(output, "%d", response);
}