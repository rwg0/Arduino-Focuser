// Adafruit Motor shield library
// copyright Adafruit Industries LLC, 2009
// this code is public domain, enjoy!

#ifndef _AFMotor_h_
#define _AFMotor_h_

#include <inttypes.h>
#include <avr/io.h>

#define BACKWARD 0
#define FORWARD 1

class AF_Stepper {
 public:
  AF_Stepper();
  void step(uint16_t steps, uint8_t dir);
  void setSpeed(uint16_t);
  void onestep(uint8_t dir);
  void release(void);
   uint32_t usperstep;
 private:
};



#endif
