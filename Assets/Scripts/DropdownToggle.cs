using UnityEngine;

public class DropdownToggle : MonoBehaviour
{
    public GameObject dropdownPanel;

    private bool isOpen = false;

    void Start()
    {
        dropdownPanel.SetActive(false);
    }

    public void ToggleDropdown()
    {
        isOpen = !isOpen;
        dropdownPanel.SetActive(isOpen);
    }
}