using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Script
{
    public enum ButtonType
    {
        Attack,
        Heal,
        Defense,
        Move
    }
  
    public class GamePlayCanvas : UICanvas
    {
        [Header("Button Configuration")]
        [SerializeField] private ButtonConfig[] buttonConfigs;
        [SerializeField] private float spacingBetweenButtons = 120f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Transform buttonsParent;
    
        [Header("Layout Settings")]
        [SerializeField] private float startX = -180f;
        [SerializeField] private float buttonY = 0f;

        [Header("Animation Settings")]
        [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private List<GameObject> allButtons = new List<GameObject>();
        [SerializeField] private List<GameObject> activeButtons = new List<GameObject>();
        private bool isCountingDown = true; 
        public Slider countdownSlider; // Slider đếm ngược
        private float turnDuration = 10f;  // Thời gian mỗi lượt (10 giây)
        private float turnTimer;  // Bộ đếm thời gian
        private int currentTurn = 0;
        private GameManager gameManager;
        public Text HpText;
        public Text Floor;
        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            buttonConfigs = gameManager.buttonConfigs;
        }
        private void Start()
        {
            InitializeButtonPool();
            SpawnInitialButtons();
            countdownSlider.maxValue = turnDuration; // Đặt giá trị tối đa cho slider
            StartNewTurn();
        }

        private void Update()
        {
            if (isCountingDown && turnTimer > 0)
            {
                turnTimer -= Time.deltaTime;
                countdownSlider.value = turnTimer; // Cập nhật giá trị của slider
            }

            gameManager.countTurn = currentTurn;
            
            // Hiển thị HP và maxHP theo định dạng 01/08
            HpText.text = Player.Instance.hp.ToString("D2") + "/" + Player.Instance.maxHp.ToString("D2");

            // Hiển thị tên của Scene hiện tại
            Floor.text = SceneManager.GetActiveScene().name;


        }
        void StartNewTurn()
        {
            isCountingDown = true; 
            turnTimer = turnDuration;
            countdownSlider.value = turnTimer; // Cập nhật slider với giá trị mới
            // Thêm code để khởi tạo trạng thái của turn mới nếu cần
        }

        // Hàm kết thúc lượt
        void EndTurn()
        {
         
            currentTurn++;
            StopCountdown();  // Bắt đầu lượt mới
        }

        // Hàm gọi khi Player click vào các button
        public void StopCountdown()
        {
            isCountingDown = false;
            StartCoroutine(ResetCountdown());
       
        }

        IEnumerator ResetCountdown()
        {
            yield return new WaitForSeconds(1f);
            if (Player.Instance.canDo)
            {
                StartNewTurn();
             
            }
            else
            {
                yield return new WaitForSeconds(2f);
                StartNewTurn();
                
            }

        }
    
    

    private void InitializeButtonPool()
    {
        foreach (var config in buttonConfigs)
        {
            for (int i = 0; i < config.quantity; i++)
            {
                GameObject newButton = Instantiate(config.buttonPrefab, buttonsParent);
                newButton.SetActive(false);
                
                Button buttonComponent = newButton.GetComponent<Button>();
                
                switch (config.type)
                {
                    case ButtonType.Attack:
                        buttonComponent.onClick.AddListener(AtkButton);
                        break;
                    case ButtonType.Defense:
                        buttonComponent.onClick.AddListener(DefButton);
                        break;
                    case ButtonType.Heal:
                        buttonComponent.onClick.AddListener(HealButton);
                        break;
                    case ButtonType.Move:
                        buttonComponent.onClick.AddListener(MoveButton);
                        break;
                }
                
                buttonComponent.onClick.AddListener(() => OnButtonClick(newButton));
                
                var buttonData = newButton.AddComponent<ButtonData>();
                buttonData.buttonType = config.type;
                
                allButtons.Add(newButton);
            }
        }
    }

    private void AtkButton()
    {
        Player.Instance.OnAttackButtonClick();
        EndTurn();
    }

    private void DefButton()
    {
        Player.Instance.OnDefendButtonClick();
        EndTurn();
    }

    private void HealButton()
    {
        Player.Instance.OnHealButtonClick();
        Debug.Log("Heal");
        EndTurn();
    }
    
    private void MoveButton()
    {
        Player.Instance.OnMoveButtonClick();
        Debug.Log("move");
        EndTurn();
    }

    private void SpawnInitialButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            SpawnButtonAtPosition(i);
        }
    }

    private void SpawnButtonAtPosition(int position)
    {
        // Lấy danh sách các button chưa active
        var inactiveButtons = allButtons.Where(b => !b.activeInHierarchy).ToList();
        if (inactiveButtons.Count == 0) return;

        // Random một button từ danh sách inactive
        int randomIndex = Random.Range(0, inactiveButtons.Count);
        GameObject button = inactiveButtons[randomIndex];
        
        float xPos = startX + (position * spacingBetweenButtons);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, buttonY);
        
        button.SetActive(true);
        button.transform.localRotation = Quaternion.Euler(0, 0, -180f);
        StartCoroutine(AnimateButtonSpawn(button));
        
        activeButtons.Add(button);
    }

    private IEnumerator AnimateButtonSpawn(GameObject button)
    {
        float elapsed = 0;
        Quaternion startRotation = button.transform.localRotation;
        Quaternion targetRotation = Quaternion.identity;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / animationDuration;
            float curveValue = rotationCurve.Evaluate(normalizedTime);
            
            button.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, curveValue);
            
            yield return null;
        }

        button.transform.localRotation = targetRotation;
    }

    private void OnButtonClick(GameObject clickedButton)
    {
        activeButtons.Remove(clickedButton);
        StartCoroutine(AnimateButtonDespawn(clickedButton));
    }

    private IEnumerator AnimateButtonDespawn(GameObject button)
    {
        float elapsed = 0;
        Quaternion startRotation = button.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 180f);

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / animationDuration;
            float curveValue = rotationCurve.Evaluate(normalizedTime);
            
            button.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, curveValue);
            
            yield return null;
        }

        
        button.SetActive(false);
        StartCoroutine(RepositionActiveButtons());
        yield return new WaitForSeconds(0.5f);
        SpawnButtonAtPosition(3);
    }

    private IEnumerator RepositionActiveButtons()
    {
        if (activeButtons.Count == 0) yield break;

        List<(GameObject button, Vector2 startPos, Vector2 targetPos)> buttonPositions = 
            new List<(GameObject, Vector2, Vector2)>();

        for (int i = 0; i < activeButtons.Count; i++)
        {
            var button = activeButtons[i];
            if (button != null && button.activeInHierarchy)
            {
                var rectTransform = button.GetComponent<RectTransform>();
                var startPos = rectTransform.anchoredPosition;
                var targetPos = new Vector2(startX + (i * spacingBetweenButtons), buttonY);
                buttonPositions.Add((button, startPos, targetPos));
            }
        }

        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / animationDuration;
            float curveValue = movementCurve.Evaluate(normalizedTime);

            foreach (var (button, startPos, targetPos) in buttonPositions)
            {
                if (button != null && button.activeInHierarchy)
                {
                    button.GetComponent<RectTransform>().anchoredPosition = 
                        Vector2.Lerp(startPos, targetPos, curveValue);
                }
            }

            yield return null;
        }

        foreach (var (button, _, targetPos) in buttonPositions)
        {
            if (button != null && button.activeInHierarchy)
            {
                button.GetComponent<RectTransform>().anchoredPosition = targetPos;
            }
        }
    }
    }
    public class ButtonData : MonoBehaviour
    {
        public ButtonType buttonType;
    }
}