#include <HardwareSerial.h>
//ADDED FOR COMPATIBILITY WITH WIRING
extern "C" {
#include <stdlib.h>
}

#include "Focuser.h"



//
// Constructor
//
Focuser::Focuser(void) {
  speed = FASTSPEED;
  position = 0;
  reversed = false;
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
    case 'S':
      if (setSpeed(message->readInt())) {
        printOK();
        return;
      }
  }
  Serial.println("ERR");
}

bool Focuser::setSpeed(int speed) {
  if (speed >= 50 && speed <= 1000) {
    this->speed = speed;
    return true;
  } else {
    return false;
  }
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
  long dist = abs(move);
  long moved = 0;
  int fastest = 0;
  motor.setSpeed(speed / 10);
  for (int i = 1; i <= 10; i++) {
    int s = i * speed / 10;
    int steps = s/10 + 1;
    if ((moved + steps) * 2 >= dist)
      break;
    motor.setSpeed(s);
    step(steps * dir);
    moved += steps;
    fastest = i;
  }

  step(dir * (dist - 2 * moved));

  for (int i = fastest; i >= 1; i--) {
    int s = i * speed / 10;
    int steps = s/10 + 1;
    motor.setSpeed(s);
    step(steps * dir);
  }



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
      if (position % 100 == 0) {
        Serial.print("M ");
        Serial.println(position);
      }
    }
  } else if (val < 0) {  // else if move is negative, move backward
    long counter = abs(val);
    while (counter--) {
      if (Serial.available() > 0 || position == 0)
        break;
      motor.step(1, (reversed) ? BACKWARD : FORWARD);
      position--;
      if (position % 100 == 0) {
        Serial.print("M ");
        Serial.println(position);
      }
    }
  }
}
