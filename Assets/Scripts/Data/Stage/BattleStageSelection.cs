public static class BattleStageSelection
{
    public static StageData SelectedStage;

    public static int SelectedStageIndex = -1;

    public static bool ShowAdventureAfterReturn;

    public static void SelectStage(
        StageData stage,
        int index)
    {
        SelectedStage = stage;

        SelectedStageIndex = index;
    }

    public static void Clear()
    {
        SelectedStage = null;

        SelectedStageIndex = -1;

        ShowAdventureAfterReturn = false;
    }
}
