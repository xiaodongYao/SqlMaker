using System.Threading;

namespace sqlmake
{
    internal class ThreadLogFile
    {
        public ThreadLogFile()
        {
            new Thread(CheckMsg).Start();
        }

        public void CheckMsg()
        {
            Logger.Instance().Process();
        }
    }
}