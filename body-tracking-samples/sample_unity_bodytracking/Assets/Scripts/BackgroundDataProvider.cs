public abstract class BackgroundDataProvider
{
    // protected volatile bool m_runBackgroundThread;
    private BackgroundData m_frameBackgroundData = new BackgroundData();
    public abstract void StartClientThread(int id);

    protected abstract void RunBackgroundThreadAsync(int id);
}
