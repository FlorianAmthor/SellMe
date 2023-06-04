using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class ShowcaseUi : MonoBehaviour
    {
        #region Exposed private fields
        [SerializeField]
        private List<ShowcaseSlotUi> _slots;
        #endregion

        public void InitSlot(int slotIndex, Item item)
        {
            _slots[slotIndex].Init(item);
        }

        public void OnSlotContentChange()
        {
            foreach (var slot in _slots)
                slot.UpdateValues();
        }
    }
}