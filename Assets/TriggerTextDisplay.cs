using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerTextDisplay : MonoBehaviour
{
  public TextMeshProUGUI displayText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            displayText.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            displayText.gameObject.SetActive(false);
        }
    }
}
