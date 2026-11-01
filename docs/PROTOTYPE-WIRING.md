# Prototype wiring — ESP32 development board and two buffers

This is the budget perfboard route, separate from the bare ESP32 module in `StreamPixel.kicad_sch`. Use the **printed GPIO names**, not the module's physical pad numbers or a guessed position on the 30-pin board. Verify the actual board pin labels before soldering.

## Power and ground

Power the three panels from separate protected 5V supply branches using their supplied power leads. Do not carry panel current through the ESP32, perfboard or the custom controller's 1A fuse. Connect the controller and panel supply grounds together.

Both 74HCT245 chips: pin **20 → +5V**, pin **10 → GND**, pin **1 (DIR) → +5V**, pin **19 (/OE) → GND**. Add one **100nF capacitor directly between pins 20 and 10** on each chip. Do not connect 5V to the ESP32's 3V3 pin.

Use the development board's documented 5V/VIN input for external power. Program the ESP32 before attaching it to the panel harness. Disconnect the external 5V source and display harness for later USB programming unless the purchased board explicitly confirms safe power isolation.

## ESP32 GPIO → buffer input

U2:

- GPIO25 → pin 2 (red upper rows)
- GPIO26 → pin 3 (green upper)
- GPIO27 → pin 4 (blue upper)
- GPIO14 → pin 5 (red lower)
- GPIO13 → pin 6 (green lower)
- GPIO32 → pin 7 (blue lower)
- GPIO23 → pin 8 (address A)
- GPIO19 → pin 9 (address B)

U3:

- GPIO18 → pin 2 (address C)
- GPIO21 → pin 3 (address D)
- GPIO16 → pin 4 (CLK)
- GPIO17 → pin 5 (LAT)
- GPIO22 → pin 6 (display OE)
- GPIO33 → pin 7 only for optional E-address support; its corresponding output is U3 pin 13.
- Pins 8 and 9 → GND. Leave output pins 12 and 11 unused.

Add **10kΩ between GPIO22 and 3V3**, keeping display OE high/blank during reset. Initialize OE high before starting the panel driver. The ESP32 development board already includes its own regulator, reset/boot circuitry and USB-UART interface; do not also buy the bare-module power/programming parts from the custom-board BOM for this route.

## Buffer output → proposed J1 HUB75 connector

The mapping below is the schematic's proposed standard wiring, **not proof of the seller's exact panel pinout**. Identify pin 1 and the panel's **IN** connector from the supplied hardware; verify the entire connector before connection.

- U2 pin 18 → J1 pin 1 (R1)
- U2 pin 17 → J1 pin 2 (G1)
- U2 pin 16 → J1 pin 3 (B1)
- U2 pin 15 → J1 pin 5 (R2)
- U2 pin 14 → J1 pin 6 (G2)
- U2 pin 13 → J1 pin 7 (B2)
- U2 pin 12 → J1 pin 9 (A)
- U2 pin 11 → J1 pin 10 (B)
- U3 pin 18 → J1 pin 11 (C)
- U3 pin 17 → J1 pin 12 (D)
- U3 pin 16 → J1 pin 13 (CLK)
- U3 pin 15 → J1 pin 14 (LAT)
- U3 pin 14 → J1 pin 15 (display OE)
- J1 pins 4 and 16 → GND.
- J1 pin 8 → selector centre: either GND or buffered E/U3 pin 13, **only after checking the real panel**. Never short buffered E to ground.

Data chain: controller → panel 1 IN; panel 1 OUT → panel 2 IN; panel 2 OUT → panel 3 IN. Power remains separate to each panel.

## What is not yet verified

Panel scan ratio, driver IC initialization, any unusual row mapping, the batch-specific pin 8 assignment, included lead gauge and the exact devboard power isolation. First test one panel at low brightness after checking those details. The presence of a standard connector or the vendor's general compatibility claim does not establish a working firmware configuration.
