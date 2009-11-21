#ifdef __cplusplus
    extern "C" {
#endif

////////////////////////
////////////////////////
//
//	WATCH SETTINGS
//
////////////////////////
////////////////////////

//	Polar AXN Outdoor computers do have two independent time zones. The active time zone 
//	can be selected with iActiveTime.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	// TIME
	int		iTime1;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// If iTime1 = -1, current system time is automatically set to iTime1
	int		iTime2;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// If iTime2 = -1, current system time is automatically set to iTime2
								// Only full hours and minutes are valid, seconds will be set to zero

	int		iTime1HourMode;		// 0 = 24h mode, 1 = 12h mode
	int		iTime2HourMode;		// 0 = 24h mode, 1 = 12h mode
	int		iActiveTime;		// 0 = time1 active, 1 = time2 active

	// DATE
	int		iDate;				// Date in format yyyymmdd, Jan 1 2000 - Dec 31 2099
								// If iDate = -1, current system date is automatically set to iDate

	// ALARMS
	BOOL	bAlarmEnabled[3];	// Alarm enabled FALSE = off, TRUE = on
	int		iAlarmTime[3];		// Alarm time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// Only full hours and minutes are valid, seconds will be set to zero
	TCHAR	szAlarmText[3][8];	// Alarm information text (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero
} POLAR_ADSSET_WATCH;

__declspec (dllexport) void CALLBACK fnHRMCom_A_ResetWatchSettings		(POLAR_ADSSET_WATCH*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_SendWatchSettings 		(POLAR_SSET_GENERAL*, POLAR_ADSSET_WATCH*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ReadWatchSettings		(POLAR_SSET_GENERAL*, POLAR_ADSSET_WATCH*);

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_PrepareWatchSettings	(POLAR_ADSSET_WATCH*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_UnPackWatchSettings		(POLAR_ADSSET_WATCH*);

////////////////////////
////////////////////////
//
// USER SETTINGS
//
////////////////////////
////////////////////////

//	User settings include both information about the person and the usage of monitor features.
//	All the settings are not available in all Polar AXN outdoor computers, see more details
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
	int		iRestHR;			// Resting heart rate value 15 - 120 bpm
	int		iVO2max;			// VO2max value 10 - 95 mmol/l/kg
	int		iUserSex;			// Sex of user: 0 = male, 1 = female

	int		iWeightKg;			// Weight in kilograms: 0, 20 - 199 kg
	int		iWeightLbs;			// Weight in pounds: 0, 44 - 439 lbs
								// If both weight values are specified, lbs value takes precedence.
	int		iHeightCm;			// Height in centimeters, 0, 90 - 211 cm
	int		iHeightFt;			// Height in feet: 0, 3 - 7 ft
	int		iHeightInches;		// Height in inches: 0 - 11 inches
								// If both height values are specified, cm value takes precedence.

	// Monitor Features and Functions
	BOOL	bUS_Units;			// Measurement units:	FALSE = EURO units, TRUE = US units
	BOOL	bAnimations;		// Animations shown at menu change TRUE/FALSE
	BOOL	bButtonSound;		// Button sounds enabled TRUE/FALSE

	BOOL	bHRLimitFormatBPM;	// HR limit format TRUE = bpm, FALSE = % of max HR

	BOOL	bHRUpperLimitInUse[3];// HR upper limit enabled TRUE/FALSE
	BOOL	bHRLowerLimitInUse[3];// HR lower limit enabled TRUE/FALSE
	int		iHRUpperLimit[3];	// HR upper limit value 30 - 240 bpm / 1 - 100 % of max HR
	int		iHRLowerLimit[3];	// HR lower limit value 30 - 240 bpm / 1 - 100 % of max HR (must be less than upper limit)

	int		iSamplingRate;		// 0 = 5s, 1 = 15s, 2 = 60s, 3 = 5min
								// Sampling rate selection is available only with AXN500 and AXN700
								// For other monitors, sampling rate selection is ignored

	int		iHeartTouch;		// Usage of Wireless Button trigger (heart touch feature)
								// 0 = off, 1 = light, 2 = change state, 3 = set marker

	int		iCountdownTimer;	// Countdown timer value in full, max 99:59:59 = 359 999 sec

	// Altitude settings
	BOOL	bAltitudeActive;	// TRUE = altitude active, FALSE = barometer active
	BOOL	bVerticalSpeedUnit;	// TRUE = xx/hour, FALSE = xx/min (xx is m or feet, depending on bUS_Units)

	BOOL	bAltitudeAlarm[2];	// Altitude alarm enabled TRUE/FALSE
	int		iAltitudeAlarmFt[2];// Altitude alarm value in feet
	int		iAltitudeAlarmM[2];	// Altitude alarm value in meters
								// If both values are specified, feet value takes precedence.

	int		iAltitudeScaling;	// 0 = scale * 1, 1 = scale * 10, 2 = scale * 100

	int		iDeclination;		// Compass declination, -99 ... 0 ... +99 (E=minus)

} POLAR_ADSSET_USER;

__declspec (dllexport) void CALLBACK fnHRMCom_A_ResetUserSettings		(POLAR_ADSSET_USER *);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_SendUserSettings 		(POLAR_SSET_GENERAL*, POLAR_ADSSET_USER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ReadUserSettings 		(POLAR_SSET_GENERAL*, POLAR_ADSSET_USER*);

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_PrepareUserSettings		(POLAR_ADSSET_USER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_UnPackUserSettings		(POLAR_ADSSET_USER*);

//	If sending was succesfull, function returns TRUE, otherwise FALSE

////////////////////////
////////////////////////
//
//	CALIBRATION LIST ITEM
//
////////////////////////
////////////////////////

typedef struct
{
	int		iItemNumber;		// Number of calibration item 0 - 19
	BOOL	bItemInUse;			// Calibration item in use TRUE/FALSE

	TCHAR	szItemName[8];		// Calibration item name (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero
	int		iAltitudeFt;		// Calibration altitude in feet
	int		iAltitudeM;			// Calibration altitude in meters
								// If both values are specified, feet value takes precedence.
} POLAR_ADSSET_CALIBRATION;

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	POLAR_ADSSET_CALIBRATION	AltCalibPoint[20];	// Altitude calibration points

} POLAR_ADSSET_ROUTE;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_SendAltCalibRoute (POLAR_SSET_GENERAL*, POLAR_ADSSET_ROUTE*, int);	// Third parameter HRM_AXN500 or HRM_AXN700
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ReadAltCalibRoute (POLAR_SSET_GENERAL*, POLAR_ADSSET_ROUTE*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_UnPackCalibration (int, POLAR_ADSSET_ROUTE*);

////////////////////////
////////////////////////
//
// SEASON TOTALS
//
////////////////////////
////////////////////////

typedef struct
{
	int		iSize;					// Structure size for version control
									// Get using sizeof (STRUCTURE)
	// Momentary values
	int		iAltitudeMax;			// Maximum altitude value (always in meters)
	int		iAltitudeMaxDate;		// Date of action file when maximum altitude was reached
	int		iAltitudeMin;			// Minimum altitude value (always in meters)
	int		iAltitudeMinDate;		// Date of action file when minimum altitude was reached

	int		iTemperatureMax;		// Maximum temperature value (always in Fahrenheit)
	int		iTemperatureMaxDate;	// Date of action file when maximum temperature was reached
	int		iTemperatureMin;		// Minimum temperature value (always in Fahrenheit)
	int		iTemperatureMinDate;	// Date of action file when minimum temperature was reached

	int		iVertSpeedUp;			// Maximum vertical speed upwards (divide by 32 to get actual f/s)
	int		iVertSpeedUpDate;		// Date of action file when maximum vertical speed upwards was reached
	int		iVertSpeedDown;			// Maximum vertical speed downwards (divide by 32 to get actual f/s)
	int		iVertSpeedDownDate;		// Date of action file when maximum vertical speed downwards was reached

	// Cumulative values
	int		iAscent;				// Total ascent made from start date (always in meters)
	int		iAscentStartDate;		// Start date of ascent cumulation
	int		iDescent;				// Total descent made from start date (always in meters)
	int		iDescentStartDate;		// Start date of descent cumulation

	int		iSlopes;				// Total number of slopes made from start date
	int		iSlopesStartDate;		// Start date of slopes cumulation

	int		iCalories;				// Total calories consumed from start date
	int		iCaloriesStartDate;		// Start date of calories cumulation

	int		iActionTime;			// Total action time from start date (in full seconds)
	int		iActionTimeStartDate;	// Start date of total action time cumulation

} POLAR_ADSSET_SEASONTOTALS;

// Note:	All dates are in yyyymmdd format.

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ResetSeasonTotals (POLAR_SSET_GENERAL*, int);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ReadSeasonTotals (POLAR_SSET_GENERAL*, POLAR_ADSSET_SEASONTOTALS*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_UnPackSeasonTotals (POLAR_ADSSET_SEASONTOTALS*);

////////////////////////
////////////////////////
//
// TEST RESULTS
//
////////////////////////
////////////////////////

typedef struct
{
	int		iSize;					// Structure size for version control
									// Get using sizeof (STRUCTURE)

	int		iFitTestDate;			// Date of fitness test in yyyymmdd format
	int		iFitTestResult;			// Result of fitness test

	int		iRestTestDate[5];		// Table of dates of rest tests in yyyymmdd format
	int		iRestTestResult[5];		// Table of rest test results in [bpm]
	int		iRestTestBaseline[5];	// Table of rest test baseline results

} POLAR_ADSSET_TESTRESULTS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ResetTestResults (POLAR_SSET_GENERAL*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ReadTestResults (POLAR_SSET_GENERAL*, POLAR_ADSSET_TESTRESULTS*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_UnPackTestResults (POLAR_ADSSET_TESTRESULTS*);

////////////////////////
////////////////////////
//
// REMINDERS
//
////////////////////////
////////////////////////

//	Reminders are available at all Polar AXN outdoor computers.
//	There are 5 reminder "slots" available in each HR monitor and those can be modified only by using
//	computer. Each reminder can be individually set to be activated at selected date & time.
//	One reminder at time can be sent to HR monitor, select reminder "slot" to be updated by iNumber.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iNumber;			// Number of reminder, 0 - 6
	BOOL	bActive;			// Reminder activated TRUE/FALSE
	int		iDate;				// Date of reminder in format yyyymmdd, Jan 1 2000 - Dec 31 2020
	int		iTime;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// Only full hours and minutes are valid, seconds will be set to zero
	TCHAR	szText[8];			// Reminder Text (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx
								// Max number of characters is 7 + ending zero

} POLAR_ADSSET_REMINDER;

//	Integer value at resetting and reading functions include reminder number (0 - 4)
//	If sending was succesfull, function returns TRUE, otherwise FALSE

__declspec (dllexport) void CALLBACK fnHRMCom_A_ResetReminder		(int, POLAR_ADSSET_REMINDER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_SendReminder		(POLAR_SSET_GENERAL*, POLAR_ADSSET_REMINDER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_ReadReminder		(POLAR_SSET_GENERAL*, int, POLAR_ADSSET_REMINDER*);

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_PrepareReminder		(POLAR_ADSSET_REMINDER*, BOOL);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_UnPackReminder		(int, POLAR_ADSSET_REMINDER*, BOOL);

////////////////////////
////////////////////////
//
// INFO TEXTS
//
////////////////////////
////////////////////////

__declspec (dllexport) BOOL CALLBACK fnHRMCom_A_SendInfoTexts		(POLAR_SSET_GENERAL*, LPTSTR, int);
__declspec (dllexport) LPTSTR CALLBACK fnHRMCom_A_UnPackInfoText	(void);
__declspec (dllexport) LPTSTR CALLBACK fnSplitInfoText				(LPTSTR, int);

#ifdef __cplusplus
}
#endif
