# Rebuilding manufacturing exports

1. Edit the native Onshape features and verify actual component dimensions. Keep the CNC-only checkbox off in the printed studio and on in the acrylic studio.
2. Export STEP from each complete studio with hidden instances included. Save as `cad/exports/StreamPixel-3D-printed-parts.step` and `cad/exports/StreamPixel-CNC-acrylic-3mm.step`. Keep the default Z-axis orientation and millimetre geometry.
3. Run `export_streampixel_manufacturing.py` with FreeCAD's command-line Python executable (`FreeCADCmd` / `freecadcmd.exe`). It imports the two STEP files, checks 35 + 6 valid solids and all pairwise solid overlaps, exports 35 closed STL meshes, creates editable FreeCAD solid copies and writes the dimension audit and acrylic contours. Existing generated exports will be overwritten.
4. With ordinary Python, install `ezdxf==1.4.5`, then run `make_streampixel_dxf.py`. It exports one DXF per acrylic panel plus the combined layout, reopens the files, checks DXF structure and verifies individual 165 × 190 mm extents. Millimetres are declared in each DXF.
5. Inspect the files visually and in the target slicer/CAM software. A successful geometry check does not verify physical fit, material strength, thermal performance, safe mains installation or machining setup. Update dimensions, screenshots and BOM when changing the design.

Run `python tools/check_package.py` with ezdxf installed to check the four README image links, STL count, DXF closed paths and their enclosed areas against the STEP-derived profiles, the geometry audit and the historical priced BOM subtotal.

FreeCAD 1.0.2 and ezdxf 1.4.5 were used for this review. Native feature history remains in Onshape; imported STEP/FreeCAD files contain editable solid geometry rather than the original parametric history.
