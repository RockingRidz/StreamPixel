# StreamPixel firmware

Target: a classic ESP32-WROOM development board with CP2102 USB interface. The GPIO assignment follows the active perfboard schematic. ESP32-S2/S3/C3, ESP32-WROVER and boards using GPIO16/17 for PSRAM need a separate pin/platform review.

## Included functions

- Local Wi-Fi access point and phone/browser control page.
- Scrolling English text, colour animation, red/green/blue test and relative microphone level display.
- MAX9814 sampling on GPIO34 (ADC1), DC-offset removal, smoothing and clipping indication. This is an activity meter, not an FFT spectrum or a calibrated decibel meter.
- Output starts off, with a low configurable brightness ceiling. Settings reset at restart. A fresh random Wi-Fi password is generated on first boot, stored on the board and printed to Serial at every boot. No personal network credentials are stored in the source.

Spotify authentication, track artwork and an Internet-connected companion service are not implemented. There is no native mobile app; the controller serves a local browser page.

## Build

Use Arduino IDE or Arduino CLI with the Espressif **esp32:esp32** board package. Open `StreamPixel/StreamPixel.ino`, select **ESP32 Dev Module**, and install **ESP32 HUB75 LED MATRIX PANEL DMA Display 3.0.14** plus its library dependencies. Record versions when changing them.

CLI build from the repository root:

```sh
arduino-cli compile --fqbn esp32:esp32:esp32 FIRMWARE/StreamPixel
```

See `build-log.txt` for the actual compile result from this review. Compilation does not prove correct wiring or panel compatibility. No physical hardware was available for this review and nothing was flashed automatically.

## First boot

1. Verify the actual development-board pin labels. Disconnect external 5V and the panel harness while programming unless the board documents safe USB/external-supply isolation.
2. Leave `PANEL_PROFILE_VERIFIED = false` in `config.h` for the initial controller-only test. Keep the display harness disconnected.
3. Upload to the correct serial port, open Serial Monitor at **115200**, and reset the board. Note the printed `StreamPixel-…` Wi-Fi name and unique password. Do not publish that password in screenshots or logs.
4. Join that Wi-Fi network from a phone and open the printed address, normally `http://192.168.4.1`. It has no Internet connection. Confirm that status, microphone activity and control validation respond. The page will reject an attempt to turn the panel on while its profile is unverified.
5. Identify the supplied panel's entire IN connector, pin8, driver IC, scan ratio and row mapping. The initial library geometry is a normal two-row HUB75 configuration, **not an implementation of an unidentified 1/4-scan P10 remap**. If your panel needs special mapping or initialization, implement and validate it first. Do not just flip the verification flag to bypass this requirement.
6. Once the profile and protected wiring have been checked, configure the correct driver/mapping and set the flag true. Keep `PANEL_CHAIN = 1` and the brightness ceiling low. Output still starts off and must be enabled from the page. Test one panel before setting the chain to three.
7. Verify each colour, every row/column, reset blanking, measured current, fuse/wire suitability and temperature before running the enclosed assembly. A software brightness cap is not an electrical current limiter.

GPIO33 is held low for a defined unused E-input level. Optional E operation requires changing `PIN_E`, confirming the connector and selecting the correct J4 position. Never ground a driven E output.

## Sources

- [Display library and panel compatibility guidance](https://github.com/mrcodetastic/ESP32-HUB75-MatrixPanel-DMA)
- [Espressif ADC API](https://docs.espressif.com/projects/arduino-esp32/en/latest/api/adc.html)
- [Espressif Wi-Fi API](https://docs.espressif.com/projects/arduino-esp32/en/latest/api/wifi.html)
- [MAX9814 operating explanation](https://learn.adafruit.com/adafruit-agc-electret-microphone-amplifier-max9814/overview) — verify the purchased module as well.
