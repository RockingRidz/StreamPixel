# StreamPixel completion checklist — 1 November 2026

## CAD and actual component fit

- [x] Main frame 990 × 190 × 60 mm; 3 mm walls; 958 × 158 mm aperture; 962 × 162 mm locating pocket.
- [x] Six removable retainers, aligned model holes and nut pockets; controller tray and supply cradle.
- [x] Vent bridges, rear wall spacers, wall-hanger geometry, rail splice plates and provisional cable exit.
- [x] Recessed StreamPixel and Ridith Shetty lettering in the front border.
- [x] Separate print and CNC studios: 35 printed bodies and six flat acrylic panels; each XY footprint within an assumed 220 × 220 mm bed.
- [x] Nominal reference envelopes checked for solid overlap with printed parts: zero, for the placeholder dimensions only.
- [ ] Obtain actual panel outline, thickness, connector locations, LED edge clearance and mounting specifications. Adjust the 15 mm placeholder and retainers. Confirm three panels actually fit with manufacturing tolerances.
- [ ] Obtain actual supply case and terminal/connector dimensions. Adjust the 220 × 60 × 25 mm reference and cradle. Keep safe wiring and service clearance.
- [ ] Confirm the PG9 gland requires the modeled 15.6 mm hole and can clamp the 8 mm local wall thickness. Change the boss/hole or choose a suitable gland if needed.
- [ ] Verify the chosen printer's usable XY area and Z height, material and temperature suitability. Slice each piece; inspect supports and orientation.
- [ ] Print one splice/boss/retainer fit test. Check 3.4 mm holes, M3x10 reach, 6 mm nut pockets, tools and bolt-head clearance. Compensate printer tolerances as needed.
- [ ] Confirm both supply-cradle halves are secured to the cover; the split halves rely on the cover fasteners for support.
- [ ] Confirm the six rear-cover sections are each bolted to their matching frame section and that seam alignment is acceptable.
- [ ] Establish actual finished weight, wall material, suitable anchors and load-bearing support. Test without electronics before mounting the complete device.

## Electronics and firmware

- [ ] Use the budget development-board/perfboard architecture; keep the unfinished custom PCB separate.
- [ ] Check actual panel IN pinout, pin 1, pin 8, scan ratio and driver IC. Do not connect an optional E signal to a grounded pin.
- [ ] Check controller input-power arrangement and USB isolation before connecting external 5V.
- [ ] Verify branch current, wire/connector and fuse ratings, connections, polarity and common ground.
- [ ] Add verified MAX9814 wiring: 3.3V and ground, suitable ADC1 input, and confirmed output range. Its AGC output is not a calibrated sound-level reading.
- [ ] Provide suitable supply terminal guarding, earth, strain relief and accessible isolation. The generic printed cradle is not a mains-rated electrical enclosure.
- [ ] Implement firmware using a panel-compatible library; record exact dependencies and build steps. Begin with one panel at low brightness, then chain three.
- [ ] Test solid colors, row addressing, text, update stability, brightness/current limits, microphone response and recovery after reset.
- [ ] Run and document an enclosed temperature test with the final brightness limit, supply and actual wall gap.
- [ ] For any future custom PCB: route all remaining nets, verify schematic parity/footprints/connector orientation, review ERC warnings, and achieve zero unconnected items before manufacturing exports.

## Purchasing and budget

- [ ] Recheck all vendor prices, stock, delivery and cart totals. Historical parts/delivery provision: INR 4,965.
- [ ] Price 22 additional M3x10 bolts, four M3x25 bolts, four M3x16 bolts and 30 additional nuts. CAD plan is 50 bolts/nuts; historical cart has 20 M3x10 and 20 nuts. Add suitable washers for acrylic.
- [ ] Price six 165 × 190 × 3 mm acrylic panels with cutting, ties, wall anchors, protected supply installation, any extra wires/terminals and printing.
- [ ] Keep final payable build total within INR 5,500. The old INR 500 printing allowance is not a quote; current missing items cannot be assumed free.

## Repository and submission

- [x] Organize editable CAD, exports, KiCad sources, evidence and price records; preserve the old concept files in an archive.
- [x] Final STEP exports opened independently: 41 valid solids, zero overlaps; 35 closed STL meshes generated. Six individual DXFs and a combined layout reopened with no DXF audit errors; individual outlines verified 165 × 190 mm.
- [ ] Add actual tested firmware. An empty status folder does not meet a firmware requirement.
- [ ] Review the README and journal against the program's authorship rules. Do not claim AI-assisted text or CAD as unaided work.
- [ ] Add at least two relevant project screenshots/photos, a meaningful description and accurate personal build/test notes.
- [ ] Refresh cart screenshots with totals visible for every vendor and reconcile every BOM line.
- [ ] Add real printed/assembled photos and an honest demo video showing what works and what fails. Record the requested closing reel.
- [ ] Confirm logged-session, project-description, cover-image and tier-hour requirements in the submission form. Never invent hours or identity status.
- [ ] Publish reviewed changes to the public repository; verify all CAD, images and source links work when signed out.

Nothing in this checklist certifies physical fit, load capacity, electrical safety, program acceptance or a working build without the corresponding evidence.
