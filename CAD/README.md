# CAD handoff — StreamPixel / Ridith Shetty

Only the power-supply holder and controller tray are 3D printed. Everything in `CNC/` is assigned to the CNC shop; screws and nuts are purchased hardware.

## Source files

`SOURCE/StreamPixel-Ridith-Shetty.FCStd` is a genuine editable FreeCAD solid model with separate CNC and printed-component groups. `SOURCE/CNC-components.step` and `SOURCE/PRINT-holders.step` preserve the assembled positions of those sets. `SOURCE/StreamPixel-features.fs` contains the native Onshape feature code. Native editable feature history is available through the two Onshape links in the root README. These files are not Fusion `.f3d` files.

Individual STEP files use a local origin at each part's minimum X/Y/Z. Use the assembled source files when checking relative hole positions. Millimetres throughout.

## CNC job

Make one of each of the fourteen individually named STEP components:

- Whole frame: 990 × 190 × 60 mm, including bosses, nut pockets, microphone/cable openings and 0.6 mm recessed front lettering.
- Whole rear plate: 990 × 190 × 3 mm, including ventilation and all mounting openings.
- Six 3 mm panel retainers.
- Four 16 × 16 × 15 mm wall spacers, with shaft holes and rear bolt-head recesses.
- Two 24 × 34 × 6 mm hanger pads, with keyholes and fixing holes.

The deep frame, spacers and hanger pads are 3D milling geometry. Their STEP files control the complete shape. The shop must review material, stock, cutter access, workholding, internal radii, setups, fixturing and achievable tolerances. A router that cuts sheet profiles is not automatically able to make the deep frame. There is no machine-specific CAM or G-code in this package.

The seven flat DXFs are the whole rear plate and six retainers. They are 1:1 millimetre closed cutting profiles. The additional frame-overall DXF is clearly marked **REFERENCE ONLY** and omits 3D machining details; do not cut the frame from that drawing alone. Apply tool/kerf compensation once in the shop's CAM. Get the material and machining quote before ordering; this job is not confirmed within INR 6,445.

## Printed job

Print one of each of the three STLs in `3D_PRINTED/`: controller tray, supply-holder left and supply-holder right. The halves attach independently to the backplate. Check orientation, support needs, material ratings, hole/nut tolerances and the printer's usable bed. Nominal XY extents fit an assumed 220 × 220 mm bed before brim/supports.

## Dimensions that still need actual parts

Panel thickness: **15 mm placeholder**. Supply case: **220 × 60 × 25 mm placeholder**. The front aperture is 958 × 158 mm and panel pocket 962 × 162 mm for a nominal 960 × 160 mm array. Check actual panel tolerances, LED edge visibility, connectors and supply terminals/bend space before manufacture.

The provisional gland hole is 15.6 mm through an 8 mm local boss; verify the actual gland's mounting diameter and thread grip. Retainers have a modeled 0.5 mm side-wall clearance. Hardware plan is 22 M3x10, four M3x25, four M3x16 and 30 nuts, plus suitable washers and wall anchors. Verify bolt reach, nut insertion and tool access physically.

The backplate/spacers extend total depth to 78 mm. Wall mounting must suit the actual assembly weight and wall. No physical fit, load or thermal test has been performed. The generic supply holder does not provide a mains-rated enclosure or terminal protection. Computer geometry checks do not replace these verifications.
