
#ifndef Focuser_h
#define Focuser_h


#define FASTSPEED 200


#include <inttypes.h>
#include <avr/io.h>
#include "AFMotor.h"
#include "Messenger.h"

class Focuser
{
  public:
    Focuser(void); // Constructor
    void interpretCommand(Messenger *message); // Function for interpreting a command string
  private:
    void move(long val); // Function for moving the focuser
    void setPosition(long newpos); // For setting position
    void reverse(bool rev); // For setting motor polarity
    bool setSpeed(int speed);
    void printPosition(); // Prints current position to serial
    void step(long val);
    long position;
    bool reversed;
    int speed;
    AF_Stepper motor;  // Create our stepper motor. I have mine on port 2 of the motor shield.

};

#endif
