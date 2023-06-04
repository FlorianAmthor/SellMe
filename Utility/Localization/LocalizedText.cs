using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.SellMe.Utility.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        #region Exposed private fields
        [SerializeField] private string _key = null;
        #endregion
        void Start()
        {
            GetComponent<Text>().text = LocalizationManager.GetLocalizedValue(_key);
        }
    }
}