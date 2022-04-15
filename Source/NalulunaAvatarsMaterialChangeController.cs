using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;
using NalulunaAvatarsMaterialChange.Views.MaterialChange;
using IPALogger = IPA.Logging.Logger;
using VRUIControls;

namespace NalulunaAvatarsMaterialChange
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class NalulunaAvatarsMaterialChangeController : MonoBehaviour
    {
        public static NalulunaAvatarsMaterialChangeController instance { get; private set; }
        private ModMainFlowCoordinator mainFlowCoordinator;
        internal static string Name => "NalulunaAvatarsMaterialChange";
        internal static bool CopyStart = false;
        internal static bool CopyMode = false;
        internal static bool AutoDestroy = true;

        private InputManager inputManager;

        #region Monobehaviour Messages

        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (instance != null)
            {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            instance = this;
            instance.name = Name;
            Logger.log?.Debug($"{name}: Awake()");

            //MaterialChangeMenu
            MenuButton menuButton2 = new MenuButton("Material Change", "Material Change", ShowModFlowCoordinator, true);
            MenuButtons.instance.RegisterButton(menuButton2);

            //コントローラーフック
            //InputManager.instance.BeginPolling();
        }

        /// <summary>
        /// MaterialChangeメニューコントローラー
        /// </summary>
        public void ShowModFlowCoordinator()
        {
            if (this.mainFlowCoordinator == null)
                this.mainFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModMainFlowCoordinator>();
            if (mainFlowCoordinator.IsBusy) return;

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(mainFlowCoordinator);
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {

            Logger.log?.Debug($"{name}: OnDestroy()");
            instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }

        private void Update()
        {
            MaterialChangeKeyDown();
            AvatarCopy();
            ControllerInput();
        }


        Int32 RightTriggerDownCount;
        float RightTriggerDownTime;
        protected VRPointer _vrPointer;


        /// <summary>
        /// 1秒以内に3回右トリガーされたら、次のアニメーションへ遷移
        /// </summary>
        private void ControllerInput()
        {
            if (inputManager == null)
            {
                inputManager = new GameObject(nameof(InputManager)).AddComponent<InputManager>();
                inputManager.BeginGameCoreScene();
            }

            if (inputManager.GetLeftGripClicked())
                Logger.log?.Debug($"GetLeftGripClicked True");

            if (inputManager.GetRightGripClicked())
                Logger.log?.Debug($"GetRightGripClicked True");


            //左手トリガー握りっぱなしで右トリガー3連続で入力されたらチェンジアニメーション
            if (!(bool)(inputManager.GetLeftTriggerDown()))
            {
                RightTriggerDownCount = 0;
                RightTriggerDownTime = 0;
            }


            if ((bool)(inputManager.GetRightTriggerClicked()))
            {
                RightTriggerDownCount += 1;
                RightTriggerDownTime = 0;   //最後に入力があってから１秒経過しても３回目入力しなかった場合のみクリアするようにする
                Logger.log?.Debug($"{name}: ControllerInput() RightTrigerCnt:{RightTriggerDownCount}");

                if (RightTriggerDownCount >= 3)
                {
                    RightTriggerDownCount = 0;
                    Logger.log?.Debug($"{name}: ToggleAnimation()");

                    NalulunaMaterialChange.instance.ToggleAnimation();
                }

            }


            if (RightTriggerDownCount != 0)
                RightTriggerDownTime += Time.deltaTime;

            if (RightTriggerDownTime > 0.5f)
            {
                Logger.log?.Debug($"{name}: ControllerInput() ResetDeltaTime:{RightTriggerDownTime}");
                RightTriggerDownCount = 0;
                RightTriggerDownTime = 0;
            }


            if (Input.GetKeyDown(KeyCode.T))
            {
                NalulunaMaterialChange.instance.ToggleAnimation();
            }

        }


        bool OldInput = false;
        private void MaterialChangeKeyDown()
        {

            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isKey = Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3);

            if (isShift && isKey && !OldInput)
            {
                OldInput = true;
                if (Input.GetKey(KeyCode.Alpha1)) NalulunaMaterialChange.instance.SetOtherMaterialSelecter(0);
                else if (Input.GetKey(KeyCode.Alpha2)) NalulunaMaterialChange.instance.SetOtherMaterialSelecter(1);
                else if (Input.GetKey(KeyCode.Alpha3)) NalulunaMaterialChange.instance.SetOtherMaterialSelecter(2);
            }
            else
                OldInput = false;

        }

        private DateTime StartTime;
        private void AvatarCopy()
        {
            if (!CopyStart) return;
            if (StartTime == DateTime.MinValue) StartTime = DateTime.Now;
            if ((DateTime.Now - StartTime).TotalMilliseconds >= 5000)
            {
                Logger.log?.Debug($"{name}: OnDestroy()");
                NalulunaMaterialChange.instance.VRMCopy(CopyMode);
                StartTime = DateTime.MinValue;
                CopyStart = false;
            }
        }


        #endregion
    }
}
