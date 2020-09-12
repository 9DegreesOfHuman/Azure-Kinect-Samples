using UnityEngine;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    void Start()
    {
        SkeletalTrackingProvider m_skeletalTrackingProvider = new SkeletalTrackingProvider();
        m_skeletalTrackingProvider.StartClientThread();
    }
}
