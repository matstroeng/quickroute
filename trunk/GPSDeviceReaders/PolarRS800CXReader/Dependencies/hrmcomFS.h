#ifdef __cplusplus
    extern "C" {
#endif

////////////////////////
////////////////////////
//
// READ DATA USING SONICLINK
//
////////////////////////
////////////////////////

#define		NOT_SUPPORTED	-1677216

typedef struct
{
	int		iTotalTarget;
	int		iTargetEnergy;					// 0 - 65000 kcal, nearest 50 kcal
	int		iLightZonetime;					// 1 - 600 minutes, nearest 5 minute
	int		iModerateZonetime;				// 1 - 600 minutes, nearest 5 minute
	int		iHardZonetime;					// 1 - 600 minutes, nearest 5 minute
	int		iNumberOfExercises;				// 2 - 7
	int		iLowLimit;						// 1 - 600
	int		iHighLimit;						// 1 - 600
	int		iEnergyExpenditureLight;		// 0 - 65000 kcal
	int		iEnergyExpenditureModerate;		// 0 - 65000 kcal
	int		iEnergyExpenditureHard;			// 0 - 65000 kcal
	int		iTargetType;					// 0=maintain 1=improve 2=maximize, -1=program off, Old F11 = NOT_SUPPORTED

} POLAR_PROGRAM_SUMMARY;

typedef struct
{
	int		iDuration;						// 10 - 600 min
	int		iEnduranceTime;					// 10 - 600 min
	int		iPowerTime;						// 10 - 600 min
	int		iMaximumTime;					// 10 - 600 min
	int		iCalories;						// 0 - 65000 kcal, nearest 50 kcal
	TCHAR	sExerciseName[8];
	int		iType;							// 0 = short, 1 = normal, 2 = long
	int		iHRset;							// 0 = OwnZone, 1 = General, 2 = Silent

} POLAR_PROGRAM_FILE;

typedef struct
{
	int						iMonitor;			// HR Monitor type
	POLAR_PROGRAM_SUMMARY	CardioSummary;		// Cardio program weekly summary
	POLAR_PROGRAM_FILE		CardioFile[7];		// Cardio exercise plans

} POLAR_FSSET_CARDIOPROGRAM;

// Functions
__declspec (dllexport) BOOL  CALLBACK		fnHRMCom_GetProgramSummary		(POLAR_PROGRAM_SUMMARY*);
__declspec (dllexport) BOOL  CALLBACK		fnHRMCom_GetProgramFileInfo		(int, POLAR_PROGRAM_FILE*);

__declspec (dllexport) int  CALLBACK		fnHRMCom_GetFitnessDataType		(void);

__declspec (dllexport) int  CALLBACK		fnHRMCom_GetFitnessFileCount	(void);

__declspec (dllexport) int  CALLBACK		fnHRMCom_GetFitnessTestCount	(void);
__declspec (dllexport) int  CALLBACK		fnHRMCom_GetFitnessTestDate		(int);
__declspec (dllexport) int  CALLBACK		fnHRMCom_GetFitnessTestValue	(int);
__declspec (dllexport) int  CALLBACK		fnHRMCom_GetOwnRelaxTestDate	(int);
__declspec (dllexport) int  CALLBACK		fnHRMCom_GetOwnRelaxTestValue	(int);

__declspec (dllexport) void  CALLBACK		fnHRMCom_GetFitnessExeName		(TCHAR*,int);

__declspec (dllexport) int CALLBACK			fnHRMCom_ReadF55ExerciseFiles	(HWND);
__declspec (dllexport) BOOL CALLBACK		fnHRMCom_AnalyzeFile_Fitness	(int);
__declspec (dllexport) int CALLBACK			fnHRMCom_ReadF55TestResults		(HWND, int);	// 0 = Fitness Test, 1 = OwnRelax
__declspec (dllexport) int CALLBACK			fnHRMCom_ReadF55Program			(HWND);

__declspec (dllexport) BOOL CALLBACK		fnHRMCom_FS_SendCardioProgram	(POLAR_SSET_GENERAL*, POLAR_FSSET_CARDIOPROGRAM*);

////////////////////////
////////////////////////
//
// USER SETTINGS
//
////////////////////////
////////////////////////

//	User settings include information about the person.
//	All the settings are not available in all Polar FS HR monitors, see more details
//	from monitor specifications. If any data send to monitor is not supported, it will
//	be ignored automatically.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iMonitor;			// HR Monitor type

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
} POLAR_FSSET_USER;

__declspec (dllexport) BOOL CALLBACK		fnHRMCom_FS_SendUserSettings		(POLAR_SSET_GENERAL*, POLAR_FSSET_USER*);
__declspec (dllexport) BOOL CALLBACK		fnHRMCom_FS_PrepareUserSettings		(POLAR_FSSET_USER*);
__declspec (dllexport) void CALLBACK		fnHRMCom_FS_ResetUserSettings		(POLAR_FSSET_USER*);
__declspec (dllexport) BOOL CALLBACK		fnHRMCom_ReadF55UserSettings		(HWND, POLAR_FSSET_USER*);

////////////////////////
////////////////////////
//
// EXERCISE SETTINGS
//
////////////////////////
////////////////////////

//	All the settings are not available in all Polar FS HR monitors, see more details
//	from monitor specifications. If any data send to monitor is not supported, it will
//	be ignored automatically.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iMonitor;			// HR Monitor type

	// Information about exercise
	int		iHRLimitMode1;		// 0 = bpm, 1 = % HR max
	int		iHRLimitUpper1;		// HR Limit upper value 30 - 240 bpm or 0 - 100%
	int		iHRLimitLower1;		// HR Limit lower value 30 - 240 bpm or 0 - 100% (must be less than upper limit)

	int		iHRIntensity1;		// 0 = hard, 1 = moderate, 2 = light, 3 = basic
	int		iHRLimitType1;		// 0 = off, 1 = manual, 2 = automatic, 3 = ownzone
	int		iHRAlarm1;			// 0 = off, 1 = vol1, 2 = vol2

} POLAR_FSSET_EXERCISE;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_FS_SendExerciseSettings (POLAR_SSET_GENERAL*, POLAR_FSSET_EXERCISE*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_FS_PrepareExerciseSettings (POLAR_FSSET_EXERCISE*);
__declspec (dllexport) void CALLBACK fnHRMCom_FS_ResetExerciseSettings (POLAR_FSSET_EXERCISE*);

////////////////////////
////////////////////////
//
// WATCH SETTINGS
//
////////////////////////
////////////////////////

// The watch settings will be automatically got from computer's real time clock
// and Windows regional settings.

__declspec (dllexport) BOOL CALLBACK fnHRMCom_FS_SendWatchSettings (POLAR_SSET_GENERAL*);
__declspec (dllexport) BOOL CALLBACK fnHRMCom_FS_PrepareWatchSettings (int);

////////////////////////
////////////////////////
//
// MONITOR SETTINGS
//
////////////////////////
////////////////////////

//	All the settings are not available in all Polar FS HR monitors, see more details
//	from monitor specifications. If any data send to monitor is not supported, it will
//	be ignored automatically.

typedef struct
{
	int		iSize;				// Structure size for version control
								// Get using sizeof (STRUCTURE)

	int		iMonitor;			// HR Monitor type

	// General information about monitor
	BOOL	bUS_Units;			// Measurement units:	FALSE = EURO units, TRUE = US units
	int		iSounds;			// 0 = off, 1-2 = vol (F4/E40: 0 or 1 only)
	BOOL	bAutoKeyLock;		// Automatic key lock: TRUE / FALSE
	BOOL	bHelp;				// Feature help function enabled: TRUE/FALSE

} POLAR_FSSET_MONITOR;

__declspec (dllexport) BOOL CALLBACK fnHRMCom_FS_SendMonitorSettings (POLAR_SSET_GENERAL*, POLAR_FSSET_MONITOR*);

#ifdef __cplusplus
}
#endif
