from pathlib import Path
import re,json,csv,math
import ezdxf
root=Path(__file__).resolve().parents[1]
text=(root/'README.md').read_text(encoding='utf8')
images=re.findall(r'!\[[^\]]*\]\(([^)]+)\)',text)
assert len(images)==4,images
for name in images:assert (root/name).is_file(),name
report=json.loads((root/'cad/dimension-audit.json').read_text())
assert report['printed_count']==35 and report['acrylic_count']==6
assert not report['solid_overlaps']
assert len(list((root/'cad/print').glob('*.stl')))==35
expected={p['panel_index']:p for p in json.loads((root/'cad/acrylic-outlines.json').read_text())}
for index,p in expected.items():
    path=root/'cad/cnc'/f'StreamPixel-acrylic-panel-{index}-165x190x3mm.dxf'
    doc=ezdxf.readfile(path);assert doc.units==4 and not doc.audit().has_errors
    edges=[];areas=[];nodes={}
    key=lambda p:(round(p[0],5),round(p[1],5))
    for entity in doc.modelspace():
        t=entity.dxftype()
        if t=='CIRCLE':areas.append(math.pi*entity.dxf.radius**2);continue
        if t=='LINE':
            a,b=entity.dxf.start,entity.dxf.end
            area=(a.x*b.y-b.x*a.y)/2
        elif t=='ARC':
            c=entity.dxf.center;r=entity.dxf.radius
            aa=math.radians(entity.dxf.start_angle);delta=math.radians((entity.dxf.end_angle-entity.dxf.start_angle)%360);bb=aa+delta
            a=(c.x+r*math.cos(aa),c.y+r*math.sin(aa));b=(c.x+r*math.cos(bb),c.y+r*math.sin(bb))
            area=(r*c.x*(math.sin(bb)-math.sin(aa))+r*c.y*(math.cos(aa)-math.cos(bb))+r*r*delta)/2
        else:raise AssertionError(t)
        ka,kb=key(a),key(b);ei=len(edges);edges.append((ka,kb,area))
        nodes.setdefault(ka,[]).append(ei);nodes.setdefault(kb,[]).append(ei)
    assert all(len(v)==2 for v in nodes.values()),f'Open or ambiguous cutting path: {path}'
    seen=set()
    for ei,e in enumerate(edges):
        if ei in seen:continue
        start=e[0];current=start;total=0;walk=ei
        while walk not in seen:
            seen.add(walk);a,b,area=edges[walk]
            if a==current:total+=area;current=b
            else:assert b==current;total-=area;current=a
            walk=next((n for n in nodes[current] if n not in seen),walk)
        assert current==start
        areas.append(abs(total))
    area=2*max(areas)-sum(areas)
    assert abs(area-p['area_mm2'])<0.01,(path,area,p['area_mm2'])
    assert len(areas)==len(p['wires'])
with (root/'bom.csv').open(newline='',encoding='utf8') as f:rows=list(csv.DictReader(f))
assert all(r['Vendor'] and r['Link'].startswith('https://') and r['Unit_price_INR'] for r in rows)
assert abs(sum(float(r['Line_total_INR']) for r in rows)-4965)<.01
print('PACKAGE CHECK PASSED: four linked images; 35 STLs; six closed DXF profiles matching STEP areas; 41 valid solids with zero overlaps; priced BOM INR4965 plus separately unquoted requirements.')
