'''
=============================================================================

    AWS Bureau Serial Sensor Simulator SERIAL COMMS - RUN Sensor Simulators, Log and return any raised errors.
    Python interpreter Version = 3.9.12
    Ver |   Date        |   Author      |   Comment
    001 |   May 2023    |   A.Galbraith |   Initial version for Release 1 Testing
    002 |   Nov 2023    |   A.Galbraith |   Updated with additional features for Release 1 Testing
    003 |   Nov 2023    |   A.Galbraith |   Version 1 release.
=============================================================================

   

    Has the following features:
        - Intiates one or many sensor object to simulate sensor input using python threading module
        - writes messages to serial port and logs to python logger
        - All related settings are read from a SETTINGS.conf file which allows setting of Sensor, serial config
        - Allows overall duration to be set via command line
        - Allows dry run from command line which skips opening and writing to a serial port.
        - Exception handling for the serial device removal, termination of single thread vs termination of program
        - Allows for for multi line messages and terminiation to be defined
        - handle log filename and location


        
    #TODO LIST
    The following features are yet to be coded
    01 
    02  
	03 
=============================================================================
'''
#imports bureau logger and python serial modules
import bureau.tools.logger, bureau.tools.thread
import bureau.tools.sensor, time
import os, sys
SETTINGS_FILE = 'SETTINGS.conf'
VER = "V1.1"
VER_DATE = "NOV-2023"


def loadSettings(settings_file):
    """
    This function parses the settings file and returns the settings as a nest dictionary 
    
    @return Dictionary
    """
    loadedSettings = {}
    sensorCount = 0
    #open file 
    f = open(settings_file, "r")
    #loop through lines, split to new sensor on header tag and ignore comment lines starting with #
    #Assign settings to dictionary
    for line in f:
        line = line.strip()
        if line.startswith("<HEADER"):
            sensorCount = sensorCount + 1
            
        else:
            if not line.startswith("#") and not sensorCount==0:
                setting = line.split("=")
                loadedSettings.setdefault(sensorCount, {})[setting[0]] = setting[1]

    #return the settings nested dictionary
    f.close
    return loadedSettings

#
#   MAIN FUNCTION
#


#System command arguments    Required: duration for the simulation in mins Optional: [log file path and name.csv] [-dryrun] option for simulation without serial ports connected 
if(len(sys.argv)<2) | ('-h' in sys.argv):
    #not enough parameters passed, or the help option
    sys.stdout.write('AWS Serial SIM v1.0 -h usage '+os.path.basename(sys.argv[0]) + ' <duration in mins> [-logfile "log file path and name.csv"] [-dryrun]')
    sys.exit(1)
else:
    #duration for the simulator
    duration = float(sys.argv[1])

if('-dryrun' in sys.argv):
    #if dryrun option passed then set flag to true
    dryrun=True
else:
    dryrun=False

#setup the logger    
if('-logfile' in sys.argv):
    #if logfile option passed then read in filename
    logfile=sys.argv[sys.argv.index('-logfile')+1]
    bureau.tools.logger.setuplogger(logfile)
else:
    bureau.tools.logger.setuplogger()


bureau.tools.logger.debug(f"---AWS Bureau Serial Sensor Simulator VER={VER} VERDATE={VER_DATE}---")

#Use current file as starting point for paths
config_path = os.path.dirname(os.path.realpath(__file__))

#Sensor Simulator Threads
senseThreads = []

# end time from command line
endTime = time.time()+(duration*60)


#Load settings from file
loadedSettings = loadSettings(os.path.join(config_path,SETTINGS_FILE))
if (len(loadedSettings)<1):
    bureau.tools.logger.error(f"Sensor Simulator settings not correctly loaded from {SETTINGS_FILE} ")
else:
    #LOOP through the sensors configured in the SETTINGS file
    for sensorSettings in loadedSettings:
        if loadedSettings[sensorSettings]['STATUS'] == 'ON':

            #Append to path if not absolute
            if loadedSettings[sensorSettings]['ABSO_PATH'] == 'YES':
                dirPath = loadedSettings[sensorSettings]['MSG_PATH']
            else:
                dirPath = os.path.join(config_path,loadedSettings[sensorSettings]['MSG_PATH'])
            
            #Set start time is not set then set it to now otherwise set epoch start time to today date + start time
            if loadedSettings[sensorSettings]['START_TIME']=='0':
                startTime=time.time()
            else:
                startTime = time.mktime(time.strptime(time.strftime('%d-%m-%Y ')+loadedSettings[sensorSettings]['START_TIME'],'%d-%m-%Y %H:%M:%S'))
            
            #Covert setting of Loop file to Bool
            if loadedSettings[sensorSettings]['LOOP_MSG_FILE'] == 'YES':
                loopMsgFile = True
            else:
                loopMsgFile = False 

            #Covert setting of multi transaction to Bool
            if loadedSettings[sensorSettings]['MULTI_TRANS'] == 'YES':
                multiTrans = True
            else:
                multiTrans = False

            #Covert setting of multi line to Bool
            if loadedSettings[sensorSettings]['LINE_TERM_FILE'] == 'NO':
                LineTermFile = None
            else:
                LineTermFile = os.path.join(dirPath,loadedSettings[sensorSettings]['LINE_TERM_FILE'])               

            #create sensor object and assign to a thread    
            sensor = bureau.tools.sensor.bureauSensor(loadedSettings[sensorSettings]['SENSOR_NAME'], os.path.join(dirPath,loadedSettings[sensorSettings]['MESSAGE_FILE']), os.path.join(
                dirPath,loadedSettings[sensorSettings]['HEADER_FILE']), os.path.join(dirPath,loadedSettings[sensorSettings]['FOOTER_FILE']), int(loadedSettings[sensorSettings]['MSG_FREQUENCY']), loadedSettings[sensorSettings]['COM_PORT'], endTime,
                startTime,loopMsgFile,dryrun,multiTrans,float(loadedSettings[sensorSettings]['SERIAL_TRANSMISSION_DELAY']),int(loadedSettings[sensorSettings]['SERIAL_BAUD']),int(loadedSettings[sensorSettings]['SERIAL_DATA_BITS']),
                int(loadedSettings[sensorSettings]['SERIAL_STOP_BITS']),loadedSettings[sensorSettings]['SERIAL_PARITY'],int(loadedSettings[sensorSettings]['NUM_MSG_LINES']),LineTermFile)
            thread = bureau.tools.thread.bureauThread(sensorSettings,sensor)
            
            senseThreads.append(thread)
    #Iterates over all the sensor threads        
    for senseThread in senseThreads:  
        #start each thread
        senseThread.start() 

    #Handle a Keyboard Interrupt of Clt-C
    import threading
    try:
       while (threading.active_count()>1):
        time.sleep(.1)
    except KeyboardInterrupt:
        bureau.tools.logger.error(f"Shutting down {threading.active_count()-1} thread(s) please wait.....")   
        for senseThread in senseThreads:  
            #start each thread
            senseThread.sensor.endTime = 0  


