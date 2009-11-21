///////////////////////////////////////////////////////////////////////////////
//
//  SYSTEM:          Polar HRMCom
//
//  UNIT FILE NAME:  HRMCOM.H
//
//  MODULE:			 HRMCOM.DLL
//
//  AUTHORS:         MEr / Polar Electro Oy
//
//  VERSION:         1.2
//
//  DATE:            20.03.2001
//
//  ABSTRACT:        Main header file for Polar HRMCOM.DLL function library file.
//
//  REMARKS:
//
//  COPYRIGHT (C) 2001 BY POLAR ELECTRO OY
//
///////////////////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////////////////
//
//	BOOLEAN VARIABLES
//	-----------------
//
//	Function library uses boolean variables as following:
//
//		TRUE	equals	1
//		FALSE	equals	0
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	FUNCTION CALLS
//	--------------
//
//	Definition of function calls:
//
//		__declspec (dllexport) BOOL CALLBACK fnHRMCom...
//
//	can be replaced by
//
//		BOOL fnHRMCom...
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	INITIALIZING DATA STRUCTURES
//	----------------------------
//
//	It is recommended to initialize all data structure always before usage by using
//	for example the functions memset or ZeroMemory. Uninitialized data structure
//	passed to functions may cause errors in communication.
//
///////////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
    extern "C" {
#endif

///////////////////////////////////////////////////////////////////////////////
//
// POLAR CHARACTER SET
// -------------------
//
//	The following characters are valid at text strings in settings:
//	- Capital letters:	ABCDEFGHIJKLMNOPQRSTUVWXYZ
//	- Small letters:	abcdefghijklmnopqrstuvwxyz
//	- Numbers:			0123456789
//	- Special chars:	-%/()*+.:? and space
//
//	Unrecognized characters will be converted automatically to spaces.
//	The text strings have to be ended by zero character (NULL).
//	Strings can be checked before sending by using function fnHRMCom_CheckPolarCharStringEx
//
__declspec (dllexport) BOOL CALLBACK fnHRMCom_CheckPolarCharString (LPTSTR);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_CheckPolarCharStringEx (LPTSTR, int);
//
//	Use function fnHRMCom_CheckPolarCharStringEx with zero as second parameter to check string.
//	The second parameter is reserved for future HR monitor models, character set may change.
//	Function fnHRMCom_CheckPolarCharString is available for compatible cases.
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	DATE FORMAT
//	-----------
//
//	Date values are processed in yyyymmdd format:
//
//		yyyy	year	4 digits
//		mm		month	2 digits
//		dd		day		2 digits
//
//	For example:	August 2nd 2000		=> 20000802
//					December 24th 2003	=> 20031224
//
//	NOTE: Leading zero with days and months is always obligatory.
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	ERROR CHECKING
//	--------------
//
//	All input parameters will be checked before sending to heart rate monitor.
//	If any erratic values are determined, function call returns FALSE and does not
//	continue sending data to monitor. The latest error code can be checked by function:
//	iError = fnHRMCom_GetErrorCode ();
//
__declspec (dllexport) int CALLBACK fnHRMCom_GetErrorCode			(void);
//
//	## UNDER CONSTRUCTION ## //
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	POLAR HR MONITOR TYPES
//	----------------------
//
#define		HRM_S210						8			// Polar S210
#define		HRM_S410						9			// Polar S410
#define		HRM_S510						10			// Polar S510
#define		HRM_S520						6			// Polar S520
#define		HRM_S610						11			// Polar S610, S610i
#define		HRM_S710						12			// Polar S710, S710i, S720i
#define		HRM_S810						13			// Polar S810, S810i
#define		HRM_E200						14			// Polar E200
#define		HRM_E600						15			// Polar E600
#define		HRM_S120						16			// Polar S120
#define		HRM_S150						17			// Polar S150
#define		HRM_E30							18			// Polar E30

#define		HRM_AXN300						19			// Polar AXN300
#define		HRM_AXN500						20			// Polar AXN500
#define		HRM_AXN700						21			// Polar AXN700

#define		HRM_S625						22			// Polar S625X
#define		HRM_S725						23			// Polar S725

#define		HRM_F4							24			// Polar F4
#define		HRM_F6							25			// Polar F6, use this also to F7
#define		HRM_F11							26			// Polar F11
#define		HRM_F55							27			// Polar F55

#define		HRM_RS100						28			// Polar RS100
#define		HRM_CS100						29			// Polar CS100
#define		HRM_RS200						30			// Polar RS200
#define		HRM_CS200						31			// Polar CS200
#define		HRM_CS300						32			// Polar CS300
#define		HRM_CS400						33			// Polar CS400
#define		HRM_CS600X						34			// Polar CS600X
#define		HRM_CS600						35			// Polar CS600

#define		HRM_RS400						36			// Polar RS400
#define		HRM_RS800						37			// Polar RS800
#define		HRM_RS800CX						38			// Polar RS800CX

#define		HRM_E40							40			// Polar E40
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	MONITOR CONNECTION METHODS
//	--------------------------
//
#define		HRMCOM_CONNECTION_UPLINK		0
#define		HRMCOM_CONNECTION_IR			1
//
//	Polar UpLink technology can be used only for transferring settings from computer
//	to Polar S/AXN series monitor (one-way). "Read" functions can be called with
//	HRMCOM_CONNECTION_UPLINK as connection method, but method is automatically
//	changed to HRMCOM_CONNECTION_IR. Infrared connection is automatically two-way,
//	this means all the settings etc. can be read and written.
//
//	When infrared is used for writing or reading data to/from HR monitor, the communication
//	have to be started by using function fnHRMCom_StartIRCommunication. After calling this
//	function, all the other reading and writing functions can be used normally. To end
//	infrared communication, call function fnHRMCom_EndIRCommunication.
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	POLAR UPLINK WAVE FILES
//
//	By default wave file (random name to Temp folder) will be created, played and deleted.
//	Wave file will be created automatically to Temp folder defined at the Windows system.
//	The new wave file will be automatically named as HRMxxx.WAV, where xxx is a random number.
//	The playing of wav file do not allow cancelling.
//
///////////////////////////////////////////////////////////////////////////////


////////////////////////
////////////////////////
//
//	LIBRARY VERSION DATA
//
////////////////////////
////////////////////////

// Get hrmcom library file version
__declspec (dllexport) int CALLBACK fnHRMCom_GetLibraryVersion		(void);

// Version 1.00 will be returned as 100

////////////////////////
////////////////////////
//
//	GENERAL	SETTINGS DATA
//
////////////////////////
////////////////////////

//	The following data structure will be used with the most of the functions to give general information
//	about communication, for example are we using Polar UpLink or Infrared connection.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iConnection;		// Connection method: HRMCOM_CONNECTION_UPLINK or HRMCOM_CONNECTION_IR
								// NOTE: Polar UpLink connection can be used only for writing information to HR monitor.

	int		iMonitorID;			// Unique monitor ID, 0 = message to all monitors
								// Monitor will accept the messages if monitor id to send is same as already
								// set by User settings or if message was meant for all monitors available.
								// Other ID numbers used mainly with IR communication

	TCHAR	szWaveFile[MAX_PATH];// Wave file name, use NULL to create random file name
								// to Temp folder (MAX_PATH = 260)

								// EXCLUSION FLAGS
								// ---------------
	BOOL	bNoCreateWave;		// Don't create wave file at all, this allows testing of values in the data structure
	BOOL	bNoPlayWave;		// Don't play created Polar UpLink WAV file
	BOOL	bNoDeleteWave;		// Don't delete created Polar UpLink WAV file after it have been played

								// DATA FILE MANAGEMENT
								// --------------------
	BOOL	bLoadFromDataFile;	// Load information from binary data file, file name have to be at szWaveFile
								// If trying to load the data file with not the same data as data structure
								// specified in call, all the calling functions will return FALSE
								// When file will be loaded, other actions (create, play, delete wave) are not done.
								// If loaded file includes incorrect data, default values will be set automatically.

	BOOL	bSaveAsDataFile;	// Save information to binary data file, file name have to be at szWaveFile
								// When file will be saved, other actions (create, play, delete wave) are not done.

								// CONNECTION DIALOG
								// -----------------
	BOOL	bConnectionDlg;		// Usage of connection dialog to user. Dialog is shown always with IR connection.
	HWND	hOwnerWnd;			// Owner window handle to connection dialog
								// If connection dialog has been selected to be shown, owner window
								// handle have to be specified. If not, dialog won't be shown and connection fails.
								// If connection dialog is not in use, this parameter is ingnored.

	TCHAR	szDlgMsg[50];		// Connection dialog message to user, max 50 characters
								// If message text is not specified, default English texts will be used
								// If connection dialog is not in use, this parameter is ingnored.

								// MISC PARAMETERS
								// ---------------
	BOOL	bFixErrors;			// Errors in settings can be fixed automatically and error messages
								// are not returned in normal cases.

	int		iParam;				// Parameter reserved for future usage, use zero
	long	lParam;				// Parameter reserved for future usage, use zero

} POLAR_SSET_GENERAL;


////////////////////////
////////////////////////
//
// SET HR MONITOR TO WATCH MODE
//
////////////////////////
////////////////////////

//	Sets monitor to watch mode, monitor do not accept other
//	messages, until it has been switched back to Connect mode.

__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendMonitorToWatchMode (POLAR_SSET_GENERAL*);

//	If sending was succesfull, function returns TRUE, otherwise FALSE



////////////////////////
////////////////////////
//
// FACTORY DEFAULTS
//
////////////////////////
////////////////////////

//	Sets monitor factory defaults, resets all monitor data including EEPROM memory. Use very carefully!!!
//	Setting factory defaults is not meant for normal software usage, only for service software products.
//	When settings factory defaults, confirmation of the operation should be asked always.

__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendFactoryDefaultCommand (POLAR_SSET_GENERAL*);



////////////////////////
////////////////////////
//
// DELETING EXERCISE FILES FROM HR MONITOR
//
////////////////////////
////////////////////////

//	Exercise files can be deleted by using the following function call. Files can be deleted
//	from Polar S610, S610i, S625X, S710, S710i, S720i, S725, S810, S810i and E600 HR monitors.
//
//	NOTE: The first version of Polar S610 (DataVersion = 1) can't handle deleting one exercise
//	file correctly, all exercise files can still be deleted.
//
//	NOTE: The first version of Polar S810 (DataVersion = 3) can't handle deleting all exercise
//	files correctly, one exercise file can still be deleted.
//
//	Check monitor type and data version before sending file delete message to monitor!
//
//	Parameters:		int		iExerciseIndex		Index of exercise to be deleted (0 ... n)
//												-1 = Delete all files
//												-2 = Date and starting time given in next 3 parameters
//					int		iDate				Exercise date in yyyymmdd format
//					int		iStartingTime		Exercise starting time in full seconds
//					BOOL	b12hMode			Usage of 12 hour mode (for exercise starting time)

__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendFileDeleteCommand (POLAR_SSET_GENERAL*, int, int, int, BOOL);



///////////////////////
////////////////////////
//
// INFRARED COMMUNICATION FUNCTIONS
//
////////////////////////
////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	fnHRMCom_ResetIRCommunication
//	-----------------------------
//	Call in the startup of software to reset all the communication parameters.
//
//	Parameters:
//	int iParam		Reserved in future usage, use 0 (zero).
//
//	Return value:
//	TRUE			- Resetting made succesfully
//	FALSE			- Resetting was not made because of communication is already running.
//
///////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) BOOL CALLBACK fnHRMCom_ResetIRCommunication	(int);



///////////////////////////////////////////////////////////////////////////////
//
//	fnHRMCom_StartIRCommunication
//	-----------------------------
//	Call to open communication port and start IR communication.
//
//	Parameters:
//	int iParam		Parameter for connection settings (when used multiple parameters, use OR)
//
#define HRMCOM_PARAM_SERIALPORT			0	// Use serial communication port (COM1: - COM9:)
#define HRMCOM_PARAM_INTERNALIR			1	// Use internal IR port (Win95 only)
#define HRMCOM_PARAM_KEEPCONNECT		2	// Keep monitor in Connect mode during connection
#define	HRMCOM_PARAM_FILTERHRDATA		4	// Reserved for future usage

#define	HRMCOM_PARAM_DIRECT_USB		   16	// Direct USB port usage
#define HRMCOM_PARAM_IRDA			   32	// IrDA connection usage
#define	HRMCOM_PARAM_DUMPFRAMES		   64	// Dump frames to c:\frames.txt or c:\all.txt text files
											// Dumping can be used for internal data error detection

#define HRMCOM_PARAM_ONLINE			  128	// Online recording mode (Polar S810 and S810i only)

//	LPTSTR tcPort	Communication port name, for example "COM1:" or "COM2:"
//					Remember to use to use colon : at the end of port name
//

__declspec (dllexport) BOOL CALLBACK fnHRMCom_StartIRCommunication	(int, LPTSTR);

//	Return value:
//	BOOL bStartOK
//	 TRUE			- Starting of communication made succesfully
//	 FALSE			- Problems encountered, check the following possible errors:
//						* Communication has already been started and it is running
//						* Communication port already reserved for some other device
//						* Maybe call was made from 16-bit program. A 32-bit DLL cannot
//						  create an additional thread when that DLL is being called by
//						  a 16-bit program.
//
///////////////////////////////////////////////////////////////////////////////




///////////////////////////////////////////////////////////////////////////////
//
//	fnHRMCom_ReadMonitorInfo
//	-----------------------------
//	Call to read basic monitor information.
//

typedef struct
{
	int		iSize;					// Structure size for version control

	int		iMonitorInUse;			// HR monitor in use: HRM_RS400, HRM_RS800, HRM_S610, HRM_S710, etc.
	int		iDataVersion;			// HR monitor data version (for internal usage)

	int		iTotalFiles;			// Total count of all files inside HR monitor
	int		iFreeMemoryInBytes;		// Free memory inside HR monitor (in bytes)
	int		iTotalMemoryInBytes;	// Total memory inside HR monitor (in bytes)

	int		iLowBattery;			// Low battery indicator, see detailed information below

	BOOL	bHorseMode;				// Reserved for Equine usage
	int		iSpeedSensor;			// Speed measurement type: 0 = bike speed sensor / foot pod (other values reserved)

	int		iMonitorSubModel;		// HR Monitor sub model identifier (for internal usage):

									// HR Monitor		SubModel	Handled as
									// Polar S610		0x11		HRM_S610
									// Polar S610i		0x12		HRM_S610
									// Polar S625X		0x13		HRM_S625
									// Polar S710		0x22		HRM_S710
									// Polar S710i		0x23		HRM_S710
									// Polar S720i		0x24		HRM_S710
									// Polar S725		0x25		HRM_S725
									// Polar S725X		0x26		HRM_S625
									// Polar S810		0x33		HRM_S810
									// Polar S810i		0x34		HRM_S810

									// Note: Because of internal data similarity, all sub models within
									// one monitor type (iMonitorInUse) are handled in similar way.
} POLAR_SSET_MONITORINFO;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadMonitorInfo		(POLAR_SSET_GENERAL*, POLAR_SSET_MONITORINFO*);

// To get the real name of HR monitor type, use the following function:

__declspec (dllexport) LPTSTR fnHRMCom_GetPolarHRMonitorName (int, int);

//	Parameters:	int iMonitorInUse
//				int iMonitorSubModel

//	If iMonitorSubModel is -1, the function returns all submodels.
//	For example:	strcpy (tcHelp, fnHRMCom_GetPolarHRMonitorName (HRM_S610, -1));
//					=> tcHelp is now "Polar S610i / S610"
//	This special feature can be used to indicate monitors receiving Polar UpLink data.
//
//	Parameter iLowBattery can contain low battery information from several sources (depending on monitor type)
//	The low battery information of the sources are saved in iLowBattery and can be detected by using AND operator:

#define BATTERY_LOW_WRISTUNIT		1	// Low battery at Wrist unit
#define BATTERY_LOW_HR				2	// Low battery at Heart Rate transmitter (RS800, RS800CX, CS600)
#define BATTERY_LOW_SPEED			4	// Low battery at S3 Speed sensor (RS800, CS600) or bike speed sensor (RS800CX)
#define BATTERY_LOW_CADENCE			8	// Low battery at bike cadence sensor (RS800CX, CS600)
#define BATTERY_LOW_POWER			16	// Low battery at power output sensor (CS600)
#define BATTERY_LOW_GPS				32	// Low battery at G3 GPS sensor (RS800CX)
#define BATTERY_LOW_FOOTPOD			64	// Low battery at S3 Speed sensor (RS800CX)

//
///////////////////////////////////////////////////////////////////////////////




///////////////////////////////////////////////////////////////////////////////
//
//	fnHRMCom_EndIRCommunication
//	-----------------------------
//	Call to close communication port and end IR communication.
//
//	Parameters:
//	int iParam		- Reserved in future usage, use 0 (zero).
//
//	Return value:
//	BOOL bEndOK
//	 TRUE			- Ending of communication made succesfully
//	 FALSE			- Problems with ending of communication
//
///////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) BOOL CALLBACK fnHRMCom_EndIRCommunication	(int);



///////////////////////////////////////////////////////////////////////////////
//
//	Communication Texts
//	-------------------
//
//	Communication texts are shown with infrared communication process. By default
//	English texts for buttons and message texts are defined. If texts need localization,
//	the following functions can be used to change communication texts before calling
//	communication functions. The text at the end of the following defines shows the
//	default text string for each text item.
//
#define		HRMCOM_TEXT_CANCEL				0		// Cancel
#define		HRMCOM_TEXT_RETRY				1		// Retry
#define		HRMCOM_TEXT_READING				2		// Reading...
#define		HRMCOM_TEXT_NOANSWER			3		// No answer from HR Monitor
#define		HRMCOM_TEXT_ERRORS				4		// Errors with Connection
#define		HRMCOM_TEXT_STARTING			5		// Starting Connection...
#define		HRMCOM_TEXT_TITLE				6		// Infrared Connection
#define		HRMCOM_TEXT_WRITING				7		// Writing...
//
//	To set each communication text, call function fnHRMCom_SetComText.
//	For example this call will change internal text for informing user about
//	not getting any answers from HR monitor within answer time:
//
//	  static char	szText[30];
//	  ZeroMemory (&szText, sizeof (szText));
//	  strcpy (szText, "Ei vastausta sykemittarilta");
//	  fnHRMCom_SetComText (HRMCOM_TEXT_NOANSWER, szText);
//
__declspec (dllexport) BOOL  CALLBACK	fnHRMCom_SetComText		(int, LPTSTR);
//
//	NOTE: Use static of global string variable when setting communication texts.
//
//	When HRMCOM.DLL is initialized by starting software calling it, all the communication
//	texts are resetted automatically. To reset all communication texts back to English
//	default texts, the following reset function can be used:
//
__declspec (dllexport) void  CALLBACK	fnHRMCom_ResetComTexts	(void);
//
//	NOTE: Title text for Polar UpLink Communication is always "Polar UpLink".
//
///////////////////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////////////////
//
//	Basic information about exercise
//	--------------------------------
//
//	Structure POLAR_EXERCISEFILE includes basic information about the exercise data
//	file requested. For usage see the following function calls.
//
typedef struct
{
	int		iSize;					// Structure size for version control

	int		iTime;					// Start time of exercise in seconds
	int		iDate;					// Start date of exercise in yyyymmdd
	int		iDuration;				// Duration of exercise in seconds
	BOOL	bUSTimeMode;			// Usage of 12h time mode in exercise
	int		iSamplingRate;			// Sampling rate of exercise
	BOOL	bRRRecording;			// RR-recording
	BOOL	bDeleted;				// Exercise has been marked to be deleted

	BOOL	bSpeed;					// Speed sensor data available
	BOOL	bCadence;				// Cadence sensor data available
	BOOL	bAltitude;				// Altitude sensor data available
	BOOL	bPower;					// Power sensor data available
	BOOL    bLocation;				// Location data available.	

	BOOL	bInterval;				// Interval data available

	TCHAR	szName[11];				// Exercise set/profile name used in exercise
									// Max number of characters is 10 + ending zero

	int		iBike;					// Bike used at exercise (0 = off, 1 = bike1, 2 = bike2)
	int		iExerciseID;

} POLAR_EXERCISEFILE;

//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	Reading Exercises Data (RS products)
//	----------------------
//
//	Exercise file data will be read with one function call from Polar RS products
//	by using IR connection. The progress bar dialog will be created and updated
//	automatically. Communication port has to be opened before reading exercise.
//	To read one selected exercise to memory of DLL, use the following function:

__declspec (dllexport) int CALLBACK fnHRMCom_ReadExerciseFile (HWND, int, int);

//	Parameters:
//		HWND	hOwnerWnd	- Handle to owner window
//		int		iFileNumber	- Number of file to be read (see iTotalFiles at structure POLAR_SSET_MONITORINFO)
//		int		iParam		- Reserved parameter
//
//	File is automatically analysed after reading and all the exercise information can be read
//	by using the functions and defines shown in the following chapters.
//
//	If just basic information of exercise file is needed for example to check, whether
//	entire file have to be read at all, the following function call can be used:

__declspec (dllexport) BOOL CALLBACK fnHRMCom_GetExeFileInfoEx (HWND, int, POLAR_EXERCISEFILE*);

//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	Reading Exercises Data (S / AXN products)
//	----------------------
//
//	All exercises will be read with one function call from Polar S/ AXN products
//	by using IR connection. The progress bar dialog will be created and updated
//	automatically. Communication port has to be opened before reading exercises.
//	To read all exercises to memory of DLL, use the following function:
//
__declspec (dllexport) int CALLBACK fnHRMCom_ReadExercisesData		(HWND, BOOL);
//
//	Parameters:
//		HWND	hOwnerWnd	- Handle to owner window
//		BOOL	bOneWay		- Reserved flag
//
//	After all exercises have been read from HR monitor, the basic information
//	about each exercise can be read by using the following function fnHRMCom_GetExeFileInfo.
//
__declspec (dllexport) BOOL CALLBACK fnHRMCom_GetExeFileInfo		(int, POLAR_EXERCISEFILE*);
//
//	Parameters:
//	int iExercise				Parameter for specifying exercise of which the information will be retrieved
//	POLAR_EXERCISEFILE* pef*	Address to exercise file information data structure
//
//	Before reading detailed exercise information from HRMCOM.DLL's memory, each
//	exercise file have to be analyzed by using the following function:
//
__declspec (dllexport) BOOL CALLBACK	fnHRMCom_AnalyzeFile		(int, int);
//
//	Parameters:
//	int iExercise	Parameter for specifying exercise to be analysed
//	int iAction		Parameter reserved for future usage
//
//	After the succesfull analyzing, all the exercise information can be read
//	by using the functions and defines shown in the following chapters.
//
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	fnHRMCom_SaveExerciseHRM
//	-----------------------------
//	Save the analyzed exercise as HRM file.
//
//	Parameters:
//	HWND hDlg;		- Owner dialog handle
//	LPTSTR tcFile	- HRM file name with drive + folder information.
//					  If file is NULL, HRM file name will be asked.
//	int iParam		- Reserved in future usage, use 0 (zero).
//
//	Return value:
//	BOOL bSaveOK
//	 TRUE			- Saving completed succesfully
//	 FALSE			- Problems with saving file
//
///////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) BOOL CALLBACK	fnHRMCom_SaveExerciseHRM	(HWND, LPTSTR, int);




///////////////////////////////////////////////////////////
//
// HRM DATA OUTPUT FUNCTIONS
//
///////////////////////////////////////////////////////////

__declspec (dllexport) int  CALLBACK	fnHRMCom_GetRecParam		(int);		// returns recording parameters
__declspec (dllexport) BOOL CALLBACK	fnHRMCom_GetRecFlags		(int);		// returns recording flags

__declspec (dllexport) int  CALLBACK	fnHRMCom_GetNbrOfHRMSamples	(void);		// returns nbr. of samples
__declspec (dllexport) int  CALLBACK	fnHRMCom_GetHRMSamples		(int, int);	// returns HR/CC samples

__declspec (dllexport) int  CALLBACK	fnHRMCom_GetNbrOfIntTimes	(void);		// returns number of lap times
__declspec (dllexport) int  CALLBACK	fnHRMCom_GetIntTimeData		(int, int);	// returns lap time data

__declspec (dllexport) int CALLBACK		fnHRMCom_GetNbrOfSwapTimes	(void);		// returns number of HR limit swaps
__declspec (dllexport) int  CALLBACK	fnHRMCom_GetLimitSwapData	(int, int);	// returns limit swap data

///////////////////////////////////////////////////////////
//
// HRM DATA FLAGS
//
//	Get these parameters by using function: fnHRMCom_GetRecFlags
//
///////////////////////////////////////////////////////////

#define		FLAG_CYCLO_DATA                 3				// TRUE, cycling data
#define		FLAG_3LIMITS_IN_USE	            6				// three HR limits has been used
#define		FLAG_SPEED_DATA	                8				// file has speed data
#define		FLAG_ALT_DATA                   9				// file has altitude data
#define		FLAG_CAD_DATA                   10				// file has cadence data
#define		FLAG_POWER_DATA	                11				// file has power data
#define		FLAG_INTERVAL_DATA              12				// file has interval data
#define		FLAG_LAP_DATA                   13				// file has lap data
#define		FLAG_LIMSWAP_DATA               14				// file has limit swap data
#define		FLAG_POWER_BALANCE              18				// file has LR balance data
#define		FLAG_POWER_INDEX                19				// file has pedalling index data
#define		FLAG_AIRPR						20				// file has air pressure data
#define		FLAG_HEARTRATE					21				// file has heart rate data
#define		FLAG_LOCATION_DATA				23				// GPS location data available (RS800CX)


///////////////////////////////////////////////////////////
//
// HRM DATA GENERAL RECORDING INFORMATION
//
//	Get these parameters by using function: fnHRMCom_GetRecParam
//	Note: Some of the values are monitor dependent, all monitors do not support all values
//
///////////////////////////////////////////////////////////

#define		REC_AM_PM						1				// 0 = AM, 1 = PM
#define		REC_MONITOR_TYPE				5				// Polar Product type
#define		REC_EURO_US_UNITS				8				// 0 = Euro, 1 = US
#define		REC_START_DATE					9				// Exercise start date in yyyymmdd format
#define		REC_START_TIME					10				// Exercise start time hh:mm:ss.s/10 in 1/10 of seconds
#define		REC_REC_LENGTH					11				// Duration on exercise (in ms)
#define		REC_SAMPLING_RATE				12				// Recording rate
#define		REC_UPPER_LIMIT_1				13				// 0 - 250 bpm
#define		REC_LOWER_LIMIT_1				14				// 0 - 250 bpm
#define		REC_UPPER_LIMIT_2				15				// 0 - 250 bpm
#define		REC_LOWER_LIMIT_2				16				// 0 - 250 bpm
#define		REC_UPPER_LIMIT_3				17				// 0 - 250 bpm
#define		REC_LOWER_LIMIT_3				18				// 0 - 250 bpm
#define		REC_ANAEROB_LIMIT				19				// Upper summary limit / Anaerobic threshold value 0 - 250 bpm
#define		REC_AEROB_LIMIT					20				// Lower summary limit / Aerobic threshold value 0 - 250 bpm
#define		REC_TIMER_1						21				// timer 1 in seconds
#define		REC_TIMER_2						22				// timer 2 in seconds
#define		REC_TIMER_3						23				// timer 3 in seconds
#define		REC_MAX_HR						25				// UpperLimit+1 - 250
#define		REC_REST_HR						26				// 0 - LowerLimit-1
#define		REC_RR_START_DELAY				27				// R-R recording start delay (S810 / S810i only)
#define		REC_START_SAMPLE				29				// 0 - 250 bpm

#define		REC_STOP_TIME					30				// hh:mm:ss.d in 1/10 of seconds
#define		REC_STOP_SAMPLE					31				// stop hr 0 - 250 bpm
#define		REC_STOP_SPEED					32				// stop speed
#define		REC_STOP_CAD					33				// stop cadence
#define		REC_STOP_ALT					34				// stop altitude

#define		REC_MIN_HRATE					35				// lowest heart rate
#define		REC_AVE_HRATE					36				// average heart rate
#define		REC_MAX_HRATE					37				// highest heart rate

#define		REC_TRIP_DIST_STOP				38				// trip distance at stop, 1/10 km or 1/10 miles
#define		REC_ASCENT						39				// ascent
#define		REC_TOT_TIME_STOP				40				// total time at stop
#define		REC_AVG_ALT						41				// average altitude
#define		REC_MAX_ALT						42				// maximum altitude
#define		REC_AVG_SPEED					43				// average speed, 1/10 km/h or 1/10 mph
#define		REC_MAX_SPEED					44				// maximum speed, 1/10 km/h or 1/10 mph
#define		REC_ODOM_STOP					45				// odometer stop, km or miles
#define		REC_MIN_SPEED					46				// minimum speed, 1/10 km/h or 1/10 mph

#define		REC_RECOVERY_TIME				47
#define		REC_RECOVERY_HR					48

#define		REC_TARGETZONE_S_ABOVE			66				// Summary limits, target zone times in 1/10 seconds
#define		REC_TARGETZONE_S_IN				67
#define		REC_TARGETZONE_S_BELOW			68

#define		REC_TARGETZONE_1_ABOVE			69				// Limits1, target zone times in 1/10 seconds
#define		REC_TARGETZONE_1_IN				70			
#define		REC_TARGETZONE_1_BELOW			71			

#define		REC_TARGETZONE_2_ABOVE			72				// Limits2, target zone times in 1/10 seconds
#define		REC_TARGETZONE_2_IN				73			
#define		REC_TARGETZONE_2_BELOW			74			

#define		REC_TARGETZONE_3_ABOVE			75				// Limits3, target zone times in 1/10 seconds
#define		REC_TARGETZONE_3_IN				76			
#define		REC_TARGETZONE_3_BELOW			77			

#define		REC_MAX_POWER					78				// Maximum power in watts
#define		REC_AVE_POWER					79				// Average power in watts

#define		REC_CALORIES					80				// Calory consumption kcal
#define		REC_FATCONSUMPTION				108				// Fat percentage of calories

#define		REC_AVE_TEMP					81				// Average temperature 1/10 'C or 1/10 'F
#define		REC_MAX_TEMP					82				// Maximum temperature 1/10 'C or 1/10 'F

#define		REC_NBR_OF_LIMITS_IN_USE		83				// Nbr. of HR limits in use

#define		REC_VSPD_UP_MAX					95				// Vertical speed up max (ft/min)
#define		REC_VSPD_DOWN_MAX				96				// Vertical speed down max (ft/min)
#define		REC_VSPD_UP_AVG					97				// Vertical speed up avg (ft/min)
#define		REC_VSPD_DOWN_AVG				98				// Vertical speed down avg (ft/min)

#define		REC_MIN_TEMP					99				// Minimum temperature 1/10 'C or 1/10 'F
#define		REC_SLOPES						100				// Slope count
#define		REC_MIN_ALT						101				// Minimum altitude
#define		REC_DESCENT						102				// Descent

#define		REC_AVG_CAD						103				// average cadence
#define		REC_MAX_CAD						104				// maximum cadence

#define		REC_MAX_PEDALINGINDEX			105				// Maximum pedalling index
#define		REC_AVG_PEDALINGINDEX			106				// Average pedalling index
#define		REC_AVG_LRBALANCE				107				// Average left/right balance
 
#define		REC_PROGRAM						109				// Fittnes program used

#define		REC_UPPER_LIMIT_4				110				// 0 - 250 bpm
#define		REC_LOWER_LIMIT_4				111				// 0 - 250 bpm

#define		REC_TARGETZONE_4_ABOVE			112				// Limits4, target zone times in 1/10 seconds
#define		REC_TARGETZONE_4_IN				113			
#define		REC_TARGETZONE_4_BELOW			114			

#define		REC_SPORT_ZONE1_LOWER_LIMIT		115				// RS400, RS800, RS800CX, CS400, CS600 can have 1 to 10 sportzones
#define		REC_SPORT_ZONE1_UPPER_LIMIT		116				// SEE BELOW for zone limits 5-10 and times in those zones
#define		REC_SPORT_ZONE2_LOWER_LIMIT		117
#define		REC_SPORT_ZONE2_UPPER_LIMIT		118
#define		REC_SPORT_ZONE3_LOWER_LIMIT		119
#define		REC_SPORT_ZONE3_UPPER_LIMIT		120
#define		REC_SPORT_ZONE4_LOWER_LIMIT		121
#define		REC_SPORT_ZONE4_UPPER_LIMIT		122
#define		REC_SPORT_ZONE5_LOWER_LIMIT		123
#define		REC_SPORT_ZONE5_UPPER_LIMIT		124


#define		REC_SPORT_ZONE1_TIME			125
#define		REC_SPORT_ZONE2_TIME			126
#define		REC_SPORT_ZONE3_TIME			127
#define		REC_SPORT_ZONE4_TIME			128
#define		REC_SPORT_ZONE5_TIME			129

#define		REC_MAX_PACE					130				// sec / (1/10 km)
#define		REC_AVG_PACE					135				// sec / (1/10 km)

#define		REC_HR_LIMIT_TYPE				131				// 0=off , 1 = manual, 2 = automatic, 3 = OwnZone
#define		REC_LIMIT_TYPE					132				// 0=HR, 1=Speed

#define		REC_HR_LIMIT_VIEW				133				// 0=Limit type is bpm, 1 = Limit type is % of max Hr (CS200 only)

#define		REC_USER_HR_MAX					134

#define		REC_RUNNINGINDEX				137				// Running index (RS400, RS800, RS800CX)
#define		REC_STRIDE_AVG					138				// Average stride length during the exercise in cm (RS400, RS800, RS800CX)

#define		REC_RR_RECORDING				146				// RR Recording enabled during exercise (RS800, RS800CX)
#define		REC_MEMFULL						147				// Memory full during recording
#define		REC_RANKING						148				// Ranking information (RS400, RS800, RS800CX)
#define		REC_FEELING						149				// Feeling information (RS400, RS800, RS800CX)

#define		REC_EXERCISE_ID					153				// Exercise ID read from monitor (RS400, RS800, RS800CX, CS400, CS600)

#define		REC_SPORT_ZONE6_TIME			155				// RS400, RS800, RS800CX, CS400, CS600 can have 1 to 10 sportzones instead of 5
#define		REC_SPORT_ZONE7_TIME			156				// last five zones 5-10 are saved here.
#define		REC_SPORT_ZONE8_TIME			157
#define		REC_SPORT_ZONE9_TIME			158
#define		REC_SPORT_ZONE10_TIME			159

#define		REC_SPORT_ZONE6_LOWER_LIMIT		160				// RS400, RS800, RS800CX, CS400, CS600 can have 1 to 10 sportzones
#define		REC_SPORT_ZONE7_LOWER_LIMIT		161				// times in sportzones 5-10 are saved here.
#define		REC_SPORT_ZONE8_LOWER_LIMIT		162
#define		REC_SPORT_ZONE9_LOWER_LIMIT		163
#define		REC_SPORT_ZONE10_LOWER_LIMIT	164
#define		REC_SPORT_ZONE10_UPPER_LIMIT	165

#define		REC_SPORT_ZONE_TYPE				167				//Sport Zone Limits TYPE (0=HR, 1=HR%, 2=HRR%)

#define		REC_MULTISPORT					168				//multisport flag 0=not part of multisport exercise,
															//1=first in the chain
															//2=in the middle of the chain
																
///////////////////////////////////////////////////////////
//
// HRM DATA SAMPLE TYPES
//
//	Before getting measured values (samples), get the number of samples by using
//	function fnHRMCom_GetNbrOfHRMSamples. After this operation, samples can be get
//	by calling function fnHRMCom_GetHRMSamples for example in the following way:
//
//	iTotal = fnHRMCom_GetNbrOfHRMSamples ();
//
//	for (i = 0; i < iTotal; i++)
//	{
//		iHR[i]	  = fnHRMCom_GetHRMSamples (CC_HRATE, i);
//		iSpeed[i] = fnHRMCom_GetHRMSamples (CC_SPEED, i);
//		iCad[i]   = fnHRMCom_GetHRMSamples (CC_CAD, i);
//	}
//
//	Speed and altitude values unit depends of recording parameter REC_EURO_US_UNITS.
//	To get the correct units, use for example the following call:
//
//	if (1 == fnHRMCom_GetRecParam (REC_EURO_US_UNITS))
//	{
//		Speed in mph, altitude in feet
//	}
//	else
//	{
//		Speed in km/h, altitude in meters
//	}
//
///////////////////////////////////////////////////////////

#define		CC_HRATE						1				// heart rate values (bpm / msec)
#define		CC_SPEED						2				// speed values (10 * km/h / 10 * mph)
#define		CC_CAD							3				// cadence values (rpm)
#define		CC_ALT							4				// altitude values (m / ft)
#define		CC_POWER						5				// power values (Watts)
#define		CC_POWER_BALANCE				6				// power LR Balance (left%)
#define		CC_POWER_INDEX					7				// power pedalling index (%)
#define		CC_DIST							8				// distance values
#define		CC_AIRPR						9				// air pressure


///////////////////////////////////////////////////////////
//
// LAP TIME DATA INFORMATION
//
//	Before getting lap time data, get the number of laps by using
//	function fnHRMCom_GetNbrOfIntTimes. After this operation, lap
//	information can be get by calling function fnHRMCom_GetIntTimeData
//	for example in the following way:
//
//	iTotal = fnHRMCom_GetNbrOfIntTimes ();
//
//	for (i = 0; i < iTotal; i++)
//	{
//		iTime  = fnHRMCom_GetIntTimeData (i, INT_INT_TIME);
//		iHR    = fnHRMCom_GetIntTimeData (i, INT_SAMPLE);
//		iSpeed = fnHRMCom_GetIntTimeData (i, INT_SPEED);
//	}
//
///////////////////////////////////////////////////////////

#define		INT_INT_TIME					601				// Lap time in 1/10 seconds
#define		INT_LAP_INTRVAL					603				// Lap type: 0 = normal lap, 1 = interval
#define		INT_LAP_DISTANCE				604				// Lap distance in meters / yards
#define		INT_SAMPLE						607				// Momentary HR, 0 - 250 bpm
#define		INT_MIN_SAMPLE					608				// Lap's min HR, 0 - 250 bpm
#define		INT_AVE_SAMPLE					609				// Lap's avg HR, 0 - 250 bpm
#define		INT_MAX_SAMPLE					610				// Lap's max HR, 0 - 250 bpm
#define		INT_SPEED						611				// Momentary speed, 10 * km/h or mph
#define		INT_AVG_SPEED					612				// Average speed, 10 * km/h or mph
#define		INT_CADENCE						613				// Momentary cadence, 0 - 180 rpm
#define		INT_AVG_CADENCE					614				// Average cadence, 0 - 180 rpm
#define		INT_ALTITUDE					615				// Momentary altitude, (-1000 - 2047) * 10 m / ft
#define		INT_AVG_ALTITUDE				616				// Average altitude, (-1000 - 2047) * 10 m / ft
#define		INT_POWER						617				// Momentary power, 0 - 2000 Watts
#define		INT_MAX_POWER					618				// Maximum power, 0 - 2000 Watts
#define		INT_AVE_POWER					619				// Average power, 0 - 2000 Watts
#define		INT_TEMP						621				// Momentary temperature, 10 * -100 - +100 'C or 'F
#define		INT_DIST_REC					624				// Distance recovery, 10 * km or miles
#define		INT_RECOVERY					625				// Recovery calculation, 0 = No recovery, 1 = Time Recovery, 2 = +HR Recovery, 3 = -HR Recovery, 4 = Distance Recovery
#define		INT_HR_REC						626				// HR recovery value, 0 - 3599 seconds
#define		INT_TIME_REC					627				// Time recovery value, 0 - 240 bpm
#define		INT_LAP_ASCENT					636				// Lap ascent, trip up,  m / feet
#define		INT_LAP_DESCENT					637				// lap descent, trip down, m / feet
#define		INT_PHASELAP_INFO				645				// internal information about laps, phases etc.
#define		INT_SEALEVEL_PRESSURE			646				// Sea level air pressure mbar
#define		INT_TYPE						648				// 1 = manual,	 2=autolap
#define		INT_PACE						649				// sec/km
#define		INT_STRIDE_AVG					651				// Stride length average (cm) (RS800, RS800CX)


///////////////////////////////////////////////////////////////////////////////
//
// HR LIMIT SWAPS, indexes for 'fnHRMCom_GetLimitSwapData'
//
///////////////////////////////////////////////////////////////////////////////

#define		LIM_SWAP_TIME					900					// HR limit swap time
#define		LIM_SWAP_CODE					901					// HR limit swap code

////////////////////////
////////////////////////
//
// MONITOR BITMAP LOGO
//
////////////////////////
////////////////////////

////////////////////////////////////////////////////////////////////////////////////////
//
//	The monitor logo is available for most of the Polar products.
//	To detect the size of bitmap logo, use the following function calls:

__declspec (dllexport) int CALLBACK fnHRMCom_GetLogoRowCount (int);	// iMonitor: HRM_RS400, HRM_S710, etc
__declspec (dllexport) int CALLBACK fnHRMCom_GetLogoColCount (int);	// iMonitor: HRM_RS400, HRM_S710, etc

//	The bitmap logo can be send to Polar product using UpLink or Infrared connection and the following function call:

__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendMonitorLogo (POLAR_SSET_GENERAL*, int, int*, int);	// psg, iMonitor, *iBitmap, iParam = 0

//	If sending was succesfull, function returns TRUE, otherwise FALSE

// Example:	Each pixel column in one integer value => int iBitmapPixelCol[47];
//			First pixel in the bottom is 2^0, second 2^1, third 2^2, etc.
//			If three pixels in bottom are ON => iBitmapPixelCol[iColumn] = 7 (1+2+4)
//			If entire column is ON => iBitmapPixelCol[iColumn] = 255 (1+2+4+8+16+32+64+128)
//			Send to monitor fnHRMCom_SendBitmap (&iBitmapPixelCol[0]);
//
//	To read bitmap logo from Polar product, use the following function call (infrared only):

__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadMonitorLogo (POLAR_SSET_GENERAL*, int, int*, int); 	// psg, iMonitor, *iBitmap, iParam = 0

//
////////////////////////////////////////////////////////////////////////////////////////




////////////////////////
////////////////////////
//
// CHECK CONNECTION
//
////////////////////////
////////////////////////

////////////////////////////////////////////////////////////////////////////////////////
//
//		iReturn = fnHRMCom_TestUSBPort ();
// 
//	Return value:	0 to 4	Polar USB Interface is available
//					-1      INVALID_HANDLE_VALUE, if USB device is not available
//					-10     USB_NOT_SUPPORTED, if USB is not supported by operating system      
//
////////////////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) int CALLBACK		fnHRMCom_TestUSBPort		(void);

////////////////////////////////////////////////////////////////////////////////////////
//
//		bIrDAConnected = fnHRMCom_IsIrDAConnected ();
//
//	Return value:	TRUE	Polar IrDA enabled HR Monitor connected
//					FALSE	Polar IrDA enabled HR Monitor NOT connected
//
////////////////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) BOOL CALLBACK	fnHRMCom_IsIrDAConnected	(void);

////////////////////////
////////////////////////
//
// SONICLINK CONNECTION
//
////////////////////////
////////////////////////

////////////////////////////////////////////////////////////////////////////////////////
//
//	Call function fnHRMCom_ReadSonicLinkData to read SonicLink data from Polar S410, S510, S520, F6, F11, CS200, RS200 HR monitors.
//
__declspec (dllexport) int CALLBACK fnHRMCom_ReadSonicLinkData	(HWND, int, int, int, BOOL);
//
//	Input parameters:	HWND	hWnd		-	Dialog owner window handle
//						int		iMonitor	-	Monitor type: HRM_S410, HRM_S510, HRM_S520, HRM_F6, HRM_F11, HRM_CS200, HRM_RS200
//						int		iFrequency	-	Sampling frequency: 0 = 44100 Hz, 1 = 22050 Hz
//						int		iAmplify	-	Amplification of sound level: 1 - 10
//						BOOL	bSaveData	-	Save data to file for internal debugging purposes: FALSE / TRUE
//
//	Return value:		0 - Reading cancelled
//						1 - Reading succesfull
//						2 - "Retry" button pressed, wait for 500 msec and call the same function again
//
//	When exercise file has been succesfully read from Polar HR monitor. All exercise data can be read by using
//	the same functions than used with infrared communication, see the following chapters:
//	- HRM DATA OUTPUT FUNCTIONS
//	- HRM DATA FLAGS
//	- HRM DATA GENERAL RECORDING INFORMATION
//	- HRM DATA SAMPLE TYPES
//	- LAP TIME DATA INFORMATION
//
//	See also HRMComFS.h and HRMComRSCS.h for more information about reading received data.
//
////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//
//	fnHRMCom_ResetSonicCommunication
//	-----------------------------
//	Call in the startup of software to reset all the communication parameters.
//
//	Parameters:
//	int iParam		Reserved in future usage, use 0 (zero).
//
//	Return value:
//	TRUE			- Resetting made succesfully
//	FALSE			- Resetting was not made because of communication is already running.
//
///////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) BOOL CALLBACK fnHRMCom_ResetSonicCommunication	(int);


////////////////////////
////////////////////////
//
// MOBILE CONNECTION
//
////////////////////////
////////////////////////

////////////////////////////////////////////////////////////////////////////////////////
//
//	Call function fnHRMCom_ReadFromMobile to read exercise data from Polar MobileLink mobile
//	application (at compatible mobile phone).
//
__declspec (dllexport) int CALLBACK fnHRMCom_ReadFromMobile	(HWND);
//
//	Input parameters:	HWND	hWnd		-	Dialog owner window handle
//
//	Return value:		0 - Reading cancelled
//						1 - Reading succesfull
//						2 - "Retry" button pressed, wait for 500 msec and call the same function again
//
//	When exercise file has been succesfully read from Polar HR monitor. All exercise data can be read by using
//	the same functions than used with infrared communication, see the following chapters:
//	- HRM DATA OUTPUT FUNCTIONS
//	- HRM DATA FLAGS
//	- HRM DATA GENERAL RECORDING INFORMATION
//	- HRM DATA SAMPLE TYPES
//	- LAP TIME DATA INFORMATION
//
//	The note saved into Polar MobileLink mobile application, use the following function:
//
__declspec (dllexport) LPTSTR CALLBACK fnHRMCom_GetExerciseNote (void);
//
////////////////////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif
