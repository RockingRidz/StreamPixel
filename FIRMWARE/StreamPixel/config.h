#pragma once

// Change only after identifying the actual scan ratio, driver and row mapping.
// A 32x16 panel is NOT automatically compatible with a normal half-height scan.
constexpr bool PANEL_PROFILE_VERIFIED = false;
constexpr uint16_t PANEL_WIDTH = 32;
constexpr uint16_t PANEL_HEIGHT = 16;
constexpr uint8_t PANEL_CHAIN = 1;  // first test one; use 3 after that test passes
constexpr uint8_t BRIGHTNESS_CAP = 16; // 0..255; not a current limiter
constexpr int MIC_PIN = 34; // ADC1; remains available while Wi-Fi is running
constexpr int DISPLAY_OE_PIN = 22;

// Same GPIO assignment as electronics/schematic and docs/PROTOTYPE-WIRING.md.
constexpr int PIN_R1=25, PIN_G1=26, PIN_B1=27;
constexpr int PIN_R2=14, PIN_G2=13, PIN_B2=32;
constexpr int PIN_A=23, PIN_B=19, PIN_C=18, PIN_D=21;
constexpr int PIN_E=-1; // use GPIO33 only after verifying J4 and panel pin8
constexpr int PIN_CLK=16, PIN_LAT=17;
