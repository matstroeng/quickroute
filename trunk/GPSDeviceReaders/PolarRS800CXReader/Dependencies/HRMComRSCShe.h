#ifdef __cplusplus
    extern "C" {
#endif

////////////////////////
////////////////////////
//
// DEFINED CONSTANTS
//
////////////////////////
////////////////////////

#define LENGHT_OF_DESCRIPTION_TEXT			500	// In product 20 char
#define LENGTH_OF_NAME_TEXT					500	// In product 10 char
#define LENGTH_OF_USERNAME_TEXT				11
#define NUMBER_OF_DYNEXESETS				22
#define NUMBER_OF_WEEKLYTARGETS				9
#define NUMBER_OF_DAILYTARGETS				63
#define NUMBER_OF_SPORTZONES				10
#define LENGTH_OF_SPORTPROFILE_NAME_TEXT	10
#define NUMBER_OF_PHASES					12

#define LENGTH_OF_REMINDER_TEXT				10
#define NUMBER_OF_REMINDERS					7

#define	UNPACK_RRREC_ONLY					2

////////////////////////
////////////////////////
//
// USER SETTINGS
//
////////////////////////
////////////////////////

//	User settings include information about the person.
//	Monitor features and their usage is done in Monitor Settings

//	All the settings are not available in all Polar products, see more details
//	from monitor specifications. If any data send to monitor is not supported, it will
//	be ignored automatically.

typedef struct
{
	// User details
	int		iMonitor;           // HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600

	int		iPlayerNumber;		// Player Number. Range 0 - 99

	// Information about User
	TCHAR	szName[LENGTH_OF_USERNAME_TEXT];	// User first name (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx

	TCHAR	szName2[LENGTH_OF_USERNAME_TEXT];	// User last name (see Polar Character Set)
								// String can be checked using function fnHRMCom_CheckPolarCharStringEx

	int		iDateOfBirth;		// Date of birth in format yyyymmdd, Jan 1 1921 - Dec 31 2020.
	int		iActivityLevel;		// Activity level: 0 = low, 1 = moderate, 2 = high, 3 = top.

	int		iMaxHR;				// Maximum heart rate value 0, 100 - 240 bpm. By default = 220 - Age.
    int		iSitHR;				// Sit heart rate value min? - max? bpm, default 70 bpm
    int		iVO2max;			// VO2max value 10 - 95 mmol/l/kg.
	int		iUserSex;			// Sex of user: 0 = male, 1 = female.

    int		iWeightKg;			// Weight in kilograms: 0, 20 - 199 kg
    int		iWeightLbs;			// Weight in pounds: 0, 44 - 439 lbs
								// If both weight values are specified, lbs value takes precedence.

	int		iHeightCm;			// Height in centimeters, 0, 100 - 211 cm
	int		iHeightFt;			// Height in feet: 0, 3 - 7 ft
	int		iHeightInches;		// Height in inches: 0 - 11 inches
								// If both height values are specified, cm value takes precedence.
} POLAR_RSCS_SET_USER;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendUserSettings_RSCS 		(POLAR_SSET_GENERAL*, POLAR_RSCS_SET_USER*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_ReadUserSettings_RSCS			(POLAR_SSET_GENERAL*, POLAR_RSCS_SET_USER*);

//	If sending was succesfull, function returns TRUE, otherwise FALSE

////////////////////////
////////////////////////
//
// EXERCISE REMINDERS
//
////////////////////////
////////////////////////

//	Reminders are available at Polar RS400, RS800, RS800CX, CS400, CS600 products.
//	There are 7 reminder "slots" available in each product and those can be modified only by using
//	computer. Each reminder can be individually set to be activated at selected date & time.
//	One reminder at time can be sent to HR monitor, select reminder "slot" to be updated by iNumber.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iNumber;			// Number of reminder, 0 - 6
	BOOL	bActive;			// Reminder activated TRUE/FALSE.
	BOOL	bConnectedExe;		// Reminder is connected to an exercise. yes = TRUE, no = FALSE.
	int		iConnectionMode;	// Appearence mode. At preset time = 0, 10 min before = 1, 30 min before = 2, 60 min before = 3, 24h before = 4. By default = 0.    
	int		iExercise;			// Connected exercise
	int		iDate;				// Date of reminder in format yyyymmdd, Jan 1 2000 - Dec 31 2020
	int		iTime;				// Time in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec
								// Only full hours and minutes are valid, seconds will be set to zero

	int		iRepeat;			// Occurrence: Off = 0, Once = 1, Hourly = 2, Daily = 3, Weekly = 4, Monthly = 5, Yearly = 6
	int		iVolume;			// Volume. Normal = 0, Beep = 1, Silent = 2
	
	TCHAR	szText[LENGTH_OF_REMINDER_TEXT];	// Reminder Text, max number of characters is 9 + ending zero?

} POLAR_RSCS_SET_REMINDER;

typedef struct
{
	int		iSize;				// Structure size for version control
	int     iMonitor;			// HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600

	POLAR_RSCS_SET_REMINDER Reminder[NUMBER_OF_REMINDERS];	// Number of reminders.

} POLAR_RSCS_SET_REMINDERS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_SendReminders			(POLAR_SSET_GENERAL *psg, POLAR_RSCS_SET_REMINDERS *psr);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_ReadReminders			(POLAR_SSET_GENERAL *psg, POLAR_RSCS_SET_REMINDERS *psr);

////////////////////////
////////////////////////
//
// SPORT PROFILE
//
////////////////////////
////////////////////////

// Sport profile information for completing Exercise Set

typedef struct
{
	int		iSportProfileType;	// 0 = Use Product Settings
								// 1 = Use SportProfile Settings

	TCHAR   szName[LENGTH_OF_SPORTPROFILE_NAME_TEXT + 1];	// The name of sport profile, max 10 char

	int		iMonitorType;		// HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600
    
	// Recording
	int		iRecordingRate;		// 0 = 5s, 1 = 15s, 2 = 60s, 3 = 1s, 4 = RR.  By default = 0?.
	int		iRR_Recording;		// 0 = off, 1 = on	// HR measurement with RR-variation

	int		iOnlineRecording;	// (Online training) 0 = off, 1 = on. RS3 and CS6 only.

	// HR View
	int		iHRViewType;		// HR View: bpm = 0, HR%MAX = 1, HRR% = 2

	// Heart Touch
	int		iHeartTouch;		// Heart Touch in use: 1=Limits+light, 2=Take Lap+light, 3=Change display+Light, 4= Light, 5=Off

	// HR Alarm
	int		iHRAlarm;			// HR alarm in use: 0 = off, 1 = on
	
	// Altitude
	int		iAltitudeSensor;	// 0 = off, 1 = on
	int		iAltiCalFactFeet;	// Altitude Calibration Factor. Range -1800 - +29500 ft. Stored as actual value + 768. Value 0 if home altitude set.
	int		iAutoAlti;			// Automatic altitude calibration in the beginning of exercise: 0 = off, 1 = on

	// Shoe foodpod & Bike sensor
	int		iSpeedFoodpodSensor; // Speed measurement in use: 0 = off, 1 = on
	int		iShoeNumber;		// For RS: Shoe number, 0=Off, 1 = shoe1, 2 = shoe2
	int		iBikeNumber;		// For CS: Bike number, 0=Off, 1 = bike1, 2 = bike2, 3 = bike3 (for CS600)

	// Speed View Type
	int		iSpeedViewType;		// Speed view: Pace = 0, Speed = 1, Horse = 2.

 	// Autolap
	int		iAutolap;			// Automatic lap in use: 0 = off, 1 = on
	int		iAutolapDistanceMeter;	// meters
			
	// Calibration Factor for Footpod
	int		iFootpodCalFact;	// Calibration Factor. Range 0.500 - 1.500. Actual value * 1000. 
	int		iFootpodFix;		// Footpod position, 0 = shoe laces / 1 =  integrated

	// Sensors
	int		iCadenceSensor;		// 0 = off, 1 = on
	int		iPowerSensor;		// 0 = off, 1 = on

	// GPS
	int		iGPS;

	// Max HR to be used , 100-240 bpm, if this zero then original MaxHr is used
	int		iMaxHRtoBeUsed;
	
} POLAR_RSCS_SET_SPORTPROFILE;

////////////////////////
////////////////////////
//
// DYNAMIC EXE PHASE
//
////////////////////////
////////////////////////

// Phase information for completing Dynamic Exercise Set

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iInUse;				// Phase is in use = 1, Phase is not in use 0.
	int		iActivePhase;		// Phase is the first active one = 1, Phase is not active one 0.

	TCHAR   szName[LENGTH_OF_NAME_TEXT + 1];
	int		iGuideType;			// PPT & Product Dynamic, PPT Zoned: 0 = Off, 1 = Manual, 2 = Timer, 3 = Distance, 4 = Increasing HR, 5 = Decreasing HR
								// Product Zoned: 0 = Off, 1 = Timer, 2 = Distance
	BOOL 	bAutoStart;			// Automatic start of phase. Auto = TRUE, Manual = FALSE. By default = TRUE.
	int		iZoneType;			// PPT Dynamic & Zoned: 0 = Off, 1 = Free, 2 = SportZone, 3 = HR, 4 = %HR, 5 = %HRR, 6 = Pace, 7 = Speed, 8 = Cadence, 9 = Power, 10 = OwnZone
										// TODO: Remove -> PPT Zoned: 0 = Off, 1 = HR , 2 = speed, 3 = pace, 4 = cadence, 5 = power, 6 = HR OwnZone, 7 = HR SportZone
								// Product Dynamic & Zoned: 0 = HR, 1 = Pace, 2 = Speed, 3 = Cadence, 4 = Power.
								// Zone type if HR selected (in product 0 = off -> not can use!)
								// Product: 1=manual, 2=sportzone, 3=ownzone (only zoned) (OwnZone limits can be used only in Ownzone exercise)
	int		iHRType;			// PPT & Product, HR Format: 0 = bpm, 1 = % of max HR, 2 = % of HRR
	int		iRepeats;			// Number of phase repeats. Range 1 - 30. By default 1?.
	int		iRepeatTo;			// The immediate Next or some earlier Phase to jump to  
	
	// Estimated speed for phase preview editing. Note! this values will be lost when exercise read from product!
	int		iEstimateSpeed;

	// Guide
	// HR Guided phase
	int		iGuideHRbpm;		// Range 15 - 240 bpm.

	// Time Guided phase
	int		iGuideTimeSecs;		// Phase guide seconds. Range 0:00:10 - 1:39:59 (99:59). Value stored as 1-5999 secs. Default ?.

	// Distance Guided phase
	int		iGuideDistMeters;	// Phase guide in distance. Range 0.10 - 99.90 km. Value stored as 100-99900. Default ?.

	// Zoned specific data
	BOOL	bDurationEnabled;	// Duration: Timer/distance enabled TRUE/FALSE
	BOOL	bLimitEnabled;		// Limits enabled

	// Zone Limits
	int		iLimitsLower;			// Range ?. By default ?
	int		iLimitsUpper;			// Range ?. By default ?

} POLAR_RSCS_DYNEXESET_PHASE;

////////////////////////
////////////////////////
//
// DYNAMIC EXERCISE SET
//
////////////////////////
////////////////////////

// Exercise Set information will be send to monitor one set at a time.
// Exercise Set can be set as an active set to monitor (i.e. set will be shown
// as the first set when next time starting exercise).

typedef struct
{
	int		iMonitorType;		// HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600

	// Exercise Set Type
	int		iExeID;				// ID number given by application, this will be saved to REC_EXERCISE_ID of realized exercise
								// Can be used for example for sport identification

	// Exercise Type
	int		iExeType;			// Exercise Type: 0 = Free, 1 = Zoned (static), 5 = Phased (dynamic)
	
	// Planned exercise set
	TCHAR	szName[LENGTH_OF_NAME_TEXT + 1];	// Exercise set name (see Polar Character Set)
												// String can be checked using function fnHRMCom_CheckPolarCharStringEx
												// Max number of characters is 10 + ending zero

	TCHAR	szDescription[LENGHT_OF_DESCRIPTION_TEXT + 1];	// Exercise set name (see Polar Character Set)
															// String can be checked using function fnHRMCom_CheckPolarCharStringEx
															// Max number of characters is 10 + ending zero

	// Planned exercise date and time
	int     iExeDateYyyymmdd;	// Planned Date of exercise in format yyyymmdd, Jan 1 2000 - Dec 31 2020
	int		iExeStartTimeSecs;	// Planned StartTime of exercise in seconds from midnight (0:00:00), max 23:59:59 = 86399 sec

	// Integrated reminder
	int		iRemRepeat;			// Occurrence: Off = 0, Once = 1, Hourly = 2, Daily = 3, Weekly = 4, Monthly = 5, Yearly = 6. By default = 0.
	int		iRemMode;			// Appearence mode. At preset time = 0, 10 min before = 1, 30 min before = 2, 60 min before = 3, 24h before = 4. By default = 0.
	int		iRemVolume;			// Volume. Normal = 0, Beep = 1, Silent = 2. By default = 0.

	// Targets
	int		iTargetTimeSecs;		// Target Duration of exercise Range 0 - 99 hours. By default = 00?.
	int		iTargetDistanceMeters;	// Target Distance of exercise in kms. Range 0 - 6553.5 kms or miles. By default = 0?.
	int		iTargetCalories;		// Target Calory consumption of exercise in kCal. Range 0 - 65535 kCal. By default = ?.

	int		iProductZoneAmount;
	int     iTargetSZTimesSecs[NUMBER_OF_SPORTZONES];	// Target Sport Zone Times of exercise, 10 pcs, Range 00:00 - hh:mm?, By default ?

	// Phases
	int		iPhasesInUse;		// Number of Phases in this exercise set. Range 1 - 12 pcs. By default 1?

    // Phase Structure
	POLAR_RSCS_DYNEXESET_PHASE Phase[NUMBER_OF_PHASES];
	
	// Sport Profile Structure (dynamic exercise only)
	POLAR_RSCS_SET_SPORTPROFILE SportProfile;

} POLAR_RSCS_DYNEXESET;

typedef struct
{
	int		iMonitorType;		// HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600

	BOOL	bHorseEdition;
	
	int		iDynamicSetAmount;	// Total amount of dynamic exerices sets
								// Program: max 21. Favourites: max 10

	int		iZonedSetAmount;	// Total amount of zoned exerices sets
								// Program: max 0. Favourites: max 10

								// Note: At Favourites iDynamicSetAmount + iZonedSetAmount <= 10

	POLAR_RSCS_DYNEXESET DynExeSet[NUMBER_OF_DYNEXESETS];

	BOOL	bCalculateTargets;	// Reserved for future use. Calculate daily and weekly targets from exercises so
								// not need send these values separately, expect weekly name and description
								// must get some where and 21 exercises can limit's daily and weekly targets
								// under 9 weeks

} POLAR_RSCS_SET_EXERCISESETS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_SendExerciseSettings	(POLAR_SSET_GENERAL*, POLAR_RSCS_SET_EXERCISESETS*, int ); // iType; 0 = plans, 1 = favourites
__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_ReadExerciseSettings	(POLAR_SSET_GENERAL*, POLAR_RSCS_SET_EXERCISESETS*, int ); // iType; 0 = plans, 1 = favourites


////////////////////////
////////////////////////
//
// WEEKLY TARGETS
//
////////////////////////
////////////////////////

typedef struct
{
	TCHAR	szName[LENGTH_OF_NAME_TEXT + 1];	// User settable name of the training week (see Polar Character Set)
												// Max number of characters is 10 + ending zero

	TCHAR	szDescription[LENGHT_OF_DESCRIPTION_TEXT + 1];	// User settable description of the training week (see Polar Character Set)
															// Max number of characters is 20 + ending zero
	// Targets
	int		iTargetTimeSecs;		// Target Duration of exercise Range 0 - 99 hours
	int		iTargetDistanceMeters;	// Target Distance of exercise in kms. Range RS 0 - 6553.5 km, CS 0 - 65535 kms
	int		iTargetCalories;		// Target Calory consumption of exercise in kCal. Range 0 - 65535 kCal
	int     iTargetSZTimesSecs[NUMBER_OF_SPORTZONES];	// Target Sport Zone Times of exercise, 10 pcs, Range 00:00 - hh:mm?

} POLAR_RSCS_WEEKLYTARGET;

typedef struct
{
	int		iMonitorType;		// HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600
	int		iWeekAmount;		// Total amount of week targets, 0 - 9 weeks

	POLAR_RSCS_WEEKLYTARGET Week[NUMBER_OF_WEEKLYTARGETS];

} POLAR_RSCS_WEEKLYTARGETS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_SendWeeklyTargets	(POLAR_SSET_GENERAL*, POLAR_RSCS_WEEKLYTARGETS*);

////////////////////////
////////////////////////
//
// DAILY TARGETS
//
////////////////////////
////////////////////////

typedef struct
{
	// Targets
	int		iTargetTimeSecs;		// Target Duration of exercise Range 0 - 99 hours. By default = 00?.
	int		iTargetDistanceMeters;	// Target Distance of exercise in kms. Range RS 0 - 6553.5 km, CS 0 - 65535 kms
	int		iTargetCalories;		// Target Calory consumption of exercise in kCal. Range 0 - 65535 kCal. By default = ?.
	int     iTargetSZTimesSecs[NUMBER_OF_SPORTZONES];	// Target Sport Zone Times of exercise, 10 pcs, Range 00:00 - hh:mm?, By default ?

} POLAR_RSCS_DAILYTARGET;

typedef struct
{
	int		iMonitorType;		// HRM_RS400, HRM_RS800, HRM_RS800CX, HRM_CS400, HRM_CS600
	int		iDayAmount;			// Total amount of day targets, 0 - 63 days

	POLAR_RSCS_DAILYTARGET Day[NUMBER_OF_DAILYTARGETS];

} POLAR_RSCS_DAILYTARGETS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_SendDailyTargets	(POLAR_SSET_GENERAL*, POLAR_RSCS_DAILYTARGETS*);

#ifdef __cplusplus
}
#endif
