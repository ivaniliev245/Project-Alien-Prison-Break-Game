using UnityEngine;

public class DynamicColliderAttachment : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public string referenceBoneName; // Assign the name of the bone to use as a reference

    private void Update()
    {
        if (skinnedMeshRenderer != null && !string.IsNullOrEmpty(referenceBoneName))
        {
            // Find the reference bone by name
            Transform referenceBone = FindBoneByName(referenceBoneName);

            if (referenceBone != null)
            {
                // Update the collider's position to match the reference bone
                transform.position = referenceBone.position;
            }
        }
    }

    private Transform FindBoneByName(string boneName)
    {
        if (skinnedMeshRenderer != null)
        {
            Transform[] bones = skinnedMeshRenderer.bones;
            foreach (Transform bone in bones)
            {
                if (bone.name == boneName)
                {
                    return bone;
                }
            }
        }
        return null;
    }
}
