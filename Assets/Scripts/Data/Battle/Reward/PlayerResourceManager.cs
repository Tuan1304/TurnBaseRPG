using UnityEngine;

public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // GET
    // =========================

    public int Gold =>
        GameDataManager
        .Instance
        .Data
        .gold;

    public int Wood =>
        GameDataManager
        .Instance
        .Data
        .wood;

    public int Stone =>
        GameDataManager
        .Instance
        .Data
        .stone;

    public int Food =>
        GameDataManager
        .Instance
        .Data
        .food;

    // =========================
    // ADD
    // =========================

    public void AddGold(int amount)
    {
        GameDataManager
            .Instance
            .Data
            .gold += amount;

        GameDataManager
            .Instance
            .SaveGame();
    }

    public void AddWood(int amount)
    {
        GameDataManager
            .Instance
            .Data
            .wood += amount;

        GameDataManager
            .Instance
            .SaveGame();
    }

    public void AddStone(int amount)
    {
        GameDataManager
            .Instance
            .Data
            .stone += amount;

        GameDataManager
            .Instance
            .SaveGame();
    }

    public void AddFood(int amount)
    {
        GameDataManager
            .Instance
            .Data
            .food += amount;

        GameDataManager
            .Instance
            .SaveGame();
    }

    // =========================
    // REWARD
    // =========================

    public void AddReward(
        StageReward reward)
    {
        AddGold(reward.gold);

        AddWood(reward.wood);

        AddStone(reward.stone);

        AddFood(reward.food);

    }
}