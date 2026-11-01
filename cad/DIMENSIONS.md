# Mechanical dimensions — current Onshape fit-review design

All values below are millimetres. X is the long horizontal axis, Y the short vertical axis, and Z runs from the front face toward the wall. Origin is at the front-face centre. The native Onshape history is the parametric source; the STEP/FreeCAD review copy preserves editable solid geometry without that feature history.

## Overall shell

- Frame: X −495 to +495, Y −95 to +95, Z 0 to 60: **990 × 190 × 60**.
- Outer wall thickness: **3**. Clear rear cavity: **984 × 184** behind the front face.
- Front aperture: **958 × 158**. Nominal three-panel array: **960 × 160**. Check LED edge visibility before printing.
- Panel locating ridge: **966 × 166 outer**, **962 × 162 inner**, from Z3 to Z7: 2 thick, 4 high.
- Front outer-edge chamfer: **1**.
- Rear-cover skin: Z60 to Z63, **3 thick**. Rear spacers extend to Z78: **15 wall gap**, **78 total installed depth**.
- Four rear feet: **16 × 16**, centres X±450, Y±87. Rear driver recess is 7 diameter × 4 deep, above a 3.4 diameter shaft hole. Four M3x25 bolts pass through the spacers and acrylic into the frame bosses.
- Rear ventilation openings retain cross bridges, 8 wide, every 80 along their main run. Ventilation performance needs an actual thermal test.
- Two keyhole hangers: X±350; 8 diameter entry and 4 wide stem. Each printed hanger pad also has two 3.4 diameter fastening holes at X = hanger centre ±7, Y71, for M3x16 bolts and nuts through the acrylic. These modeled holes do not establish anchor suitability or load capacity.

## Retention and cover fastening

- Six retainers: X−320, −40, +320, on both Y sides. Each clip is **16 × 16.5 × 3** after the wall-clearance correction.
- Clip reaches |Y|75 to 91.5, leaving **0.5 clearance** to the wall inner face at |Y|92.
- Clip underside: Z = 3 + actual panel thickness + 0.3. With the placeholder 15 panel thickness: underside **18.3**, top **21.3**.
- Retainer screw centres: Y±88; **3.4 diameter** holes cut through matching clip/boss geometry together.
- Cover screw centres: X−450, −247.5, −82.5, +82.5, +247.5, +450; Y±87: **12 screws**.
- M3 nut pockets are nominal **6 wide × 2.5 high**. Actual nut size, print shrinkage and insertion direction need a test coupon.
- Hardware plan: 8 ordinary cover + 6 retainers + 4 tray + 4 supply + 20 splice = **42 M3x10**, plus **4 M3x25** spacer/cover bolts and **4 M3x16** hanger-pad bolts: **50 bolts and 50 M3 nuts**. Wall anchors are separate. Check bolt reach and head access physically.

## Internal mounts

- Perfboard tray: **160 × 100**, base Z42–45. Intended board: **150 × 90**, with ties rather than a guessed board-hole pattern. Component height and ESP32 antenna clearance remain to be verified.
- Tray-post screw centres: X−323.5 and −171.5, Y±46.
- Supply cradle: **240 × 80**, base X95–335, Y±40, Z21–24. Printed as two halves at X215; both halves screw to the cover.
- Supply reference: **220 × 60 × 25**, X105–325, Y±30, Z24–49. This is an **unverified envelope**, not measured product geometry. Terminal/wire access is not represented by the box.
- Supply screw centres: X100 and 330, Y±35.
- Microphone: generic tie mount at X−400 and a **3 diameter acoustic port** in the lower side wall. Board dimensions, acoustic response and ADC wiring remain to be checked.
- Cable exit: X400, Z40 through the lower wall; **15.6 diameter**, reinforced locally to **8 wall thickness**. Confirm the actual gland mounting hole and thread grip before printing.
- Front lettering: StreamPixel at 7 nominal text height; Ridith Shetty at 5; both recessed **0.6** into the front border. Names stay inside individual rail sections.

## Print preparation

Five transverse cuts at X−330, −165, 0, +165 and +330 give 165-wide sections. Because the middle of the frame is open, the result is 10 frame pieces, not six closed rings.

The final inventory is **10 frame pieces + 6 retainers + 1 controller tray + 2 supply-cradle halves + 10 rail splice plates + 4 wall spacers + 2 hanger pads = 35 printed pieces**, plus **6 acrylic rear panels**. Four reference boxes are excluded from both manufacturing studios. Rear cover is CNC-cut from nominal 3 mm acrylic; its pads and spacers are printed separately.

The independent final STEP audit found 41 valid solids, zero solid overlaps, approximately **770.08 cm³ of printed solid geometry**, and **0.1519 m² net acrylic area** after cutouts. Six rectangular blanks total 0.1881 m² before stock waste. Slicer material use and purchased sheet area differ from those geometric values.

Every model piece has XY size within an assumed **220 × 220** bed. This does not include slicer brim, skirt, supports or printer keepouts. Long end/cover sections are 165 × 190 in XY and need the actual printer checked. Splice plates are 32 × 4 × 20, with two 3.4 diameter holes on 16 pitch.

## Required physical checks

Real panel tolerance and thickness, supply outline/terminals, mounting threads, gland fit, bolt-head/nut fit, connector bend radii, thermal clearance, material properties, print orientation and wall load remain unresolved until real parts and printing are available. See `dimension-audit.json` for export geometry checks, when present; those checks cannot replace physical verification.
