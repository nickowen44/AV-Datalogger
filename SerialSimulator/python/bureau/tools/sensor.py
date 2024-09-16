'''
=============================================================================

    AWS Bureau Serial Sensor Simulator SERIAL COMMS - Setup Sensor Simulator and send messages, Log and return any raised errors.
    Python interpreter Version = 3.9.12
    Ver |   Date        |   Author      |   Comment
    001 |   May 2023    |   A.Galbraith |   Initial version for Release 1 Testing
    002 |   Nov 2023    |   A.Galbraith |   Updated with additional features for Release 1 Testing
=============================================================================

   

    Has the following features:
        - Intiates a sensor object to generate a raw text input based on input files
        - Intiates  serial to write output to specificed com port any  raised errors
        - writes to the serial port based on frequency and settings provided.


        
    #TODO LIST
    The following features are yet to be coded
    01 -  
=============================================================================
'''
#imports bureau logger and python serial modules
import bureau.tools.logger
import bureau.tools.serial, time
import ast

class bureauSensor :
    def __init__(self, name, message_file, header_file, footer_file, message_frequency, serial_port, end_time, start_time=time.time(), loop_message_file=True, dryrun=False, multi_trans=True, transmission_delay=0, serial_baud = 9600, serial_data_bits = 8, serial_stop_bits = 1, serial_parity='NONE', numberMessageLines = 1, messageLineTermFile = None) -> None:
        # Setup globals and Attempt to open the serial port and message related files
        # name is the name of sensor, message_file txt file location including path with single sensor sample a each line, header_file/footer_file txt file location including path with header or footer on first line
        # message_frequency reporting frequency of sensor in seconds, serial_port - COM port of simulator on the laptop, end_time is the future time for simulation to end as seconds past epoch, loop_message_file toggle eof behavour i.e restart at top of file  
        #Called using: sensor1 = bureauSensor(name, message_file, header_file, footer_file, message_frequency, serial_port, <serial_baud, serial_data_bits, serial_parity, serial_stop_bits, transmission_delay>)
        #E.g. ser = bureauSensor(self, "Barometer-PTB330", "PTB330-RAW-SAMPLES.TXT", "PTB330-HEADER.TXT", "PTB330-FOOTER.TXT", 15, "COM6", 1685061262.2881634)
        try:
            self.name = name
            #attempt to open header and footer files read first line and set as header and footer
            
            self.header = bytes(ast.literal_eval(f'"{open(header_file, "r").read()}"'),'UTF-8')
            self.footer = bytes(ast.literal_eval(f'"{open(footer_file, "r").read()}"'),'UTF-8')
            #attempt to open message file
            self.messageFile = open(message_file, "r")
            self.messageLineTerm = messageLineTermFile
            if self.messageLineTerm is not None :
                self.messageLineTerm = bytes(ast.literal_eval(f'"{open(messageLineTermFile, "r").read()}"'),'UTF-8')
                 
            self.numberMessageLines = numberMessageLines

            #setup other parameters
            self.sampleFrequency = message_frequency
            self.messageCount = 0
            self.loopMessage = loop_message_file
            self.endTime = end_time
            self.startTime = start_time
            self.dryrun = dryrun
            if serial_parity == 'EVEN':
                serial_parity_setting = bureau.tools.serial.serial.PARITY_EVEN
            elif serial_parity == 'ODD':
                serial_parity_setting = bureau.tools.serial.serial.PARITY_ODD
            elif serial_parity == 'MARK':
                serial_parity_setting = bureau.tools.serial.serial.PARITY_MARK
            elif serial_parity == 'SPACE':
                serial_parity_setting = bureau.tools.serial.serial.PARITY_SPACE
            else :
                serial_parity_setting = bureau.tools.serial.serial.PARITY_NONE

            #Log files opened ok
            bureau.tools.logger.debug(f"Sensor {name} message file {message_file} opened ok with header {header_file} footer {footer_file} and msg term file {messageLineTermFile}")
        except:
             # failure
            bureau.tools.logger.error(f"Sensor {name}  message, header or footer files do not exist or cannot be opened {message_file} with header {header_file} footer {footer_file} and msg term file {messageLineTermFile}")
        
        # attempt to set the serial connection settings
        self.connection = bureau.tools.serial.bureauSerial(serial_port, dryrun, multi_trans, transmission_delay, serial_baud, serial_data_bits, serial_stop_bits, serial_parity_setting)

    pass

    def report(self):
            #start sensor reporting based on start time
            if self.startTime > time.time():
                time.sleep(self.startTime-time.time())
            
            #loop while current time is less than simulation end time
            while (time.time() < self.endTime):
                
               #Reset the message
                message = b''
                lineCount = 0

                while lineCount < self.numberMessageLines:
                    #check if file has header footer already or use default
                    if self.messageLineTerm is not None :
                        message += bytes(ast.literal_eval(f'"{self.messageFile.readline().rstrip()}"'),'UTF-8')+self.messageLineTerm
                    else :
                        message += bytes(ast.literal_eval(f'"{self.messageFile.readline().rstrip()}"'),'UTF-8')

                    #increment line counter
                    lineCount += 1


                #if message is at end either loop or stop
                if len(message) == 0 :
                      if self.loopMessage == True :
                           self.messageFile.seek(0,0)
                      else :
                           return
                else :
                    #increment message counter
                    self.messageCount += 1
                    #send message
                    sentmsg = self.connection.send(self.header, message, self.footer)
                    if len(sentmsg) > 0:
                        bureau.tools.logger.debug(f"Sensor {self.name}, {self.messageCount}, '{sentmsg}', SENT OK")
                    #wait until next sample
                    time.sleep(self.sampleFrequency)
            return 1

