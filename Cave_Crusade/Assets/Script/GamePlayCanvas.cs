using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime.Collections;

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
        [Header("Button Configuration")] [SerializeField]
        private ButtonConfig[] buttonConfigs;

        [SerializeField] private float spacingBetweenButtons = 120f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Transform buttonsParent;

        [Header("Layout Settings")] [SerializeField]
        private float startX = -180f;

        [SerializeField] private float buttonY = 0f;

        [Header("Animation Settings")] [SerializeField]
        private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private List<GameObject> allButtons = new List<GameObject>();
        [SerializeField] private List<GameObject> activeButtons = new List<GameObject>();
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameManager previousGameManager;
        [SerializeField] private bool canClick = true;
        private bool isCountingDown = true;
        public Slider countdownSlider; // Slider đếm ngược
        public float turnDuration = 10f; // Thời gian mỗi lượt (10 giây)
        public float turnTimer; // Bộ đếm thời gian
        private int currentTurn = 0;

        public Text HpText;
        public Text Floor;
        private bool hasLost = false;

        private void Awake()
        {


            buttonConfigs = gameManager.buttonConfigs;
          
        }

        private void Start()
        {
            CheckGameManagerChange();
            countdownSlider.maxValue = turnDuration;
            StartNewTurn();
            canClick = true;
        }

        private void Update()
        {
            CheckGameManagerChange();

            if (isCountingDown && turnTimer > 0)
            {
                turnTimer -= Time.deltaTime;
                countdownSlider.value = turnTimer;
            }

            gameManager.countTurn = currentTurn;

            HpText.text = Player.Instance.hp.ToString("D2") + "/" + Player.Instance.maxHp.ToString("D2");
            Floor.text = SceneManager.GetActiveScene().name;

            if (!hasLost && (Player.Instance.hp == 0 || turnTimer <= 0))
            {
                Debug.Log(Player.Instance.hp);
                UIManager.Instance.OpenUI<LoseCanvas>();
                hasLost = true;
                return;
            }
        }

        private void CheckGameManagerChange()
        {
            // Tìm GameManager mới nếu chưa có hoặc đã bị hủy
            if (gameManager == null && previousGameManager == null)
            {
                InitializeGameManager();
            }

            // Kiểm tra xem GameManager có thay đổi không
            if (gameManager != previousGameManager)
            {
                
                ClearAllButtons();
                InitializeButtonPool();
                SpawnInitialButtons();
                previousGameManager = gameManager;
            }
        }

        private void InitializeGameManager()
        {
            gameManager = FindObjectOfType<GameManager>();

        }

        private void ClearAllButtons()
        {
            // Hủy tất cả button cũ
            foreach (var button in allButtons)
            {
                if (button != null)
                {
                    Destroy(button);
                }
            }

            allButtons.Clear();
            activeButtons.Clear();
        }

        private void InitializeButtonPool()
        {
            if (gameManager == null || gameManager.buttonConfigs == null) return;

            foreach (var config in gameManager.buttonConfigs)
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
            StopCountdown(); // Bắt đầu lượt mới
        }

        // Hàm gọi khi Player click vào các button
        public void StopCountdown()
        {
            isCountingDown = false;
            StartCoroutine(ResetCountdown());

        }

        IEnumerator ResetCountdown()
        {
            yield return new WaitForSeconds(0.5f);
            if (Player.Instance.canDo)
            {
                StartNewTurn();

            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                StartNewTurn();

            }

        }




        private void AtkButton()
        {
            if(canClick)
            {
                Player.Instance.OnAttackButtonClick();
                StartCoroutine(DisableClickTemporarily()); 
                EndTurn();
            }
            
        }

        private void DefButton()
        {
            if(canClick)
            {
                Player.Instance.OnDefendButtonClick();
                EndTurn();
                StartCoroutine(DisableClickTemporarily()); 
            }
            
        }

        private void HealButton()
        {
            Player.Instance.OnHealButtonClick();
            EndTurn();
                
            
            
        }

        private void MoveButton()
        {
            if(canClick)
            {
                Player.Instance.OnMoveButtonClick();
              
                StartCoroutine(DisableClickTemporarily()); 
                EndTurn();
                
            }
            
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
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = button.AddComponent<CanvasGroup>();
            }

            canvasGroup.interactable = true;
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
                if (button == null)
                {
                    yield break; // Dừng coroutine nếu button đã bị hủy
                }

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
            if (canClick)
            {  
                StartCoroutine(DisableClickTemporarily()); 
            }
            activeButtons.Remove(clickedButton); 
            StartCoroutine(AnimateButtonDespawn(clickedButton)); 
        }
        private IEnumerator DisableClickTemporarily() { 
            canClick = false; // Vô hiệu hóa các button 
            yield return new WaitForSeconds(1f); // Đợi 1 giây 
            canClick = true; // Kích hoạt lại các button
        }
        private IEnumerator AnimateButtonDespawn(GameObject button)
        {
            float elapsed = 0;
            Quaternion startRotation = button.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, 180f);
            // Vô hiệu hóa tương tác của nút
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = button.AddComponent<CanvasGroup>();
            }

            canvasGroup.interactable = false;
            while (elapsed < animationDuration)
            {
                if (button == null)
                {
                    yield break; // Dừng coroutine nếu button đã bị hủy
                }

                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / animationDuration;
                float curveValue = rotationCurve.Evaluate(normalizedTime);

                button.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, curveValue);

                yield return null;
            }


            button.SetActive(false);
            StartCoroutine(RepositionActiveButtons());
            yield return new WaitForSeconds(0.05f);
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

        public class ButtonData : MonoBehaviour
        {
            public ButtonType buttonType;
        }
    }
}