using SuspiciousGames.SellMe.Core.Adventures;
using TMPro;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class AdventureCard : MonoBehaviour
    {
        private static AdventureMenu _adventureMenu;
        private static PlayMakerFSM _guiFSM;

        #region Private exposed fields
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _goldCostText;
        [SerializeField]
        private TextMeshProUGUI _gameTimeCostText;
        [SerializeField]
        private TextMeshProUGUI _stepGoalText;
        [SerializeField]
        private TextMeshProUGUI _buttonText;
        #endregion

        #region Private fields
        private AdventureData _adventureData;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            if (_adventureMenu == null)
                _adventureMenu = FindObjectOfType<AdventureMenu>();
            if (_guiFSM == null)
            {
                foreach (var fsm in PlayMakerFSM.FsmList)
                {
                    if (fsm.FsmName == "GUI_FSM")
                    {
                        _guiFSM = fsm;
                        break;
                    }
                }
            }
        }
        #endregion

        #region Public methods
        public void Init(AdventureData adventureData)
        {
            _adventureData = adventureData;
            if (_nameText != null)
                _nameText.text = _adventureData.Name;
            _descriptionText.text = _adventureData.Decsription;
            _goldCostText.text = _adventureData.GoldCost.ToString();
            _gameTimeCostText.text = GameTime.ConvertToTimeData(_adventureData.TimeCost);
            _stepGoalText.text = _adventureData.Length.maxSteps.ToString();
            _buttonText.text = "Take Route " + _adventureData.Length.length.ToString();
        }
        public void Preselect()
        {
            _adventureMenu.SelectedAdventureData = _adventureData;
            _guiFSM.SendEvent("dialogue");
        }
        #endregion
    }
}