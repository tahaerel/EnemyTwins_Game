#define BUTTON_PIN 2
#define BUTTON_PIN2 3
#define BUTTON_PIN3 4
#define BUTTON_PIN4 5
#define BUTTON_PIN5 6

void setup() {
  Serial.begin(9600);
  pinMode(BUTTON_PIN, INPUT_PULLUP);
  pinMode(BUTTON_PIN2, INPUT_PULLUP);
  pinMode(BUTTON_PIN3, INPUT_PULLUP);
  pinMode(BUTTON_PIN4, INPUT_PULLUP);
   pinMode(BUTTON_PIN5, INPUT_PULLUP);
}

void loop() {
    int buttonState1 = digitalRead(BUTTON_PIN);
    int buttonState2 = digitalRead(BUTTON_PIN2);
    int buttonState3 = digitalRead(BUTTON_PIN3);
    int buttonState4 = digitalRead(BUTTON_PIN4);
    int buttonState5 = digitalRead(BUTTON_PIN5);
    // Eğer Button 1'e basıldıysa
    if (buttonState1 == LOW) {
        Serial.println("Button 1 Pressed");
    }
    if (buttonState1 == HIGH) {
        Serial.println("Button 1 Release");
    }

    // Eğer Button 2'ye basıldıysa
      if (buttonState2 == LOW) {
        Serial.println("Button 2 Pressed");
    }
    if (buttonState2 == HIGH) {
        Serial.println("Button 2 Release");
    }

     if (buttonState3 == LOW) {
        Serial.println("Button 3 Pressed");
    }
    if (buttonState3 == HIGH) {
        Serial.println("Button 3 Release");
    }


     if (buttonState4 == LOW) {
        Serial.println("Button 4 Pressed");
    }
        if (buttonState4 == HIGH) {
        Serial.println("Button 4 Release");
    }
     if (buttonState5 == LOW) {
        Serial.println("Button 5 Pressed");
    }
    if (buttonState5 == HIGH) {
        Serial.println("Button 5 Release");
    }

}
