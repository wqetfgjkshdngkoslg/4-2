using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Object;

public class MobileJobSelect : NetworkBehaviour
{
    public static MobileJobSelect Instance;

    // ──────────────────────────────────────
    // UI 슬롯
    // ──────────────────────────────────────
    [Header("상태 텍스트")]
    public TextMeshProUGUI statusText;

    [Header("2인 버튼")]
    public Button btn2_1; 
    public Button btn2_2; 

    [Header("3인 버튼")]
    public Button btn3_1; 
    public Button btn3_2; 
    public Button btn3_3; 

    [Header("4인 버튼")]
    public Button btn4_1; // 현장감식관
    public Button btn4_2; // CCTV분석관
    public Button btn4_3; // 목격자조사관
    public Button btn4_4; // 사이버수사관

    // 직업명
    private string[] jobs2 = { "수사관1", "수사관2" };
    private string[] jobs3 = { "수사관1", "수사관2", "수사관3" };
    private string[] jobs4 = { "현장감식관", "CCTV분석관", "목격자조사관", "사이버수사관" };

    private bool isLocked = true; // 오프닝 중 잠금

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 처음엔 잠금 상태
        LockJobSelect();

        int maxPlayers = DataManager.Instance?.MaxPlayers ?? 4;
        ShowButtons(maxPlayers);

        // 버튼 이벤트
        btn2_1?.onClick.AddListener(() => TrySelectJob(jobs2[0]));
        btn2_2?.onClick.AddListener(() => TrySelectJob(jobs2[1]));

        btn3_1?.onClick.AddListener(() => TrySelectJob(jobs3[0]));
        btn3_2?.onClick.AddListener(() => TrySelectJob(jobs3[1]));
        btn3_3?.onClick.AddListener(() => TrySelectJob(jobs3[2]));

        btn4_1?.onClick.AddListener(() => TrySelectJob(jobs4[0]));
        btn4_2?.onClick.AddListener(() => TrySelectJob(jobs4[1]));
        btn4_3?.onClick.AddListener(() => TrySelectJob(jobs4[2]));
        btn4_4?.onClick.AddListener(() => TrySelectJob(jobs4[3]));
    }

    // ──────────────────────────────────────
    // 인원수에 따라 버튼 표시
    // ──────────────────────────────────────
    void ShowButtons(int maxPlayers)
    {
        btn2_1?.gameObject.SetActive(maxPlayers == 2);
        btn2_2?.gameObject.SetActive(maxPlayers == 2);

        btn3_1?.gameObject.SetActive(maxPlayers == 3);
        btn3_2?.gameObject.SetActive(maxPlayers == 3);
        btn3_3?.gameObject.SetActive(maxPlayers == 3);

        btn4_1?.gameObject.SetActive(maxPlayers == 4);
        btn4_2?.gameObject.SetActive(maxPlayers == 4);
        btn4_3?.gameObject.SetActive(maxPlayers == 4);
        btn4_4?.gameObject.SetActive(maxPlayers == 4);
    }

    // ──────────────────────────────────────
    // 직업 선택 시도
    // ──────────────────────────────────────
    void TrySelectJob(string jobName)
    {
        if (isLocked)
        {
            statusText.text = "PC에서 오프닝 영상 재생 중입니다.\n잠시 기다려주세요!";
            return;
        }

        // GameManager에 직업 선택 전송
        var gm = FindFirstObjectByType<GameManager>();
        gm?.SelectJobServerRpc(jobName);
        statusText.text = $"{jobName} 선택 중...";
    }

    // ──────────────────────────────────────
    // 직업 선택 잠금
    // ──────────────────────────────────────
    void LockJobSelect()
    {
        isLocked = true;
        if (statusText != null)
            statusText.text = "PC에서 오프닝 영상 재생 중입니다.\n잠시 기다려주세요!";
    }

    // ──────────────────────────────────────
    // 직업 선택 잠금 해제 (GameManager에서 호출)
    // ──────────────────────────────────────
    public void UnlockJobSelect()
    {
        isLocked = false;
        if (statusText != null)
            statusText.text = "직업을 선택하세요!";
    }

    // ──────────────────────────────────────
    // GameManager 콜백
    // ──────────────────────────────────────
    public void OnJobConfirmed(string jobName)
    {
        DataManager.Instance.SelectedJob = jobName;
        statusText.text = $"{jobName} 선택 완료!";
        UnityEngine.SceneManagement.SceneManager
            .LoadScene("Mobile_LobbyScene");
    }

    public void OnJobRejected(string jobName)
    {
        statusText.text = $"{jobName}은 이미 선택됐어요!\n다른 직업을 선택하세요.";
    }

    public void OnJobStatusUpdated(string jobName, bool isTaken)
    {
        // 이미 선택된 직업 버튼 비활성화
        UpdateButtonState(jobName, isTaken);
    }

    void UpdateButtonState(string jobName, bool isTaken)
    {
        Button[] allBtns = { btn2_1, btn2_2, btn3_1, btn3_2, btn3_3, btn4_1, btn4_2, btn4_3, btn4_4 };
        string[] allJobs = { jobs2[0], jobs2[1], jobs3[0], jobs3[1], jobs3[2], jobs4[0], jobs4[1], jobs4[2], jobs4[3] };

        for (int i = 0; i < allBtns.Length; i++)
        {
            if (allBtns[i] != null && allJobs[i] == jobName)
                allBtns[i].interactable = !isTaken;
        }
    }
}