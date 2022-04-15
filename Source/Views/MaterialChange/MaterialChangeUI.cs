using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.IO;

namespace NalulunaAvatarsMaterialChange.Views.MaterialChange
{
    class MaterialChangeUI : BSMLAutomaticViewController
    {
        //public override string ResourceName => "NalulunaAvatarsMaterialChange.Views.MaterialChange.MaterialChangeUI.bsml";

        public ModMainFlowCoordinator mainFlowCoordinator { get; set; }
        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
        }
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }
        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            OtherMaterialChangeSetting.Instance.SaveConfiguration();
            NalulunaMaterialChange.instance.VRMCopyDestroy();
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }



        [UIComponent("MaterialChangeList")]
        private CustomListTableData materialNameList = new CustomListTableData();
        private int selectRow;
        [UIAction("materialSelect")]
        private void Select(TableView _, int row)
        {
            selectRow = row;
        }

        [UIValue("AutoChangeToggle")]
        private bool autoChange = OtherMaterialChangeSetting.Instance.OtherParameter.AutoMaterialChange;
        [UIAction("OnAutoChangeStateChange")]
        private void OnSaveStateChange(bool value)
        {
            OtherMaterialChangeSetting.Instance.OtherParameter.AutoMaterialChange = autoChange = value;
        }


        [UIAction("goMaterialChange")]
        private void GoMaterialChange()
        {
            Logger.log.Debug($"autoChange {autoChange}");
            OtherMaterialChangeSetting.Instance.OtherParameter.AutoMaterialChange = autoChange;
            SharedCoroutineStarter.instance.StartCoroutine(NalulunaMaterialChange.instance.OtherMaterialStartup(selectRow));
        }
        [UIAction("Save")]
        private void Save()
        {
            Logger.log.Debug($"autoChange {autoChange}");
            OtherMaterialChangeSetting.Instance.OtherParameter.AutoMaterialChange = autoChange;
            NalulunaMaterialChange.instance.OtherSettingSet(selectRow, 0, 0);
        }


        [UIAction("#post-parse")]
        public void SetupList()
        {

            materialNameList.data.Clear();
            materialNameList.data.Add(new CustomListTableData.CustomCellInfo("None"));
            var names = NalulunaMaterialChange.instance.GetMaterialsName();
            if (names != null)
                foreach (var materialName in names)
                {
                    var customCellInfo = new CustomListTableData.CustomCellInfo(materialName);
                    materialNameList.data.Add(customCellInfo);
                }

            materialNameList.tableView.ReloadData();

            Logger.log.Debug($"Row Select {NalulunaMaterialChange.instance.VRMMetaKey}");
            if (OtherMaterialChangeSetting.Instance.OtherParameter.List.ContainsKey(NalulunaMaterialChange.instance.VRMMetaKey))
            {
                for (int i =0; i < materialNameList.data.Count; i++)
                    if (materialNameList.data[i].text == Path.GetFileName(OtherMaterialChangeSetting.Instance.OtherParameter.List[NalulunaMaterialChange.instance.VRMMetaKey].FileAddress1))
                    { selectRow = i; break; }
            }
            if (materialNameList.data.Count > 0)
            {
                materialNameList.tableView.SelectCellWithIdx(selectRow);
                materialNameList.tableView.ScrollToCellWithIdx(selectRow, TableView.ScrollPositionType.Beginning, false);
            }
        }

        #region AvatarCopy


        [UIAction("copy-avatarWPos")]
        private void AvatarCopyWPos()
        {
            NalulunaMaterialChange.instance.VRMCopy(true);
        }
        [UIAction("copy-avatarWPosTimer")]
        private void AvatarCopyWPosTime()
        {
            NalulunaAvatarsMaterialChangeController.CopyMode = true;
            NalulunaAvatarsMaterialChangeController.CopyStart = true;
        }
        [UIAction("copy-avatarAPos")]
        private void AvatarCopyAPos()
        {
            NalulunaMaterialChange.instance.VRMCopy(false);
        }
        [UIAction("copy-avatarAPosTimer")]
        private void AvatarCopyAPoTime()
        {
            NalulunaAvatarsMaterialChangeController.CopyMode = false;
            NalulunaAvatarsMaterialChangeController.CopyStart = true;
        }


        [UIValue("DestroyState")]
        private bool DestroyState = NalulunaAvatarsMaterialChangeController.AutoDestroy;
        [UIAction("auto-AvatarDestroyStateChange")]
        private void AvatarDestroyStateChange(bool value)
        {
            NalulunaAvatarsMaterialChangeController.AutoDestroy = DestroyState = value;
        }

        [UIAction("copy-avatarDestroy")]
        private void AvatarDestroy()
        {
            NalulunaMaterialChange.instance.VRMCopyDestroy();
        }

        #endregion

    }
}
