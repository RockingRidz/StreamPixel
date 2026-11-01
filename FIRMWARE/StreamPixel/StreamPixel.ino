#include <Arduino.h>
#include <WiFi.h>
#include <WebServer.h>
#include <Preferences.h>
#include <esp_system.h>
#include <ESP32-HUB75-MatrixPanel-I2S-DMA.h>
#include <math.h>
#include "config.h"

WebServer server(80);
MatrixPanel_I2S_DMA *display = nullptr;
bool outputEnabled = false;
uint8_t brightness = 8;
String mode = "text", message = "StreamPixel", apName;
float soundLevel = 0;
bool micClipping = false;
unsigned long lastFrame = 0;

const char PAGE[] PROGMEM = R"HTML(<!doctype html><html lang="en"><meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1"><title>StreamPixel</title>
<style>body{font:17px system-ui;background:#101821;color:#edf6ff;max-width:620px;margin:30px auto;padding:20px}h1{margin-bottom:6px}section{background:#1d2c3b;padding:20px;border-radius:14px;margin:18px 0}label{display:block;margin:14px 0}input,select,button{font:inherit;box-sizing:border-box;padding:10px;border-radius:7px;width:100%;margin-top:6px}button{background:#9be5dc;border:0;color:#10232c;cursor:pointer}small{color:#bfcedd}#result{min-height:24px}</style>
<h1>StreamPixel</h1><small>Ridith Shetty · local display controls</small>
<section><strong id="status">Connecting…</strong><p>Sound level: <span id="level">—</span></p><small id="clipping"></small></section>
<section><form id="controls"><label>Display mode<select name="mode"><option value="text">Scrolling text</option><option value="sound">Sound meter</option><option value="rainbow">Colour animation</option><option value="test">Red / green / blue test</option></select></label>
<label>Your text<input name="text" maxlength="48" value="StreamPixel" autocomplete="off"></label>
<label>Brightness <input id="brightness" name="brightness" type="range" min="0" max="16" value="8"></label>
<label>Output<select name="enabled"><option value="0">Off</option><option value="1">On</option></select></label>
<button>Apply</button></form><p id="result" role="status"></p></section>
<small>The sound meter shows relative microphone activity. It does not measure decibels. Spotify integration is not included.</small>
<script>const form=document.querySelector('form'),result=document.querySelector('#result');
async function refresh(){try{const response=await fetch('/api/status',{cache:'no-store'});if(!response.ok)throw Error('Status unavailable');const s=await response.json();document.querySelector('#status').textContent=s.profileVerified?(s.enabled?'Display on':'Display off'):'Panel output disabled — panel profile needs verification';document.querySelector('#level').textContent=Math.round(s.sound*100)+'%';document.querySelector('#clipping').textContent=s.clipping?'Microphone input is clipping. Check wiring and gain.':'';document.querySelector('#brightness').max=s.brightnessCap;}catch(e){document.querySelector('#status').textContent='Disconnected. Reconnect to StreamPixel Wi-Fi.';}}
form.addEventListener('submit',async e=>{e.preventDefault();result.textContent='Applying…';try{const response=await fetch('/api/settings',{method:'POST',body:new URLSearchParams(new FormData(form))});const s=await response.json();if(!response.ok)throw Error(s.error||'Could not apply');result.textContent='Saved for this session.';await refresh();}catch(error){result.textContent=error.message;}});refresh();setInterval(refresh,1500);</script></html>)HTML";

String freshPassword() {
  const char alphabet[]="ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
  String password;
  for (int i=0;i<16;i++) password += alphabet[esp_random()%32];
  return password;
}

void settings() {
  const String requestedMode=server.arg("mode");
  if (requestedMode!="text" && requestedMode!="sound" && requestedMode!="rainbow" && requestedMode!="test") {
    server.send(400,"application/json","{\"error\":\"Choose a valid mode\"}");return;
  }
  String text=server.arg("text");
  if (text.length()>48) {server.send(400,"application/json","{\"error\":\"Text limit is 48 characters\"}");return;}
  for (size_t i=0;i<text.length();i++) {
    if (text[i]<32 || text[i]>126) {server.send(400,"application/json","{\"error\":\"Use English letters, numbers and punctuation\"}");return;}
  }
  const String b=server.arg("brightness");
  if (b.isEmpty() || b.length()>3) {server.send(400,"application/json","{\"error\":\"Invalid brightness\"}");return;}
  for (size_t i=0;i<b.length();i++) if (!isDigit(b[i])) {server.send(400,"application/json","{\"error\":\"Invalid brightness\"}");return;}
  const int newBrightness=b.toInt();
  const String enabled=server.arg("enabled");
  if (newBrightness>BRIGHTNESS_CAP || (enabled!="0" && enabled!="1")) {server.send(400,"application/json","{\"error\":\"Settings out of range\"}");return;}
  if (enabled=="1" && !display) {server.send(409,"application/json","{\"error\":\"Panel profile has not been verified, or display initialization failed\"}");return;}
  mode=requestedMode;message=text;brightness=newBrightness;outputEnabled=(enabled=="1");
  if(display) {display->setBrightness8(outputEnabled?brightness:0);if(!outputEnabled)display->clearScreen();}
  server.send(200,"application/json","{\"ok\":true}");
}

void status() {
  String json="{\"profileVerified\":";json+=(PANEL_PROFILE_VERIFIED?"true":"false");
  json+=",\"enabled\":";json+=(outputEnabled?"true":"false");
  json+=",\"brightnessCap\":"+String(BRIGHTNESS_CAP)+",\"sound\":"+String(soundLevel,3);
  json+=",\"clipping\":";json+=(micClipping?"true":"false");json+="}";
  server.sendHeader("Cache-Control","no-store");server.send(200,"application/json",json);
}

void sampleSound() {
  uint32_t sum=0;uint64_t squares=0;bool clipping=false;
  constexpr int count=64;
  for(int i=0;i<count;i++) {
    uint32_t value=analogRead(MIC_PIN);sum+=value;squares+=uint64_t(value)*value;
    if(value<20 || value>4075)clipping=true;
    delayMicroseconds(125);
  }
  const float mean=float(sum)/count;
  const float variance=max(0.0f,float(squares)/count-mean*mean);
  const float normalized=min(1.0f,sqrtf(variance)/300.0f);
  soundLevel=0.75f*soundLevel+0.25f*normalized;micClipping=clipping;
}

void drawFrame() {
  if(!display || !outputEnabled)return;
  const int w=display->width(),h=display->height();display->clearScreen();
  if(mode=="text") {
    display->setTextWrap(false);display->setTextSize(1);display->setTextColor(display->color565(80,220,200));
    int travel=w+int(message.length())*6;if(travel<1)travel=1;
    display->setCursor(w-int((millis()/90)%travel),max(0,(h-8)/2));display->print(message);
  } else if(mode=="sound") {
    const int lit=int(soundLevel*w+0.5f);
    for(int x=0;x<lit;x++)display->drawFastVLine(x,0,h,display->color565(255*x/max(1,w-1),255-255*x/max(1,w-1),20));
  } else if(mode=="test") {
    const int phase=(millis()/1500)%3;
    display->fillScreen(display->color565(phase==0?255:0,phase==1?255:0,phase==2?255:0));
  } else {
    for(int x=0;x<w;x++) {
      uint8_t phase=(x*256/max(1,w)+millis()/15)%256;
      uint8_t red=phase<85?255-phase*3:(phase<170?0:(phase-170)*3);
      uint8_t green=phase<85?phase*3:(phase<170?255-(phase-85)*3:0);
      uint8_t blue=phase<85?0:(phase<170?(phase-85)*3:255-(phase-170)*3);
      display->drawFastVLine(x,0,h,display->color565(red,green,blue));
    }
  }
}

void setup() {
  pinMode(DISPLAY_OE_PIN,OUTPUT);digitalWrite(DISPLAY_OE_PIN,HIGH);
  pinMode(33,OUTPUT);digitalWrite(33,LOW); // defined idle level for unused buffered E
  Serial.begin(115200);analogReadResolution(12);analogSetPinAttenuation(MIC_PIN,ADC_11db);
  if(PANEL_PROFILE_VERIFIED) {
    HUB75_I2S_CFG cfg(PANEL_WIDTH,PANEL_HEIGHT,PANEL_CHAIN);
    cfg.gpio.r1=PIN_R1;cfg.gpio.g1=PIN_G1;cfg.gpio.b1=PIN_B1;
    cfg.gpio.r2=PIN_R2;cfg.gpio.g2=PIN_G2;cfg.gpio.b2=PIN_B2;
    cfg.gpio.a=PIN_A;cfg.gpio.b=PIN_B;cfg.gpio.c=PIN_C;cfg.gpio.d=PIN_D;cfg.gpio.e=PIN_E;
    cfg.gpio.clk=PIN_CLK;cfg.gpio.lat=PIN_LAT;cfg.gpio.oe=DISPLAY_OE_PIN;
    cfg.i2sspeed=HUB75_I2S_CFG::HZ_10M;
    display=new MatrixPanel_I2S_DMA(cfg);display->setBrightness8(0);
    if(!display->begin()) {delete display;display=nullptr;digitalWrite(DISPLAY_OE_PIN,HIGH);Serial.println("Display initialization failed; output remains off.");}
    else {display->clearScreen();display->setBrightness8(0);}
  }
  Preferences preferences;preferences.begin("streampixel",false);
  String password=preferences.getString("ap-password","");
  if(password.length()<12) {password=freshPassword();preferences.putString("ap-password",password);}
  preferences.end();
  apName="StreamPixel-"+String(uint32_t(ESP.getEfuseMac()&0xffffff),HEX);
  WiFi.mode(WIFI_AP);
  if(!WiFi.softAP(apName.c_str(),password.c_str()))Serial.println("Wi-Fi access point failed.");
  Serial.println("StreamPixel | Ridith Shetty");Serial.println("Wi-Fi: "+apName);
  Serial.println("Password: "+password);Serial.print("Open http://");Serial.println(WiFi.softAPIP());
  Serial.println(PANEL_PROFILE_VERIFIED?"Panel profile configured; output starts OFF.":"Panel output disabled: verify scan ratio, driver, pin8 and row mapping first.");
  server.on("/",HTTP_GET,[]{server.send_P(200,"text/html",PAGE);});
  server.on("/api/status",HTTP_GET,status);server.on("/api/settings",HTTP_POST,settings);
  server.onNotFound([]{server.send(404,"text/plain","Not found");});server.begin();
}

void loop() {
  server.handleClient();
  const unsigned long now=millis();
  if(now-lastFrame>=33) {lastFrame=now;sampleSound();drawFrame();}
  delay(1);
}
