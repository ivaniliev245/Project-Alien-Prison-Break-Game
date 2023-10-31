using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class AlembicControl : MonoBehaviour
{
    public AlembicStreamPlayer alembicPlayer;
    public GameObject targetObject;
    public float maxDistance = 10f;
    public float timeScale = 1f;
    public bool invertAnimation = false;

    
    void Awake()
    {
        // Mark the AlembicStreamPlayer object as DontDestroyOnLoad so that it is not destroyed when the scene changes
        DontDestroyOnLoad(alembicPlayer);}
    
    
    
    
    
    void Update()
    {
        // Check if the application is playing (in runtime mode)
        
        if (alembicPlayer == null)
    {
        // The AlembicStreamPlayer object has been destroyed, so return
        return;
    }
        
        
        
        if (!Application.isPlaying)
            return;

        if (alembicPlayer == null || targetObject == null)
            return;

        float distance = Vector3.Distance(transform.position, targetObject.transform.position);
        float normalizedTime = Mathf.InverseLerp(0f, maxDistance, distance);
        normalizedTime = Mathf.Clamp01(normalizedTime);

        // Invert the animation if the boolean is set to true
        if (invertAnimation)
            normalizedTime = 1 - normalizedTime;

        // Set the time value of the Alembic animation based on the normalized time
        float timeValue = normalizedTime * alembicPlayer.Duration;
        alembicPlayer.CurrentTime = timeValue;

        // Adjust the playback speed using timeScale
        alembicPlayer.VertexMotionScale = timeScale;
    }
}
