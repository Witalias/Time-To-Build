using Gameplay.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BuildingPanel
{
    [RequireComponent(typeof(Animator))]
    public class BuildingPanel : MonoBehaviour
    {
        private const string SHOW_ANIMATOR_BOOL = "Show";

        [SerializeField] private GameObject _baseLots;
        [SerializeField] private GameObject _marketLots;
        [SerializeField] private GameObject _lotPrefab;
        [SerializeField] private BuildingType[] _baseBuildings;

        private Animator _animator;
        private BuildingArea _selectedArea;

        public bool BuildingSiteSelected { get => _selectedArea != null; }

        public static event Func<GameObject[]> GetScreenRaycastResults;
        public static event Action RemoveBuildingAreaSelection;
        public static event Func<int> GetMoney;
        public static event Action<ResourceType, int> AddMoney;
        public static event Action UpdateLotColors;

        public void Show(BuildingArea selectedArea)
        {
            _selectedArea = selectedArea;
            _animator.SetBool(SHOW_ANIMATOR_BOOL, true);
            UpdateLotColors?.Invoke();
        }

        public void Hide()
        {
            _selectedArea = null;
            _animator.SetBool(SHOW_ANIMATOR_BOOL, false);
            RemoveBuildingAreaSelection?.Invoke();
        }

        public void BuildOnSelectedArea(BuildingType buildingType, int cost)
        {
            var money = GetMoney();
            if (_selectedArea == null || money < cost)
                return;

            _selectedArea.StartBuilding(buildingType);
            Hide();
            AddMoney?.Invoke(ResourceType.Money, -cost);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            foreach (var type in _baseBuildings)
            {
                var lot = Instantiate(_lotPrefab, _baseLots.transform);
                lot.GetComponent<BuildingLot>().Set(type);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hits = GetScreenRaycastResults?.Invoke();
                if (hits.Length == 0)
                {
                    Hide();
                    return;
                }
                foreach (var obj in hits)
                {
                    if (obj.GetComponent<UIElement>() != null)
                        return;
                }
                Hide();
            }
        }
    }
}