// Adafruit Motor shield library
// copyright Adafruit Industries LLC, 2009
// this code is public domain, enjoy!


#include <avr/io.h>
#include "Arduino.h"
#include "AFMotor.h"
#include "HardwareSerial.h"

// For CNC shield, motor channel X
#define dirPin 5
#define stepPin 2
// For CNC shield, need to enable by putting pin 8 low
#define enablePin 8

#define PULSE_TIME 20

AF_Stepper::AF_Stepper() {
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
  pinMode(enablePin, OUTPUT);
  digitalWrite(enablePin, LOW);
}

void AF_Stepper::setSpeed(uint32_t stepsPerSecond) {
  usperstep = 1000000 / stepsPerSecond;
}


void AF_Stepper::step(uint16_t steps, uint8_t dir) {
  uint32_t uspers = usperstep - 2 * PULSE_TIME;

  while (steps--) {
    onestep(dir);
    if (uspers < 10000)
    {
      delayMicroseconds(uspers);
    }
    else
    {
      delay(uspers/1000); // in ms
    }
  }
}

void AF_Stepper::onestep(uint8_t dir) {
  digitalWrite(dirPin, dir ? HIGH : LOW) ;

  digitalWrite(stepPin, HIGH);
  delayMicroseconds(PULSE_TIME);
  digitalWrite(stepPin, LOW);
  delayMicroseconds(PULSE_TIME);
}

