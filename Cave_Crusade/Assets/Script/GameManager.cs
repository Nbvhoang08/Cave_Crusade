using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace Script
{
    [System.Serializable]
    public class ButtonConfig
    {
        public ButtonType type;
        public GameObject buttonPrefab;
        public int quantity;
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField]private int _countTurn;
        public int countTurn
        {
            get { return _countTurn; }
            set
            {
                if (_countTurn != value)
                {
                    _countTurn = value;
                    OnVariableChange(_countTurn);
                }
            }
        }
        public delegate void OnVariableChangeDelegate(int newVal);
        public event OnVariableChangeDelegate OnVariableChange;
        [SerializeField] public ButtonConfig[] buttonConfigs;

        public List<Enemies> enemies = new List<Enemies>();
        private void Awake()
        {
            OnVariableChange += HandleTurnChange;
            Debug.Log("ctrl");
        }

        private void HandleTurnChange(int newTurn)
        {
            if (enemies.Count >= 0)
            {
                foreach (var enemy in enemies)
                {
                    enemy.NextState();
                }
            }
            
        }


    }
}