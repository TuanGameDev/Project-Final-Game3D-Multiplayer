using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CGUI : MonoBehaviour
{

    [HideInInspector]
    public Character character;
    public Texture crosshair;
    public Texture interactiveActive; //Texture for interactive objects
    public Texture interactiveInactive; //Texture for interactive objects

    //Bools for menus
    private bool showIneractionMenu = false;
    private bool showInventory = false;
    private bool showMainMenu = true;

    // Screen coefficient
    private float screenCoefficientX = 1f;
    private float screenCoefficientY = 1f;

    private InteractiveObject activeInteractiveObject;

    public AudioClip failSound;
    public AudioClip buildSound;
    [HideInInspector]
    public AudioSource guiAudio;

    //new UI
    [SerializeField]
    private InteractionButton interactionButtonTemplate;
    private List<InteractionButton> interactionButtons = new List<InteractionButton>();

    [SerializeField]
    private ItemButton itemButtonTemplate;
    [SerializeField]
    private CraftItemButton craftItemButton;
    private List<ItemButton> itemButtons = new List<ItemButton>();
    private List<CraftItemButton> craftButtons = new List<CraftItemButton>();
    [SerializeField]
    private RectTransform selectInventoryButton;
    [SerializeField]
    private RectTransform selectCraftButton;

    [SerializeField]
    private RectTransform inventoryCraftSelector;
    [SerializeField]
    private RectTransform inventoryButtonsParent;
    [SerializeField]
    private RectTransform craftButtonsParent;

    [SerializeField]
    private Text itemDescriptionText;
    [SerializeField]
    private GameObject itemDescriptionPanel;

    [SerializeField]
    private RectTransform craftSelector;
    [SerializeField]
    private RectTransform itemSelector;

    [SerializeField]
    private GameObject inventoryAndCrafting;

    [SerializeField]
    private GameObject equipButton;

    private int selectedItemIndex;
    private int selectedCraftItemIndex;

    [SerializeField]
    private GameObject qualityButton;

    [SerializeField]
    private GameObject mainMenuElements;
    [SerializeField]
    private GameObject gameplayElements;

    [SerializeField]
    private Text currentFirearmText;
    [SerializeField]
    private GameObject currentFirearmInfoPanel;

    void Awake()
    {
        guiAudio = GetComponent<AudioSource>();
    }

    void OnGUI()
    {
        if (!character.input.isAI)
        {
            screenCoefficientX = Screen.width / 1280f;
            screenCoefficientY = Screen.height / 720f;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                showMainMenu = true;
                SwitchControl(false);
                mainMenuElements.SetActive(true);
                gameplayElements.SetActive(false);
            }

            if (!showMainMenu)
            {
                if (character.cameraController.isZoom)
                {
                    Rect crosshairRect = new Rect((Screen.width - crosshair.width) / 2.0f, (Screen.height - crosshair.height) / 2.0f, crosshair.width, crosshair.height);
                    GUI.DrawTexture(crosshairRect, crosshair, ScaleMode.ScaleToFit);
                }
                if (character.interaction.availableInteractiveObjects.Count > 0)
                {
                    foreach (InteractiveObject intObj in character.interaction.availableInteractiveObjects)
                    {
                        Vector3 intObjScreenPos = character.cameraController.GetComponent<Camera>().WorldToScreenPoint(intObj.GUIPosition.position);
                        if (intObj == activeInteractiveObject)
                        {
                            float posX = intObjScreenPos.x - (interactiveActive.width / 2f) * screenCoefficientX;
                            float posY = Screen.height - intObjScreenPos.y - (interactiveActive.height / 2f) * screenCoefficientY;

                            Rect intObjRect = new Rect(posX, posY, interactiveActive.width * screenCoefficientX, interactiveActive.height * screenCoefficientY);
                            GUI.DrawTexture(intObjRect, interactiveActive, ScaleMode.ScaleToFit);
                        }
                        else
                        {
                            float posX = intObjScreenPos.x - (interactiveInactive.width / 2f) * screenCoefficientX;
                            float posY = Screen.height - intObjScreenPos.y - (interactiveInactive.height / 2f) * screenCoefficientY;

                            Rect intObjRect = new Rect(posX, posY, interactiveInactive.width * screenCoefficientX, interactiveInactive.height * screenCoefficientY);
                            GUI.DrawTexture(intObjRect, interactiveInactive, ScaleMode.ScaleToFit);
                        }
                    }
                }
                UpdateIteraction();
                UpdateIndicators();
            }
        }
    }

    private void UpdateIteraction()
    {
        if (!showIneractionMenu && !showInventory)
        {
            //Ineraction Menu
            if (character.interaction.closestInteractiveObject)
            {
                interactionButtonTemplate.SetText(character.interaction.closestInteractiveObject.description);
                interactionButtonTemplate.gameObject.SetActive(true);
                activeInteractiveObject = character.interaction.closestInteractiveObject;
            }
            else interactionButtonTemplate.gameObject.SetActive(false);
        }
        else interactionButtonTemplate.gameObject.SetActive(false);
    }

    private void UpdateIndicators()
    {
        currentFirearmText.text = character.weaponsController.skillFirearmsVariables.currentFirearmIndicator;
        
        if (character.weaponsController.GetIsArmed()) currentFirearmInfoPanel.SetActive(true);
        else currentFirearmInfoPanel.SetActive(false);
    }

    private void UpdateInteractions(bool show)
    {
        if (interactionButtons.Count > 0)
        {
            foreach (InteractionButton bu in interactionButtons)
            {
                Destroy(bu.gameObject);
            }
            interactionButtons.Clear();
        }

        if (show)
        {
            for (int counter = 0; counter < character.interaction.availableInteractiveObjects.Count; counter++)
            {
                GameObject newButtonGO = Instantiate(interactionButtonTemplate.gameObject) as GameObject;
                InteractionButton newButton = newButtonGO.GetComponent<InteractionButton>();
                interactionButtons.Add(newButton);
                newButtonGO.transform.SetParent(interactionButtonTemplate.transform.parent);
                RectTransform newRect = newButtonGO.GetComponent<RectTransform>();

                Vector3 newPos = new Vector3(interactionButtonTemplate.gameObject.transform.localPosition.x, interactionButtonTemplate.gameObject.transform.localPosition.y - counter * 50f, interactionButtonTemplate.gameObject.transform.localPosition.z);
                newRect.localPosition = newPos;
                newRect.localEulerAngles = Vector3.zero;
                newRect.localScale = Vector3.one;

                newButton.SetText(character.interaction.availableInteractiveObjects[counter].description);
                newButton.SetIndex(counter);
            }
        }
    }

    public void UpdateItems()
    {
        if (showInventory && !showIneractionMenu)
        {
            if (character.inventory.items.Count > 0)
            {
                if (itemButtons.Count > 0)
                {
                    foreach (ItemButton bu in itemButtons)
                    {
                        Destroy(bu.gameObject);
                    }
                    itemButtons.Clear();
                }

                for (int counter = 0; counter < character.inventory.items.Count; counter++)
                {
                    Item item = character.inventory.items[counter];

                    GameObject newButtonGO = Instantiate(itemButtonTemplate.gameObject) as GameObject;
                    newButtonGO.SetActive(true);
                    ItemButton newButton = newButtonGO.GetComponent<ItemButton>();
                    itemButtons.Add(newButton);
                    newButtonGO.transform.SetParent(itemButtonTemplate.transform.parent);
                    RectTransform newRect = newButtonGO.GetComponent<RectTransform>();

                    Vector3 newPos = new Vector3(itemButtonTemplate.gameObject.transform.localPosition.x, itemButtonTemplate.gameObject.transform.localPosition.y - counter * 50f, itemButtonTemplate.gameObject.transform.localPosition.z);
                    newRect.localPosition = newPos;
                    newRect.localEulerAngles = Vector3.zero;
                    newRect.localScale = Vector3.one;

                    newButton.SetText(item.GetDescription());
                    newButton.SetIndex(counter);
                }
            }
            selectInventoryButton.gameObject.SetActive(true);
            selectCraftButton.gameObject.SetActive(true);
            inventoryCraftSelector.gameObject.SetActive(true);

            if (itemButtons.Count > 0)
            {
                itemButtons[0].Select();
            }
            else
            {
                itemDescriptionPanel.SetActive(false);
                itemSelector.gameObject.SetActive(false);
            }
        }
        else
        {
            if (itemButtons.Count > 0)
            {
                foreach (ItemButton bu in itemButtons)
                {
                    Destroy(bu.gameObject);
                }
                itemButtons.Clear();
            }
            selectInventoryButton.gameObject.SetActive(false);
            selectCraftButton.gameObject.SetActive(false);
            inventoryCraftSelector.gameObject.SetActive(false);
        }
    }

    public void StartInteraction(int index)
    {
        character.StartGoToInteraction(character.interaction.availableInteractiveObjects[index]);
        ShowIneractionMenu(false);
    }
    public void SelectInteraction(int index)
    {
        activeInteractiveObject = character.interaction.availableInteractiveObjects[index];
    }

    public void ShowIneractionMenu(bool show)
    {
        if (!showMainMenu)
        {
            showIneractionMenu = show;
            showInventory = false;
            SwitchControl(!show);
            UpdateInteractions(show);
        }
    }

    public void ShowInventory(bool show)
    {
        if (!showMainMenu)
        {
            showIneractionMenu = false;
            showInventory = show;
            SwitchControl(!show);
            UpdateInteractions(!show);
            inventoryButtonsParent.gameObject.SetActive(show);
            craftButtonsParent.gameObject.SetActive(false);
            MoveInventoryCraftSelector(selectInventoryButton);

            inventoryAndCrafting.SetActive(show);

            UpdateItems();
        }
    }

    void SwitchControl(bool switcher)
    {
        if (switcher) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        Cursor.visible = !switcher;
        character.SwitchControl(switcher);
    }

    public void MoveInventoryCraftSelector(Transform moveTo)
    {
        inventoryCraftSelector.localPosition = moveTo.localPosition;
        inventoryCraftSelector.SetAsLastSibling();
    }

    public void UpdateCraftMenu()
    {
        if (craftButtons.Count > 0)
        {
            foreach (CraftItemButton bu in craftButtons)
            {
                Destroy(bu.gameObject);
            }
            craftButtons.Clear();
        }
        for (int counter = 0; counter < character.inventory.craftItems.Count; counter++)
        {
            Item item = character.inventory.craftItems[counter];

            GameObject newButtonGO = Instantiate(craftItemButton.gameObject) as GameObject;
            newButtonGO.SetActive(true);
            CraftItemButton newButton = newButtonGO.GetComponent<CraftItemButton>();
            craftButtons.Add(newButton);
            newButtonGO.transform.SetParent(craftItemButton.transform.parent);
            RectTransform newRect = newButtonGO.GetComponent<RectTransform>();

            Vector3 newPos = new Vector3(craftItemButton.gameObject.transform.localPosition.x, craftItemButton.gameObject.transform.localPosition.y - counter * 50f, craftItemButton.gameObject.transform.localPosition.z);
            newRect.localPosition = newPos;
            newRect.localEulerAngles = Vector3.zero;
            newRect.localScale = Vector3.one;

            newButton.SetText(item.GetDescription());
            newButton.SetIndex(counter);
            craftSelector.SetAsLastSibling();
        }
        if (craftButtons.Count > 0)
        {
            craftButtons[0].Select();
        }
    }

    public void SelectCraftItem(Transform moveTo, int index)
    {
        craftSelector.localPosition = moveTo.localPosition;
        craftSelector.SetAsLastSibling();
        itemDescriptionText.text = character.inventory.craftItems[index].GetDitailedDescription();
        itemDescriptionPanel.SetActive(true);
        selectedCraftItemIndex = index;
    }

    public void SelectItem(Transform moveTo, int index)
    {
        selectedItemIndex = index;
        Item item = character.inventory.items[index];

        itemSelector.gameObject.SetActive(true);
        itemSelector.localPosition = moveTo.localPosition;
        itemSelector.SetAsLastSibling();
        itemDescriptionText.text = item.GetDitailedDescription();
        itemDescriptionPanel.SetActive(true);

        equipButton.SetActive(character.inventory.consumableItems.Contains(item));
    }

    public void EquipConsumableItem()
    {
        Item item = character.inventory.items[selectedItemIndex];
        //Consumable items
        if (character.inventory.consumableItems.Contains(item))
        {
            character.inventory.selectedItem = item;
            character.inventory.selectedItemVisualIndex = item.visualIndex;
            guiAudio.PlayOneShot(buildSound);
        }
    }

    public void UpdateQualityMenu()
    {
        string[] qualitySettingsNames = QualitySettings.names;
        if (qualitySettingsNames.Length > 0)
        {
            for (int counter = 0; counter < qualitySettingsNames.Length; counter++)
            {
                GameObject newButtonGO = Instantiate(qualityButton.gameObject) as GameObject;

                QualityButton newButton = newButtonGO.GetComponent<QualityButton>();
                newButton.SetIndex(counter);
                newButton.SetName(qualitySettingsNames[counter]);

                newButtonGO.SetActive(true);
                newButtonGO.transform.SetParent(qualityButton.transform.parent);
                RectTransform newRect = newButtonGO.GetComponent<RectTransform>();

                Vector3 newPos = new Vector3(qualityButton.gameObject.transform.localPosition.x, qualityButton.gameObject.transform.localPosition.y - counter * 50f, qualityButton.gameObject.transform.localPosition.z);
                newRect.localPosition = newPos;
                newRect.localEulerAngles = Vector3.zero;
                newRect.localScale = Vector3.one;
            }
        }
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        showMainMenu = false;
        if (!showIneractionMenu && !showInventory) SwitchControl(true);
        gameplayElements.SetActive(true);
        mainMenuElements.SetActive(false);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Fullscreen()
    {
        Screen.fullScreen = true;
    }

    public void CraftItem()
    {
        if (character.inventory.craftItems[selectedCraftItemIndex].canCraft(character))
        {
            character.inventory.AddItem(character.inventory.craftItems[selectedCraftItemIndex]);
            guiAudio.PlayOneShot(buildSound);
        }
    }
}