using System;
using System.Threading.Tasks;

public abstract class BackgroundDataProvider
{
    // protected volatile bool m_runBackgroundThread;
    private BackgroundData m_frameBackgroundData = new BackgroundData();
    // public bool IsRunning { get; set; } = false;

    public abstract void StartClientThread(int id);

    protected abstract void RunBackgroundThreadAsync(int id);

    // public void StopClientThread()
    // {
    //     UnityEngine.Debug.Log("Stopping BackgroundDataProvider thread.");
    //     // m_runBackgroundThread = false;
    // }
}
