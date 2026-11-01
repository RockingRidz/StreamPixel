# Firmware status

No tested StreamPixel firmware was present in the working files during this review. This folder is deliberately not populated with a fake working demo.

Before testing: identify panel scan ratio and driver IC; confirm the IN connector pinout and pin 8; select a compatible ESP32 HUB75 library; configure the GPIO assignment in docs/PROTOTYPE-WIRING.md; initialize display OE blank; test one panel at low brightness; then test the three-panel chain. Add microphone ADC1 wiring and audio response only after checking the module output range. Record library versions, build instructions and actual results here.
