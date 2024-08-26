'''
=============================================================================

    AWS Bureau Serial Sensor Simulator SERIAL COMMS - Setup Serial connection and write to specified serial port Log and return any raised errors.
    Python interpreter Version = 3.9.12
    Ver |   Date        |   Author      |   Comment
    001 |   May 2023    |   A.Galbraith |   Initial version for Release 1 Testing
    002 |   Nov 2023    |   A.Galbraith |   Updated with additional features for Release 1 Testing    
=============================================================================

   
    Called using: bureauSerial(self, serial_port, <dryrun, transmission_delay, serial_baud, serial_data_bits, serial_stop_bits, serial_parity>)
    E.g. ser = bureauSerial(self, "COM6", 0, 9600)
    Has the following features:
        - Intiates  serial to write output to specificed com port any  raised errors
        - Sets the serial object
        - writes to the serial port


        
    #TODO LIST
    The following features are yet to be coded
    01 -  
=============================================================================
'''
#imports bureau logger and python serial modules
import bureau.tools.logger
import serial, time


class bureauSerial :
    def __init__(self, serial_port, dryrun=False, multi_trans=True, transmission_delay=0, serial_baud = 9600, serial_data_bits = 8, serial_stop_bits = 1, serial_parity = serial.PARITY_NONE) -> None:
        # Attempt to open the serial port
        try:
            # set the serial connection settings
            self.connection = serial.Serial()
            self.connection.port = serial_port
            self.connection.baudrate = serial_baud
            self.connection.bytesize = serial_data_bits
            self.connection.parity = serial_parity
            self.connection.stopbits = serial_stop_bits
            # No flow control
            self.connection.xonxoff=False
            self.connection.rtscts=False
            self.connection.dsrdtr=False
            # write timeout 5 seconds
            self.connection.write_timeout = 5
            #set transmission delay during message
            self.transmissionDelay = transmission_delay
            self.multiTrans = multi_trans
            #set flag for dry run i.e. no serial ports opened or messages sent
            self.dryrun = dryrun
            if not self.dryrun:
                # open port and flush buffers
                self.connection.open()
                self.connection.flushInput()
                self.connection.flushOutput()

            # Log serial status details
            bureau.tools.logger.debug(f"serial port {serial_port} opened ok at {serial_baud}/ {serial_data_bits} {serial_parity} {serial_stop_bits}")
        except:
             # failure to open the port exit the thread
            bureau.tools.logger.error(f" could not open serial port {serial_port}, check the config settings")
            import sys
            sys.exit(1)
        pass

    def send(self, header=b'', message=b'', footer=b''):
            #new message
            
            #create serial message as multi-transmission or single transmission
            if self.multiTrans:
                serial_msg = header + message
            else:
                serial_msg = header + message + footer
            
           
            try:
                if not self.dryrun:
                    #write formatted byte array to serial port
                    self.connection.write(serial_msg)
                
                if self.multiTrans:
                    #Only log if expecting a transmission delay
                    if self.transmissionDelay > 0 :
                        bureau.tools.logger.debug(f" Header + Message '{serial_msg}' on PORT {self.connection.port} SENT OK")

                    # Send the footer after a delay
                    time.sleep(self.transmissionDelay)
                    serial_msg2 = footer
                    
                    #only send message if not a dry run
                    if not self.dryrun:
                        self.connection.write(serial_msg2)
                    serial_msg=serial_msg+serial_msg2
                    #Only log if expecting a transmission delay
                    if self.transmissionDelay > 0 :
                        bureau.tools.logger.debug(f" Delayed Footer '{serial_msg2}' on PORT {self.connection.port} SENT OK")
            except:
                 #write failure and exit the thread
                 bureau.tools.logger.error(f" Error writing serial message check port connection {self.connection.port}")
                 import sys
                 sys.exit(1)
                 

            return serial_msg

