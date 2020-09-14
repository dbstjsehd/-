#include <Keyboard.h>
#include <Mouse.h>



String Command = "";
char Temp = "";
int xpos = 0, ypos = 0;
bool move_flag = false;
bool xpositive = false;
bool ypositive = false;

//swith setting
const char command_mouseMove = '!';
const char command_end = '+';
const char command_special = '@';
const char command_reservation = '#';
const char command_setDelay = '$';


int delayRate = 14;






//variable



void setup() {
  // put your setup code here, to run once:

  Serial.begin(9600);
  Keyboard.begin();
  Mouse.begin();
  

}

void loop() {
  // put your main code here, to run repeatedly:
  if(move_flag){
    int moveX,moveY;
    if(xpos > 127){
      xpos -= 127;
      moveX = 127;
    }
    else if(xpos < -127){
      xpos += 127;
      moveX = -127;
    }
    else{
      moveX = xpos;
      xpos = 0;
    }

    if(ypos > 127){
      ypos -= 127;
      moveY = 127;
    }
    else if(ypos < -127){
      ypos += 127;
      moveY = -127;
    }
    else{
      moveY = ypos;
      ypos = 0;
    }

    if( (xpos == 0) && (ypos == 0) ){
      move_flag = false;
    }
    Mouse.move(moveX,moveY);


    return;
  }


  Command="";
  if(Serial.available()){
    Temp = Serial.read();
    switch(Temp){
      case command_mouseMove:
        while(Serial.available()){
          Temp = Serial.read();
          if(Temp == command_end){
            
            int first = Command.indexOf(",");
            int second = Command.indexOf(".",first+1);
            xpos = Command.substring(0, first).toInt();
            ypos = Command.substring(first + 1, second).toInt();

            
            move_flag = true;         //moving flag

            if(xpos > 0){
              xpositive = true;
            }
            else{
              xpositive = false;
            }
            if(ypos >0){
              ypositive = true;
            }
            else{
              ypositive = false;
            }
            

            
        
     
            break;
          }
          
          Command.concat(Temp);


          
          
        }

        break;

      case command_setDelay:
        while(Serial.available()){
          Temp = Serial.read();
          if(Temp == command_end){
            delayRate = Command.toInt();


            return;
          }
          Command.concat(Temp);
        }
      break;

      case command_reservation:
        while(Serial.available()){
          Temp = Serial.read();
          if(Temp == command_end){
            int first = Command.indexOf(",");
            int second = Command.indexOf(".",first+1);
            char ch = Command.charAt(0);
            int iterator = Command.substring(first +1 , second).toInt();

            Keyboard.press(KEY_LEFT_CTRL);
            for(int i =0; i < iterator;i++)
            {
              Keyboard.press(ch);
              delay(delayRate);
              Keyboard.release(ch);
              delay(delayRate);
              Mouse.press();
              delay(delayRate);
              Mouse.release();
              delay(delayRate);
            }
            Keyboard.release(KEY_LEFT_CTRL);
            return;
          }

          Command.concat(Temp);
        }
        break;
      case command_special:
        while(Serial.available()){
          Temp = Serial.read();
          if(Temp == command_end){
            if(Command == "LBC"){
              Mouse.press();
              delay(delayRate);
              Mouse.release();
              delay(delayRate);
            }
            else if(Command == "RBC"){
              Mouse.press(MOUSE_RIGHT);
              delay(delayRate);
              Mouse.release(MOUSE_RIGHT);
              delay(delayRate);
              
            }
            else if(Command == "ENTER"){
              Keyboard.press(KEY_RETURN);
              delay(delayRate);
              Keyboard.release(KEY_RETURN);
              delay(delayRate);
            }
            else if(Command == "HOME"){
              Keyboard.press(KEY_HOME);
              delay(delayRate);
              Keyboard.release(KEY_HOME);
              delay(delayRate);
            }
            else if(Command == "LARROW"){
              Keyboard.write(KEY_LEFT_ARROW);
            }
            else if(Command == "F1"){
              Keyboard.press(KEY_F1);
              delay(delayRate);
              Keyboard.release(KEY_F1);
              delay(delayRate);
            }
            else if(Command == "F2"){
              Keyboard.press(KEY_F2);
              delay(delayRate);
              Keyboard.release(KEY_F2);
              delay(delayRate);
            }
            else if(Command == "F3"){
              Keyboard.press(KEY_F3);
              delay(delayRate);
              Keyboard.release(KEY_F3);
              delay(delayRate);
            }
            else if(Command == "F4"){
              Keyboard.press(KEY_F4);
              delay(delayRate);
              Keyboard.release(KEY_F4);
              delay(delayRate);
            }
            else if(Command == "F5"){
              Keyboard.press(KEY_F5);
              delay(delayRate);
              Keyboard.release(KEY_F5);
              delay(delayRate);
            }
            else if(Command == "F7"){
              Keyboard.press(KEY_F7);
              delay(delayRate);
              Keyboard.release(KEY_F7);
              delay(delayRate);
            }
            else if(Command == "F8"){
              Keyboard.press(KEY_F8);
              delay(delayRate);
              Keyboard.release(KEY_F8);
              delay(delayRate);
            }
            else if(Command == "F9"){
              Keyboard.press(KEY_F9);
              delay(delayRate);
              Keyboard.release(KEY_F9);
              delay(delayRate);
            }
            else if(Command == "F10"){
              Keyboard.press(KEY_F10);
              delay(delayRate);
              Keyboard.release(KEY_F10);
              delay(delayRate);
            }
            else if(Command == "F11"){
              Keyboard.press(KEY_F11);
              delay(delayRate);
              Keyboard.release(KEY_F11);
              delay(delayRate);
            }
            else if(Command == "F12"){
              Keyboard.press(KEY_F12);
              delay(delayRate);
              Keyboard.release(KEY_F12);
              delay(delayRate);
            }
            else if(Command == "ALTLC"){
              Keyboard.press(KEY_LEFT_ALT);
              delay(delayRate);
              Mouse.press();
              delay(delayRate);
              Keyboard.release(KEY_LEFT_ALT);
              Mouse.release();
              delay(delayRate);
            }
            else if(Command == "ALTRC"){
              Keyboard.press(KEY_LEFT_ALT);
              delay(delayRate);
              Mouse.press(MOUSE_RIGHT);
              delay(delayRate);
              Keyboard.release(KEY_LEFT_ALT);
              Mouse.release(MOUSE_RIGHT);
              delay(delayRate);
            }
            
            else if(Command == "DARD"){
              Keyboard.press(KEY_DOWN_ARROW);
              delay(1000);
              Keyboard.release(KEY_DOWN_ARROW);
            }
            else if(Command == "LBD"){
              Mouse.press();
            }
            else if(Command == "LBU"){
              Mouse.release();
            }
            else if(Command == "RBD"){
              Mouse.press(MOUSE_RIGHT);
            }
            else if(Command == "RBU"){
              Mouse.release(MOUSE_RIGHT);
            }
            else if(Command == "LCD"){
              Keyboard.press(KEY_LEFT_CTRL);
            }
            else if(Command == "LCU"){
              Keyboard.release(KEY_LEFT_CTRL);
            }
            else if(Command =="ESC"){
              Keyboard.press(KEY_ESC);
              delay(delayRate);
              Keyboard.release(KEY_ESC);

              
            }
            else if(Command =="LRD"){
              Keyboard.press(KEY_LEFT_ALT);
            }
            else if(Command =="LRU"){
              Keyboard.release(KEY_LEFT_ALT);
            }
            else if(Command =="TAB"){
              Keyboard.press(KEY_TAB);
              delay(delayRate);
              Keyboard.release(KEY_TAB);
            }
            

            return;
          }
          

          Command.concat(Temp);
        }
        break;

        
      default:
        Keyboard.press(Temp);
        delay(delayRate);
        Keyboard.release(Temp);
        delay(delayRate);
        break;
      
    }

      
  }




  

}
