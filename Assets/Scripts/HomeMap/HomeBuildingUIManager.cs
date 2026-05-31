using UnityEngine;

public static class HomeBuildingUIManager
{
    public static void Open(
        BuildingType type)
    {
        GameObject panel =
            FindPanel(
                GetPanelName(type));

        if (panel == null)
        {
            Debug.LogWarning(
                "Missing UI panel for building: " +
                type);

            return;
        }

        CloseHomePanels();

        panel.SetActive(true);

        panel.transform.SetAsLastSibling();
    }

    private static string GetPanelName(
        BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Recruit:

                return "RecruitUI";

            case BuildingType.Upgrade:

                return "TrainingUI";

            case BuildingType.Forge:

                return "ForgeUI";

            case BuildingType.Shop:

                return "ShopUI";

            case BuildingType.Adventure:

                return "AdventureUI";

            case BuildingType.Storage:

                return "StorageUI";
        }

        return "";
    }

    private static void CloseHomePanels()
    {
        SetPanelActive(
            "RecruitUI",
            false);

        SetPanelActive(
            "TrainingUI",
            false);

        SetPanelActive(
            "ForgeUI",
            false);

        SetPanelActive(
            "ShopUI",
            false);

        SetPanelActive(
            "AdventureUI",
            false);

        SetPanelActive(
            "StorageUI",
            false);

        SetPanelActive(
            "DetailPanel",
            false);

        SetPanelActive(
            "EquipmentSelectionPanel",
            false);
    }

    private static void SetPanelActive(
        string panelName,
        bool active)
    {
        GameObject panel =
            FindPanel(panelName);

        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    private static GameObject FindPanel(
        string panelName)
    {
        if (string.IsNullOrEmpty(panelName))
            return null;

        Transform[] transforms =
            Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        foreach (Transform item
            in transforms)
        {
            if (item.name == panelName)
            {
                return item.gameObject;
            }
        }

        return null;
    }
}
