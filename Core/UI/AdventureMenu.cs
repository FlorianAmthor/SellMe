using SuspiciousGames.SellMe.Core.Adventures;
using SuspiciousGames.SellMe.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class AdventureMenu : MonoBehaviour
    {
        #region Exposed private fields
        [SerializeField]
        private GameObject _adventureCardPrefab;
        [SerializeField]
        private GameObject _cardListObject;
        #endregion

        #region Private fields
        private List<AdventureData> _adventureDatas;
        private List<AdventureCard> _adventureCards;
        #endregion


        #region Properties
        public AdventureData SelectedAdventureData { private get; set; }
        #endregion

        #region MonoBehaviour
        private void OnEnable()
        {
            if (_adventureCards == null)
                _adventureCards = new List<AdventureCard>();

            _adventureDatas = AdventureGenerator.Instance.UpdateAdventureList();
            while (_adventureCards.Count > 0)
            {
                var card = _adventureCards[0];
                _adventureCards.RemoveAt(0);
                Destroy(card.gameObject);
            }

            foreach (var adventureData in _adventureDatas)
            {
                AdventureCard cardObject = Instantiate(_adventureCardPrefab).GetComponent<AdventureCard>();
                cardObject.Init(adventureData);
                _adventureCards.Add(cardObject);
                cardObject.transform.SetParent(_cardListObject.transform, false);
                //cardObject.transform.parent = _cardListObject.transform;
            }
        }
        #endregion

        #region Public methods
        public void StartAdventure()
        {
            ActiveAdventure adv = new ActiveAdventure(SelectedAdventureData);
            SaveGame.SaveData(SaveId.ActiveAdventure, adv);
            //TODO: Check if adventure has more uses left or the time for it hasn't run out
            _adventureDatas.Remove(SelectedAdventureData);
            SaveGame.SaveData(SaveId.AdventureList, _adventureDatas);
        }
        #endregion
    }
}

