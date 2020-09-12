using UnityEngine;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    void Start()
    {
        SkeletalTrackingProvider m_skeletalTrackingProvider = new SkeletalTrackingProvider();

        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider.StartClientThread(TRACKER_ID);
    }
}
