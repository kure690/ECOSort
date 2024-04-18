#include <Servo.h>

// Constants and variables for soil moisture, IR sensor, and servo
const int soilSensorPin = A0;
const int ledPin = 13;
const int servoPin = 9;
const int irSensorPin = 10;
Servo myservo;

// Constants for ultrasonic sensors
const int trigPin1 = 2;
const int echoPin1 = 3;
const int trigPin2 = 4;
const int echoPin2 = 5;

// Function prototypes
float getSoilMoisture();
int getDistance(int trigPin, int echoPin);

void setup() {
  // Setup for soil moisture, IR sensor, and servo
  pinMode(ledPin, OUTPUT);
  pinMode(servoPin, OUTPUT);
  pinMode(irSensorPin, INPUT);
  myservo.attach(servoPin);
  
  // Setup for ultrasonic sensors
  pinMode(trigPin1, OUTPUT);
  pinMode(echoPin1, INPUT);
  pinMode(trigPin2, OUTPUT);
  pinMode(echoPin2, INPUT);

  // Start serial communication
  Serial.begin(9600);
}

void loop() {
  // Logic for soil moisture and IR sensor
  float soilMoisture = getSoilMoisture();
  bool isObjectDetected = digitalRead(irSensorPin) == HIGH;
  float threshold = 80.0;

  if (isObjectDetected) {
    digitalWrite(ledPin, HIGH);
    myservo.write(soilMoisture < threshold ? 130 : 80);
  } else {
    digitalWrite(ledPin, LOW);
    myservo.write(15); // Default position when no object is detected
  }

  // Logic for ultrasonic sensors
  int distance1 = getDistance(trigPin1, echoPin1);
  int distance2 = getDistance(trigPin2, echoPin2);

  // Print distance measurements
  Serial.print("Distance 1: ");
  Serial.print(distance1);
  Serial.println(" cm");
  Serial.print("Distance 2: ");
  Serial.print(distance2);
  Serial.println(" cm");

  // Delay to prevent too frequent movements
  delay(1000);
}

float getSoilMoisture() {
  int sensorValue = analogRead(soilSensorPin);
  float voltage = sensorValue * (5.0 / 1023.0);
  float moisturePercentage = map(voltage, 0.8, 4.0, 0, 100);
  moisturePercentage = constrain(moisturePercentage, 0, 100);
  return moisturePercentage;
}

int getDistance(int trigPin, int echoPin) {
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  long duration = pulseIn(echoPin, HIGH);
  return duration * 0.034 / 2;
}
