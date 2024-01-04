using UnityEngine;

public class YourLoadingScene : MonoBehaviour
{
    public LoadingBar loadingBar;

    void Start()
    {
        // Gọi hàm SetProgress để cập nhật thanh trạng thái
        loadingBar.SetProgress(100f); // Ví dụ: 25% đã tải
    }
}
