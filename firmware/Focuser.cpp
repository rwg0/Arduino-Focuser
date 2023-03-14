#include <HardwareSerial.h>
//ADDED FOR COMPATIBILITY WITH WIRING
extern "C" {
#include <stdlib.h>
}

//#include "WProgram.h"
#include "Focuser.h"

AF_Stepper motor;  // Create our stepper motor. I have mine on port 2 of the motor shield.

long position = 0;
bool reversed = false;

//
// Constructor
//
Focuser::Focuser(void) {
  motor.setSpeed(10);  // Set a default RPM of 10
}

void printOK() {
  Serial.println("OK");
}

//
// Function for interpreting a command string of the format ": COMMAND <ARGUMENT> #"
//
void Focuser::interpretCommand(Messenger *message) {
  message->readChar();  // Reads ":"

  char command = message->readChar();  // Read the command
  switch (command) {
    case 'M':  // Move
      move(message->readLong());
      return;
    case 'R':  // Release motor coils
      reverse(message->readInt());
      printOK();
      return;
    case 'L':
      motor.release();
      printOK();
      return;
    case 'P':
      setPosition(message->readLong());
      return;
    case 'G':
      printPosition();
      return;
    case 'H':
      Serial.flush();
      printPosition();
      return;
    case 'I':
      Serial.println("R Simple.Arduino.Focuser");
      return;
  }
  Serial.println("ERR");
}

void Focuser::setPosition(long newpos) {
  position = newpos;
  printPosition();
}

void Focuser::printPosition() {
  Serial.print("P ");
  Serial.println(position);
}

void Focuser::reverse(bool rev = false) {
  reversed = rev;
}

//
// Function for issuing moves
//
void Focuser::move(long val) {
  long move = val - position;  // calculate move
  int dir = move > 0 ? 1 : -1;
  int dist = abs(move);
  if (dist > FAST) {
    int moved = 0;
    int fastest = 0;
    motor.setSpeed(FASTSPEED / 10);
    for (int i = 1; i <= 10; i++) {
      int steps = i + 5;
      if ((moved + steps) * 2 >= dist)
        break;
      motor.setSpeed(i * FASTSPEED / 10);
      step(steps * dir);
      moved += steps;
      fastest = i;
    }

    step(dir * (dist - 2 * moved));

    for (int i = fastest; i >= 1; i--) {
      int steps = i + 5;
      motor.setSpeed(i * FASTSPEED / 10);
      step(steps * dir);
    }

  } else {
    motor.setSpeed(SLOWSPEED);
    step(move);
  }

  motor.release();  // Release the motors when done. This works well for me but might not for others
  Serial.print("P ");
  Serial.println(position);
}

void Focuser::step(long val) {
  if (val > 0) {  // If move is positive, move forward
    while (val--) {
      if (Serial.available() > 0)
        break;
      motor.step(1, (reversed) ? FORWARD : BACKWARD);
      position++;
    }
  } else if (val < 0) {  // else if move is negative, move backward
    long counter = abs(val);
    while (counter--) {
      if (Serial.available() > 0 || position == 0)
        break;
      motor.step(1, (reversed) ? BACKWARD : FORWARD);
      position--;
    }
  }
}
