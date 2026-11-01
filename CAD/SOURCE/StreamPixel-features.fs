FeatureScript 3095;
import(path : "onshape/std/common.fs", version : "3095.0");

function block(context is Context, id is Id, a is array, b is array) returns Query
{
 fCuboid(context,id,{"corner1":vector(a[0],a[1],a[2])*millimeter,"corner2":vector(b[0],b[1],b[2])*millimeter});
 return qCreatedBy(id,EntityType.BODY);
}
function cylinder(context is Context,id is Id,a is array,b is array,r is number) returns Query
{
 fCylinder(context,id,{"bottomCenter":vector(a[0],a[1],a[2])*millimeter,"topCenter":vector(b[0],b[1],b[2])*millimeter,"radius":r*millimeter});
 return qCreatedBy(id,EntityType.BODY);
}
function join(context is Context,id is Id,target is Query,tool is Query)
{
 opBoolean(context,id,{"tools":qUnion([target,tool]),"operationType":BooleanOperationType.UNION});
}
function subtract(context is Context,id is Id,target is Query,tool is Query)
{
 opBoolean(context,id,{"tools":tool,"targets":target,"operationType":BooleanOperationType.SUBTRACTION});
}
annotation {"Feature Type Name":"StreamPixel enclosure finish"}
export const enclosureFinish = defineFeature(function(context is Context,id is Id,definition is map)
 precondition
 {
  annotation {"Name":"Frame","Filter":EntityType.BODY && BodyType.SOLID,"MaxNumberOfPicks":1}
  definition.frame is Query;
  annotation {"Name":"Back cover","Filter":EntityType.BODY && BodyType.SOLID,"MaxNumberOfPicks":1}
  definition.cover is Query;
  annotation {"Name":"Wall air gap","Default":"15 mm"}
  isLength(definition.airGap,LENGTH_BOUNDS);
  annotation {"Name":"Panel body thickness - VERIFY BEFORE PRINT","Default":"15 mm"}
  isLength(definition.panelThickness,LENGTH_BOUNDS);
  annotation {"Name":"Supply case length - UNVERIFIED","Default":"220 mm"}
  isLength(definition.supplyLength,LENGTH_BOUNDS);
  annotation {"Name":"Supply case width - UNVERIFIED","Default":"60 mm"}
  isLength(definition.supplyWidth,LENGTH_BOUNDS);
  annotation {"Name":"Supply case height - UNVERIFIED","Default":"25 mm"}
  isLength(definition.supplyHeight,LENGTH_BOUNDS);
 }
 {
  var gap=definition.airGap/millimeter;
  var panel=definition.panelThickness/millimeter;
  if(gap<10 || gap>40) throw regenError("Use an air gap between 10 and 40 mm.",["airGap"]);
  if(panel<8 || panel>25) throw regenError("Measure panel thickness; supported adjustment is 8-25 mm.",["panelThickness"]);
  // Rebuild the base frame cleanly while keeping all earlier features reversible in history.
  opDeleteBodies(context,id+"replaceBase",{"entities":definition.frame});
  definition.frame=block(context,id+"frameBlank",[-495,-95,0],[495,95,60]);
  var window=block(context,id+"frontWindow",[-479,-79,-1],[479,79,61]);
  subtract(context,id+"frontWindowCut",definition.frame,window);
  var cavity=block(context,id+"cavity",[-492,-92,3],[492,92,61]);
  subtract(context,id+"cavityCut",definition.frame,cavity);
  var frontEdges=qOwnedByBody(definition.frame,EntityType.EDGE);
  opChamfer(context,id+"frontBevel",{"entities":qUnion([
    qClosestTo(frontEdges,vector(0,-95,0)*millimeter),
    qClosestTo(frontEdges,vector(0,95,0)*millimeter),
    qClosestTo(frontEdges,vector(-495,0,0)*millimeter),
    qClosestTo(frontEdges,vector(495,0,0)*millimeter)]),"chamferType":ChamferType.EQUAL_OFFSETS,"width":1*millimeter});
  setProperty(context,{"entities":definition.frame,"propertyType":PropertyType.NAME,"value":"Frame"});
  var ribA=block(context,id+"ribA",[-483,-83,3],[483,-81,7]);
  join(context,id+"ribJoinA",definition.frame,ribA);
  var ribB=block(context,id+"ribB",[-483,81,3],[483,83,7]);
  join(context,id+"ribJoinB",definition.frame,ribB);
  var ribC=block(context,id+"ribC",[-483,-81,3],[-481,81,7]);
  join(context,id+"ribJoinC",definition.frame,ribC);
  var ribD=block(context,id+"ribD",[481,-81,3],[483,81,7]);
  join(context,id+"ribJoinD",definition.frame,ribD);
  // Crossbars bridge the user's long vent slots while preserving open sections.
  var n=0;
  for(var x in [-400,-320,-240,-160,-80,0,80,160,240,320,400])
  {
   var k=id+"ventBridge"+n;
   var b=block(context,k+"solid",[x-4,-95,60],[x+4,95,63]);
   join(context,k+"join",definition.cover,b);
   n+=1;
  }
  // Eight removable cover bolts. Side-entry pockets accept ordinary M3 nuts.
  n=0;
  for(var x in [-450,-247.5,-82.5,82.5,247.5,450])
  {
   for(var s in [-1,1])
   {
    var k=id+"coverFix"+n;
    var y=87*s;
    var b=block(context,k+"boss",[x-8,min(80*s,95*s),50],[x+8,max(80*s,95*s),60]);
    join(context,k+"join",definition.frame,b);
    var gs=newSketchOnPlane(context,k+"gussetSketch",{"sketchPlane":plane(vector(x-8,0,0)*millimeter,vector(1,0,0),vector(0,1,0))});
    skPolyline(gs,"triangle",{"points":[vector(92*s,38)*millimeter,vector(80*s,50)*millimeter,vector(92*s,50)*millimeter,vector(92*s,38)*millimeter]});
    skSolve(gs);
    opExtrude(context,k+"gusset",{"entities":qSketchRegion(k+"gussetSketch"),"direction":vector(1,0,0),"endBound":BoundingType.BLIND,"endDepth":16*millimeter});
    join(context,k+"gussetJoin",definition.frame,qCreatedBy(k+"gusset",EntityType.BODY));
    opDeleteBodies(context,k+"gussetCleanup",{"entities":qCreatedBy(k+"gussetSketch",EntityType.BODY)});
    var pocket=block(context,k+"nut",[x-3.0,min(79.5*s,90*s),53],[x+3.0,max(79.5*s,90*s),55.5]);
    subtract(context,k+"nutCut",definition.frame,pocket);
    var h=cylinder(context,k+"hole",[x,y,51],[x,y,64],1.7);
    subtract(context,k+"holeCut",qUnion([definition.frame,definition.cover]),h);
    n+=1;
   }
  }
  var hangerIndex=0;
  for(var hx in [-350,350])
  {
   for(var dx in [-7,7])
   {
    var hk=id+"hangerFastener"+hangerIndex;
    var hangerHole=cylinder(context,hk+"tool",[hx+dx,71,59],[hx+dx,71,70],1.7);
    subtract(context,hk+"cut",definition.cover,hangerHole);
    hangerIndex+=1;
   }
  }
  // Four stand-off feet hold the vents clear of the wall. Recesses give screwdriver access.
  n=0;
  for(var x in [-450,450])
  {
   for(var s in [-1,1])
   {
    var k=id+"wallFoot"+n;
    var y=87*s;
    var b=block(context,k+"solid",[x-8,y-8,63],[x+8,y+8,63+gap]);
    join(context,k+"join",definition.cover,b);
    var h=cylinder(context,k+"access",[x,y,63+gap-4],[x,y,64+gap],3.5);
    subtract(context,k+"cut",definition.cover,h);
    var shaft=cylinder(context,k+"shaft",[x,y,59],[x,y,64+gap],1.7);
    subtract(context,k+"shaftCut",definition.cover,shaft);
    n+=1;
   }
  }
  // Adjustable removable panel retainers. Their measured-height setting is mandatory.
  var seat=3+panel+0.3;
  n=0;
  for(var x in [-320,-40,320])
  {
   for(var s in [-1,1])
   {
    var k=id+"retainer"+n;
    var y=88*s;
    var boss=block(context,k+"boss",[x-8,min(84*s,95*s),3],[x+8,max(84*s,95*s),seat]);
    join(context,k+"join",definition.frame,boss);
    var nut=block(context,k+"nut",[x-3,min(83.5*s,91*s),seat-6],[x+3,max(83.5*s,91*s),seat-3.5]);
    subtract(context,k+"nutCut",definition.frame,nut);
    var clip=block(context,k+"clip",[x-8,min(75*s,91.5*s),seat],[x+8,max(75*s,91.5*s),seat+3]);
    setProperty(context,{"entities":clip,"propertyType":PropertyType.NAME,"value":"Panel retainer " ~ (n+1) ~ " - measured height required"});
    setProperty(context,{"entities":clip,"propertyType":PropertyType.APPEARANCE,"value":color(0.12,0.8,0.25)});
    var h=cylinder(context,k+"hole",[x,y,seat-8],[x,y,seat+4],1.7);
    subtract(context,k+"holeCut",qUnion([definition.frame,clip]),h);
    n+=1;
   }
  }
  // Known-size perfboard tray, electronics face toward the panels. Keep measured component heights clear.
  var tray=block(context,id+"controllerTray",[-327.5,-50,42],[-167.5,50,45]);
  n=0;
  for(var x in [-323.5,-171.5])
  {
   for(var y in [-46,46])
   {
    var k=id+"trayPost"+n;
    var post=block(context,k+"solid",[x-5,y-5,45],[x+5,y+5,60]);
    join(context,k+"trayJoin",tray,post);
    n+=1;
   }
  }
  setProperty(context,{"entities":tray,"propertyType":PropertyType.NAME,"value":"Controller tray 160x100 - verify component clearance"});
  // Tie slots fit the 150x90 perfboard without assuming a mounting-hole pattern.
  n=0;
  for(var x in [-323.5,-171.5])
  {
   for(var y in [-28,28])
   {
    var k=id+"tieSlot"+n;
    var tool=block(context,k+"solid",[x-1.5,y-6,41],[x+1.5,y+6,46]);
    subtract(context,k+"cut",tray,tool);
    n+=1;
   }
  }
  // Four tray bolts and captive-nut pockets match the existing M3x10 hardware.
  n=0;
  for(var x in [-323.5,-171.5])
  {
   for(var y in [-46,46])
   {
    var k=id+"trayFix"+n;
    var y0=y<0?y-3:y-6;
    var y1=y<0?y+6:y+3;
    var nut=block(context,k+"nut",[x-3,y0,53],[x+3,y1,55.5]);
    subtract(context,k+"nutCut",tray,nut);
    var h=cylinder(context,k+"hole",[x,y,51],[x,y,64],1.7);
    subtract(context,k+"holeCut",qUnion([tray,definition.cover]),h);
    n+=1;
   }
  }
  // Reinforced keyhole hangers: wall anchors must be selected for the real wall and weight.
  n=0;
  for(var x in [-350,350])
  {
   var k=id+"hanger"+n;
   var pad=block(context,k+"pad",[x-12,42,63],[x+12,76,69]);
   join(context,k+"join",definition.cover,pad);
   var entry=cylinder(context,k+"entry",[x,54,59],[x,54,70],4);
   var neck=block(context,k+"neck",[x-2,54,59],[x+2,67,70]);
   var end=cylinder(context,k+"end",[x,67,59],[x,67,70],2);
   subtract(context,k+"cut",definition.cover,qUnion([entry,neck,end]));
   n+=1;
  }
  // Universal microphone tie mount, with an acoustic opening through its centre.
  n=0;
  for(var x in [-415,-388])
  {
   var k=id+"micSupport"+n;
   var b=block(context,k+"solid",[x,-95,3],[x+3,-86,38]);
   join(context,k+"join",definition.frame,b);
   n+=1;
  }
  var micPlate=block(context,id+"micPlate",[-415,-89,3],[-385,-86,38]);
  join(context,id+"micPlateJoin",definition.frame,micPlate);
  n=0;
  for(var x in [-410,-390])
  {
   var k=id+"micTie"+n;
   var t=block(context,k+"solid",[x-1.5,-90,22],[x+1.5,-85,34]);
   subtract(context,k+"cut",definition.frame,t);
   n+=1;
  }
  // Sound entry; not an electrical connector opening.
  var mic=cylinder(context,id+"micPort",[-400,-99,28],[-400,-85,28],1.5);
  subtract(context,id+"micCut",definition.frame,mic);
  // Supply cradle and fit envelope. These dimensions are assumptions until measured.
  var sl=definition.supplyLength/millimeter;
  var sw=definition.supplyWidth/millimeter;
  var sh=definition.supplyHeight/millimeter;
  if(sl<100 || sl>220 || sw<20 || sw>60 || sh<10 || sh>30)
    throw regenError("Supply exceeds this bay: maximum case 220x60x30 mm. Revise layout for a larger case.");
  var supply=block(context,id+"powerTray",[95,-40,21],[335,40,24]);
  n=0;
  for(var x in [95,332])
  {
   var k=id+"powerEnd"+n;
   var rail=block(context,k+"solid",[x,-40,24],[x+3,40,35]);
   join(context,k+"join",supply,rail);
   n+=1;
  }
  n=0;
  for(var y in [-40,37])
  {
   var k=id+"powerSide"+n;
   var rail=block(context,k+"solid",[98,y,24],[332,y+3,35]);
   join(context,k+"join",supply,rail);
   n+=1;
  }
  n=0;
  for(var x in [100,330])
  {
   for(var y in [-35,35])
   {
    var k=id+"powerPost"+n;
    var post=block(context,k+"solid",[x-5,y-5,24],[x+5,y+5,60]);
    join(context,k+"join",supply,post);
    var y0=y<0?y-3:y-6;
    var y1=y<0?y+6:y+3;
    var nut=block(context,k+"nut",[x-3,y0,53],[x+3,y1,55.5]);
    subtract(context,k+"nutCut",supply,nut);
    var h=cylinder(context,k+"hole",[x,y,51],[x,y,64],1.7);
    subtract(context,k+"holeCut",qUnion([supply,definition.cover]),h);
    n+=1;
   }
  }
  n=0;
  for(var x in [130,300])
  {
   var k=id+"powerTie"+n;
   var t=block(context,k+"solid",[x-2,-30,20],[x+2,30,25]);
   subtract(context,k+"cut",supply,t);
   n+=1;
  }
  setProperty(context,{"entities":supply,"propertyType":PropertyType.NAME,"value":"Supply cradle - fit and electrical protection review required"});
  var caseRef=block(context,id+"supplyEnvelope",[215-sl/2,-sw/2,24],[215+sl/2,sw/2,24+sh]);
  setProperty(context,{"entities":caseRef,"propertyType":PropertyType.NAME,"value":"REFERENCE ONLY - supply envelope UNVERIFIED - DO NOT PRINT"});
  setProperty(context,{"entities":caseRef,"propertyType":PropertyType.APPEARANCE,"value":color(0.85,0.65,0.15)});
  // Explicit display references allow a collision check of the nominal, unmeasured panels.
  var panelRefs=[];
  n=0;
  for(var x in [-480,-160,160])
  {
   var k=id+"panelRef"+n;
   var p=block(context,k,[x,-80,3],[x+320,80,3+panel]);
   panelRefs=append(panelRefs,p);
   setProperty(context,{"entities":p,"propertyType":PropertyType.NAME,"value":"REFERENCE ONLY - panel " ~ (n+1) ~ " thickness UNVERIFIED - DO NOT PRINT"});
   setProperty(context,{"entities":p,"propertyType":PropertyType.APPEARANCE,"value":color(0.3,0.4,0.5)});
   n+=1;
  }
  // Provisional PG9-sized cable exit; confirm actual gland thread and mounting diameter.
  var glandBoss=block(context,id+"glandBoss",[388,-95,28],[412,-87,52]);
  join(context,id+"glandJoin",definition.frame,glandBoss);
  var glandHole=cylinder(context,id+"glandHole",[400,-97,40],[400,-85,40],7.8);
  subtract(context,id+"glandCut",definition.frame,glandHole);
  // Recessed lettering on the front lower border; stays within individual rail sections.
  var labels=[{"text":"StreamPixel","x":15,"height":7},{"text":"Ridith Shetty","x":-145,"height":5}];
  var labelIndex=0;
  for(var label in labels)
  {
   var labelId=id+"branding"+labelIndex;
   var lettering=newSketchOnPlane(context,labelId+"sketch",{"sketchPlane":plane(vector(0,0,-0.1)*millimeter,vector(0,0,-1),vector(1,0,0))});
   skText(lettering,"name",{"text":label.text,"fontName":"OpenSans-Bold.ttf","firstCorner":vector(label.x,83)*millimeter,"secondCorner":vector(label.x+140,83+label.height)*millimeter});
   skSolve(lettering);
   opExtrude(context,labelId+"tool",{"entities":qSketchRegion(labelId+"sketch"),"direction":vector(0,0,1),"endBound":BoundingType.BLIND,"endDepth":0.7*millimeter});
   subtract(context,labelId+"engrave",definition.frame,qCreatedBy(labelId+"tool",EntityType.BODY));
   opDeleteBodies(context,labelId+"cleanup",{"entities":qCreatedBy(labelId+"sketch",EntityType.BODY)});
   labelIndex+=1;
  }
  var frameBounds=evBox3d(context,{"topology":definition.frame,"tight":true});
  var coverBounds=evBox3d(context,{"topology":definition.cover,"tight":true});
  println("Frame bounds, mm: " ~ (frameBounds.minCorner/millimeter) ~ " to " ~ (frameBounds.maxCorner/millimeter));
  println("Cover bounds, mm: " ~ (coverBounds.minCorner/millimeter) ~ " to " ~ (coverBounds.maxCorner/millimeter));
  var solids=qBodyType(qEverything(EntityType.BODY),BodyType.SOLID);
  println("Solid part count: " ~ size(evaluateQuery(context,solids)));
  if(size(evaluateQuery(context,solids))!=14) throw regenError("Unexpected split parts: expected frame, cover, six retainers, two trays and four reference envelopes.");
  println("Printable parts volume excluding supply reference, cubic cm: " ~ (evVolume(context,{"entities":qSubtraction(solids,qUnion(append(panelRefs,caseRef)))})/(centimeter^3)));

  var printBodies=qSubtraction(solids,qUnion(append(panelRefs,caseRef)));
  var internalBad=0;
  for(var printed in evaluateQuery(context,printBodies))
  {
   for(var clash in evCollision(context,{"tools":printed,"targets":qSubtraction(printBodies,printed)}))
    if(clash["type"]==ClashType.INTERFERE || clash["type"]==ClashType.TARGET_IN_TOOL || clash["type"]==ClashType.TOOL_IN_TARGET) internalBad+=1;
  }
  println("Printed-part solid interferences: " ~ internalBad);
  if(internalBad>0) throw regenError("Printed parts interfere; review the geometry.");
  var bad=0;
  for(var c in evCollision(context,{"tools":qUnion(append(panelRefs,caseRef)),"targets":printBodies}))
  {
    if(c["type"]==ClashType.INTERFERE || c["type"]==ClashType.TARGET_IN_TOOL || c["type"]==ClashType.TOOL_IN_TARGET) bad+=1;
  }
  println("Reference-envelope solid interferences: " ~ bad);
  if(bad>0) throw regenError("A reference envelope intersects a printed part. Revise the layout before printing.");
 },{"airGap":15*millimeter,"panelThickness":15*millimeter});


annotation {"Feature Type Name":"StreamPixel CNC and printed holders"}
export const preparePrint = defineFeature(function(context is Context,id is Id,definition is map)
 precondition
 {
  annotation {"Name":"Frame","Filter":EntityType.BODY && BodyType.SOLID,"MaxNumberOfPicks":1}
  definition.frame is Query;
  annotation {"Name":"Back cover","Filter":EntityType.BODY && BodyType.SOLID,"MaxNumberOfPicks":1}
  definition.cover is Query;
  annotation {"Name":"Supply cradle","Filter":EntityType.BODY && BodyType.SOLID,"MaxNumberOfPicks":1}
  definition.supply is Query;
  annotation {"Name":"Four REFERENCE ONLY bodies to exclude","Filter":EntityType.BODY && BodyType.SOLID}
  definition.references is Query;
  annotation {"Name":"Keep CNC components only","Default":false}
  definition.acrylicOnly is boolean;
 }
 {
  if(size(evaluateQuery(context,definition.references))!=4) throw regenError("Select all four reference bodies.");
  opDeleteBodies(context,id+"removeReferences",{"entities":definition.references});
  // The CNC frame and rear plate remain whole; no 165mm rail cuts or splice plates.
  opSplitPart(context,id+"rearMounts",{"targets":definition.cover,"tool":plane(vector(0,0,63)*millimeter,vector(0,0,1))});
  // Only the supply holder is split to fit the assumed 220mm print bed.
  opSplitPart(context,id+"holderSplit",{"targets":definition.supply,"tool":plane(vector(215,0,0)*millimeter,vector(1,0,0))});
  var printed=[];
  var cnc=[];
  var retainers=0;
  var spacers=0;
  var hangers=0;
  for(var body in evaluateQuery(context,qBodyType(qEverything(EntityType.BODY),BodyType.SOLID)))
  {
   var bb=evBox3d(context,{"topology":body,"tight":true});
   var z=bb.minCorner[2]/millimeter;
   var upper=bb.maxCorner[2]/millimeter;
   var name="";
   if(abs(z-21)<0.01)
   {
    printed=append(printed,body);
    name="PRINT - supply holder " ~ (bb.maxCorner[0]<=215.01*millimeter?"left":"right");
   }
   else if(abs(z-42)<0.01)
   {
    printed=append(printed,body);name="PRINT - controller tray 160x100";
   }
   else
   {
    cnc=append(cnc,body);
    if(abs(z)<0.01) name="CNC MILL - whole frame 990x190x60 - STEP controls";
    else if(abs(z-60)<0.01 && abs(upper-63)<0.01) name="CNC CUT - whole backplate 990x190x3";
    else if(abs(z-63)<0.01 && upper>70)
    {spacers+=1;name="CNC MILL - wall spacer " ~ spacers;}
    else if(abs(z-63)<0.01)
    {hangers+=1;name="CNC MILL - hanger pad " ~ hangers;}
    else
    {retainers+=1;name="CNC CUT - panel retainer " ~ retainers;}
   }
   setProperty(context,{"entities":body,"propertyType":PropertyType.NAME,"value":name});
  }
  if(size(printed)!=3 || size(cnc)!=14) throw regenError("Expected three printed holder pieces and fourteen CNC components.");
  opDeleteBodies(context,id+"manufacturingSet",{"entities":definition.acrylicOnly?qUnion(printed):qUnion(cnc)});
  println(definition.acrylicOnly?"CNC: one whole frame, one whole backplate, six retainers, four wall spacers and two hanger pads.":"PRINT: controller tray and two supply-holder halves ONLY.");
  println("Hardware: 22 M3x10, four M3x25, four M3x16 bolts and 30 M3 nuts; screws are purchased hardware.");
  println("CNC milling quote and tool access review required. Physical dimensions, fit, thermal and wall-load testing remain unverified.");
 });
