# StreamPixel carts — updated 30 October 2026

No purchase or payment was made. Use the prepared in-app browser carts; other browser sessions can contain duplicate panel baskets.

## Verified carts

- Koshi: 3 × P10 RGB 2727 panels ₹1,950; 5V 20A slim SMPS ₹549; existing HD-WF1 ₹412. Visible merchandise ₹2,911. Reserve ₹240 delivery/difference from the earlier store quote: **₹3,151**. Final checkout must confirm this charge. Each panel advertises an included power lead and FRC ribbon.
- Quartz: **₹1,236**, 33 item units across 20 product lines. Prepaid shipping is free above ₹500 under the store's current shipping policy; confirm at checkout.
- Industrybuying: **2 × SN74HCT245N DIP-20**, ₹280 including GST, **₹298 shipping**, **₹578 total payable**. Cart quantity 2 and ₹280 line total verified. Estimated delivery shown: 20 November 2026. Login required to order. Non-returnable.

Koshi cart: https://koshielectronics.in/cart
Quartz cart: https://quartzcomponents.com/cart
Buffer cart: https://www.industrybuying.com/order/cart

## Quartz contents

- Added: MAX9814 amplified electret microphone module, one, ₹221. Power from 3.3V; analog output is suitable for a proposed ADC1 input after wiring review. This is an AGC microphone for reactive effects, not a calibrated sound-level meter.

- ESP32 30-pin CP2102 development board ₹375; 90 × 150mm double-sided perfboard ₹108; micro-USB cable ₹32; 100nF 1206 capacitors pack10 ₹16.
- Male 40-pin header ₹7; female 40-pin header ₹10; 10kΩ 1/4W resistors pack10 ₹7; 470µF 25V radial capacitor ₹8.
- 23AWG single-core red+black signal wire, 1m each ₹25; 18AWG silicone red 1m ₹42 and black 1m ₹42.
- 3-pin 10A 1.3m AC lead ₹135. Seller describes grounding; earth continuity and conductor suitability must be checked before installation. Do not substitute its cheaper two-core lead.
- Four panel-mount 5×20mm fuse holders ₹72; three 5A fuses ₹12; one 1A fuse ₹5. These are DC branch protection parts, not a complete mains protection design.
- 5mm black heat-shrink 1m ₹19; PG9 gland ₹5, advertised cable range 4–8mm.
- Twenty M3×10 metal bolts ₹40; twenty M3 metal nuts ₹55. Intended for rail splices; panel mounting screw size remains unverified.

A 40-pin male strip can provide two rows of eight for an unshrouded 16-pin ribbon interface. There is no keyed/shrouded IDC header in this cart: mark pin 1 and verify the ribbon orientation. Female headers can socket the 30-pin board. DIP sockets are optional and were not added.

## Budget

Koshi reserved ₹3,151 + Quartz ₹1,236 + Industrybuying ₹578 = **₹4,965** for the prepared parts and delivery provision.

With the earlier **₹500 self-print allowance**, planning total is **₹5,465**, leaving **₹35** against ₹5,500. Printing is an allowance, not a commercial quote. This is NOT a fully priced, complete enclosure/build.

Still unquoted: covered/earthed SMPS installation, suitable rear covers and load-bearing wall fixings, exact panel screws, any extra insulation/terminals and tools or paid assembly. Shared soldering equipment and a multimeter are assumed. The remaining ₹35 cannot be assumed to cover these. Do not pay until the final enclosure and delivery costs fit the limit. No battery is included; this is a mains-powered 5V build.

The ₹412 HD-WF1 is unnecessary for the custom ESP32 music/app route. Automatic approval review rejected removing this existing cart item earlier, so it remains and is included in the estimate. Removing it would require explicit authorization, and would reduce the merchandise estimate by ₹412 before any delivery recalculation. The unrelated Robu basket remains untouched and excluded.

## Assembly/CAD dependencies

Do not run all panel current through the perfboard or a shared 18AWG 20A trunk. Use separately fused panel branches. Verify actual lead/connector ratings and operating current before accepting the proposed 5A fuses. The 1A controller branch is separate. Keep mains terminals covered, provide earth and strain relief, and have competent installation verified.

Each nominal panel is 320 × 160mm; array 960 × 160mm. Existing frame concept is 990 × 190 × 60mm, but supply depth, panel connector clearance and mounting holes still need actual measurements. The earlier concept locates the SMPS separately. Putting it inside requires revising the rear compartment and ventilation; 60mm depth is not guaranteed. The custom KiCad PCB still has unrouted connections and must not be ordered.
