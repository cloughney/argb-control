#include <FastLED.h>

using namespace std;

#define PORT_COUNT 5
#define PORT_MAX_LIGHT_COUNT 12

#define PORT_1_PIN 5
#define PORT_1_LIGHT_COUNT 12

#define PORT_2_PIN 6
#define PORT_2_LIGHT_COUNT 12

#define PORT_3_PIN 9
#define PORT_3_LIGHT_COUNT 12

#define PORT_4_PIN 10
#define PORT_4_LIGHT_COUNT 12

#define PORT_5_PIN 11
#define PORT_5_LIGHT_COUNT 12

CRGB leds[PORT_COUNT][PORT_MAX_LIGHT_COUNT];

const byte inputBufferSize = 32;
char input[inputBufferSize];
bool inputReady = false;

void setColor(int portIndex, int lightIndex, int r, int g, int b) {
	leds[portIndex][lightIndex].r = r;
	leds[portIndex][lightIndex].g = g;
	leds[portIndex][lightIndex].b = b;

	FastLED.show();
}

bool validateSetColorCommand(long portIndex, long lightIndex, long r, long g, long b) {
	if (portIndex < 0 || portIndex >= PORT_COUNT) {
		Serial.println("bad port");
		return false;
	}

	if (lightIndex < 0 || lightIndex >= PORT_MAX_LIGHT_COUNT) { //TODO better validation
		Serial.println("bad light");
		return false;
	}

	if (r < 0 || r > 255 ||
		g < 0 || g > 255 ||
		b < 0 || b > 255) {
		Serial.println("bad color");
		return false;
	}

	
	Serial.println("ok");
	return true;
}

void handleSetColorCommand(char *arguments) {
	long portIndex = strtol(arguments, &arguments, 10) - 1;
	long lightIndex = strtol(arguments, &arguments, 10) - 1;

	long r = strtol(arguments, &arguments, 16);
	long g = strtol(arguments, &arguments, 16);
	long b = strtol(arguments, &arguments, 16);

	if (validateSetColorCommand(portIndex, lightIndex, r, g, b)) {
		setColor(portIndex, lightIndex, r, g, b);
	}
}

void handlePersistStateCommand() {
	Serial.println("ok");
}

void handleSerialInput() {
	if (!inputReady) {
		return;
	}

	char *arguments;
	long command = strtol(input, &arguments, 10);
	
	switch (command) {
		case 100: // Set color
			handleSetColorCommand(arguments);
			break;
		case 500: // Persist state
			handlePersistStateCommand();
			break;
		default:
			Serial.println("bad command");
			break;
	}

	inputReady = false;
}

void readSerialInput() {
	static byte index = 0;
	char current;

	while (Serial.available() > 0 && !inputReady) {
		current = Serial.read();

		if (current != '\n') {
			input[index++] = current;

			if (index >= inputBufferSize) {
				index = inputBufferSize - 1;
			}

			continue;
		}
		
		input[index] = '\0';
		index = 0;

		Serial.println("thanks");

		inputReady = true;
	}
}

void setup() {
	Serial.begin(115200);

	FastLED.addLeds<WS2812B, PORT_1_PIN, GRB>(leds[0], PORT_1_LIGHT_COUNT);
	FastLED.addLeds<WS2812B, PORT_2_PIN, GRB>(leds[1], PORT_2_LIGHT_COUNT);
	FastLED.addLeds<WS2812B, PORT_3_PIN, GRB>(leds[2], PORT_3_LIGHT_COUNT);
	FastLED.addLeds<WS2812B, PORT_4_PIN, GRB>(leds[3], PORT_4_LIGHT_COUNT);
	FastLED.addLeds<WS2812B, PORT_5_PIN, GRB>(leds[4], PORT_5_LIGHT_COUNT);

	FastLED.setCorrection(CRGB(TypicalSMD5050));

	//TODO initializeState();

	Serial.println("argb-controller v0.1");
}

void loop() {
	readSerialInput();
	handleSerialInput();
}