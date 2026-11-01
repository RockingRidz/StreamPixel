import FreeCAD as App, Part, Import, MeshPart, json, math, re
from pathlib import Path
root=Path(__file__).resolve().parents[1]
records=[]; solids=[]; outlines=[]
for kind,filename in [('printed','StreamPixel-3D-printed-parts.step'),('acrylic','StreamPixel-CNC-acrylic-3mm.step')]:
    doc=App.newDocument('StreamPixel_'+kind)
    Import.insert(str(root/'cad/exports'/filename),doc.Name)
    doc.recompute()
    index=0
    for obj in doc.Objects:
        if not hasattr(obj,'Shape') or obj.Shape.isNull() or obj.TypeId=='App::Part':continue
        for shape in obj.Shape.Solids:
            index+=1;b=shape.BoundBox
            label=obj.Label
            if kind=='printed' and abs(b.ZMin-63)<0.01:
                mx=(b.XMin+b.XMax)/2;my=(b.YMin+b.YMax)/2
                label=(f'Wall spacer X{round(mx)} Y{round(my)} - M3x25' if b.ZLength>10 else f'Hanger pad X{round(mx)} - two M3x16')
            if kind=='acrylic':label='CNC acrylic panel '+str(round((b.XMin+495)/165)+1)
            record={'name':label,'material_set':kind,'valid':shape.isValid(),'size_mm':[b.XLength,b.YLength,b.ZLength],'min_mm':[b.XMin,b.YMin,b.ZMin],'max_mm':[b.XMax,b.YMax,b.ZMax],'volume_mm3':shape.Volume}
            records.append(record);solids.append(shape)
            assert shape.isValid(),label
            assert b.XLength<=220.01 and b.YLength<=220.01,label
            if kind=='printed':
                normalized=shape.copy();normalized.translate(App.Vector(-b.XMin,-b.YMin,-b.ZMin))
                mesh=MeshPart.meshFromShape(Shape=normalized,LinearDeflection=0.05,AngularDeflection=0.15,Relative=False)
                assert mesh.isSolid(),label
                safe=re.sub(r'[^A-Za-z0-9_-]+','-',label).strip('-')
                mesh.write(str(root/'cad/print'/f'{index:02d}-{safe}.stl'))
                record['stl_file']=f'{index:02d}-{safe}.stl'
            else:
                assert abs(b.ZLength-3)<0.001
                face=max([f for f in shape.Faces if f.BoundBox.ZLength<1e-5 and abs(f.BoundBox.ZMin-b.ZMin)<1e-5],key=lambda f:f.Area)
                wire_data=[]
                for wi,wire in enumerate(face.Wires):
                    edges=[]
                    assert wire.isClosed()
                    for edge in wire.Edges:
                        curve=edge.Curve
                        p=edge.valueAt(edge.FirstParameter);q=edge.valueAt(edge.LastParameter)
                        if isinstance(curve,Part.Circle):
                            c=curve.Center
                            data={'type':'CIRCLE' if edge.isClosed() else 'ARC','center':[c.x-b.XMin,c.y-b.YMin],'radius':curve.Radius}
                            if not edge.isClosed():
                                m=edge.valueAt((edge.FirstParameter+edge.LastParameter)/2)
                                ang=lambda v:math.degrees(math.atan2(v.y-c.y,v.x-c.x))%360
                                a,z,mid=ang(p),ang(q),ang(m)
                                if (mid-a)%360>(z-a)%360:a,z=z,a
                                data.update(start_angle=a,end_angle=z)
                            edges.append(data)
                        elif isinstance(curve,Part.Line):
                            edges.append({'type':'LINE','start':[p.x-b.XMin,p.y-b.YMin],'end':[q.x-b.XMin,q.y-b.YMin]})
                        else:
                            pts=edge.discretize(Deflection=0.01)
                            for p,q in zip(pts,pts[1:]):edges.append({'type':'LINE','start':[p.x-b.XMin,p.y-b.YMin],'end':[q.x-b.XMin,q.y-b.YMin]})
                    wire_data.append(edges)
                outlines.append({'name':label,'panel_index':round((b.XMin+495)/165)+1,'size_mm':[b.XLength,b.YLength],'thickness_mm':3,'area_mm2':face.Area,'wires':wire_data})
    assert index==(35 if kind=='printed' else 6),(kind,index)
    doc.saveAs(str(root/'cad/source'/('StreamPixel-'+kind+'-Ridith-Shetty.FCStd')))
overlaps=[]
for i,a in enumerate(solids):
    for j in range(i+1,len(solids)):
        b=solids[j]
        if a.BoundBox.intersect(b.BoundBox):
            vol=a.common(b).Volume
            if vol>0.01:overlaps.append({'a':records[i]['name'],'b':records[j]['name'],'volume_mm3':vol})
report={'source':'Final separated Onshape STEP exports, 1 November 2026','solid_count':len(records),'printed_count':35,'acrylic_count':6,'all_shapes_valid':True,'all_XY_within_assumed_220mm_bed':True,'printed_volume_cm3':sum(r['volume_mm3'] for r in records if r['material_set']=='printed')/1000,'acrylic_area_m2':sum(p['area_mm2'] for p in outlines)/1e6,'solid_overlaps':overlaps,'parts':records,'limitations':['Panel thickness 15mm unverified','Supply envelope 220x60x25mm unverified','15.6mm gland hole and 8mm grip require actual gland specification','No physical fit, thermal or load test','DXF geometry is nominal; CAM tool compensation belongs to manufacturer']}
(root/'cad/dimension-audit.json').write_text(json.dumps(report,indent=2))
(root/'cad/acrylic-outlines.json').write_text(json.dumps(outlines,indent=2))
assert not overlaps,overlaps
print('MANUFACTURING AUDIT PASSED: 35 printed, 6 acrylic, valid solids, zero overlaps.')
