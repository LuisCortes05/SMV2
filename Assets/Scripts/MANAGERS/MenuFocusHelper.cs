using UnityEngine;
using UnityEngine.EventSystems;

public class MenuFocusHelper : MonoBehaviour
{
    [SerializeField] private GameObject firstButtonPanel2;

    public void FocusNewPanel()
    {
        // Limpiar selección anterior y asignar el botón del nuevo panel
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(firstButtonPanel2);
    }
}