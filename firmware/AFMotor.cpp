// Adafruit Motor shield library
// copyright Adafruit Industries LLC, 2009
// this code is public domain, enjoy!


#include <avr/io.h>
#include "Arduino.h"
#include "AFMotor.h"
#include "HardwareSerial.h"

#define dirPin 2
#define stepPin 3


AF_Stepper::AF_Stepper() {
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
}

void AF_Stepper::setSpeed(uint16_t stepsPerSecond) {
  usperstep = 1000000 / stepsPerSecond;

}

void AF_Stepper::release(void) {
}

void AF_Stepper::step(uint16_t steps, uint8_t dir) {
  uint32_t uspers = usperstep;

  while (steps--) {
    onestep(dir);
    delayMicroseconds(uspers); // in ms
  }
}

void AF_Stepper::onestep(uint8_t dir) {
  digitalWrite(dirPin, dir ? HIGH : LOW) ;

  digitalWrite(stepPin, HIGH);
  delayMicroseconds(200);
  digitalWrite(stepPin, LOW);
  delayMicroseconds(200);
}

