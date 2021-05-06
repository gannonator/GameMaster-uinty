using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;

namespace GameMaster
{
    /// <summary>
    /// Simple Locking system 
    /// </summary>
    public static class LoadingLocks
    {
        /// <summary>
        /// A struct that contains information about each call for a loading lock.
        /// Implemented for debug reasons.
        /// </summary>
        private struct LockInformation
        {
            public string CallingMethodName;
            public string CallingMethodFile;
           // public Guid LockID;

            public LockInformation(string callingMethodName, string callingMethodFile) => (CallingMethodName, CallingMethodFile) = (callingMethodName, callingMethodFile);
        }


        public static int LockCount { get { return _lockCount; } }

        /// <summary>
        /// Number of jobs to be done or Locks acquired 
        /// </summary>
        static int _lockCount = 0;

        private static Dictionary<Guid,LockInformation> Locks = new Dictionary<Guid, LockInformation>();


        /// <summary>
        /// Is Loading Locked
        /// </summary>
        /// <returns></returns>
        public static bool IsLocked()
        {
            return (_lockCount > 0);
        }

        /// <summary>
        /// Set a job lock on loading
        /// </summary>
        public static Guid Lock()
        {
            StackTrace stack = new StackTrace(true);
            StackFrame lastCall = stack.GetFrame(1);

            string callingMethodName = lastCall.GetMethod().Name;
            string callingMethodFile = lastCall.GetFileName();
            Guid newID = Guid.NewGuid(); 
            //UnityEngine.Debug.Log(callingMethodName + " " + callingMethodFile + " " + newID);
            Locks.Add(newID, new LockInformation(callingMethodName, callingMethodFile));
            _lockCount++;

            return newID;
        }

        /// <summary>
        /// Remove a job lock
        /// </summary>
        public static void Unlock(Guid lockID)
        {
            //UnityEngine.Debug.Log(lockID);
            Locks.Remove(lockID);
            
            //Debug.Log(Environment.StackTrace);
            _lockCount--;

            if (_lockCount < 0)
            {
                UnityEngine.Debug.LogError("Extra unlocks called");
                _lockCount = 0;
            }
        }
    }
}
