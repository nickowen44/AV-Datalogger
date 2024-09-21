'''
=============================================================================

    AWS Bureau Serial Sensor Simulator SERIAL COMMS - Setup SubClass of the Python Threading module.
    Python interpreter Version = 3.9.12
    Ver |   Date        |   Author      |   Comment
    001 |   May 2023    |   A.Galbraith |   Initial version for Release 1 Testing
=============================================================================

   
    Called using: bureauThread(self, threadID, sensor)
    E.g. ser = bureauSerial(self, 1, sensor)
    The class is sent a Bureau Sensor object and commences the simulation on a seprate thread.
    Has the following features:
        - Intiates  threading on the sensor object.
                
    #TODO LIST
    The following features are yet to be coded
    01 - Exceptioning handling  
=============================================================================
'''
#imports bureau logger and python threading modules
import bureau.tools.logger
import threading, time

class bureauThread (threading.Thread):
   def __init__(self, threadID, sensor):
      threading.Thread.__init__(self)
      self.threadID = threadID
      self.sensor = sensor
   def run(self):
      bureau.tools.logger.debug(f"Thread for Sensor {self.sensor.name} START")

      self.sensor.report()
      bureau.tools.logger.debug(f"Thread for Sensor {self.sensor.name} END")
