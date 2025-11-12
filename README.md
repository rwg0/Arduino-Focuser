What is it?
==========

This project is forked from [EJHolmes/Arduino Focuser](http://ejholmes.github.com/2010/03/28/the-arduino-focuser.html) and substantially modified.

The goal was to create a simple Arduino Focuser with ASCOM driver. Yes, there are lots of Arduino focusers already, but many are now complex projects that support displays, rotary dials, etc. Others are hosted on SourceForge and have hundreds of different files making it hard to work out what is what. All I wanted was to make a stepper motor turn under ASCOM control without too much complexity. The EJHolmes Arduino sketch made a good start, but I replaced the ASCOM driver, since I already had code for an ASCOM focuser driver that I could re-use.

The hardware used is

* An [Arduino](http://www.arduino.cc/) Uno
* A [CNC Shield](https://ooznest.co.uk/product/arduino-cnc-shield/)
* An [A4988 stepper motor controller](https://ooznest.co.uk/product/a4988-stepper-motor-driver/) - often sold with the CNC shield
* A [NEMA 17](https://ooznest.co.uk/product/nema17-stepper-motors/) Stepper motor
* A couple of wires and a connector for the stepper motor

Construction is simple:

* Load the sketch onto the Arduino
* Plug A4988 driver into the CNC Shield 'X' socket. If you wish to use microstepping, you can place jumpers on the 3 pairs of pins under the board (all 3 give x16 microstepping)
* Solder 2 wires to the VIN and GND pins at the bottom of the CNC Shield and plug them into the + and - motor supply screw terminals on the shield. 

        View the shield from the side with the sockets for the stepper drivers on, with the reset button top left. If the shield is plugged into the Arduino then the Arduino USB socket will be on the left near the reset button on the shield.
        
        There are 2 groups of 6 pins in the bottom row of the CNC shield. You need the right-hand most of the left hand group (VIN) and the one to the left of it (GND). Roughly under the center of the 'A' socket.

        These wires take the 12V power from the Arduino supply to run the stepper motor, since it cannot run from 5V USB power. Expect it to draw about 0.2A

* Plug the shield into the Arduino
* Connect the stepper motor to the 4 pins to the right of the A4988 board (when viewed with the shield reset button top left). The wiring order from top to bottom should be blue, red, black, green.
* Connect the Arduino to 12V power and USB.

If everything is working then the motor spindle should now be almost impossible to turn because the stepper motor fields will be activated to hold it in place.

![Wiring](https://github.com/rwg0/Arduino-Focuser/blob/6ff70877a0cab6d062db13310570f78681115545/WiringExample.jpg)

Building Your Own
-----------------

1. Download an install the ASCOM driver from the [downloads page]() (link pending).
2. Download and install the [Arduino IDE](http://arduino.cc/).
3. Download the [source code](https://github.com/ejholmes/Arduino-Focuser).
4. Connect your Arduino to your computer and compile and upload the firmware within the "firmware" directory in the source code.



Technical Information
---------------------

Since the Arduino Focuser is based on the Arduino board, the focuser is controlled through the COM port created by the Arduino. This is accomplished by parsing serial commands sent from the ASCOM Driver. When a command is received in the `: command <arg> #` format, the Messenger object is passed to the `Focuser::interpretCommand()` function, which looks at the `command` and acts accordingly.
 
The following commands are currently available:

- `: M <position> #` Moves the focuser to `position`.
- `: R <0-1> #` Reverses motor direction. 0=false, 1=true.
- `: P <position> #` Sets the current position to `position`. Note, this does not issue a move, it just tells the focuser that it's at said position.
- `: G #` Prints out the current position to the serial port. Useful for syncing.
- `: H #` Doesn't really do anything since anything in the serial buffer will halt the motor. Just prettier.
- `: S #` Sets motor speed to between 50 and 1000 steps per second. Note that the motor will accelerate and decelerate at the start/end of a move
- `: I #` Repeats the identification text sent by the focuser at connection. The text is 'R Simple.Arduino.Focuser'

Note that carriage return/line feeds after the commands are not required. A `:` character is always interpreted as starting a new command.

The following replies should be expected

- `P XXXX` Reports the current position and indicates that the motor is stopped. This is the final response to 'M' commands and to 'P' and 'G' commands.

- `M XXXX` Reports the current position during a move, reported when the step position is a multiple of 100, so may not be reported during short moves

- `OK` Indicates that a command has been accepted (ie set speed)
- `ERR` Indicates that a command was not accepted either due to a bad command or bad parameter

Responses are followed by a line termination of `\r\n`




