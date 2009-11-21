#ifdef __cplusplus
    extern "C" {
#endif



///////////////////////////////////////////////////////////////////////////////
//
//	POLAR S-SERIES MONITOR FEATURES
//	-------------------------------
//
//	Feature				S210	S410	S510	S610	S710	S810	E200	E600
//										S520	S610i	S710i	S810i
//												S625X	S720i
//														S725
//	--------------------------------------------------------------------------------
//	Watch Settings		 x		 x		 x		 x		 x		 x		x4		x4
//	Exercise Sets		 x		 x		 x		 x		 x		 x1		x4		x4
//	User Settings		 x		 x		 x		 x2		 x2		 x2		x4		x4
//	Reminders			  		  		  		 x		 x		 x
//	Exercise Profiles	 		 		 		 		 		 x
//	Monitor Bitmaps		 		 		 		 x		 x		 x
//	Bikes		  		  		  		 x		 		 x3
//
//	x	= feature available
//	x1	= Only one exercise set available
//	x2	= User settings extended with user name string
//	x3	= Also power output settings
//	x4	= Education models' features are limited, see function definitions
//
//	For more details about feature difference, refer to each function call definitions
//	and HR monitor user's manuals. Another good hint is also to use Polar Precision
//	Performance SW and it's HR Monitor Connection. This software products utilizes
//	HRMCom.dll function library.
//
///////////////////////////////////////////////////////////////////////////////

////////////////////////
////////////////////////
//
//	WATCH SETTINGS
//
////////////////////////
////////////////////////

//	All Polar S-series HR monitors do have two independent time zones. The active time zone 
//	can be selected with iActiveTime.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iTime1;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// If iTime1 = -1, current system time is automatically set to iTime1
	int		iTime2;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// If iTime2 = -1, current system time is automatically set to iTime2
								// Only full hours and minutes are valid, seconds will be set to zero

	int		iTime1HourMode;		// 0 = 24h mode, 1 = 12h mode
	int		iTime2HourMode;		// 0 = 24h mode, 1 = 12h mode
	int		iActiveTime;		// 0 = time1 active, 1 = time2 active
	int		iDate;				// Date in format yyyymmdd, Jan 1 2000 - Dec 31 2099
								// If iDate = -1, current system date is automatically set to iDate
	BOOL	bAlarmEnabled;		// FALSE = off, TRUE = on
	int		iAlarmTime;			// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// Only full hours and minutes are valid, seconds will be set to zero

} POLAR_SSET_WATCH;

__declspec (dllexport) void CALLBACK fnHRMCom_ResetWatchSettings	(POLAR_SSET_WATCH*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendWatchSettings 	(POLAR_SSET_GENERAL*, POLAR_SSET_WATCH*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadWatchSettings		(POLAR_SSET_GENERAL*, POLAR_SSET_WATCH*);

// NOTE: Education HR monitors E200 and E600 do have only one time (iTime1) and no alarm available.
// Set iTime2 to the same as iTime1, hour mode should be the same for both times.
// Alarm time should be zero and alarm should be not enabled.



////////////////////////
////////////////////////
//
// EXERCISE SET
//
////////////////////////
////////////////////////

// Exercise Set information will be send to monitor one set at a time.
// Exercise Set can be set as an active set to monitor (i.e. set will be shown
// as the first set when next time starting exercise).

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iSetNumber;			// Exercise set number: 0, 1, 2, 3, 4, 5.
								// Set number 0 can be used only for setting "Basic Set" active.
								// Sets 2 - 5 are not available for all monitors (see Polar S-series Monitor Features).

	BOOL	bActiveSet;			// Will this set to be set as an active set in monitor? TRUE/FALSE
	TCHAR	szName[8];			// Exercise set name (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero
								// "BasicUse" name is permanent for S610, S610i, S625X, S710, S710i, S720i, S725, S810 and S810i,
								// it can't be modified.

	// Timers
	BOOL	bTimer1Enabled;		// Timer 1 enabled TRUE/FALSE
	int		iTimer1;			// Timer 1 value in seconds, max 99 min 59 sec (= 5999 sec)
	BOOL	bTimer2Enabled;		// Timer 2 enabled TRUE/FALSE
	int		iTimer2;			// Timer 2 value in seconds, max 99 min 59 sec
								// Timer 2 used as interval timer, if intervals enabled.
	BOOL	bTimer3Enabled;		// Timer 3 enabled TRUE/FALSE
	int		iTimer3;			// Timer 3 value in seconds, max 99 min 59 sec

	// HR Limits
	BOOL	bHRLimit1Enabled;	// HR Limits 1 enabled
	int		iHRLimit1Upper;		// HR Limit 1 upper value 30 - 240 bpm
	int		iHRLimit1Lower;		// HR Limit 1 lower value 30 - 240 bpm (must be less than upper limit)
	BOOL	bHRLimit2Enabled;	// HR Limits 2 enabled
	int		iHRLimit2Upper;		// HR Limit 2 upper value 30 - 240 bpm
	int		iHRLimit2Lower;		// HR Limit 2 lower value 30 - 240 bpm (must be less than upper limit)
	BOOL	bHRLimit3Enabled;	// HR Limits 3 enabled
	int		iHRLimit3Upper;		// HR Limit 3 upper value 30 - 240 bpm
	int		iHRLimit3Lower;		// HR Limit 3 lower value 30 - 240 bpm (must be less than upper limit)

	BOOL	bMaxHRInUse;		// Are HR limit values in percentage of maximum HR given in iMaxHR variable?
								// If TRUE, all HR limit values are used as percentage values (50 - 100%)
	int		iMaxHR;				// Maximum HR value to be used for calculation of HR limit values.
								// HR value in bpm, 100 - 240 bpm

	// Intervals
	BOOL	bIntervalsEnabled;	// TRUE/FALSE
	int		iIntervalType;		// 0 = manual, 1 = timer (use Timer2), 2 = HR,
								// 3 = distance (distance only with cycling models)
	int		iIntervalCount;		// The number of intervals, 0 - 30 (0 = unlimited)
	int		iIntervalEndHR;		// Interval ending HR bpm 10 - 240 bpm
	int		iIntervalDistKm;	// The distance of interval in 0.1 km (max 99.9 km)
								// If monitor does not support cycling features, this value is ignored
	int		iIntervalDistMiles;	// The distance of interval in 0.1 miles (max 99.9 miles)
								// If monitor does not support cycling features, this value is ignored
								// If both distance values are specified, km value takes precedence.

	// Recovery
	BOOL	bRecoveryEnabled;	// TRUE/FALSE
	int		iRecoveryType;		// 0 = timer recovery, 1 = HR recovery
								// 2 = distance recovery  (distance only with cycling models)
	int		iRecoveryTime;		// Recovery time in seconds, max 99 min 59 sec (max 5999 sec)
	int		iRecoveryHR;		// recovery HR value 10 - 240 bpm
	int		iRecoveryDistKm;	// The distance of recovery in 0.1 km (max 99.9 km)
								// If monitor does not support cycling features, this value is ignored
	int		iRecoveryDistMiles;	// The distance of recovery in 0.1 miles (max 99.9 miles)
								// If monitor does not support cycling features, this value is ignored
								// If both distance values are specified, km value takes precedence.

	// HR Summary Limit for S625X and S725 
	int		iHRLimitType ;			// HR limit type and flags, 0 = bpm, 1 = %max, 2 = pace.

	BOOL	bHRSummaryLimitEnabled;	// HR Summary Limits enabled
	int		iHRSummaryLimitUpper;	// HR Summary Limit upper value 30 - 240 bpm
	int		iHRSummaryLimitLower;	// HR Summary Limit lower value 30 - 240 bpm (must be less than upper limit)

	int		iHRPercentLimits1Lower ;	// 20-110%, bin
	int		iHRPercentLimits1Upper ;	// 20-110%, bin
	int		iHRPercentLimits2Lower ;	// 20-110%, bin
	int		iHRPercentLimits2Upper ;	// 20-110%, bin
	int		iHRPercentLimits3Lower ;	// 20-110%, bin
	int		iHRPercentLimits3Upper ;	// 20-110%, bin
	int		iHRPercentSummaryLimitsLower ;	// 20-110%, bin
	int		iHRPercentSummaryLimitsUpper ;	// 20-110%, bin

	// Pace limits are available only for S625X
	int		iPaceLimits1Lower;			// seconds
	int		iPaceLimits1Upper;			// seconds
	int		iPaceLimits2Lower;			// seconds
	int		iPaceLimits2Upper;			// seconds
	int		iPaceLimits3Lower;			// seconds
	int		iPaceLimits3Upper;			// seconds
	int		iPaceSummaryLimitsLower ;	// seconds
	int		iPaceSummaryLimitsUpper ;	// seconds

} POLAR_SSET_EXERCISESET;

__declspec (dllexport) void CALLBACK fnHRMCom_ResetExerciseSet		(int, POLAR_SSET_EXERCISESET*, int, int);	// ..., iExerciseType, iMonitor
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendExerciseSet 		(POLAR_SSET_GENERAL*, POLAR_SSET_EXERCISESET*, int); // ExeSet Version (0 is for all, exept 1 is forS725 and S625X)
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadExerciseSet 		(POLAR_SSET_GENERAL*, int, POLAR_SSET_EXERCISESET*, int); // ExeSet Version (0 is for all, exept 1 is forS725 and S625X)

//	Integer value at resetting and reading functions include exe set number (1 - 5)
//	If sending was succesfull, function returns TRUE, otherwise FALSE

// NOTE: Education HR monitors E200 and E600 do have only one Exercise Set, iSetNumber should be 1 (one).
// E200 and E600 HR monitors do have the following features:
//	- Timers 1 and 2
//	- HR Limits 1
//	- Recovery calculation (type timer always)
// Other exercise settings should be set to default values


////////////////////////
////////////////////////
//
// EXERCISE LIMITS
//
////////////////////////
////////////////////////

// Exercise Limits information is designed only for Polar E30 education HR monitor (UpLink only).

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	// HR Limits
	BOOL	bHRLimitAlarm;		// HR Limits 1 enabled
	int		iHRLimitUpper;		// HR Limit 1 upper value 30 - 240 bpm
	int		iHRLimitLower;		// HR Limit 1 lower value 30 - 240 bpm (must be less than upper limit)

} POLAR_SSET_EXERCISELIMITS;

__declspec (dllexport) void CALLBACK fnHRMCom_ResetExerciseLimits	(POLAR_SSET_EXERCISELIMITS*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendExerciseLimits	(POLAR_SSET_GENERAL*, POLAR_SSET_EXERCISELIMITS*);



////////////////////////
////////////////////////
//
// USER SETTINGS
//
////////////////////////
////////////////////////

//	User settings include both information about the person and the usage of monitor features.
//	All the settings are not available in all Polar S-serie monitor, see more details
//	from monitor specifications. If any data send to monitor is not supported, it will
//	be ignored automatically.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)
	// Information about user
	int		iDateOfBirth;		// Date of birth in format yyyymmdd, Jan 1 1921 - Dec 31 2020
	int		iActivityLevel;		// Activity level: 0 = low, 1 = moderate, 2 = high, 3 = top
	int		iMaxHR;				// Maximum heart rate value 100 - 240 bpm
	int		iRestHR;			// Resting heart rate value, not currently in use
	int		iVO2max;			// VO2max value 10 - 95 mmol/l/kg
	int		iUserSex;			// Sex of user: 0 = male, 1 = female

	int		iWeightKg;			// Weight in kilograms: 0, 20 - 199 kg
	int		iWeightLbs;			// Weight in pounds: 0, 44 - 499 lbs
								// If both weight values are specified, kg value takes precedence.
	int		iHeightCm;			// Height in centimeters, 0, 90 - 211 cm
	int		iHeightFt;			// Height in feet: 0, 3 - 7 ft
	int		iHeightInches;		// Height in inches: 0 - 11 inches
								// If both height values are specified, cm value takes precedence.

	TCHAR	szName[11];			// User name (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero
								// If monitor does not support user name, this value is ignored
	TCHAR	szName2[11];		// User name 2 (see Polar Character Set), not currently in use

	// Monitor Features and Functions
	int		iMonitorID;			// Monitor ID number (for example player number) 0 - 99
	BOOL	bOwnCal;			// OwnCal calculation enabled TRUE/FALSE
	BOOL	bHRMaxP;			// HRmax-p calculation enabled TRUE/FALSE
	BOOL	bOwnIndex;			// OwnIndex calculation enabled TRUE/FALSE
	BOOL	bAltimeter;			// Altimeter enabled TRUE/FALSE, available only for S710, S710i, S720i
	BOOL	bButtonSound;		// Button sounds enabled TRUE/FALSE
	BOOL	bOptionsLock;		// Options mode lock enabled TRUE/FALSE
	BOOL	bHelp;				// Feature help function enabled TRUE/FALSE
	BOOL	bUS_Units;			// Measurement units:	FALSE = EURO units, TRUE = US units
	BOOL	bAutoLap;			// AutoLap enabled TRUE/FALSE
	int		iSamplingRate;		// 0 = 5s, 1 = 15s, 2 = 60s, 3 = R-R intervals
								// Sampling rate selection is available only with S610, S610i, S625X, S710, S710i, S720i, S725, S810 and S810i
								// R-R intervals recording is available only with S810 and S810i
								// Monitor S210 do not have sampling rate selection
								// Monitors S410, S510 and S520 have always dynamic sampling rate
	int		iHeartTouch;		// Usage of Wireless Button trigger (heart touch feature)
								// 0 = normal, 1 = lap, 2 = change display and limits
								// Wireless button action selection is available with S610, S610i, S625X, S710, S710i, S720i, S725, S810 and S810i
	int		iRLXBaseLine;		// Relaxation base line only for S810 and S810i, 4 - 150 mseconds
								// Leave iRLXBaseLine value to zero, if sending settings to other monitors than S810 or S810i
	BOOL	bOnlineRecording;	// Online recording enabled TRUE/FALSE, S810 and S810i only
	int		iAutoLapDistance;	// AutoLap distance value, 1 - 999 * (0.1 km or 0.1 miles), S625X only
	BOOL	bOptimizerSound;	// OwnOptimizer sound TRUE/FALSE, S625X only

} POLAR_SSET_USER;

__declspec (dllexport) void CALLBACK fnHRMCom_ResetUserSettings		(POLAR_SSET_USER *);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendUserSettings 		(POLAR_SSET_GENERAL*, POLAR_SSET_USER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadUserSettings 		(POLAR_SSET_GENERAL*, POLAR_SSET_USER*);

//	If sending was succesfull, function returns TRUE, otherwise FALSE

// NOTE: Education HR monitors E200 and E600 do have only the following features:
// Options Lock, User Name, Monitor ID, Sampling Rate (E600 only)

///////////////////////////////////////////////////////////////////////////////
//
// VO2max and HRmax-p values are used in OwnCal calories calculation
// and those values can be updated as follows:
//
//	PC using UpLink/IR ----->
//	OwnIndex from FitTest --> UserSet in Monitor ---> OwnCal calculation in monitor
//	Manually set ----------->
//
///////////////////////////////////////////////////////////////////////////////



////////////////////////
////////////////////////
//
// REMINDER
//
////////////////////////
////////////////////////

//	Reminders are available with S610, S610i, S625X, S710, S710i, S720i, S725, S810 and S810i heart rate monitors.
//	There are 7 reminder "slots" available in each HR monitor and those can be modified only by using
//	computer. Each reminder can be individually set to be activated at selected date & time.
//	One reminder at time can be sent to HR monitor, select reminder "slot" to be updated by iNumber.
//	Reminder can be repeated automatically hourly, daily, monthly, weekly, monthly and yearly.
//	An exercise (ExeSet / ExeProfile) can be set to be active after reminder has alarmed.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iNumber;			// Number of reminder, 0 - 6
	BOOL	bActive;			// Reminder activated TRUE/FALSE
	int		iDate;				// Date of reminder in format yyyymmdd, Jan 1 2000 - Dec 31 2020
	int		iTime;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// Only full hours and minutes are valid, seconds will be set to zero
	int		iRepeat;			// Repetition of reminder. 0 = Off,  1 = Hourly,
								// 2 = Daily, 3 = Weekly,  4 = Monthly, 5 = Yearly
	int		iExercise;			// S810 and S810i: Exercise Profile to be set as default profile after reminder alarm
								// 0 = Off,  1 = BasicUse, 2 - 8 Profile Number (remember to update also exercise profiles)
								// S610, S610i, S625X, S710, S710i, S720i, S725: Exercise Set to be set as default profile after reminder alarm
								// 0 = Off,  1 = BasicUse, 2 - 7 ExeSet Number (remember to update also exercise sets)
	TCHAR	szText[8];			// Reminder Text (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero

} POLAR_SSET_REMINDER;

__declspec (dllexport) void CALLBACK fnHRMCom_ResetReminder			(int, POLAR_SSET_REMINDER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendReminder 			(POLAR_SSET_GENERAL*, POLAR_SSET_REMINDER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadReminder 			(POLAR_SSET_GENERAL*, int, POLAR_SSET_REMINDER*);

//	Integer value at resetting and reading functions include reminder number (0 - 6)
//	If sending was succesfull, function returns TRUE, otherwise FALSE



////////////////////////
////////////////////////
//
// BIKES
//
////////////////////////
////////////////////////

//	Bike information is available only with S510, S520, S710, S710i, S720i, S725, S725X, S625X, CS100, CS200. Bike's power settings will
//	be transferred only to Polar S710, S710i and S720i HR monitors. Odometer & ETA values are only for CS monitors.

typedef struct
{
	// Bike Information
	TCHAR	szBikeID[5];		// Bike ID (name) (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 4 + ending zero
	int		iWheelSize;			// Wheel size in millimeters (1000 - 3000 mm)
	BOOL	bAutoStart;			// Is autostart feature in use TRUE/FALSE
	int		iOdometerKm;		// Odometer value in km
	int		iETADistKm;			// Distance for Estimated Time of Arrival (km * 10)

	BOOL	bSensorSpeed;		// Speed sensor in use TRUE/FALSE. This flag is not in use, speed sensor is always in use.
	BOOL	bSensorCadence;		// Cadence sensor in use TRUE/FALSE
	BOOL	bSensorPower;		// Power sensor in use TRUE/FALSE
								// Power sensor is available only with Polar S710 HR monitor
								// If monitor does not support power sensor, this value is ignored

	// Power Sensor Settings
	int		iChainMass;			// Weight of chain in grams (200 - 400 g)
	int		iChainLength;		// Length of chain in mm (1000 - 2000 mm)
	int		iChainWank;			// The length of vibrating part (span) chain in mm (300 - 600 mm)

} POLAR_BIKE_INFO;

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)
	int		iMonitorType;
	int		iBikeInUse;			// Which bike has been selected to be in use right now?
								// 0 = None, 1 = Run, 2 = Bike1, 3 = Bike2	(Polar S625X)
								// 0 = None, 1 = Bike1, 2 = Bike2			(other monitors)
	BOOL	bWriteOdometer;

	POLAR_BIKE_INFO	Bike[3];

} POLAR_SSET_BIKES;

__declspec (dllexport) void CALLBACK fnHRMCom_ResetBikeSettings		(POLAR_SSET_BIKES*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendBikesSettings 	(POLAR_SSET_GENERAL*, POLAR_SSET_BIKES*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadBikesSettings 	(POLAR_SSET_GENERAL*, POLAR_SSET_BIKES*);

//	If sending was succesfull, function returns TRUE, otherwise FALSE



////////////////////////
////////////////////////
//
// EXERCISE PROFILE
//
////////////////////////
////////////////////////

typedef struct
{
	BOOL	bPhaseEnabled;		// Has exercise phase been enabled? TRUE/FALSE

	// HR Limits
	BOOL	bHRLimitEnabled;	// HR Limits enabled
	int		iHRLimitUpper;		// HR Limit upper value 30 - 240 bpm
	int		iHRLimitLower;		// HR Limit lower value 30 - 240 bpm (must be less than upper limit)

	// Interval period
	BOOL	bIntervalsEnabled;	// Is entire work period enabled? TRUE/FALSE
	int		iIntervalType;		// 0 = manual, 1 = timer, 2 = End HR
	int		iIntervalCount;		// The number of intervals, 0 - 30 (0 = unlimited)
	int		iIntervalTimer;		// Timer value in seconds, max 99 min 59 sec (= 5999 sec)
	int		iIntervalEndHR;		// Interval ending HR bpm 10 - 240 bpm

	// Recovery period
	BOOL	bRecoveryEnabled;	// Is entire recovery period enabled ? TRUE/FALSE
	int		iRecoveryType;		// 0 = timer recovery, 1 = HR recovery
	int		iRecoveryTime;		// Recovery time in seconds, max 99 min 59 sec (max 5999 sec)
	int		iRecoveryHR;		// recovery HR value 10 - 240 bpm

} POLAR_EXEPHASE;

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iNumber;			// Exercise profile number, 1 - 7
	BOOL	bActiveProfile;		// Will this profile to be set as an active set in monitor? TRUE/FALSE
	TCHAR	szName[8];			// Exercise profile name (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero

	BOOL	bMaxHRInUse;		// Are HR limit values in percentage of maximum HR given in iMaxHR variable?
								// If TRUE, all HR limit values are used as percentage values (50 - 100%)
	int		iMaxHR;				// Maximum HR value to be used for calculation of HR limit values.
								// HR value in bpm, 100 - 240 bpm

	POLAR_EXEPHASE	Phase[6];	// One exercise profile includes 6 exercise phases
								// Each phase should be defined as POLAR_EXEPHASE structure

} POLAR_SSET_EXERCISEPROFILE;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_ResetExerciseProfile 	(int, POLAR_SSET_EXERCISEPROFILE*, int);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendExerciseProfile 	(POLAR_SSET_GENERAL*, POLAR_SSET_EXERCISEPROFILE*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadExerciseProfile 	(POLAR_SSET_GENERAL*, int, POLAR_SSET_EXERCISEPROFILE*);

//	NOTE: Exercise profiles are available only with Polar S810 and S810i HR monitors.
//	Integer value at resetting and reading functions include Exe Profile number (1 - 7)
//	If sending was succesfull, function returns TRUE, otherwise FALSE



////////////////////////
////////////////////////
//
// ODOMETER EDITING
//
////////////////////////
////////////////////////

typedef struct
{
	int		iSize;					// Structure size for version control

	int		iKilometers;			// Odometer value in kilometers
									// If both odometer values are specified, km value takes precedence.
	int		iMiles;					// Odometer value in miles

	int		iHRMonitor;				// HRM_S510, HRM_S520, HRM_S710, HRM_S725

} POLAR_SSET_ODOMETER;

__declspec (dllexport) BOOL CALLBACK	fnHRMCom_WriteOdometer		(POLAR_SSET_GENERAL*, POLAR_SSET_ODOMETER*);
__declspec (dllexport) BOOL CALLBACK	fnHRMCom_ReadOdometer		(POLAR_SSET_GENERAL*, POLAR_SSET_ODOMETER*);

//	If reading/writing was succesfull, function returns TRUE, otherwise FALSE
//	Odometer value can be read by using IR only from Polar S710, S710i, S720i, S725.
//
//	NOTE1:	Writing odometer value by using IR, an automatic check for the correct HR monitor type is done.
//			If trying to write odometer value to for example Polar S610, function returns FALSE.
//
//	NOTE2:	When writing odometer value by using Polar UpLink, make sure that the correct HR monitor type
//			is receiving data. Writing odometer value to for example Polar S610 causes memory damage.
//
//	NOTE3:	After value has been written, HR monitor have to be resetted to take value to  use.
//			Exercise files or settings won't be lost at reset, only date & time have to be set.

////////////////////////
////////////////////////
//
// ID NUMBER
//
////////////////////////
////////////////////////

//	Functions to write & read "secret" ID number from HR monitors memory.
//	If returned ID number is 16 777 215 (FFFFFFh), this means that there is not ID number set.
//	ID number can be in range 0 - 16 777 215

__declspec (dllexport) BOOL CALLBACK	fnHRMCom_WriteIDNumber		(int);
__declspec (dllexport) int CALLBACK		fnHRMCom_ReadIDNumber		(void);

////////////////////////
////////////////////////
//
// TOTAL CALORIES
//
////////////////////////
////////////////////////

//	Functions to read & write total calories count.
//	Total calories can be in range 0 - 999 999 calories

__declspec (dllexport) BOOL CALLBACK	fnHRMCom_WriteTotalCalories (int);
__declspec (dllexport) int CALLBACK		fnHRMCom_ReadTotalCalories	(void);

//	NOTE:	After value has been written, HR monitor have to be resetted to take value to  use.
//			Exercise files or settings won't be lost at reset, only date & time have to be set.

////////////////////////
////////////////////////
//
// OWNOPTIMIZER TEST RESULTS
//
////////////////////////
////////////////////////

////////////////////////////////////////////////////////////////////////////////////////
//
//	Call function fnHRMCom_ReadOwnOptimizerResults to read Polar OwnOptimizer test results from
//	Polar S625X HR monitor. Polar S625X saves up to 14 previous test results, which can be
//	then read from dll's memory by using function fnHRMCom_GetOwnOptimizerResult.
//
__declspec (dllexport) int CALLBACK fnHRMCom_ReadOwnOptimizerResults (HWND, BOOL, int);
//
//	Input parameters:	HWND	hWnd		-	Dialog owner window handle
//						BOOL	bReserved	-	Reserved for future use, use FALSE
//						int		iMonitor	-	Monitor type, for example HRM_S625
//
//	Return parameter:	int		iTestCount	-	Count of test results
//
typedef struct
{
	int		iSize;					// Structure size for version control

	int		iDate;					// Date of test in yyyymmdd
	int		iState;					// OwnOptimizer state
	int		iRestAvgHR;				// Resting average HR
	int		iPeakHR;				// Peak HR
	int		iStandAvgHR;			// Standing average HR

} POLAR_OWNOPT_RESULT;
//
__declspec (dllexport) BOOL CALLBACK fnHRMCom_GetOwnOptimizerResult (int, int, POLAR_OWNOPT_RESULT*);
//
//	Input parameters:	int					iTestIndex	-	Index of test 0 - 13
//						int					iMonitor	-	Monitor type, for example HRM_S625
//						POLAR_OWNOPT_RESULT	Result		-	POinter to result structure
//
//	For detailed information about Polar OwnOptimizer test states, see PPP help,
//	Polar web site or Monitor user's guide.
//
////////////////////////////////////////////////////////////////////////////////////////

////////////////////////
////////////////////////
//
// ONLINE RECORDING
//
////////////////////////
////////////////////////

///////////////////////////////////////////////////////////
//	Online recording is available only with Polar S810 and S810i HR monitors. To start online recording,
//	function fnHRMCom_StartIRCommunication have to be called with parameter HRMCOM_PARAM_ONLINE.
//	Function 'fnHRMCom_GetOnlineData' returns online data samples received from the S810 or S810i
//	HR monitor or ONLINE_BUFF_EMPTY if there aren't any new online samples in the buffer.
//
//		iData = fnHRMCom_GetOnlineData (iParam);
//
//	Parameter 'iParam' is 32-bit integer and it is reserved for future use and it should
//	be 0 (zero) now. Return value is 32-bit integer and it is R-R value in milliseconds
//	or ONLINE_BUFF_EMPTY if there aren't any new samples in buffer.
//
///////////////////////////////////////////////////////////

#define		ONLINE_BUFF_EMPTY				-1				// online buffer is empty

__declspec (dllexport) int CALLBACK	fnHRMCom_GetOnlineData		(int);		// return online data samples
////////////////////////////////////////////////////////////////////////////////////////
//
//		bOK = fnHRMCom_PauseOnline (bPause);
// 
//  Function stops and continues S-810 online recording. Parameter 'bPause' is a 32-bit 
//  boolean. TRUE stops online recordingand FALSE starts or continues online recording. 
//  Return value is 32-bit boolean and it is TRUE if parameter bPause is not illegal.
//
//  ====================================================================================
//
//		bOK = fnHRMCom_OnlineAddLapTime (void);
// 
//	Function adds online lap time. Return value is 32-bit boolean and it is TRUE if 
//	online recording is in progress and it is not paused.
//
////////////////////////////////////////////////////////////////////////////////////////

__declspec (dllexport) BOOL CALLBACK	fnHRMCom_PauseOnline		(BOOL);		// stops / restarts online recording
__declspec (dllexport) BOOL CALLBACK	fnHRMCom_AddOnlineLaptime	(int);		// adds online lap time


#ifdef __cplusplus
}
#endif
