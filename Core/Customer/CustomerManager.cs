using SuspiciousGames.SellMe.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SuspiciousGames.SellMe.Core.Customer
{
    public class CustomerManager : MonoBehaviour
    {
        #region Singleton
        private static CustomerManager _instance;
        public static CustomerManager Instance => _instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }
        #endregion

        #region Private exposed fields
        [Header("ManagerVariables")]
        [SerializeField]
        private int _customerCap;
        [SerializeField]
        private float _timeBetweenSpawns;
        [SerializeField]
        private Transform _spawnPoint;
        [Header("Prefabs")]
        [SerializeField]
        private GameObject _customerPrefab;
        [Header("Shop Time")]
        [SerializeField]
        private float _minShopTime;
        [SerializeField]
        private float _maxShopTime;
        [Header("Customer Traits")]
        [SerializeField]
        private int _minCustomerTraits;
        [SerializeField]
        private int _maxCustomerTraits;
        [SerializeField]
        private List<CustomerTrait> _customerTraits;
        #endregion

        #region Private fields
        private List<Customer> _customers;
        #endregion

        #region Properties
        public Transform SpawnPoint => _spawnPoint;
        #endregion

        #region MonoBehaviour
        private void OnEnable()
        {
            EventManager.Subscribe(GameEventID.CustomerLeft, OnCustomerLeft);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(GameEventID.CustomerLeft, OnCustomerLeft);
        }
        #endregion

        #region Public methods
        public void Init()
        {
            _customers = new List<Customer>();
            StartCoroutine(CustomerSpawnRoutine());
        }

        public void Finalize(Action finalizeCallback)
        {
            finalizeCallback.Invoke();
            StopAllCoroutines();
            StartCoroutine(CustomerFinalizeRoutine(finalizeCallback));
        }
        #endregion

        #region Private methods
        private IEnumerator CustomerFinalizeRoutine(Action finalizeCallback)
        {
            foreach (var customer in _customers)
                customer.LeaveShop(_spawnPoint.position);

            yield return new WaitUntil(() => { return _customers.Count == 0; });
            finalizeCallback.Invoke();
            Destroy(_instance.gameObject);
        }

        private IEnumerator CustomerSpawnRoutine()
        {
            while (GameManager.Instance.GameState == GameState.Selling)
            {
                if (_customers.Count < _customerCap)
                {
                    float pSpawn = 1 - (_customers.Count / _customerCap);
                    float r = UnityEngine.Random.value;
                    if (r <= pSpawn)
                    {
                        SpawnCustomer(out Customer c);
                    }
                }
                yield return new WaitForSeconds(_timeBetweenSpawns);
            }
        }

        private void SimulateBuyRoutine()
        {

        }

        private bool SpawnCustomer(out Customer c)
        {
            c = null;
            if (_customers.Count >= _customerCap)
                return false;
            NavMesh.SamplePosition(_spawnPoint.position, out NavMeshHit hit, 1, 1);
            var position = new Vector3(hit.position.x, hit.position.y, 0);
            c = Instantiate(_customerPrefab, position, Quaternion.identity).GetComponent<Customer>();
            c.Init(_minShopTime, _maxShopTime, CustomerTraitGenerator.GenerateRandom(_customerTraits, _minCustomerTraits, _maxCustomerTraits));
            c.transform.parent = gameObject.transform;
            _customers.Add(c);
            return true;
        }
        #endregion

        #region Callbacks
        private void OnCustomerLeft(GameEvent arg0)
        {
            Customer c = arg0.Data[0] as Customer;
            _customers.Remove(c);
            Destroy(c.gameObject);
        }
        #endregion
    }
}