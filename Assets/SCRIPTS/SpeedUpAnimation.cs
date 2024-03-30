using UnityEngine;

public class SpeedUpAnimation : MonoBehaviour
{
    public AnimationClip animClip; // Drag your animation clip into this slot in the Inspector
    public float speedMultiplier = 2.0f; // Adjust this value to change the speed of the animation

    void Start()
    {
        if (animClip != null)
        {
            Animation anim = GetComponent<Animation>();
            if (anim != null)
            {
                anim[animClip.name].speed = speedMultiplier;
            }
        }
    }
}
