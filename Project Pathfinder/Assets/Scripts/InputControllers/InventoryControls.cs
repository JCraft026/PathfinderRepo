//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/InputControllers/InventoryControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InventoryControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InventoryControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InventoryControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""85777a84-727c-44ce-9c34-6d9cba200725"",
            ""actions"": [
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""456e59cb-16a3-49bf-9f6b-16dea674cd62"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DropItem"",
                    ""type"": ""Button"",
                    ""id"": ""1572fe3c-d997-41f4-8d10-5fb72933288a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory1"",
                    ""type"": ""Button"",
                    ""id"": ""36b91c4d-1c66-4275-b632-59f95189c5c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory2"",
                    ""type"": ""Button"",
                    ""id"": ""d98ce082-3282-4a2a-800b-308b3bb581a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory3"",
                    ""type"": ""Button"",
                    ""id"": ""9c57e969-2688-4230-b302-b8679f2a8da6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory4"",
                    ""type"": ""Button"",
                    ""id"": ""97060853-f750-49f7-b781-f043c173a5fe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory5"",
                    ""type"": ""Button"",
                    ""id"": ""f2cfc080-b18f-45f8-bc48-5410a12ef1c5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory6"",
                    ""type"": ""Button"",
                    ""id"": ""ad0d7b79-eb6f-4742-abfb-ae1873d6ef44"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory7"",
                    ""type"": ""Button"",
                    ""id"": ""5f609194-0d27-421e-a774-7166f5ae267a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory8"",
                    ""type"": ""Button"",
                    ""id"": ""c77966ec-548a-4725-90c9-0419333f0c17"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""143bb1cd-cc10-4eca-a2f0-a3664166fe91"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""415c79ae-75d1-4870-b707-6dd76f8e42a7"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DropItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31d81eb6-b803-418b-ae18-8e4979ceb133"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97429b18-2dd3-49a9-9f78-729bc3d6b400"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cdb73cb5-204a-4aab-aabf-ff9770fef1ea"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""691c6ada-8e06-46a5-8aab-a5cc3a68f1e2"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee33df2a-951c-49e2-b474-699d9abc3d91"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9986361-4cfb-47e0-9f52-01be373f1d84"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec53b3c5-42e7-4eab-8ec5-5ed7c282439f"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5759447b-709f-4173-a9bf-d8dbd8ca59ed"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Joystick"",
            ""bindingGroup"": ""Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""XR"",
            ""bindingGroup"": ""XR"",
            ""devices"": [
                {
                    ""devicePath"": ""<XRController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_UseItem = m_Player.FindAction("UseItem", throwIfNotFound: true);
        m_Player_DropItem = m_Player.FindAction("DropItem", throwIfNotFound: true);
        m_Player_Inventory1 = m_Player.FindAction("Inventory1", throwIfNotFound: true);
        m_Player_Inventory2 = m_Player.FindAction("Inventory2", throwIfNotFound: true);
        m_Player_Inventory3 = m_Player.FindAction("Inventory3", throwIfNotFound: true);
        m_Player_Inventory4 = m_Player.FindAction("Inventory4", throwIfNotFound: true);
        m_Player_Inventory5 = m_Player.FindAction("Inventory5", throwIfNotFound: true);
        m_Player_Inventory6 = m_Player.FindAction("Inventory6", throwIfNotFound: true);
        m_Player_Inventory7 = m_Player.FindAction("Inventory7", throwIfNotFound: true);
        m_Player_Inventory8 = m_Player.FindAction("Inventory8", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_UseItem;
    private readonly InputAction m_Player_DropItem;
    private readonly InputAction m_Player_Inventory1;
    private readonly InputAction m_Player_Inventory2;
    private readonly InputAction m_Player_Inventory3;
    private readonly InputAction m_Player_Inventory4;
    private readonly InputAction m_Player_Inventory5;
    private readonly InputAction m_Player_Inventory6;
    private readonly InputAction m_Player_Inventory7;
    private readonly InputAction m_Player_Inventory8;
    public struct PlayerActions
    {
        private @InventoryControls m_Wrapper;
        public PlayerActions(@InventoryControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @UseItem => m_Wrapper.m_Player_UseItem;
        public InputAction @DropItem => m_Wrapper.m_Player_DropItem;
        public InputAction @Inventory1 => m_Wrapper.m_Player_Inventory1;
        public InputAction @Inventory2 => m_Wrapper.m_Player_Inventory2;
        public InputAction @Inventory3 => m_Wrapper.m_Player_Inventory3;
        public InputAction @Inventory4 => m_Wrapper.m_Player_Inventory4;
        public InputAction @Inventory5 => m_Wrapper.m_Player_Inventory5;
        public InputAction @Inventory6 => m_Wrapper.m_Player_Inventory6;
        public InputAction @Inventory7 => m_Wrapper.m_Player_Inventory7;
        public InputAction @Inventory8 => m_Wrapper.m_Player_Inventory8;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @UseItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @DropItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropItem;
                @DropItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropItem;
                @DropItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropItem;
                @Inventory1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory1;
                @Inventory1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory1;
                @Inventory1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory1;
                @Inventory2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory2;
                @Inventory2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory2;
                @Inventory2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory2;
                @Inventory3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory3;
                @Inventory3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory3;
                @Inventory3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory3;
                @Inventory4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory4;
                @Inventory4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory4;
                @Inventory4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory4;
                @Inventory5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory5;
                @Inventory5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory5;
                @Inventory5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory5;
                @Inventory6.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory6;
                @Inventory6.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory6;
                @Inventory6.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory6;
                @Inventory7.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory7;
                @Inventory7.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory7;
                @Inventory7.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory7;
                @Inventory8.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory8;
                @Inventory8.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory8;
                @Inventory8.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory8;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @UseItem.started += instance.OnUseItem;
                @UseItem.performed += instance.OnUseItem;
                @UseItem.canceled += instance.OnUseItem;
                @DropItem.started += instance.OnDropItem;
                @DropItem.performed += instance.OnDropItem;
                @DropItem.canceled += instance.OnDropItem;
                @Inventory1.started += instance.OnInventory1;
                @Inventory1.performed += instance.OnInventory1;
                @Inventory1.canceled += instance.OnInventory1;
                @Inventory2.started += instance.OnInventory2;
                @Inventory2.performed += instance.OnInventory2;
                @Inventory2.canceled += instance.OnInventory2;
                @Inventory3.started += instance.OnInventory3;
                @Inventory3.performed += instance.OnInventory3;
                @Inventory3.canceled += instance.OnInventory3;
                @Inventory4.started += instance.OnInventory4;
                @Inventory4.performed += instance.OnInventory4;
                @Inventory4.canceled += instance.OnInventory4;
                @Inventory5.started += instance.OnInventory5;
                @Inventory5.performed += instance.OnInventory5;
                @Inventory5.canceled += instance.OnInventory5;
                @Inventory6.started += instance.OnInventory6;
                @Inventory6.performed += instance.OnInventory6;
                @Inventory6.canceled += instance.OnInventory6;
                @Inventory7.started += instance.OnInventory7;
                @Inventory7.performed += instance.OnInventory7;
                @Inventory7.canceled += instance.OnInventory7;
                @Inventory8.started += instance.OnInventory8;
                @Inventory8.performed += instance.OnInventory8;
                @Inventory8.canceled += instance.OnInventory8;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard&Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_TouchSchemeIndex = -1;
    public InputControlScheme TouchScheme
    {
        get
        {
            if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
            return asset.controlSchemes[m_TouchSchemeIndex];
        }
    }
    private int m_JoystickSchemeIndex = -1;
    public InputControlScheme JoystickScheme
    {
        get
        {
            if (m_JoystickSchemeIndex == -1) m_JoystickSchemeIndex = asset.FindControlSchemeIndex("Joystick");
            return asset.controlSchemes[m_JoystickSchemeIndex];
        }
    }
    private int m_XRSchemeIndex = -1;
    public InputControlScheme XRScheme
    {
        get
        {
            if (m_XRSchemeIndex == -1) m_XRSchemeIndex = asset.FindControlSchemeIndex("XR");
            return asset.controlSchemes[m_XRSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnUseItem(InputAction.CallbackContext context);
        void OnDropItem(InputAction.CallbackContext context);
        void OnInventory1(InputAction.CallbackContext context);
        void OnInventory2(InputAction.CallbackContext context);
        void OnInventory3(InputAction.CallbackContext context);
        void OnInventory4(InputAction.CallbackContext context);
        void OnInventory5(InputAction.CallbackContext context);
        void OnInventory6(InputAction.CallbackContext context);
        void OnInventory7(InputAction.CallbackContext context);
        void OnInventory8(InputAction.CallbackContext context);
    }
}
