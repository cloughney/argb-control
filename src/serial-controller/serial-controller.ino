#include <FastLED.h>

using namespace std;

#define FAN_COUNT 3
#define FAN_1_PIN 5
#define FAN_2_PIN 6
#define FAN_3_PIN 7 //FIXME set actual fan PWM pins

#define RGB_COUNT 3
#define RGB_MAX_LIGHT_COUNT 32
#define RGB_1_PIN 9
#define RGB_2_PIN 10
#define RGB_3_PIN 11

CRGB leds[RGB_COUNT][RGB_MAX_LIGHT_COUNT];

const uint8_t inputBufferSize = 255;
char input[inputBufferSize];
bool inputReady = false;

void setColor(uint8_t deviceIndex, uint8_t lightIndex, uint8_t r, uint8_t g, uint8_t b) {
	leds[deviceIndex][lightIndex].r = r;
	leds[deviceIndex][lightIndex].g = g;
	leds[deviceIndex][lightIndex].b = b;
}

void handleSetColorCommand(uint8_t *data, uint8_t length) {
	uint8_t dataIndex = 0;
	uint8_t deviceIndex = data[dataIndex++];

	if (deviceIndex >= RGB_COUNT) {
		return;
	}
	
	uint8_t lightIndex = 0;

	while (dataIndex < length - 3) {
		uint8_t r = data[dataIndex++];
		uint8_t g = data[dataIndex++];
		uint8_t b = data[dataIndex++];

		if (lightIndex >= RGB_MAX_LIGHT_COUNT) {
			return;
		}

		setColor(deviceIndex, lightIndex, r, g, b);
	}

	FastLED.show();

  Serial.write(0x20);
  Serial.write(0x00);
}

void setFanSpeed(uint8_t deviceIndex, uint8_t speed) {
  //TODO adjust fan pin pwm
}

void handleSetFanSpeedCommand(uint8_t *data, uint8_t length) {
  uint8_t dataIndex = 0;
  uint8_t deviceIndex = data[dataIndex++];

  if (deviceIndex >= RGB_COUNT) {
    return;
  }

  uint8_t speed = data[dataIndex++];

  setFanSpeed(deviceIndex, speed);

  Serial.write(0x21);
  Serial.write(0x00);
}

void handleSerialInput() {
	if (!inputReady) {
		return;
	}

	uint8_t command = input[0];
	uint8_t length = input[1];

	char data[length];
	memcpy(data, input[2], length);
	
	switch (command) {
		case 0x10: handleSetColorCommand(data, length); break;
    case 0x11: handleSetFanSpeedCommand(data, length); break;
		default:
		  Serial.write(0xE0);
      Serial.write(0x00);
		  break;
	}

	inputReady = false;
}

void readSerialInput() {
	static uint8_t index = 0;

	while (Serial.available() > 0 && !inputReady) {
		input[index] = Serial.read();

		if (index < inputBufferSize) {
			index++;
		}
	}

	if (index > 0) {
		index = 0;
		inputReady = true;
	}
}

void setup() {
	Serial.begin(115200);
	Serial.println();

  //TODO setup fan pwm pins

	FastLED.addLeds<WS2812B, RGB_1_PIN, GRB>(leds[0], RGB_MAX_LIGHT_COUNT);
	FastLED.addLeds<WS2812B, RGB_2_PIN, GRB>(leds[1], RGB_MAX_LIGHT_COUNT);
	FastLED.addLeds<WS2812B, RGB_3_PIN, GRB>(leds[2], RGB_MAX_LIGHT_COUNT);

	FastLED.setCorrection(CRGB(TypicalSMD5050));

	//TODO initializeState();

	Serial.println("argb-controller v0.2");
}

void loop() {
	readSerialInput();
	handleSerialInput();
}
