#ifdef __cplusplus
    extern "C" {
#endif

////////////////////////
////////////////////////
//
// WATCH SETTINGS
//
////////////////////////
////////////////////////

typedef struct
{
	int		iMonitorType;

	// Time 1 and date are always taken from computer's real time watch.
	// The used hour mode 12h / 24h will be taken from Windows regional settings.

	int     iActiveTime;	// 0 = time1 active, 1 = time2 active
	int     iTime1HourMode; // Time1 hour mode: 0 = 24h, 1 = 12h
	int		iTime2HourMode; // Time2 hour mode: 0 = 24h, 1 = 12h

	int		iAlarmType;		// Alarm type: 0 = off, 1 = once, 2 = daily, 3 = mon-fri
	int		iAlarmTime;		// Alarm time in seconds from midnight
	BOOL	bAlarmEnabled;	// FALSE = off, TRUE = on

	int		iTime2Diff;		// Time 2 difference to time 1, value * 30 min, value can be positive or negative
	BOOL	bTime2InMessage;// Time 2 value is to be written to HRM

	BOOL	bEvent;			// Event in use
	int		iEventDate;
	TCHAR	tcEventName[10];

} POLAR_RSCS_WATCH_SETTINGS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_SendWatchSettings (POLAR_SSET_GENERAL*, POLAR_RSCS_WATCH_SETTINGS*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_RSCS_ReadWatchSettings (POLAR_SSET_GENERAL *psg, POLAR_RSCS_WATCH_SETTINGS *prcw);

////////////////////////
////////////////////////
//
// EXERCISE SETTINGS for CS100 and CS200
//
////////////////////////
////////////////////////

typedef struct
{
	// Time 1 and date are always taken from computer's real time watch.
	// The used hour mode 12h / 24h will be taken from Windows regional settings.

	int		iMonitorType;

	// Timers
	BOOL	bTimer1Enabled;		// Timer 1 enabled TRUE/FALSE
	int		iTimer1;			// Timer 1 value in seconds, max 99 min 59 sec (= 5999 sec)
	BOOL	bTimer2Enabled;		// Timer 2 enabled TRUE/FALSE
	int		iTimer2;			// Timer 2 value in seconds, max 99 min 59 sec (= 5999 sec)

	// Limits
	int		iLimitType;			// 0 = Limits off, 1 = Manual limits, 2 = OwnZone

	int		iHRLimitType;		// 0 = bpm, 1 = % max HR
	int		iHRLimitUpper;		// HR Limit upper value 30 - 240 bpm or 0 -100 %
	int		iHRLimitLower;		// HR Limit lower value 30 - 240 bpm or 0 -100 % (must be less than upper limit)

	int		iOwnZone;			// 0 = Basic, 1 = Low, 2 = Moderate, 3 = Hard

	// Displays
	BOOL	bDisplayOn[6];		// Selection for each display ON = TRUE / OFF = FALSE
	int		iDisplayMatrix[6];	// Matrix row selection for each display:
								//  1 = Distance/Trip
								//  2 = Avg/max speed
								//  3 = Calorie
								//  4 = Estimated Arrival Time
								//  5 = Average Hr

} POLAR_CSLE_EXERCISE_SETTINGS;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_CSLE_SendExerciseSettings (POLAR_SSET_GENERAL*, POLAR_CSLE_EXERCISE_SETTINGS*);

////////////////////////
////////////////////////
//
// BIKE SETTINGS for CS100 and CS200
//
////////////////////////
////////////////////////

__declspec (dllexport) BOOL CALLBACK fnHRMCom_SendBikesSettingsCSLE (POLAR_SSET_GENERAL*, int iMonitor, int iOdometer1, int iOdometer2, int iDate);

////////////////////////////////////////////////////////////////////////////////
// Note: To read data from RS200, CS200, use function fnHRMCom_ReadSonicLinkData
////////////////////////////////////////////////////////////////////////////////

// Fitness tests (RS200)
// Use fuctions from hrmcomFS.h

//Weekly summary
// RS200
#define		WEEK_SUMMARY_SPORT_ZONE1_TIME	6001		// Time in SportZone1
#define		WEEK_SUMMARY_SPORT_ZONE2_TIME	6002		// Time in SportZone2
#define		WEEK_SUMMARY_SPORT_ZONE3_TIME	6003		// Time in SportZone3
#define		WEEK_SUMMARY_SPORT_ZONE4_TIME	6004		// Time in SportZone4
#define		WEEK_SUMMARY_SPORT_ZONE5_TIME	6005		// Time in SportZone5
#define		WEEK_SUMMARY_EXERCISE_TIME		6006		// Exercise time of week
#define		WEEK_SUMMARY_EXERCISE_DISTANCE	6007		// Exercise distance of week
#define		WEEK_SUMMARY_EXERCISE_COUNT		6008		// Exercise count of week
#define		WEEK_SUMMARY_EXERCISE_CALORIES	6009		// Exercise calories of week

__declspec (dllexport) int CALLBACK fnHRMCom_GetWeekSummaryCount (int); //Monitor ID
__declspec (dllexport) int CALLBACK fnHRMCom_GetWeekSummaryValue (int,int,int); //Monitor ID,week number,summary ID


// User settings
#define		USER_SETTING_WEIGHT				7001		// User weight in lbs, 44 - 439 lbs
#define		USER_SETTING_HEIGHT				7002		// User height in cm, 100 - 211cm
#define		USER_SETTING_BIRTHBAY			7003		// User birthday 
#define		USER_SETTING_HRMAX				7004		// Maximum heart rate of the user, 0,100 - 240 bpm
#define		USER_SETTING_VO2MAX				7005		// User VO2max value 
#define		USER_SETTING_HRSIT				7006		// User Hrsit
#define		USER_SETTING_ACTIVITY			7007		// User activity level, 0=Top,1=high, 2=moderate, 3=Low
#define		USER_SETTING_SEX				7008		// User sex, Male = 0 and female = 1

__declspec (dllexport) int CALLBACK fnHRMCom_GetUserSetting (int,int); //Monitor ID, Setting ID

///////////////////
// Exercise
///////////////////

__declspec (dllexport) void  CALLBACK fnHRMCom_SportExeName (TCHAR*,int); //ExerciseName, exeNum (RS200 only)

#ifdef __cplusplus
}
#endif
