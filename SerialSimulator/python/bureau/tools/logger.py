'''
=============================================================================

    AWS Bureau Serial Sensor Simulator LOGGER - Setup and Return python logger instance to capture simulator logs and any raised errors.
    Python interpreter Version = 3.9.12
    Ver |   Date        |   Author      |   Comment
    001 |   May 2023    |   A.Galbraith |   Initial version for Release 1 Testing
=============================================================================

   
    Called using: getlogger(<test result log file>)
    E.g. logger = bureau.tools.logger.getlogger("logs/test_results_log.csv")
    Has the following features:
        - Intiates  logger to capture simulator output and any  raised errors
        - Sets the logger level to DEBUG
        - Sets the logging format to CSV
        - Sets on screen and file handlers
        - wites to the log for debug and error levels
        - gets the logger object.

        
    #TODO LIST
    The following features are yet to be coded
    01 -  function to set log levels
    02 - update log function to allow writing of more log levels
=============================================================================
'''
#imports python logging and time modules
import logging
import time


#GLOBALS
DEBUG_LOG_NAME = "SensorSim"

def setuplogger(logfilename = time.strftime("%Y-%m-%d")+"_test_results_log.csv"):
    """
    This function sets up and returns a logger object 
    
    @param logfilename: string -> log path and filename
    @return object -> Logging object
    """
    #Set format for logger file
    formatter_file = logging.Formatter('%(asctime)s.%(msecs)03d , %(name)s , %(levelname)s , %(message)s', datefmt="%Y-%m-%d %H:%M:%S")
    formatter_screen = logging.Formatter('%(asctime)s.%(msecs)03d - %(name)s - %(levelname)s - %(message)s', datefmt="%Y-%m-%d %H:%M:%S")
 
    # Get logger with same name to be used as common logger
    logger = logging.getLogger(DEBUG_LOG_NAME)
    #Set File handler
    hdlr_file = logging.FileHandler(logfilename)
    hdlr_file.setFormatter(formatter_file)
    logger.addHandler(hdlr_file)

    #set logging level to DEBUG
    logger.setLevel(logging.DEBUG)

    #add screen handler
    hdlr_screen = logging.StreamHandler()
    hdlr_screen.setFormatter(formatter_screen)
    logger.addHandler(hdlr_screen)
    return logger

def getlogger():
    """
    This function gets the logger object to be used for logging
    
    @return object -> Logging object
    """
    
    # Get logger with same name to be used as common logger
    return logging.getLogger(DEBUG_LOG_NAME)

def debug(log_message = ""):
    """
    This function gets the logger object to be used for logging and logs a message in DEBUG
    
    @return object -> Nothing
    """
    # Get logger with same name to be used as common logger and write a debug message
    logging.getLogger(DEBUG_LOG_NAME).debug(log_message) 

    return 

def error(log_message = ""):
    """
    This function gets the logger object to be used for logging and logs a message in DEBUG
    
    @return object -> Nothing
    """
    
    # Get logger with same name to be used as common logger and write a error message
    logging.getLogger(DEBUG_LOG_NAME).error(log_message) 

    return 