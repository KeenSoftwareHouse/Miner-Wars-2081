using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorCopyToolSelectSector : MyGuiScreenEditorDialogBase
    {
        MyGuiControlTextbox m_playerName;
        MyGuiControlTextbox m_sector_name;
        MyGuiControlTextbox m_sectorIdentifierX;
        MyGuiControlTextbox m_sectorIdentifierY;
        MyGuiControlTextbox m_sectorIdentifierZ;

        public MyGuiScreenEditorCopyToolSelectSector()
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.03f + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X + 0.04f + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X + 0.03f, 0.55f))
        {
            m_size = new Vector2(0.65f, 0.55f);
            Vector2 controlsDelta = new Vector2(0, 0.0525f);
            Vector2 controlsColumn1Origin = new Vector2(-m_size.Value.X / 2.0f + 0.04f, -m_size.Value.Y / 2.0f + 0.06f);
            Vector2 controlsColumn2Origin = new Vector2(controlsColumn1Origin.X + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X + 0.05f, controlsColumn1Origin.Y - 0.02f);
            Vector2 controlsColumn2OriginLabel = new Vector2(controlsColumn1Origin.X + MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X + 0.03f, controlsColumn1Origin.Y - 0.02f);
            //Vector2 controls

            AddCaption(MyTextsWrapperEnum.SelectSector, MyGuiConstants.LABEL_TEXT_COLOR, new Vector2(0, 0.007f));

            // controls for typing player name
            Controls.Add(new MyGuiControlLabel(this, controlsColumn1Origin + 1 * controlsDelta, null, MyTextsWrapperEnum.PlayerName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_playerName = new MyGuiControlTextbox(this, controlsColumn1Origin + 2 * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_playerName);

            // controls for typing sector name
            Controls.Add(new MyGuiControlLabel(this, controlsColumn1Origin + 3 * controlsDelta, null, MyTextsWrapperEnum.SectorName, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_sector_name = new MyGuiControlTextbox(this, controlsColumn1Origin + 4 * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            Controls.Add(m_sector_name);

            // controls for typing sector id.
            Controls.Add(new MyGuiControlLabel(this, controlsColumn2Origin + 3 * controlsDelta, null, MyTextsWrapperEnum.SectorIdentifier, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            m_sectorIdentifierX = new MyGuiControlTextbox(this, controlsColumn2Origin + 4 * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            m_sectorIdentifierY = new MyGuiControlTextbox(this, controlsColumn2Origin + 5 * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);
            m_sectorIdentifierZ = new MyGuiControlTextbox(this, controlsColumn2Origin + 6 * controlsDelta + new Vector2(MyGuiConstants.TEXTBOX_MEDIUM_SIZE.X / 2.0f, 0), MyGuiControlPreDefinedSize.MEDIUM, "", 20, MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiControlTextboxType.NORMAL);

            Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 4 * controlsDelta, null, MyTextsWrapperEnum.X, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 5 * controlsDelta, null, MyTextsWrapperEnum.Y, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
            Controls.Add(new MyGuiControlLabel(this, controlsColumn2OriginLabel + 6 * controlsDelta, null, MyTextsWrapperEnum.Z, MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));

            Controls.Add(m_sectorIdentifierX);
            Controls.Add(m_sectorIdentifierY);
            Controls.Add(m_sectorIdentifierZ);

            AddOkAndCancelButtonControls(new Vector2(0, -0.02f));
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenEditorCopyToolSelectSector";
        }

        public override bool Update(bool hasFocus)
        {
            if (hasFocus)
            {
                bool namedSector = !string.IsNullOrEmpty(m_sector_name.Text);

                m_sectorIdentifierX.Enabled = !namedSector;
                m_sectorIdentifierY.Enabled = !namedSector;
                m_sectorIdentifierZ.Enabled = !namedSector;
            }
            return base.Update(hasFocus);
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {
            MyMwcSectorTypeEnum sectorType = string.IsNullOrEmpty(m_playerName.Text) ? MyMwcSectorTypeEnum.STORY : MyMwcSectorTypeEnum.SANDBOX;
            bool namedSector = !string.IsNullOrEmpty(m_sector_name.Text);
            MyMwcSectorIdentifier sectorIdentifier;
            if (namedSector)
            {
                sectorIdentifier = new MyMwcSectorIdentifier(sectorType, null, new MyMwcVector3Int(0, 0, 0), m_sector_name.Text);
            }
            else
            {
                int x, y, z;
                if (!int.TryParse(m_sectorIdentifierX.Text, out x) ||
                    !int.TryParse(m_sectorIdentifierY.Text, out y) ||
                    !int.TryParse(m_sectorIdentifierZ.Text, out z))
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, 
                        MyTextsWrapperEnum.BadSectorIdentifierInput, MyTextsWrapperEnum.MessageBoxCaptionError,
                        MyTextsWrapperEnum.Ok, null));
                    return;
                }

                sectorIdentifier = new MyMwcSectorIdentifier(sectorType, null, new MyMwcVector3Int(x, y, z), null);
            }

            // This screen will be closed on successfull load from progress screen
            MyGuiManager.AddScreen(new MyGuiScreenEditorLoadSectorObjectsProgress(
                this, MyTextsWrapperEnum.LoadingPleaseWait, m_playerName.Text, sectorIdentifier));
        }
    }
}
