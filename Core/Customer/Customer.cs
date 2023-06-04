using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.Core.ShopSystem;
using SuspiciousGames.SellMe.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SuspiciousGames.SellMe.Core.Customer
{
    public class Customer : MonoBehaviour
    {
        #region Exposed private fields
        [SerializeField]
        private int _numOfItemsToLookAt = 3;
        [SerializeField]
        private Buypopup _buypopup;
        [SerializeField]
        private float _buyTime;
        #endregion

        #region Private fields
        private List<CustomerTrait> _customerTraits;
        private Item _itemToBuy;
        private float _chanceForSecondItem = 0.0f;
        private int _numOfItemBuys;
        private float _shopTime;
        private float _startShopTime = 0;
        private NavMeshAgent _agent;
        private Animator _animator;
        #endregion

        #region Properties
        public List<CustomerTrait> CustomerTraits => _customerTraits;
        public NavMeshAgent Agent => _agent;
        #endregion

        #region MonoBehaviour
        private void OnEnable()
        {
            EventManager.Subscribe(GameEventID.ShopAppearanceChanged, OnShopAppearanceChanged);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(GameEventID.ShopAppearanceChanged, OnShopAppearanceChanged);
        }
        #endregion

        #region Public methods
        public void Init(float minShopTime, float maxShopTime, List<CustomerTrait> customerTraits = null)
        {
            float minTime;
            float maxTime;

            if (minShopTime == 0 || maxShopTime == 0)
            {
                minShopTime = 10;
                maxShopTime = 20;
            }

            if (minShopTime > maxShopTime)
            {
                minTime = maxShopTime;
                maxTime = minShopTime;
            }
            else
            {
                maxTime = maxShopTime;
                minTime = minShopTime;
            }

            _animator = GetComponent<Animator>();

            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _startShopTime = Time.time;
            _numOfItemBuys = 0;

            _chanceForSecondItem = ShopManager.Instance.DiverseBonus;
            _shopTime = Random.Range(minTime, maxTime);
            if (_customerTraits == null)
                _customerTraits = new List<CustomerTrait>();
            else
                _customerTraits = customerTraits;

            _itemToBuy = ItemDecider.Decide(ShopManager.Instance.Shop.PresentedItems, this, _numOfItemsToLookAt);
            StartCoroutine(IdleToItemRoutine());
        }

        internal void LeaveShop(Vector3 postionToLeave)
        {
            StartCoroutine(LeaveShopRoutine(postionToLeave));
        }
        #endregion


        #region Private methods
        private IEnumerator IdleToItemRoutine()
        {
            Vector3 viewingPoint;
            _numOfItemBuys++;
            while (Time.time - _startShopTime < _shopTime)
            {
                //var presentedItems = ShopManager.Instance.Shop.PresentedItems;
                //int rIndex = Random.Range(0, presentedItems.Count - 1);
                //var item = presentedItems[rIndex];
                //viewingPoint = ShopManager.Instance.GetViewingPointForShowcase((Showcase)item.Container);
                if (NavMesh.SamplePosition(Vector3.zero + Random.insideUnitSphere * 1000, out NavMeshHit hit, 1000, 1))
                {
                    _agent.SetDestination(new Vector3(hit.position.x, hit.position.y, 0));
                    yield return new WaitUntil(() => { return !_agent.pathPending; });
                }
                while (_agent.remainingDistance > _agent.stoppingDistance)
                {
                    _animator.SetFloat("xVel", _agent.velocity.x);
                    _animator.SetFloat("yVel", _agent.velocity.y);
                    yield return new WaitForEndOfFrame();
                }
                //yield return new WaitUntil(() => { return _agent.remainingDistance <= _agent.stoppingDistance; });
            }
            if (_itemToBuy != null)
            {
                viewingPoint = ShopManager.Instance.GetViewingPointForShowcase(((Showcase)_itemToBuy.Container));
                if (NavMesh.SamplePosition(viewingPoint, out NavMeshHit hit, 100, 1))
                {
                    _agent.SetDestination(new Vector3(hit.position.x, hit.position.y, 0));
                    yield return new WaitUntil(() => { return !_agent.pathPending; });
                }
                yield return new WaitUntil(() => { return _agent.remainingDistance <= _agent.stoppingDistance; });
                StartCoroutine(ShowItemBuyUI());
            }
            else
            {
                StartCoroutine(LeaveShopRoutine(CustomerManager.Instance.SpawnPoint.position));
            }
        }

        private IEnumerator LeaveShopRoutine(Vector3 postionToLeave)
        {
            _agent.SetDestination(postionToLeave);
            yield return new WaitUntil(() => { return !_agent.pathPending; });
            while (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _animator.SetFloat("xVel", _agent.velocity.x);
                _animator.SetFloat("yVel", _agent.velocity.y);
                yield return new WaitForEndOfFrame();
            }
            //yield return new WaitUntil(() => { return _agent.remainingDistance <= _agent.stoppingDistance; });
            EventManager.TriggerEvent(GameEventID.CustomerLeft, this);
        }

        private IEnumerator ShowItemBuyUI()
        {
            _buypopup.gameObject.SetActive(true);
            bool itemBuy = false;
            bool timerEnded = false;
            _buypopup.onBuyButtonPressed += () => { ShopManager.Instance.Shop.BuyItem(_itemToBuy); itemBuy = true; };
            _buypopup.onTimerEndAction += () => { ShopManager.Instance.Shop.BuyItem(null); timerEnded = true; };
            _buypopup.Init(_itemToBuy.Category, _buyTime);
            yield return new WaitUntil(() => itemBuy || timerEnded);
            _buypopup.gameObject.SetActive(false);
            if (_numOfItemBuys < 2)
            {
                float r = Random.value;
                if (r <= _chanceForSecondItem)
                {
                    _itemToBuy = ItemDecider.Decide(ShopManager.Instance.Shop.PresentedItems, this, _numOfItemsToLookAt);
                    _startShopTime = Time.time;
                    StartCoroutine(IdleToItemRoutine());
                }
                else
                {
                    LeaveShop(CustomerManager.Instance.SpawnPoint.position);
                }
            }
            else
            {
                LeaveShop(CustomerManager.Instance.SpawnPoint.position);
            }
        }

        #endregion

        #region Callbacks
        private void OnShopAppearanceChanged(GameEvent arg0)
        {
            _chanceForSecondItem = ShopManager.Instance.DiverseBonus;
        }
        #endregion
    }
}
