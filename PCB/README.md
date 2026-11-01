# Active schematic: perfboard build, no PCB order

Open `StreamPixel-perfboard.kicad_pro` in KiCad 10, then its schematic. The adjacent `StreamPixel.kicad_sym` and `sym-lib-table` supply the custom logical symbols. Keep those files together. A PDF and a rendered preview are included for review.

This schematic describes the purchased development-board/perfboard route. It includes the ESP32 development board, two SN74HCT245N buffers configured A-to-B, GPIO34 microphone input, decoupling, OE reset pull-up, panel connector, optional pin8 selector and separate fused low-voltage branches.

The development-board and microphone symbols use **printed function labels as logical pin identifiers**. They do not claim a verified physical header arrangement, package footprint or module pin order. Follow the printed labels on the real modules. The HCT buffer and HUB75 connector numbers are physical pin numbers; verify orientation and actual panel assignment before wiring.

The custom HCT symbol fixes pin electrical types to input/output for this permanently selected A-to-B configuration. The real chip is bidirectional; DIR is connected to +5V and /OE to ground on this sheet. Spare A inputs are grounded and spare B outputs are explicitly unused. The optional E channel remains connected to GPIO33; its panel selector must stay open until the actual panel pin8 is known.

`ERC-report.rpt` records the electrical-rule check. A clean ERC result checks the schematic model, not component authenticity, actual module pinout, soldering, wire current rating, panel mapping, mains installation or hardware operation.

No custom PCB is required. This folder contains the editable perfboard wiring schematic; the unused PCB layout is omitted from this final package.

Sources: [TI SN74HCT245 datasheet](https://www.ti.com/lit/ds/symlink/sn74hct245.pdf), [panel seller listing](https://koshielectronics.in/products/p10-rgb-led-module), [MAX9814 explanation](https://learn.adafruit.com/adafruit-agc-electret-microphone-amplifier-max9814/overview). Seller measurements and supplied-batch details still need verification.
