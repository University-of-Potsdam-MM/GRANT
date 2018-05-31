using System;

namespace TactileWeb
{

    public enum ActionThreadState
    { 
        /// <summary>0 = Not running</summary>
        Stopped  = 0, 

        /// <summary>1 = It is starting</summary>
        Starting = 1,

        /// <summary>2 = Running</summary>
        Running  = 2,

        /// <summary>3 = Aborting</summary>
        AbortRequested  = 3,
    }


        //Running = 0,
        //StopRequested = 1,
        //SuspendRequested = 2,
        //Background = 4,
        //Unstarted = 8,
        //Stopped = 16,
        //WaitSleepJoin = 32,
        //Suspended = 64,
        //AbortRequested = 128,
        //Aborted = 256,




//Member name

//Description

// ContinuePending The service continue is pending. This corresponds to the Win32 SERVICE_CONTINUE_PENDING constant, which is defined as 0x00000005. 
// Paused The service is paused. This corresponds to the Win32 SERVICE_PAUSED constant, which is defined as 0x00000007. 
// PausePending The service pause is pending. This corresponds to the Win32 SERVICE_PAUSE_PENDING constant, which is defined as 0x00000006. 
// Running The service is running. This corresponds to the Win32 SERVICE_RUNNING constant, which is defined as 0x00000004. 
// StartPending The service is starting. This corresponds to the Win32 SERVICE_START_PENDING constant, which is defined as 0x00000002. 
// Stopped The service is not running. This corresponds to the Win32 SERVICE_STOPPED constant, which is defined as 0x00000001. 
// StopPending The service is stopping. This corresponds to the Win32 SERVICE_STOP_PENDING constant, which is defined as 0x00000003. 




}