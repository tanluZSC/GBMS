using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GBMSAPI_NET;
using GBMSAPI_NET.GBMSAPI_NET_Defines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_AcquisitionProcessDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_DeviceCharacteristicsDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_ErrorCodesDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_RollFunctionalityDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_VisualInterfaceLCDDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_VisualInterfaceLEDsDefines;
using GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions;

using GBMSAPI_CS_Example.UTILITY;

namespace GBMSAPI_CS_Example.MAIN_FORM
{
    public partial class VUI_LED : Form
    {
        public VUI_LED()
        {
            InitializeComponent();
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            uint IndicatorMask = 0x0000;
			int Color = 0,RetVal;
            bool Blink;

			// check blink
			if (BlinkCheckBox.Checked) {
				Blink = true;
			}
			else Blink = false;
            
            // check Color
			if (GreenRadioButton.Checked){					 
				Color = GBMSAPI_NET_LEDColors.GBMSAPI_NET_COLOR_GREEN;
			}
			if (RedRadioButton.Checked){					 
				Color = GBMSAPI_NET_LEDColors.GBMSAPI_NET_COLOR_RED;
			}
			if (YellowRadioButton.Checked){					 
				Color = GBMSAPI_NET_LEDColors.GBMSAPI_NET_COLOR_YELLOW;
			}
			if (OffRadioButton.Checked){
				Color = GBMSAPI_NET_LEDColors.GBMSAPI_NET_COLOR_OFF;
			}

            // set indicator mask
			if (RightThumbCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_THUMB_RIGHT;
			}
			if (RightIndexCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_INDEX_RIGHT;
			}
			if (RightMiddleCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_MIDDLE_RIGHT;
			}
			if (RightRingCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_RING_RIGHT;
			}
			if (RightLittleCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_LITTLE_RIGHT;
			}
			if (LeftThumbCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_THUMB_LEFT;
			}
			if (LeftIndexCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_INDEX_LEFT;
			}
			if (LeftMiddleCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_MIDDLE_LEFT;
			}
			if (LeftRingCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_RING_LEFT;
			}
			if (LeftLittleCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_LITTLE_LEFT;
			}
			if (RollIndicationCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_ROLL;
			}
			if (StatusCheckBox.Checked) {
				IndicatorMask |= GBMSAPI_NET_LEDMaskValues.GBMSAPI_NET_VIL_STATUS;
			}


            RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_SetFingerIndicatorVUIState(IndicatorMask, Color, Blink);

			if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "SetButton_Click,GBMSAPI_SetFingerIndicatorVUIState");
                return;
            }

			return;
        }
    }
}