public interface ITutorialStep
{
    void StartTutorial();
    bool EndCondition(); // 將 EndCondition 改為回傳 bool 更實用，方便檢查是否結束
}
