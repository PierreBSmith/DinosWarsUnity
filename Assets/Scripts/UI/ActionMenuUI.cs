using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuUI : MonoBehaviour
{
    private RulesEngine _rulesEngine;
    [SerializeField]
    private Button moveButton;
    [SerializeField]
    private Button actionButton;
    [SerializeField]
    private Button itemButton;

    [SerializeField]
    private Text actionText;
    private RectTransform _menuAnchor;
    private CharacterMovement selectedCharacter;
    [HideInInspector]
    public Animator _animator;

    void Awake()
    {
        _rulesEngine = GameObject.Find("RulesEngine").GetComponent<RulesEngine>();
        _menuAnchor = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
    }

    public void OpenActionMenu(CharacterMovement character)
    {
        selectedCharacter = character;
        _menuAnchor.anchoredPosition = Camera.main.WorldToScreenPoint(selectedCharacter.gameObject.transform.position);

        if(character.inventory.equippedWeapon)
        {
            if(character.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
            {
                actionText.text = "Heal";
            }
            else
            {
                actionText.text = "Attack";
            }
        }
        else
        {
            actionText.text = "Attack";
        }
        
        if(character.hasMoved)
        {
            moveButton.interactable = false;
        }
        if(character.hasAttacked || character.currentStamina < character.character.attackStaminaCost
            || !character.inventory.equippedWeapon)
        {
            actionButton.interactable = false;
        }
        if(character.usedInventory)
        {
            itemButton.interactable = false;
        }
    }

    public void CloseActionMenu()
    {
        moveButton.interactable = true;
        actionButton.interactable = true;
        itemButton.interactable = true;
        selectedCharacter = null;
    }

    public void MoveButtonClicked()
    {
        if(!selectedCharacter.hasMoved)
        {
            selectedCharacter.DisplayRange(true, false, false);
            CloseActionMenu();
        }
    }

    public void ActionButtonClicked()
    {
        if(!selectedCharacter.hasAttacked && selectedCharacter.currentStamina >= selectedCharacter.character.attackStaminaCost)
        {
            if(selectedCharacter.inventory.equippedWeapon.weaponType == Item.WEAPON.SPIRIT)
            {
                selectedCharacter.DisplayRange(true,true,true);
            }
            else
            {
                selectedCharacter.DisplayRange(true, true, false);
            }
            _rulesEngine.unitAttacking(selectedCharacter);
        }
    }

    public void ItemButtonClicked()
    {
        _rulesEngine.OpenInventoryMenu(selectedCharacter);
    }

    public void PassButtonClicked()
    {
        _rulesEngine.passTurn(selectedCharacter);
        CloseActionMenu();
    }
}
