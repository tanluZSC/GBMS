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
using GBMSAPI_CS_Example.CALIBRATION;
using GBMSAPI_CS_Example.MAIN_FORM;
using GBMSAPI_CS_Example.ACQUISITION_FORM;

namespace GBMSAPI_CS_Example
{
    public partial class GBMASPI_CsExampleMainForm : Form
    {
        // ver 3.1.0.0: option mask
        public bool GetOptionMask(out uint OptionMask, out uint FrameRateOptions)
        {
            OptionMask = 0;
            FrameRateOptions = 0;
            // Auto-Capture
            if (this.AutoCaptureCheckBox.Enabled == true &&
                this.AutoCaptureCheckBox.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE;
            }

            // No roll preview stop
            if (this.NoRollPreviewCheckBox.Enabled == true &&
                this.NoRollPreviewCheckBox.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_NO_ROLL_PREVIEW;
            }

            // Ver 3.1.0.0
            if (this.cbRollAutoPositioningInPreview.Enabled == true &&
                this.cbRollAutoPositioningInPreview.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ADAPT_ROLL_AREA_POSITION;
                (FrameRateOptions) |= GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_ADAPT_ROLL_AREA_POSITION;
            }
            if (this.cbExternalRollProcedure.Enabled == true &&
                this.cbExternalRollProcedure.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_EXTERNAL_ROLL_COMPOSITION;
            }
            if (this.gbRollDirection.Enabled)
            {
                if (this.rbForceRollToLeft.Checked)
                {
                    (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FORCE_ROLL_TO_LEFT;
                }
                else if (this.rbForceRollToRight.Checked)
                {
                    (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FORCE_ROLL_TO_RIGHT;
                }
            }
            // end Ver 3.1.0.0

            // Roll Preview stop mode
            if (this.PedalRollPreviewStopModeRadioButton.Enabled == true &&
                this.PedalRollPreviewStopModeRadioButton.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP;
            }

            // Roll automatic preview type: only if automatic roll preview mode
            if (this.CenterRollPreviewStopModeRadioButton.Enabled == true &&
                this.CenterRollPreviewStopModeRadioButton.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ROLL_PREVIEW_TYPE;
            }

            // Full res preview image resolution
            if (this.FullRresolutionInPreviewCheckBox.Enabled == true &&
                this.FullRresolutionInPreviewCheckBox.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FULL_RES_PREVIEW;
                (FrameRateOptions) |= GBMSAPI_NET_FrameRateOptions.GBMSAPI_NET_FRO_HIGH_RES_IN_PREVIEW;
            }

            // VER 2600:
            // check HighSpeed Preview
            if (this.HighSpeedCheckBox.Enabled == true &&
                this.HighSpeedCheckBox.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_HIGH_SPEED_PREVIEW;
            }

            // Flat object on roll scan area
            if (this.FlatSingleFingerOnRollAreaCheckBox.Enabled == true &&
                this.FlatSingleFingerOnRollAreaCheckBox.Checked == true)
            {
                (OptionMask) |= GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FLAT_SINGLE_FINGER_ON_ROLL_AREA;
            }

            return true;
        }

        public bool GetAcquisitionSettingsOptions(out uint ObjectToScanType, out uint OptionMask, out uint scanArea, out uint FrameRateOptions)
        {
            OptionMask = 0;
            scanArea = 0;
            ObjectToScanType = GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_NO_OBJECT_TYPE;
            FrameRateOptions = 0;

            String ObjToScanName = (String)(this.ObjectToScanComboBox.SelectedItem);
            uint ObjToScan = GBMSAPI_Example_Util.GetObjectToScanIDFromName(ObjToScanName);
            ObjectToScanType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(ObjToScan);
            if (ObjectToScanType != GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_NO_OBJECT_TYPE)
            {
                /*******************************
                 * Get Scan Area
                 * ****************************/
                bool IsUsedGA = this.RollStandardCheckBox.Enabled && this.RollStandardCheckBox.Checked;
                bool IsFlatOnRoll = this.FlatSingleFingerOnRollAreaCheckBox.Enabled &&
                    this.FlatSingleFingerOnRollAreaCheckBox.Checked;
                scanArea = GBMSAPI_Example_Util.GetScanAreaFromObjectType(
                    ObjectToScanType,
                    IsFlatOnRoll,
                    IsUsedGA
                );
                if (scanArea == 0) return false;

                return GetOptionMask(out OptionMask, out FrameRateOptions);
            }
            return false;
        }
        // end ver 3.1.0.0: option mask
        public Boolean GetAcquisitionOptions(
            out uint ObjToScan,
            out uint OptionMask,
            out uint DisplayOptionMask,
            out Byte ContrastLimitToDisplay,
            out Byte CompletenessLimitToDisplay,
            out uint ClipRegionW,
            out uint ClipRegionH,
            out uint RollPreviewTimeout,
            out Byte ArtefactCleaningDepth,
            out Byte ContrastIntermediateLimitToDisplay,
            out Byte CompletenessIntermediateLimitToDisplay,
            // ver 2.10.0.0
            out bool RollAreaStandardGa,
            // end ver 2.10.0.0
            // ver 3.2.0.0
            out bool EnableRollCompositionBlock,
            // end ver 3.2.0.0
            // ver 4.0.0.0
            out int HwFfdStrictnessThreshold,
            out int SwFfdStrictnessThreshold
            // end ver 4.0.0.0
        )
        {
            ObjToScan = 0;
            OptionMask = 0;
            DisplayOptionMask = 0;
            ContrastLimitToDisplay = 0;
            CompletenessLimitToDisplay = 0;
            ClipRegionW = 0;
            ClipRegionH = 0;
            RollPreviewTimeout = 0;
            ArtefactCleaningDepth = 0;
            ContrastIntermediateLimitToDisplay = 0;
            CompletenessIntermediateLimitToDisplay = 0;
            RollAreaStandardGa = false;
            // ver 3.2.0.0
            EnableRollCompositionBlock = false;
            // end ver 3.2.0.0
            // ver 4.0.0.0
            HwFfdStrictnessThreshold = -1;
            SwFfdStrictnessThreshold = -1;
            // end ver 4.0.0.0

            try
            {
                // ver 2.10.0.0
                /////////////////////////////
                // ROLL AREA STANDARD
                /////////////////////////////
                if ( this.RollStandardCheckBox.Enabled &&this.RollStandardCheckBox.Checked)
                {
                    RollAreaStandardGa = true;
                }
                else
                {
                    RollAreaStandardGa = false;
                }
                // end ver 2.10.0.0
                /////////////////////////////
                // GET OBJECT TO SCAN
                /////////////////////////////
                String ObjToScanName = (String)(this.ObjectToScanComboBox.SelectedItem);
                ObjToScan = GBMSAPI_Example_Util.GetObjectToScanIDFromName(ObjToScanName);

                /////////////////////////////
                // GET OPTION MASK
                /////////////////////////////
                uint dummyFrOpt;
                GetOptionMask(out OptionMask, out dummyFrOpt);

                /////////////////////////////////////
                // GET DISPLAY OPTION MASK
                /////////////////////////////////////
                DisplayOptionMask = 0;

                uint ExternalDevicesMask;
                int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(out ExternalDevicesMask);

                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "GetAcquisitionOptions,GBMSAPI_NET_GetOptionalExternalEquipment");
                    return false;
                }

                if ((ExternalDevicesMask & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                {
                    // Stop screen after acquisition
                    if (this.StopScreenAfterAcquisitionCheckBox.Enabled == true &&
                        this.StopScreenAfterAcquisitionCheckBox.Checked == true)
                    {
                        (DisplayOptionMask) |= GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_FINAL_SCREEN;
                    }

                    // customized contrast
                    if (this.ShowCustomizedContrastCheckBox.Enabled == true &&
                        this.ShowCustomizedContrastCheckBox.Checked == true)
                    {
                        // read limit value
                        try
                        {
                            Byte ContrastLimit = Byte.Parse(this.ContrastLimitTextBox.Text);
                            ContrastLimitToDisplay = ContrastLimit;
                        }
                        catch (Exception ReadEx)
                        {
                            if ((ReadEx.GetType()).ToString() == ((new ArgumentNullException()).GetType()).ToString() ||
                                (ReadEx.GetType()).ToString() == ((new FormatException()).GetType()).ToString() ||
                                (ReadEx.GetType()).ToString() == ((new OverflowException()).GetType()).ToString()
                                )
                            {
                                // default value
                                ContrastLimitToDisplay = 120;
                            }
                            else
                            {
                                MessageBox.Show("Exception in GetAcquisitionOptions: " + ReadEx.Message);
                                ObjToScan = 0;
                                OptionMask = 0;
                                DisplayOptionMask = 0;
                                ContrastLimitToDisplay = 0;
                                CompletenessLimitToDisplay = 0;
                                ClipRegionW = 0;
                                ClipRegionH = 0;
                                RollPreviewTimeout = 0;
                                ArtefactCleaningDepth = 0;
                                return false;
                            }
                        }
                        (DisplayOptionMask) |= GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_CUSTOMIZED_CONTRAST;
                    }

                    // customized completeness
                    if (this.ShowCustomizedCompletenessCheckBox.Enabled == true && this.ShowCustomizedCompletenessCheckBox.Checked == true)
                    {
                        // read limit value
                        try
                        {
                            Byte CompletenessLimit = Byte.Parse(this.CompletenessLimitTextBox.Text);
                            CompletenessLimitToDisplay = CompletenessLimit;
                        }
                        catch (Exception ReadEx)
                        {
                            if ((ReadEx.GetType()).ToString() == ((new ArgumentNullException()).GetType()).ToString() ||
                                (ReadEx.GetType()).ToString() == ((new FormatException()).GetType()).ToString() ||
                                (ReadEx.GetType()).ToString() == ((new OverflowException()).GetType()).ToString()
                                )
                            {
                                // default value
                                CompletenessLimitToDisplay = 90;
                            }
                            else
                            {
                                MessageBox.Show("Exception in GetAcquisitionOptions: " + ReadEx.Message);
                                ObjToScan = 0;
                                OptionMask = 0;
                                DisplayOptionMask = 0;
                                ContrastLimitToDisplay = 0;
                                CompletenessLimitToDisplay = 0;
                                ClipRegionW = 0;
                                ClipRegionH = 0;
                                RollPreviewTimeout = 0;
                                ArtefactCleaningDepth = 0;
                                return false;
                            }
                        }
                        (DisplayOptionMask) |= GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_CUSTOMIZED_COMPLETENESS;
                    }
                }
                #region 获取裁剪数据
                ///////////////////////////////////////
                // GET CLIP DATA
                ///////////////////////////////////////
                ClipRegionW = 0;
                ClipRegionH = 0;
                if (this.ClipEnableCheckBox.Enabled == true &&
                    this.ClipEnableCheckBox.Checked == true)
                {
                    // read clip values
                    try
                    {
                        UInt32 ClipSizeX = UInt32.Parse(this.ClipRegionSizeXTextBox.Text);
                        UInt32 ClipSizeY = UInt32.Parse(this.ClipRegionSizeYTextBox.Text);
                        ClipRegionW = ClipSizeX;
                        ClipRegionH = ClipSizeY;
                    }
                    catch (Exception ReadEx)
                    {
                        if ((ReadEx.GetType()).ToString() == ((new ArgumentNullException()).GetType()).ToString() ||
                            (ReadEx.GetType()).ToString() == ((new FormatException()).GetType()).ToString() ||
                            (ReadEx.GetType()).ToString() == ((new OverflowException()).GetType()).ToString()
                            )
                        {
                            // default value
                            ClipRegionW = 0;
                            ClipRegionH = 0;
                        }
                        else
                        {
                            MessageBox.Show("Exception in GetAcquisitionOptions: " + ReadEx.Message);
                            ObjToScan = 0;
                            OptionMask = 0;
                            DisplayOptionMask = 0;
                            ContrastLimitToDisplay = 0;
                            CompletenessLimitToDisplay = 0;
                            ClipRegionW = 0;
                            ClipRegionH = 0;
                            RollPreviewTimeout = 0;
                            ArtefactCleaningDepth = 0;
                            return false;
                        }
                    }
                }
                #endregion
                ///////////////////////////////////////////
                // GET ROLL OPTIONS
                ///////////////////////////////////////////
                // Roll Preview Timeout
                RollPreviewTimeout = 0;
                if (this.AutomaticRollPreviewTimeoutTextBox.Enabled == true)
                {
                    // read timeout
                    try
                    {
                        UInt32 Timeout = UInt32.Parse(this.AutomaticRollPreviewTimeoutTextBox.Text);
                        RollPreviewTimeout = Timeout;
                    }
                    catch (Exception ReadEx)
                    {
                        if ((ReadEx.GetType()).ToString() == ((new ArgumentNullException()).GetType()).ToString() ||
                            (ReadEx.GetType()).ToString() == ((new FormatException()).GetType()).ToString() ||
                            (ReadEx.GetType()).ToString() == ((new OverflowException()).GetType()).ToString()
                            )
                        {
                            // default value
                            RollPreviewTimeout = 0;
                        }
                        else
                        {
                            MessageBox.Show("Exception in GetAcquisitionOptions: " + ReadEx.Message);
                            ObjToScan = 0;
                            OptionMask = 0;
                            DisplayOptionMask = 0;
                            ContrastLimitToDisplay = 0;
                            CompletenessLimitToDisplay = 0;
                            ClipRegionW = 0;
                            ClipRegionH = 0;
                            RollPreviewTimeout = 0;
                            ArtefactCleaningDepth = 0;
                            return false;
                        }
                    }
                }
                // Artefact Cleaning Depth
                ArtefactCleaningDepth = 0;
                if (this.ArtefactCleaningDepthTextBox.Enabled == true)
                {
                    // read Clean Depth
                    try
                    {
                        Byte CleanDepth = Byte.Parse(this.ArtefactCleaningDepthTextBox.Text);
                        if (CleanDepth > GBMSAPI_NET_CleaningDepth.GBMSAPI_NET_ARTEFACT_CLEANING_DEPTH_MAX &&
                            CleanDepth != GBMSAPI_NET_CleaningDepth.GBMSAPI_NET_ARTEFACT_CLEANING_DEPTH_UNLIMITED)
                        {
                            MessageBox.Show("Clean depth greater than maximum allowed value; will be automatically set to max", "Warning");
                            CleanDepth = GBMSAPI_NET_CleaningDepth.GBMSAPI_NET_ARTEFACT_CLEANING_DEPTH_MAX;
                        }
                        ArtefactCleaningDepth = CleanDepth;
                    }
                    catch (Exception ReadEx)
                    {
                        if ((ReadEx.GetType()).ToString() == ((new ArgumentNullException()).GetType()).ToString() ||
                            (ReadEx.GetType()).ToString() == ((new FormatException()).GetType()).ToString() ||
                            (ReadEx.GetType()).ToString() == ((new OverflowException()).GetType()).ToString()
                            )
                        {
                            // default value
                            ArtefactCleaningDepth = 0;
                        }
                        else
                        {
                            MessageBox.Show("Exception in GetAcquisitionOptions: " + ReadEx.Message);
                            ObjToScan = 0;
                            OptionMask = 0;
                            DisplayOptionMask = 0;
                            ContrastLimitToDisplay = 0;
                            CompletenessLimitToDisplay = 0;
                            ClipRegionW = 0;
                            ClipRegionH = 0;
                            RollPreviewTimeout = 0;
                            ArtefactCleaningDepth = 0;
                            return false;
                        }
                    }
                }

                // ver 3.2.0.0
                // EnableRollCompositionBlock
                if (this.cbEnableRollCompositionBlock.Checked) EnableRollCompositionBlock = true;
                // end ver 3.2.0.0

                ///////////////////////////////////////////
                // GET FFD OPTIONS
                ///////////////////////////////////////////
                // ver 4.0.0.0
                if (this.gbHwFfdSettings.Enabled)
                {
                    if (this.rbHwFfdHighStrictness.Checked)
                    {
                        HwFfdStrictnessThreshold = GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_HIGH_STRICTNESS;
                    }
                    else if (this.rbHwFfdMediumStrictness.Checked)
                    {
                        HwFfdStrictnessThreshold = GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_MEDIUM_STRICTNESS;
                    }
                    else if (this.rbHwFfdLowStrictness.Checked)
                    {
                        HwFfdStrictnessThreshold = GBMSAPI_NET_HwAntifakeStrictnessThresholds.GBMSAPI_NET_HW_ANTIFAKE_THRESHOLD_LOW_STRICTNESS;
                    }
                    else if (this.rbHwFfdPersonalizedStrictness.Checked)
                    {
                        int dummyFfdStrictTh;
                        if (int.TryParse(this.tbHwFfdPersonalizedStrictValue.Text, out dummyFfdStrictTh))
                        {
                            // ver 4.1.0: max value for hw ffd threshold set to 150
                            if (dummyFfdStrictTh >= 10 && dummyFfdStrictTh <= 150)
                            {
                                HwFfdStrictnessThreshold = dummyFfdStrictTh;
                            }
                        }
                    }
                }
                if (this.gbSwFfdSettings.Enabled)
                {
                    int dummyFfdStrictTh;
                    if (int.TryParse(this.tbSwFfdThreshold.Text, out dummyFfdStrictTh))
                    {
                        if (dummyFfdStrictTh >= 0 && dummyFfdStrictTh <= 100)
                        {
                            SwFfdStrictnessThreshold = dummyFfdStrictTh;
                        }
                    }
                }
                // end ver 4.0.0.0

                ///////////////////////////
                //// RETURN OK
                ///////////////////////////
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetAcquisitionOptions function: " + ex.Message);
                ObjToScan = 0;
                OptionMask = 0;
                DisplayOptionMask = 0;
                ContrastLimitToDisplay = 0;
                CompletenessLimitToDisplay = 0;
                ClipRegionW = 0;
                ClipRegionH = 0;
                RollPreviewTimeout = 0;
                ArtefactCleaningDepth = 0;
                EnableRollCompositionBlock = false;

                return false;
            }
        }
    }
}