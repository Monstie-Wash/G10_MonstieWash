//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/Controls/PlayerInput.inputactions
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

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""PlayerActions"",
            ""id"": ""49e27ff0-4951-4458-a576-34993a97b801"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""1ec0cb3a-b63f-409e-8bd4-c57c03cf5375"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Activate"",
                    ""type"": ""Button"",
                    ""id"": ""0521ba8d-710c-4458-a6ff-44c07adb1a55"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchTool"",
                    ""type"": ""Button"",
                    ""id"": ""c4f8f9c9-5084-45cb-a1fd-ce85f2f94cfd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Button"",
                    ""id"": ""c473e6d4-095f-4d10-a91f-e65111fc81e6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleUI"",
                    ""type"": ""Button"",
                    ""id"": ""0184552f-1d0c-4316-8dde-f5d82f6469a0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scan"",
                    ""type"": ""Button"",
                    ""id"": ""2116ca0f-2d8f-4c3c-a0de-fcba93beeed4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fb38f502-b439-42d1-a34d-e2a5636c98b4"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13084278-88f8-49e0-b91d-2e0a33962250"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7775d467-062c-4e81-9047-256c16ea19c4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Activate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d81cba58-fd5a-412f-9275-23ce327a74f3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Activate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""41197187-4f87-4a6d-93fd-a5fea2193f4d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Activate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""KBM"",
                    ""id"": ""e983739c-ea2d-4de2-98f7-65c228fdaceb"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTool"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6590d9ff-b60e-43d2-a060-465ef1a4c639"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""be650a99-efdf-4472-8b8c-90e07e74cc08"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""f108f390-ac83-4401-97e9-d3741cf7a2c2"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTool"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""daca3370-e1f4-4d6d-8364-a366c30661d3"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""69e75680-7a4e-4705-ba79-37c99423c01c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a5422ff9-1335-4e29-947c-b68341538458"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""760fdebc-5747-4764-8802-3c6104e7f727"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5cd40825-0c31-4fc6-8c50-d27c20594cf3"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0d6e5a4d-3fc3-4388-a4db-14f457db2be4"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleUI"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d4044e25-f269-4dc1-a685-efdabed9103e"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c413217d-2c6d-491b-bffa-07c564cfd38d"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuActions"",
            ""id"": ""58e2192e-b6c7-48a8-b028-01f12a3493c9"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""5d56a373-3b3c-47a3-a2b7-d60c08a656cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""64893faa-c856-47e4-a605-1f9b281073fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AltSelect"",
                    ""type"": ""Button"",
                    ""id"": ""0faa1987-e8f3-4106-9d4d-2f3cb93d9030"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Switch"",
                    ""type"": ""Button"",
                    ""id"": ""e79dfc19-5064-424e-83c5-2a9c93f85dd3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""0df51fc8-f0d7-42ea-9b74-0427a7ff0c36"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Dummy1"",
                    ""type"": ""Button"",
                    ""id"": ""9636542f-7251-4999-9041-c52f2a263409"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dummy2"",
                    ""type"": ""Button"",
                    ""id"": ""b93ac0b6-9441-4c31-9d60-85a335d2b559"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c5cba5ab-fc8d-497b-9a52-d3c39df0137d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2042bbf7-d3a2-456e-8aae-7aa5587eb309"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0a9e48a2-5a03-40ce-a5b2-2139a192d0a5"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""269f782e-0c8b-483b-8207-29ac881ef1f2"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0224b92d-caf0-4695-9639-4e7a95e69401"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""652e237c-5599-4af3-86b6-22eb73b89fc1"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64a1edd4-0176-4e5f-b042-e6b6ca54b9ec"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""455c87d6-68e1-4a04-8a4e-567cb9634610"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4d625e0-4fc5-4866-9693-9aa041a6548c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ec2e59a-8ceb-47a9-babc-530f23432c44"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dummy2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""947c9c2e-384e-423a-a90d-67a92ae257d7"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dummy1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""DebugActions"",
            ""id"": ""1cb85c75-0a21-4aa0-830d-0ff17d6b675b"",
            ""actions"": [
                {
                    ""name"": ""DebugReset"",
                    ""type"": ""Button"",
                    ""id"": ""d96a4c75-f75b-46b4-91f4-c8ba12f611e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=3)"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2e7fcc4c-deae-4be5-9e67-ad73581a01dc"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DebugReset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""608d33d2-228d-48b6-abc4-0bec60ed231d"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DebugReset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Default"",
            ""bindingGroup"": ""Default"",
            ""devices"": []
        }
    ]
}");
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_Move = m_PlayerActions.FindAction("Move", throwIfNotFound: true);
        m_PlayerActions_Activate = m_PlayerActions.FindAction("Activate", throwIfNotFound: true);
        m_PlayerActions_SwitchTool = m_PlayerActions.FindAction("SwitchTool", throwIfNotFound: true);
        m_PlayerActions_Navigate = m_PlayerActions.FindAction("Navigate", throwIfNotFound: true);
        m_PlayerActions_ToggleUI = m_PlayerActions.FindAction("ToggleUI", throwIfNotFound: true);
        m_PlayerActions_Scan = m_PlayerActions.FindAction("Scan", throwIfNotFound: true);
        // MenuActions
        m_MenuActions = asset.FindActionMap("MenuActions", throwIfNotFound: true);
        m_MenuActions_Select = m_MenuActions.FindAction("Select", throwIfNotFound: true);
        m_MenuActions_Back = m_MenuActions.FindAction("Back", throwIfNotFound: true);
        m_MenuActions_AltSelect = m_MenuActions.FindAction("AltSelect", throwIfNotFound: true);
        m_MenuActions_Switch = m_MenuActions.FindAction("Switch", throwIfNotFound: true);
        m_MenuActions_Move = m_MenuActions.FindAction("Move", throwIfNotFound: true);
        m_MenuActions_Dummy1 = m_MenuActions.FindAction("Dummy1", throwIfNotFound: true);
        m_MenuActions_Dummy2 = m_MenuActions.FindAction("Dummy2", throwIfNotFound: true);
        // DebugActions
        m_DebugActions = asset.FindActionMap("DebugActions", throwIfNotFound: true);
        m_DebugActions_DebugReset = m_DebugActions.FindAction("DebugReset", throwIfNotFound: true);
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

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private List<IPlayerActionsActions> m_PlayerActionsActionsCallbackInterfaces = new List<IPlayerActionsActions>();
    private readonly InputAction m_PlayerActions_Move;
    private readonly InputAction m_PlayerActions_Activate;
    private readonly InputAction m_PlayerActions_SwitchTool;
    private readonly InputAction m_PlayerActions_Navigate;
    private readonly InputAction m_PlayerActions_ToggleUI;
    private readonly InputAction m_PlayerActions_Scan;
    public struct PlayerActionsActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActionsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerActions_Move;
        public InputAction @Activate => m_Wrapper.m_PlayerActions_Activate;
        public InputAction @SwitchTool => m_Wrapper.m_PlayerActions_SwitchTool;
        public InputAction @Navigate => m_Wrapper.m_PlayerActions_Navigate;
        public InputAction @ToggleUI => m_Wrapper.m_PlayerActions_ToggleUI;
        public InputAction @Scan => m_Wrapper.m_PlayerActions_Scan;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Activate.started += instance.OnActivate;
            @Activate.performed += instance.OnActivate;
            @Activate.canceled += instance.OnActivate;
            @SwitchTool.started += instance.OnSwitchTool;
            @SwitchTool.performed += instance.OnSwitchTool;
            @SwitchTool.canceled += instance.OnSwitchTool;
            @Navigate.started += instance.OnNavigate;
            @Navigate.performed += instance.OnNavigate;
            @Navigate.canceled += instance.OnNavigate;
            @ToggleUI.started += instance.OnToggleUI;
            @ToggleUI.performed += instance.OnToggleUI;
            @ToggleUI.canceled += instance.OnToggleUI;
            @Scan.started += instance.OnScan;
            @Scan.performed += instance.OnScan;
            @Scan.canceled += instance.OnScan;
        }

        private void UnregisterCallbacks(IPlayerActionsActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Activate.started -= instance.OnActivate;
            @Activate.performed -= instance.OnActivate;
            @Activate.canceled -= instance.OnActivate;
            @SwitchTool.started -= instance.OnSwitchTool;
            @SwitchTool.performed -= instance.OnSwitchTool;
            @SwitchTool.canceled -= instance.OnSwitchTool;
            @Navigate.started -= instance.OnNavigate;
            @Navigate.performed -= instance.OnNavigate;
            @Navigate.canceled -= instance.OnNavigate;
            @ToggleUI.started -= instance.OnToggleUI;
            @ToggleUI.performed -= instance.OnToggleUI;
            @ToggleUI.canceled -= instance.OnToggleUI;
            @Scan.started -= instance.OnScan;
            @Scan.performed -= instance.OnScan;
            @Scan.canceled -= instance.OnScan;
        }

        public void RemoveCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);

    // MenuActions
    private readonly InputActionMap m_MenuActions;
    private List<IMenuActionsActions> m_MenuActionsActionsCallbackInterfaces = new List<IMenuActionsActions>();
    private readonly InputAction m_MenuActions_Select;
    private readonly InputAction m_MenuActions_Back;
    private readonly InputAction m_MenuActions_AltSelect;
    private readonly InputAction m_MenuActions_Switch;
    private readonly InputAction m_MenuActions_Move;
    private readonly InputAction m_MenuActions_Dummy1;
    private readonly InputAction m_MenuActions_Dummy2;
    public struct MenuActionsActions
    {
        private @PlayerInput m_Wrapper;
        public MenuActionsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_MenuActions_Select;
        public InputAction @Back => m_Wrapper.m_MenuActions_Back;
        public InputAction @AltSelect => m_Wrapper.m_MenuActions_AltSelect;
        public InputAction @Switch => m_Wrapper.m_MenuActions_Switch;
        public InputAction @Move => m_Wrapper.m_MenuActions_Move;
        public InputAction @Dummy1 => m_Wrapper.m_MenuActions_Dummy1;
        public InputAction @Dummy2 => m_Wrapper.m_MenuActions_Dummy2;
        public InputActionMap Get() { return m_Wrapper.m_MenuActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActionsActions set) { return set.Get(); }
        public void AddCallbacks(IMenuActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_MenuActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MenuActionsActionsCallbackInterfaces.Add(instance);
            @Select.started += instance.OnSelect;
            @Select.performed += instance.OnSelect;
            @Select.canceled += instance.OnSelect;
            @Back.started += instance.OnBack;
            @Back.performed += instance.OnBack;
            @Back.canceled += instance.OnBack;
            @AltSelect.started += instance.OnAltSelect;
            @AltSelect.performed += instance.OnAltSelect;
            @AltSelect.canceled += instance.OnAltSelect;
            @Switch.started += instance.OnSwitch;
            @Switch.performed += instance.OnSwitch;
            @Switch.canceled += instance.OnSwitch;
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Dummy1.started += instance.OnDummy1;
            @Dummy1.performed += instance.OnDummy1;
            @Dummy1.canceled += instance.OnDummy1;
            @Dummy2.started += instance.OnDummy2;
            @Dummy2.performed += instance.OnDummy2;
            @Dummy2.canceled += instance.OnDummy2;
        }

        private void UnregisterCallbacks(IMenuActionsActions instance)
        {
            @Select.started -= instance.OnSelect;
            @Select.performed -= instance.OnSelect;
            @Select.canceled -= instance.OnSelect;
            @Back.started -= instance.OnBack;
            @Back.performed -= instance.OnBack;
            @Back.canceled -= instance.OnBack;
            @AltSelect.started -= instance.OnAltSelect;
            @AltSelect.performed -= instance.OnAltSelect;
            @AltSelect.canceled -= instance.OnAltSelect;
            @Switch.started -= instance.OnSwitch;
            @Switch.performed -= instance.OnSwitch;
            @Switch.canceled -= instance.OnSwitch;
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Dummy1.started -= instance.OnDummy1;
            @Dummy1.performed -= instance.OnDummy1;
            @Dummy1.canceled -= instance.OnDummy1;
            @Dummy2.started -= instance.OnDummy2;
            @Dummy2.performed -= instance.OnDummy2;
            @Dummy2.canceled -= instance.OnDummy2;
        }

        public void RemoveCallbacks(IMenuActionsActions instance)
        {
            if (m_Wrapper.m_MenuActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMenuActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_MenuActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MenuActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MenuActionsActions @MenuActions => new MenuActionsActions(this);

    // DebugActions
    private readonly InputActionMap m_DebugActions;
    private List<IDebugActionsActions> m_DebugActionsActionsCallbackInterfaces = new List<IDebugActionsActions>();
    private readonly InputAction m_DebugActions_DebugReset;
    public struct DebugActionsActions
    {
        private @PlayerInput m_Wrapper;
        public DebugActionsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @DebugReset => m_Wrapper.m_DebugActions_DebugReset;
        public InputActionMap Get() { return m_Wrapper.m_DebugActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActionsActions set) { return set.Get(); }
        public void AddCallbacks(IDebugActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_DebugActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DebugActionsActionsCallbackInterfaces.Add(instance);
            @DebugReset.started += instance.OnDebugReset;
            @DebugReset.performed += instance.OnDebugReset;
            @DebugReset.canceled += instance.OnDebugReset;
        }

        private void UnregisterCallbacks(IDebugActionsActions instance)
        {
            @DebugReset.started -= instance.OnDebugReset;
            @DebugReset.performed -= instance.OnDebugReset;
            @DebugReset.canceled -= instance.OnDebugReset;
        }

        public void RemoveCallbacks(IDebugActionsActions instance)
        {
            if (m_Wrapper.m_DebugActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDebugActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_DebugActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DebugActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DebugActionsActions @DebugActions => new DebugActionsActions(this);
    private int m_DefaultSchemeIndex = -1;
    public InputControlScheme DefaultScheme
    {
        get
        {
            if (m_DefaultSchemeIndex == -1) m_DefaultSchemeIndex = asset.FindControlSchemeIndex("Default");
            return asset.controlSchemes[m_DefaultSchemeIndex];
        }
    }
    public interface IPlayerActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnActivate(InputAction.CallbackContext context);
        void OnSwitchTool(InputAction.CallbackContext context);
        void OnNavigate(InputAction.CallbackContext context);
        void OnToggleUI(InputAction.CallbackContext context);
        void OnScan(InputAction.CallbackContext context);
    }
    public interface IMenuActionsActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnAltSelect(InputAction.CallbackContext context);
        void OnSwitch(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnDummy1(InputAction.CallbackContext context);
        void OnDummy2(InputAction.CallbackContext context);
    }
    public interface IDebugActionsActions
    {
        void OnDebugReset(InputAction.CallbackContext context);
    }
}
