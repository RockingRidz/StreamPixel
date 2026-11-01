from pathlib import Path
import sys,json,math,csv
base=Path(__file__).resolve().parents[1]
import ezdxf
from ezdxf import bbox
root=base
out=root/'cad/cnc';out.mkdir(exist_ok=True)
panels=sorted(json.loads((root/'cad/acrylic-outlines.json').read_text()),key=lambda p:p['panel_index'])
def new():
    doc=ezdxf.new('R2010',setup=True);doc.units=4;doc.header['$MEASUREMENT']=1
    doc.layers.new('CUT',dxfattribs={'color':1})
    return doc
def add(doc,p,ox=0,oy=0):
    m=doc.modelspace()
    for wire in p['wires']:
        for e in wire:
            offset=lambda a:(a[0]+ox,a[1]+oy)
            attr={'layer':'CUT'}
            if e['type']=='LINE':m.add_line(offset(e['start']),offset(e['end']),dxfattribs=attr)
            elif e['type']=='CIRCLE':m.add_circle(offset(e['center']),e['radius'],dxfattribs=attr)
            else:m.add_arc(offset(e['center']),e['radius'],e['start_angle'],e['end_angle'],dxfattribs=attr)
checks=[]
for p in panels:
    doc=new();add(doc,p)
    path=out/f"StreamPixel-acrylic-panel-{p['panel_index']}-165x190x3mm.dxf"
    doc.saveas(path)
    reload=ezdxf.readfile(path);audit=reload.audit();assert not audit.has_errors
    box=bbox.extents(reload.modelspace())
    assert abs(box.size.x-165)<.001 and abs(box.size.y-190)<.001,(path,box.size)
    checks.append({'file':path.name,'units':'mm','width_mm':box.size.x,'height_mm':box.size.y,'wire_count':len(p['wires']),'entity_count':len(reload.modelspace()),'dxf_audit_errors':0})
layout=new()
for i,p in enumerate(panels):add(layout,p,(i%3)*175,(i//3)*200)
layout.saveas(out/'StreamPixel-acrylic-six-panels-layout-515x390mm.dxf')
assert not ezdxf.readfile(out/'StreamPixel-acrylic-six-panels-layout-515x390mm.dxf').audit().has_errors
(out/'dxf-audit.json').write_text(json.dumps(checks,indent=2))
(out/'CUTTING-NOTES.md').write_text('''# Acrylic cutting files

Six separate nominal 165 x 190 x 3 mm rear panels, one of each. DXFs use millimetres, 1:1 scale, with geometry on CUT. Each file starts at its own lower-left corner. Circles and arcs are retained; no bitmap tracing is used. Hole and outline geometry comes from the same STEP solids as the Onshape CNC studio, preserving the modeled alignment with printed components.

The combined layout is three columns by two rows, 10 mm between rectangular outlines, requiring 515 x 390 mm before stock-edge allowance and clamping. It is a convenience layout, not an optimized nest or machine program. Use the individual files if the cutting bed is smaller.

Material assumption: nominal 3 mm acrylic. Check actual thickness and temperature suitability. Ask the manufacturer to assess minimum features, vent bridges, small holes, edge finish, tool access, kerf/tool compensation and brittleness at mounting holes. Do not apply kerf compensation twice. Drill/finish holes if their process cannot produce the nominal diameter reliably. Do not tighten bolts directly against unsupported acrylic: use suitable washers and controlled torque, and verify fit on a sample.

Wall support, assembled weight and supply protection still require physical verification. These files are fit-review geometry, not a structural certification or approval to house exposed mains terminals.
''',encoding='utf8')
print('DXF checks passed:',len(checks),'individual files and one combined layout.')
